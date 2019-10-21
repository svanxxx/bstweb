using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class MachineState
{
	public int ID { get; set; }
	public string NAME { get; set; }
	public int TESTS { get; set; }
	public string STATUS { get; set; }
	public string VERSION { get; set; }
	public string STARTED { get; set; }
	public string PCPING { get; set; }
	public string PAUSEDBY { get; set; }
	public MachineState()
	{
	}
	public MachineState(PC pc)
	{
		ID = pc.ID;
		NAME = pc.PCNAME;
		STATUS = pc.CURRENT;
		VERSION = pc.TESTREQUEST != null ? pc.TESTREQUEST.FIPVERSION.VERSION : "";
		STARTED = pc.STARTED == null ? "" : pc.STARTED.GetValueOrDefault().ToString(IdBasedObject.defDateTimeFormat, CultureInfo.InvariantCulture);
		PCPING = pc.PCPING == null ? "" : pc.PCPING.GetValueOrDefault().ToString(IdBasedObject.defDateTimeFormat, CultureInfo.InvariantCulture);
		PAUSEDBY = pc.PERSON == null ? "" : pc.PERSON.PHONE;
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
					CbstHelper.FeedLog(string.Format("Machine '{0}' has been paused", result.PCNAME));
				}
				else
				{
					result.PAUSEDBY = null;
					CbstHelper.FeedLog(string.Format("Machine '{0}' has been resumed", result.PCNAME));
				}
				db.SaveChanges();
			}
		}
	}
}