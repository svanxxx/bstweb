using System;
using System.Text.RegularExpressions;

namespace BSTStatics
{
	public static class BSTStat
	{
		public const string networkBSTMachine = "bst_master";
		public const string globalIPAddress = "213.184.249.150";
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
			return string.Format("<a href='{0}{1}'>{2}</a>", Settings.CurrentSettings.DEFECTLINK, id, res);
		}
	}
}
