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
			wcClient.UploadValues(Settings.CurrentSettings.TASKSSERVICE + "/SetTaskTestStatus", reqparm);
		}
	}
}