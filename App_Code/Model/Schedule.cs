using System;

public class Schedule
{
	static string _MachineSpec = "PCNAME:";
	static string _Tabl = "SCHEDULE";
	static string GetParam(string strCommand, string strSearch)
	{
		string strReturn = "";
		strCommand += " ";

		int index = strCommand.ToUpper().IndexOf(strSearch.ToUpper());
		if (index > -1)
		{
			int indexEnd = Math.Min(strCommand.IndexOf("\"", index) > 0 ? strCommand.IndexOf("\"", index) : strCommand.Length, strCommand.IndexOf(" ", index) > 0 ? strCommand.IndexOf(" ", index) : strCommand.Length);
			strReturn = strCommand.Substring(index + strSearch.Length, indexEnd - index - strSearch.Length).ToUpper();
		}

		return strReturn;
	}

	public static void AddCommands(string[] Commands, string[] arrGroup, string RequestID, string strUserID, string priority)
	{
		int i = -1;
		int K = -1;
		string strSetSQL = string.Format(@"INSERT INTO {0} (COMMAND, REQUESTID, PCID, USERID, PRIORITY, SEQUENCENUMBER, SEQUENCEGUID, DBTYPE, Y3DV) VALUES ", _Tabl);

		foreach (string strCommand in Commands)
		{
			i++;

			if (string.IsNullOrEmpty(strCommand))
				continue;
			K++;

			string dbtype = GetParam(strCommand, "dbtype:");
			dbtype = (dbtype == "" ? "NULL" : "'" + dbtype + "'");

			string y3dv = GetParam(strCommand, "special:");
			y3dv = (y3dv.Contains("3DV") ? "1" : "NULL");

			// Find PCName Substring
			string sPCName = "NULL";
			int index = strCommand.ToUpper().IndexOf(_MachineSpec);
			if (index > -1)
			{
				int indexEnd = Math.Min(strCommand.IndexOf("\"", index) > 0 ? strCommand.IndexOf("\"", index) : strCommand.Length, strCommand.IndexOf(" ", index) > 0 ? strCommand.IndexOf(" ", index) : strCommand.Length);
				sPCName = strCommand.Substring(index + _MachineSpec.Length, indexEnd - index - _MachineSpec.Length).ToUpper();
			}

			// Get SQL PCName
			string strSQLPCName = "";
			if (sPCName == "NULL") strSQLPCName = sPCName;
			else strSQLPCName = "(select T1.ID from PCS T1 where T1.PCNAME = '" + sPCName + "')";
			string strGuid = (string.IsNullOrEmpty(arrGroup[i]) ? Guid.NewGuid().ToString() : arrGroup[i]);

			strSetSQL += (K != 0 ? "," : "") + "('" + strCommand + "', " + RequestID + "," + strSQLPCName + "," + strUserID + ", " + priority + "," + i.ToString() + ", '" + strGuid + "', " + dbtype + ", " + y3dv + ")";
		}
		if (K != -1)
		{
			DBHelper.SQLExecute(strSetSQL);
		}
	}
}