using System;
using System.Text.RegularExpressions;

namespace BSTStatics
{
	public static class BSTStat
	{
		public const string networkBSTMachine = "192.168.0.8";
		public const string globalIPAddress = "213.184.249.150";
		public const string mainName = "mps.resnet.com";
		public const string returnurl = "returnurl";
		public const string defDateFormat = "MM-dd-yyyy";
		public const string SQLDateFormat = "yyyy-MM-dd HH:mm:ss";
		public static string ReplaceTT(string strTT)
		{
			return Regex.Replace(strTT, "TT\\d+", TTEvaluator);
		}
		private static string TTEvaluator(Match match)
		{
			string res = match.Groups[0].Value;
			string id = Convert.ToInt32(res.Replace("TT", "")).ToString();
			return string.Format("<a href='http://{0}/taskmanager/showtask.aspx?ttid={1}'>{2}</a>", BSTStat.mainName, id, res);
		}
	}
}
