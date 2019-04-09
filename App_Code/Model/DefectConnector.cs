using System;
using System.Collections.Specialized;
using System.Net;

public static class DefectConnector
{
	public static void UpdateDefect(string ttid, bool failed, string userphone)
	{
		using (var wcClient = new WebClient())
		{
			var reqparm = new NameValueCollection();
			reqparm.Add("ttid", ttid);
			reqparm.Add("failed", failed.ToString());
			reqparm.Add("userphone", userphone);
			try
			{
				wcClient.UploadValues(Settings.CurrentSettings.TASKSSERVICE + "/SetTaskTestStatus", reqparm);
			}
			catch (Exception e)
			{
				Logger.Log(e);
			};
		}
	}
	public static void NotifyDefectWorker(string ttid, string message, string userphone)
	{
		using (var wcClient = new WebClient())
		{
			var reqparm = new NameValueCollection();
			reqparm.Add("ttid", ttid);
			reqparm.Add("message", message);
			reqparm.Add("userphone", userphone);
			try
			{
				wcClient.UploadValues(Settings.CurrentSettings.TASKSSERVICE + "/NotifyDefectWorker", reqparm);
			}
			catch (Exception e)
			{
				Logger.Log(e);
			};
		}
	}
}