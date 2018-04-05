using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using BSTStatics;

/// <summary>
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService
{
	public WebService()
	{
	}
	protected void _UpdateAppRequestInfo(String strCount, String strID, int iNotAnswerRequestCount, String idSchedules)
	{
		double dCDate = Convert.ToDouble(DateTime.Now.ToOADate());
		Application.Lock();
		try
		{
			Application["UntestedRequestDate"] = dCDate.ToString();
			Application["UntestedRequestCount"] = strCount;
			Application["UntestedRequestIDs"] = strID;
			Application["NotAnswerRequestCount"] = iNotAnswerRequestCount.ToString();
			Application["IDdSchedulesTest"] = idSchedules;

		}
		catch (System.Exception /*ex*/)
		{

		}
		finally
		{
			Application.UnLock();
		}
	}
	protected double GetRepDelay()
	{
		return 1.0 / 24.0 / 60.0; //1 min
	}
	[WebMethod()]
	public string GetTestRequests()
	{
		double dCDate = Convert.ToDouble(DateTime.Now.ToOADate());
		string strDate = Application["UntestedRequestDate"] as string;

		if (string.IsNullOrEmpty(strDate))
		{
			_UpdateAppRequestInfo("0", "", 0, "");
			return "0,0";
		}
		else
		{
			double dOldDate = Convert.ToDouble(strDate);
			if (Math.Abs((dCDate - dOldDate)) > GetRepDelay())
			{
				int iCount = 0;
				string strTestsIDs = ",";
				// select (T1.ID) from TestRequests T1 where T1.ID not in (select distinct T2.RequestID from testruns T2  where T2.requestid is not null) AND (T1.IGNORE IS NULL OR T1.IGNORE != 1)
				using (DataSet ds = CbstHelper.GetDataSet("SELECT ID FROM TESTREQUESTS Where (UserID is null) AND (IGNORE IS NULL)"))
				{
					foreach (DataRow rowCur in ds.Tables[0].Rows)
					{
						strTestsIDs += rowCur[0].ToString() + ",";
						iCount++;
					}
				}

				int iNotAnswerCount = 0;
				using (DataSet ds = CbstHelper.GetDataSet("select count (TestRequests.RequestDateTime) from TestRequests where RequestDateTime <= DATEADD(hour, -12, GETDATE())  AND ((TestRequests.IGNORE is Null) and (TestRequests.Tested is Null) and (TestRequests.ProgAbb != 'Admin'))"))
				{
					foreach (DataRow rowCur in ds.Tables[0].Rows)
					{
						iNotAnswerCount = Convert.ToInt32(rowCur[0].ToString());
					}
				}

				string idSchedules = "";
				using (DataSet ds = CbstHelper.GetDataSet("SELECT Schedule.ID from Schedule  ORDER BY Schedule.ID DESC"))
				{
					foreach (DataRow rowCur in ds.Tables[0].Rows)
					{
						idSchedules += rowCur[0].ToString() + idSchedules != "" ? "," : "";
					}
				}

				_UpdateAppRequestInfo(iCount.ToString(), strTestsIDs, iNotAnswerCount, idSchedules);
				return Application["UntestedRequestCount"] as string + ", " + Application["NotAnswerRequestCount"] as string + ", " + Application["IDdSchedulesTest"] as string;
			}
			else return Application["UntestedRequestCount"] as string + ", " + Application["NotAnswerRequestCount"] as string + ", " + Application["IDdSchedulesTest"] as string;
		}
	}
	string GetLastTestID(string strVersionId)
	{
		using (DataSet ds = CbstHelper.GetDataSet("SELECT TOP 1 ID FROM TESTRUNS WHERE (TEST_FIPVERSIONID = " + strVersionId + ") ORDER BY ID DESC"))
		{
			foreach (DataRow rowCur in ds.Tables[0].Rows)
			{
				return rowCur[0].ToString();
			}
		}
		return "";
	}
	[WebMethod()]
	public string GetCompareVersionInfo(string strVersion1, string strVersion2)
	{
		string aaa = GetLastTestID(strVersion1) + " " + GetLastTestID(strVersion2);
		return aaa;
	}
	protected string GetParam(String strCommand, string strSearch)
	{
		String strReturn = "";
		strCommand += " ";

		int index = strCommand.ToUpper().IndexOf(strSearch.ToUpper());
		if (index > -1)
		{
			int indexEnd = Math.Min(strCommand.IndexOf("\"", index) > 0 ? strCommand.IndexOf("\"", index) : strCommand.Length, strCommand.IndexOf(" ", index) > 0 ? strCommand.IndexOf(" ", index) : strCommand.Length);
			strReturn = strCommand.Substring(index + strSearch.Length, indexEnd - index - strSearch.Length).ToUpper();
		}

		return strReturn;
	}
	[WebMethod()]
	public string RunBatch4Request(string user, string batchname, string requestGUID, string priority)
	{
		try
		{
			CbstHelper.RunBatch4Request(user, batchname, requestGUID, priority);
		}
		catch (Exception e)
		{
			return e.ToString();
		}

		return "OK";
	}
	[WebMethod(EnableSession = true)]
	public string RunTest(string strReqestID, string strCommandName, string UserName, string TestRunID)
	{
		// if it is release, we need rerun test in the last TESTREQUESTS with the same version
		string SQL_Command = "select (select max(t.id) from TESTREQUESTS t where t.versionid = x.versionid  and t.ProgAbb = x.ProgAbb) , x.ProgAbb from TESTREQUESTS x where x.id = $id";

		string ProgAbb;
		string strLastReqestID;
		using (DataSet ds = CbstHelper.GetDataSet(SQL_Command.Replace("$id", strReqestID)))
		{
			ProgAbb = ds.Tables[0].Rows[0][1].ToString();
			strLastReqestID = ds.Tables[0].Rows[0][0].ToString();
		}
		if (ProgAbb.ToUpper() == "ADMIN")
		{
			strReqestID = strLastReqestID;
		}

		strCommandName = (strCommandName.Replace('`', '"').Replace('~', '\\'));

		// get db type from strCommandName
		String dbtype = GetParam(strCommandName, "dbtype:");
		dbtype = (dbtype == "" ? "NULL" : "'" + dbtype + "'");

		String y3dv = GetParam(strCommandName, "special:");
		y3dv = (y3dv.Contains("3DV") ? "1" : "NULL");

		// get PCName from strCommandName
		String strSQLPCName = "", sPCName = GetParam(strCommandName, "PCName:");
		if (sPCName == "") strSQLPCName = "NULL";
		else
		{
			strSQLPCName = "(select T1.ID from PCS T1 where T1.PCNAME = '" + sPCName + "')";
			if (strCommandName.IndexOf("\"PCName:") < 0)
				sPCName = " PCName:" + sPCName;
			else sPCName = " \"PCName:" + sPCName + "\"";

		}
		String strGuid = Guid.NewGuid().ToString();
		String strPRIORITY = "4";

		String strSetSQL =
			@"INSERT INTO SCHEDULE (COMMAND, REQUESTID, PCID, USERID, PRIORITY, SEQUENCENUMBER, SEQUENCEGUID, DBTYPE, Y3DV) VALUES" +
			" ('" + strCommandName + "', " + strReqestID + "," + strSQLPCName + ",(select T2.ID from PERSONS T2 where T2.USER_LOGIN = '" + UserName + "'), " + strPRIORITY + ",2, '" + strGuid + "', " + dbtype + ", " + y3dv + ")";

		strSetSQL = strSetSQL.ToUpper();
		CbstHelper.SQLExecute(strSetSQL);

		CbstHelper.CommentTestRun(TestRunID, "Rerun");
		FeedLog("Next command has been rerun: " + strCommandName);
		return "OK";
	}
	public string ReplaceTT(string strTT)
	{
		return Regex.Replace(strTT, "TT\\d+", TTEvaluator);
	}
	private static string TTEvaluator(Match match)
	{
		return string.Format("<a href='http://{0}/tr/ShowTT.aspx?ttid=", BSTStat.mainAddress) +
		 (Convert.ToInt32(match.Groups[0].Value.Replace("TT", ""))).ToString() +
			"'>" + match.Groups[0].Value + "</a>";
	}
	// WhatBetweenVersions '{strVersionNew:"' + strVer1 + '",strRequest:"' + strRequest + '",strVersionOld:"' + strVer2 + '" }',
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
	[WebMethod()]
	public string WhatBetweenVersions(string strVersionNew, string strRequest, string strVersionOld)
	{
		DataSet ds;
		string SQL_Command_First, FirstGuid = "", SecondGuid = "";

		string SQL_Command = @"SELECT GITHASH, VersionID, ID FROM TESTREQUESTS WHERE ($WHERE = $PARAM) ORDER BY ID DESC";

		if (string.IsNullOrEmpty(strRequest))
			SQL_Command_First = SQL_Command.Replace("$WHERE", "VersionID").Replace("$PARAM", strVersionNew);
		else SQL_Command_First = SQL_Command.Replace("$WHERE", "ID").Replace("$PARAM", strRequest);

		ds = CbstHelper.GetDataSet(SQL_Command_First);
		FirstGuid = ds.Tables[0].Rows[0][0].ToString();

		ds = CbstHelper.GetDataSet(SQL_Command.Replace("$WHERE", "VersionID").Replace("$PARAM", strVersionOld));
		SecondGuid = ds.Tables[0].Rows[0][0].ToString();

		ProcessStartInfo gitInfo = new ProcessStartInfo();
		gitInfo.CreateNoWindow = true;
		gitInfo.RedirectStandardError = true;
		gitInfo.RedirectStandardOutput = true;
		gitInfo.UseShellExecute = false;
		gitInfo.FileName = @"c:\Program Files (x86)\Git\bin\git.exe";
		//gitInfo.FileName = @"git.exe";


		Process gitProcess = new Process();
		gitInfo.Arguments = "--no-pager log " + SecondGuid + ".." + FirstGuid + " --pretty=format:\"<tr><td>%ci</td><td>%s</td><td>%an</td></tr>\""; // such as "fetch orign"
		gitInfo.WorkingDirectory = "I:\\GIT\\V8\\";
		// f:\Work_FiP_V8\
		// gitInfo.WorkingDirectory = @"f:\Work_FiP_V8\";

		gitProcess.StartInfo = gitInfo;
		gitProcess.Start();

		string stdout_str = gitProcess.StandardOutput.ReadToEnd(); // pick up STDOUT

		gitProcess.WaitForExit();
		gitProcess.Close();
		/*
		string[] arrTT = stdout_str.Split('\n');
		stdout_str = "";
		for (int i = 0; i < arrTT.Count() - 1; i++)
		{
			 stdout_str += arrTT[i].Remove(0,8) + "<br>";
		}
		*/
		if (stdout_str.Length < 5)
		{
			return "Result is null. <br> Arguments is: <br>" + gitInfo.Arguments;
		}
		else
		{
			stdout_str = "<table border='1' Width = '100%'>" + stdout_str + "</table>";
			stdout_str = ReplaceTT(stdout_str);
			stdout_str = stdout_str.Replace(" +0300", "");
		}

		return stdout_str;
	}
	[WebMethod(EnableSession = true)]
	public string GetFile(string filename)
	{
		filename = HttpUtility.UrlDecode(filename);
		filename = filename.Replace("\"", "");
		if (string.IsNullOrEmpty(filename))
			return "Failed to load file";
		try
		{
			return File.ReadAllText(filename);
		}
		catch (Exception ex)
		{
			return ex.ToString();
		}
	}
	public class Machine
	{
		public string Name;
		public string Current;
		public string Started;
		public string Pcping;
		public string Version;
		public string Pausedby;
		public string Tests;
	}
	public class Host
	{
		public string Name;
		public string IP;
		public string MAC;
		public string Info;
		public string Pcping;
		public string Started;
		public string List;
	}
	private Int64 GetTime(DateTime dt)
	{
		Int64 retval = 0;
		var st = new DateTime(1970, 1, 1);
		TimeSpan t = (dt.ToUniversalTime() - st);
		retval = (Int64)(t.TotalMilliseconds + 0.5);
		return retval;
	}
	[WebMethod(EnableSession = true)]
	public Machine[] GetMachines()
	{
		string sql = @"
             SELECT 
					 M.[PCNAME]
					,M.[CURRENT]
					,M.[STARTED]
					,M.[PCPING]
					,M.[PAUSEDBY]
					,V.[VERSION]
					,P.[USER_LOGIN]
					, (SELECT COUNT(*) FROM SCHEDULE S WHERE S.LOCKEDBY = M.ID) [TESTS]
            FROM 
	            PCS M 
            LEFT JOIN FIPVERSION V ON M.[VERSION] = V.[ID]
				LEFT JOIN [PERSONS] P ON M.[PAUSEDBY] = P.[ID]
            WHERE 
	            M.[UNUSED] = 0 
            ORDER BY 
	            M.[PCNAME]
            ";

		List<Machine> ms = new List<Machine>();

		using (DataSet ds = CbstHelper.GetDataSet(sql))
		{
			foreach (DataRow dr in ds.Tables[0].Rows)
			{
				Machine m = new Machine();
				m.Name = dr["PCNAME"].ToString();
				m.Current = dr["CURRENT"].ToString();
				m.Started = dr["STARTED"] == DBNull.Value ? "" : GetTime(Convert.ToDateTime(dr["STARTED"])).ToString();
				m.Pcping = dr["PCPING"] == DBNull.Value ? "" : GetTime(Convert.ToDateTime(dr["PCPING"])).ToString();
				m.Version = dr["VERSION"].ToString();
				m.Pausedby = dr["USER_LOGIN"].ToString();
				m.Tests = dr["TESTS"].ToString();
				ms.Add(m);
			}
		}
		return ms.ToArray();
	}
	[WebMethod(EnableSession = true, CacheDuration = 10)]
	public string GetLastRun()
	{
		string res = "";

		using (DataSet ds1 = CbstHelper.GetDataSet("SELECT MAX(P.LAST_UPDATED) M FROM PCS P"))
		{
			using (DataSet ds2 = CbstHelper.GetDataSet("SELECT MAX(R.ID) M FROM  TESTRUNS R"))
			{
				foreach (DataRow dr1 in ds1.Tables[0].Rows)
				{
					foreach (DataRow dr2 in ds2.Tables[0].Rows)
					{
						res += dr2["m"].ToString();
					}
					res += dr1["m"].ToString();
				}
			}
		}
		return res;
	}
	[WebMethod(EnableSession = true, CacheDuration = 10)]
	public string GetLastLog()
	{
		string res = "";
		using (DataSet ds = CbstHelper.GetDataSet("SELECT MAX(R.ID) M FROM BSTLOG R"))
		{
			foreach (DataRow dr1 in ds.Tables[0].Rows)
			{
				res += dr1["M"].ToString();
			}
		}
		return res;
	}
	[WebMethod(EnableSession = true, CacheDuration = 10)]
	public string[] GetLog()
	{
		return CbstHelper.GetLastLog().ToArray();
	}
	[WebMethod(EnableSession = true)]
	public void FeedLog(string str)
	{
		CbstHelper.FeedLog(str);
	}
	[WebMethod(EnableSession = true)]
	public void ChangeState(string machine)
	{
		string update = string.Format(@"
            UPDATE PCS 
            SET 
	            PAUSEDBY = CASE WHEN PAUSEDBY IS NULL THEN (SELECT ID FROM PERSONS WHERE USER_LOGIN = '{0}') ELSE NULL END
            WHERE 
            PCNAME = '{1}'
        ", CbstHelper.UserName, machine);
		CbstHelper.SQLExecute(update);

		object o = CbstHelper.GetValue(string.Format("SELECT PAUSEDBY FROM PCS WHERE PCNAME = '{0}'", machine));
		if (o == DBNull.Value)
		{
			CbstHelper.FeedLog(string.Format("Machine '{0}' has been resumed", machine));
		}
		else
		{
			CbstHelper.FeedLog(string.Format("Machine '{0}' has been paused", machine));
		}
	}
	public void UpdateMachineCommand(string machine, CbstHelper.ThosterStatus com, string comment)
	{
		object o = CbstHelper.GetValue("SELECT ACTIONFLAG FROM PCS WHERE PCNAME='" + machine + "'");
		int iThosterStatus = 0;
		if (o != DBNull.Value)
		{
			iThosterStatus = Convert.ToInt32(o);
		}
		CbstHelper.ThosterStatus ActionFlag = (CbstHelper.ThosterStatus)iThosterStatus;
		if ((ActionFlag & com) == 0)
		{
			ActionFlag |= com;
			CbstHelper.SQLExecute("DELETE FROM SCHEDULE WHERE LOCKEDBY = (SELECT P.ID FROM PCS P WHERE P.PCNAME = '" + machine + "');");
			CbstHelper.SQLExecute("UPDATE PCS SET ACTIONFLAG = " + (int)ActionFlag + " WHERE PCNAME = '" + machine + "';");
		}
		CbstHelper.FeedLog(comment);
	}
	[WebMethod(EnableSession = true)]
	public void StopMachine(string machine)
	{
		UpdateMachineCommand(machine, CbstHelper.ThosterStatus.Stop, string.Format("Machine '{0}' has been stopped", machine));
	}
	[WebMethod(EnableSession = true)]
	public void ShutMachine(string machine)
	{
		UpdateMachineCommand(machine, CbstHelper.ThosterStatus.Shutdown, string.Format("Machine '{0}' has been shut down", machine));
	}
	[WebMethod(EnableSession = true)]
	public void GetGit(string machine)
	{
		UpdateMachineCommand(machine, CbstHelper.ThosterStatus.SSGET, string.Format("Machine '{0}' has been updated with the latest Git repository code: PULL.", machine));
	}
	[WebMethod(EnableSession = true)]
	public void Restart(string machine)
	{
		UpdateMachineCommand(machine, CbstHelper.ThosterStatus.Restart, string.Format("Machine '{0}' has been restarted.", machine));
	}
	[WebMethod(EnableSession = true)]
	public Host[] GetHosts()
	{
		string sql = @"
            SELECT H.[NAME]
                  ,[IP]
                  ,[MAC]
                  ,[SYSTEMINFO]
                  ,[PCPING]
	              ,[STARTED]
	              ,(SELECT P.PCNAME + ', ' AS 'data()' FROM PCS P WHERE P.HOST_ID = H.ID FOR XML PATH('')) LIST
            FROM [BST_STATISTICS].[DBO].[HOSTS] H
            WHERE INACTIVE IS NULL
            ORDER BY H.NAME
            ";
		List<Host> ms = new List<Host>();

		using (DataSet ds = CbstHelper.GetDataSet(sql))
		{
			foreach (DataRow dr in ds.Tables[0].Rows)
			{
				Host m = new Host();
				m.Name = dr["NAME"].ToString();
				m.IP = dr["IP"].ToString();
				m.MAC = dr["MAC"].ToString();
				m.Info = dr["SYSTEMINFO"].ToString();
				m.Pcping = dr["PCPING"] == DBNull.Value ? "" : GetTime(Convert.ToDateTime(dr["PCPING"])).ToString();
				m.Started = dr["STARTED"] == DBNull.Value ? "" : GetTime(Convert.ToDateTime(dr["STARTED"])).ToString();
				m.List = dr["LIST"].ToString();
				ms.Add(m);
			}
		}
		return ms.ToArray();
	}
	public void UpdateHost(string host, bool start, string comment)
	{
		BstHost h = new BstHost(host);
		if (start)
		{
			h.POWERON = true;
		}
		else
		{
			h.POWEROFF = true;
		}
		h.Store();
		CbstHelper.FeedLog(comment);
	}
	[WebMethod(EnableSession = true)]
	public void StartHost(string host)
	{
		UpdateHost(host, true, string.Format("Machine '{0}' has been launched using web host control", host));
	}
	[WebMethod(EnableSession = true)]
	public void StopHost(string host)
	{
		UpdateHost(host, false, string.Format("Machine '{0}' has been stopped using web host control", host));
	}
	[WebMethod(EnableSession = true)]
	public string StopSequence(string SEQUENCEGUID, string ThosterID)
	{
		using (DataSet ds = CbstHelper.GetDataSet(string.Format(@"
                SELECT 
	                S.[COMMAND]
	                ,R.[TTID]
	                ,P.[PCNAME]
                FROM 
	                [SCHEDULE] S 
                LEFT JOIN [PCS] P ON P.[ID] = S.[PCID]
                LEFT JOIN [TESTREQUESTS] R ON R.[ID] = S.[REQUESTID]
                WHERE 
	            S.[SEQUENCEGUID] = '{0}'
            ", SEQUENCEGUID)))
		{
			string message = "Sequence has been stopped: ";
			string machine = "";
			string ttid = "";
			foreach (DataRow dr in ds.Tables[0].Rows)
			{
				message += dr["COMMAND"].ToString() + ",";
				machine = dr["PCNAME"].ToString();
				ttid = dr["TTID"].ToString();
			}

			message = message.Remove(message.Length - 1);
			CbstHelper.FeedLog(message);
			CbstHelper.FeedLog("Request has been changed: " + ttid);
			if (!string.IsNullOrEmpty(machine))
			{
				CbstHelper.FeedLog("Machine was affected: " + machine);
			}

			CbstHelper.SQLExecute("DELETE FROM SCHEDULE WHERE SEQUENCEGUID = '" + SEQUENCEGUID + "'");
			CbstHelper.SQLExecute("update PCS set ACTIONFLAG = 2 where ID = " + ThosterID);
		}
		return "OK";
	}
	[WebMethod(EnableSession = true)]
	public void DelM(string machine)
	{
		BstHost h = new BstHost(machine) { INACTIVE = true };
		h.Store();
		CbstHelper.FeedLog("Machine was deleted: " + machine);
	}
	[WebMethod(EnableSession = true)]
	public string IgnoreTests(string commaSeparatedIDs)
	{
		try
		{
			CbstHelper.IgnoreTestRun(commaSeparatedIDs);
		}
		catch (Exception e)
		{
			return e.ToString();
		}
		return "OK";
	}
	[WebMethod(EnableSession = true)]
	public string CommentTests(string commaSeparatedIDs, string mess)
	{
		try
		{
			CbstHelper.CommentTestRun(commaSeparatedIDs, mess);
		}
		catch (Exception e)
		{
			return e.ToString();
		}
		return "OK";
	}
	[WebMethod(EnableSession = true)]
	public string VerifyTests(string commaSeparatedIDs)
	{
		try
		{
			CbstHelper.VerifyTestRun(commaSeparatedIDs);
		}
		catch (Exception e)
		{
			return e.ToString();
		}
		return "OK";
	}
	[WebMethod(EnableSession = true)]
	public void IgnoreRequest(string id, string str1or0)
	{
		CbstHelper.IgnoreRequest(id, str1or0);
	}
	[WebMethod(EnableSession = true)]
	public void UntestRequest(string id)
	{
		CbstHelper.UntestRequest(id);
	}
	[WebMethod(EnableSession = true)]
	public string GetCommands(string strSearch)
	{
		strSearch = strSearch.ToUpper();
		string strResult = "";
		TestCommands tc = new TestCommands();
		int i = 0;
		string[] arrTests = Regex.Split(tc.CMD, "\r\n");
		foreach (string strTest in arrTests)
		{
			if (string.IsNullOrEmpty(strTest))
			{
				continue;
			}
			if (strTest.ToUpper().IndexOf(strSearch) >= 0)
			{
				string strCoolText = strTest.Replace(strSearch, "<b>" + strSearch + "</b>");
				strCoolText = strCoolText.Replace(strSearch.ToLower(), "<b>" + strSearch.ToLower() + "</b>");
				strResult += "<tr><td>" + strCoolText + "</td><td><a title=\"Add test\" style=\"color: Blue; cursor: pointer;\" onclick=\"AddTest('" + strTest.Replace('"', '`') + "');\"> Add</a></td></tr> ";
				i++;
			}
		}
		if (i == 0)
		{
			strResult = "Please click <a href=\"commands.aspx\" >here</a> if you need to add tests";
		}
		else
		{
			strResult = "<table width='100%' border=0>" + strResult + "</table>";
		}
		strResult = "<input align='right' src='IMAGES/power.GIF' style='border-width:0px;' type='image'  onclick=\"Div_Off()\" >" + strResult;
		return strResult;
	}
	[WebMethod(EnableSession = true)]
	public string GetTestRunUrl(string id)
	{
		TestRun tr = new TestRun(id);
		return tr.TestURL;
	}
	[WebMethod(EnableSession = true)]
	public string SetBatchData(string id, string text)
	{
		Batch b = new Batch(id);
		b.BATCH_DATA = text;
		b.Store();
		CbstHelper.FeedLog("Batch script has been changed: " + id);
		return "OK";
	}
}