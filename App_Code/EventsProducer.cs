using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using BSTStatics;
using System.Web;

public class EventsProducerListener
{
	public HttpResponse _Response;
	public HttpRequest _Request;
	public readonly Dictionary<string, string> _eventsState = new Dictionary<string, string>();
	public readonly EventsProducer eventsProducer = EventsProducer.getProducer();
}

public class EventsProducer
{
	protected Thread _Thread;
	private readonly ConcurrentDictionary<EventsProducerListener, bool> _clients = new ConcurrentDictionary<EventsProducerListener, bool>();
	private readonly ConcurrentDictionary<string, string> _eventsState = new ConcurrentDictionary<string, string>();
	private static readonly EventsProducer _EventsProducer = new EventsProducer();
	private static string _signal_requestsstate = "requestsstate";
	private static string _signal_lastrequest = "lastrequest";
	private static string _host_lastupdated = "host_lastupdated";

	public static EventsProducer getProducer()
	{
		return _EventsProducer;
	}
	private void ReadEvents()
	{
		if (_eventsState.Keys.Contains(_signal_requestsstate))
		{
			string requestsState = CbstHelper.GetValue(@"
						SELECT 
						(SELECT CONVERT(VARCHAR(MAX), COUNT(*)) FROM [TESTREQUESTS] WHERE IGNORE IS NULL AND TESTED IS NULL AND USERID IS NULL) + '/' +
						(SELECT CONVERT(VARCHAR(MAX), (SELECT COUNT (DISTINCT REQUESTID) FROM SCHEDULE))) + '/' +
						(SELECT CONVERT(VARCHAR(MAX), COUNT(*)) FROM [TESTREQUESTS] WHERE IGNORE IS NULL AND TESTED IS NULL AND USERID IS NOT NULL)
					").ToString();

			_eventsState[_signal_requestsstate] = requestsState;
		}
		if (_eventsState.Keys.Contains(_signal_lastrequest))
		{
			string lastrequest = CbstHelper.GetValue("SELECT TOP 1 [ID] FROM [TESTREQUESTS] ORDER BY [ID] DESC").ToString();
			_eventsState[_signal_lastrequest] = lastrequest;
		}
		if (_eventsState.Keys.Contains(_host_lastupdated))
		{
			string host_lastupdated = CbstHelper.GetValue("SELECT MAX(H.LAST_UPDATED) M FROM HOSTS H").ToString();
			_eventsState[_host_lastupdated] = host_lastupdated;
		}
	}
	void CheckWorkerState()
	{
		if (_Thread == null || !_Thread.IsAlive)
		{
			_Thread = new Thread(() =>
			{
				Thread.CurrentThread.IsBackground = true;
				do
				{
					Thread.Sleep(1000);
					ReadEvents();
					SendEventsToClients();
				}
				while (_clients.Count > 0);
			});
			_Thread.Start();
		}
	}
	void UpdateRequiredEvents()
	{
		HashSet<string> events = new HashSet<string>();
		foreach (var bs in _clients)
		{
			foreach (string key in bs.Key._eventsState.Keys)
			{
				events.Add(key);
			}
		}
		foreach (string ev in events)
		{
			if (!_eventsState.ContainsKey(ev))
			{
				_eventsState.TryAdd(ev, "");
			}
		}
		foreach (string ev in _eventsState.Keys)
		{
			if (!events.Contains(ev))
			{
				string s;
				_eventsState.TryRemove(ev, out s);
			}
		}
	}
	public void FinishContract(EventsProducerListener bs)
	{
		bool tmp = false;
		_clients.TryRemove(bs, out tmp);
		UpdateRequiredEvents();
	}
	void StartContract(EventsProducerListener bs)
	{
		_clients.TryAdd(bs, true);
		UpdateRequiredEvents();
	}
	public bool IsConnected(EventsProducerListener bs)
	{
		bool bok = false;
		if (_clients.TryGetValue(bs, out bok))
		{
			if (!bok)
			{
				//sending to this client produces exceptions or the client connection is not alive - removing client
				FinishContract(bs);
			}
			return bok;
		}
		else
		{
			StartContract(bs);
			CheckWorkerState();
			return true;
		}
	}
	private bool SendEvents(EventsProducerListener bs)
	{
		foreach (KeyValuePair<string, string> entry1 in _eventsState)
		{
			//first send all required data
			foreach (KeyValuePair<string, string> entry2 in bs._eventsState)
			{
				if (entry1.Key != entry2.Key || entry1.Value == entry2.Value)
				{
					continue;
				}
				string s1 = string.Format("event: {0}\n", entry1.Key);
				string s2 = string.Format("data: {0}\n\n", entry1.Value);
				
				try
				{
					bs._Response.Write(s1);
					bs._Response.Write(s2);
					bs._Response.Flush();

					//temp workaround - cannot fix quickly
					bs._Response.Write(s1);
					bs._Response.Write(s2);
					bs._Response.Flush();
				}
				catch
				{
					return false;
				}
				break;
			}
			//second update state to new one
			if (bs._eventsState.ContainsKey(entry1.Key))
				bs._eventsState[entry1.Key] = entry1.Value;
		}
		return true;
	}
	private bool ClientUpdate(EventsProducerListener bs)
	{
		if (!bs._Response.IsClientConnected)
		{
			return false;
		}
		return SendEvents(bs);
	}
	//trying to send new data to client, if failed - mark client as dead(so the client will be removed )
	private void SendEventsToClients()
	{
		foreach (var bs in _clients)
		{
			if (bs.Value)
			{
				try
				{
					if (!ClientUpdate(bs.Key))
					{
						_clients.TryUpdate(bs.Key, false, true);
					}
				}
				catch
				{
					_clients.TryUpdate(bs.Key, false, true);
				}
			}
		}
	}
}
