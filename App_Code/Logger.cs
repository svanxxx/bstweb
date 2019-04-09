using System;
using System.IO;

public static class Logger
{
	const string _fname = "c:\\testmanager.log";
	public static void Log(string message)
	{
		File.AppendAllText(_fname, DateTime.Now.ToString() + Environment.NewLine);
		File.AppendAllText(_fname, message + Environment.NewLine);
	}
	public static void Log(Exception e)
	{
		File.AppendAllText(_fname, "Exception!" + Environment.NewLine);
		Log(e.ToString());
	}
	public static string ClearLog()
	{
		File.WriteAllText(_fname, string.Empty);
		Log("Log Cleared by " + CurrentContext.UserName());
		return GetLog(0, 100);
	}
	public static string GetLog(int from, int to)
	{
		int iline = 0;
		string res = "";
		using (var input = File.OpenText(_fname))
		{
			string line = input.ReadLine();
			while (line != null)
			{
				if (iline > to)
				{
					return res;
				}
				if (iline >= from && iline <= to)
				{
					res += line + Environment.NewLine;
				}
				iline++;
				line = input.ReadLine();
			}
		}
		return res;
	}
}