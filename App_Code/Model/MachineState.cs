using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class MachineState
{
	[Flags]
	public enum MachineStatus
	{
		Start = 1,
		Stop = 1 << 1,
		Install = 1 << 2,
		Release = 1 << 3,
		Restart = 1 << 4,
		SSGET = 1 << 5,
		Shutdown = 1 << 6
	}

	public int ID { get; set; }
	public string NAME { get; set; }
	public string STATUS { get; set; }
	public string VERSION { get; set; }
	public string STARTED { get; set; }
	public string PCPING { get; set; }
	public string PAUSEDBY { get; set; }
	public string IP { get; set; }
	public int SCHEDULES { get; set; }
	int ACTIONFLAG { get; set; }

	void Init(PC pc)
	{
		ID = pc.ID;
		NAME = pc.PCNAME;
		if (!string.IsNullOrEmpty(pc.CURRENT))
		{
			STATUS = pc.CURRENT.Split(new string[] { "   :" }, StringSplitOptions.RemoveEmptyEntries).Last().Trim();
		}
		VERSION = pc.TESTREQUEST != null ? pc.TESTREQUEST.FIPVERSION.VERSION : "";
		STARTED = pc.STARTED == null ? "" : pc.STARTED.GetValueOrDefault().ToString(IdBasedObject.defDateTimeFormat, CultureInfo.InvariantCulture);
		PCPING = pc.PCPING == null ? "" : pc.PCPING.GetValueOrDefault().ToString(IdBasedObject.defDateTimeFormat, CultureInfo.InvariantCulture);
		PAUSEDBY = pc.PERSON == null ? "" : pc.PERSON.PHONE;
		string[] parts = NAME.ToUpper().Split('X');
		if (parts.Length > 1)
		{
			IP = "192.168.0.1" + parts[1];
		}
		ACTIONFLAG = pc.ACTIONFLAG.GetValueOrDefault(0);
		SCHEDULES = pc.SCHEDULES.GetValueOrDefault(0);
	}
	public MachineState()
	{
	}
	public MachineState(PC pc)
	{
		Init(pc);
	}
	public MachineState(string name)
	{
		List<MachineState> res = new List<MachineState>();
		using (var db = new BST_STATISTICSEntities())
		{
			foreach (var m in db.PCS.Where(b => b.PCNAME == name))
			{
				Init(m);
				return;
			}
		}
		throw new Exception("Machine not found.");
	}
	public MachineState(int id)
	{
		List<MachineState> res = new List<MachineState>();
		using (var db = new BST_STATISTICSEntities())
		{
			foreach (var m in db.PCS.Where(b => b.ID == id))
			{
				Init(m);
				return;
			}
		}
		throw new Exception("Machine not found.");
	}
	public static List<MachineState> EnumUsed()
	{
		List<MachineState> res = new List<MachineState>();
		using (var db = new BST_STATISTICSEntities())
		{
			foreach (var m in db.PCS.Where(b => !b.UNUSED))
			{
				res.Add(new MachineState(m));
			}
		}
		return res;
	}
	public static void PauseOnOff(int id, int idusr)
	{
		using (var db = new BST_STATISTICSEntities())
		{
			var result = db.PCS.SingleOrDefault(b => b.ID == id);
			if (result != null)
			{
				if (result.PAUSEDBY == null)
				{
					result.PAUSEDBY = idusr;
					Log.FeedLog(string.Format("Machine '{0}' has been paused", result.PCNAME));
				}
				else
				{
					result.PAUSEDBY = null;
					Log.FeedLog(string.Format("Machine '{0}' has been resumed", result.PCNAME));
				}
				db.SaveChanges();
			}
		}
	}
	public void UpdateMachineStatus(MachineStatus status)
	{
		int iStatus = ACTIONFLAG;
		MachineStatus ActionFlag = (MachineStatus)iStatus;
		if ((ActionFlag & status) == 0)
		{
			ActionFlag |= status;

			using (var db = new BST_STATISTICSEntities())
			{
				db.SCHEDULEs.RemoveRange(db.SCHEDULEs.Where(x => x.LOCKEDBY == ID));

				db.PCS.First(p => p.PCNAME == NAME).ACTIONFLAG = (int)ActionFlag;
				db.SaveChanges();
			}
		}
		Log.FeedLog(string.Format("Machine {0} has been changed: {1}. By {2}", NAME, status.ToString(), CurrentContext.UserName()));
	}
}