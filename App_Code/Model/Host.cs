using System;
using System.Collections.Generic;
using System.Linq;

public static class CacheCleaner
{
	public static void ResetCache()
	{
		OfflineHost.ResetCache();
		HostState.ResetCache();
	}
}

public class OfflineHost
{
	public OfflineHost() { }
	public int ID { get; set; }
	public string NAME { get; set; }

	static object _Lock = new object();
	static DateTime? _used = null;
	static List<OfflineHost> _hosts = new List<OfflineHost>();
	public static void ResetCache()
	{
		lock (_Lock) { _used = null; };
	}
	public static List<OfflineHost> Enum()
	{
		lock (_Lock)
		{
			if (_used == null || DateTime.Now.Subtract(_used.Value).Seconds > 10)
			{
				_hosts.Clear();
				using (var db = new BST_STATISTICSEntities())
				{
					foreach (var h in db.HOSTS.Where(host => host.inactive == true))
					{
						_hosts.Add(new OfflineHost() { ID = h.id, NAME = h.name});
					}
				}
				_used = DateTime.Now;
			}
			return _hosts.ConvertAll(host => (OfflineHost)host.MemberwiseClone());
		}
	}
}

public class HostState
{
	public HostState() { }
	void Init(HOST host)
	{
		ID = host.id;
		NAME = host.name;
		STARTED = IdBasedObject.DateTimeToString(host.STARTED);
		PCPING = IdBasedObject.DateTimeToString(host.pcping);
		INFO = host.systeminfo;
		IP = host.ip;
		MAC = host.mac;
	}
	public HostState(HOST host) { Init(host); }
	public int ID { get; set; }
	public string NAME { get; set; }
	public string STARTED { get; set; }
	public string PCPING { get; set; }
	public string INFO { get; set; }
	public string IP { get; set; }
	public string MAC { get; set; }
	public List<string> CHILDREN
	{
		get
		{
			return Array.ConvertAll(MachineState.EnumUsed().Where(x => x.HOSTID == ID).ToArray(), el => el.NAME.ToString()).ToList();
		}
	}

	static object _Lock = new object();
	static DateTime? _used = null;
	static List<HostState> _hosts = new List<HostState>();
	public static void ResetCache()
	{
		lock (_Lock)
		{
			_used = null;
		};
	}
	public static List<HostState> EnumUsed()
	{
		lock (_Lock)
		{
			if (_used == null || DateTime.Now.Subtract(_used.Value).Seconds > 10)
			{
				_hosts.Clear();
				using (var db = new BST_STATISTICSEntities())
				{
					foreach (var h in db.HOSTS.Where(host => host.inactive == null))
					{
						_hosts.Add(new HostState(h));
					}
				}
				_used = DateTime.Now;
			}
			return _hosts.ConvertAll(host => (HostState)host.MemberwiseClone());
		}
	}

	public static void DeleteHost(int id)
	{
		using (var db = new BST_STATISTICSEntities())
		{
			var result = db.HOSTS.SingleOrDefault(b => b.id == id);
			if (result != null)
			{
				Log.FeedLog(string.Format("Host '{0}' has been deleted", result.name));
				result.inactive = true;
				db.SaveChanges();
			}
		}
		CacheCleaner.ResetCache();
	}
	public static void OnlineHost(int id)
	{
		using (var db = new BST_STATISTICSEntities())
		{
			var result = db.HOSTS.SingleOrDefault(b => b.id == id);
			if (result != null)
			{
				Log.FeedLog(string.Format("Host '{0}' has set online", result.name));
				result.inactive = null;
				db.SaveChanges();
			}
		}
		CacheCleaner.ResetCache();
	}
	public static void StartStopHost(int id, bool start)
	{
		using (var db = new BST_STATISTICSEntities())
		{
			var result = db.HOSTS.SingleOrDefault(b => b.id == id);
			if (result != null)
			{
				if (start)
				{
					Log.FeedLog(string.Format("Host '{0}' has been started", result.name));
					result.poweron = true;
				}
				else
				{
					Log.FeedLog(string.Format("Host '{0}' has been stopped", result.name));
					result.poweroff = true;
				}
				db.SaveChanges();
			}
		}
		CacheCleaner.ResetCache();
	}
}