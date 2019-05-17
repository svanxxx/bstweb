using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Drawing;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Web.UI.WebControls;
using System.Net;
using BSTStatics;
using System.DirectoryServices.AccountManagement;

public class CbstHelper : System.Web.UI.Page
{
	[Flags]
	public enum ThosterStatus
	{
		Start = 1,
		Stop = 2,
		Install = 4,
		Release = 8,
		Restart = 16,
		SSGET = 32,
		Shutdown = 64
	}

	public string CurrentPageName
	{
		get
		{
			System.IO.FileInfo oInfo = new System.IO.FileInfo(Request.Url.AbsolutePath);
			return oInfo.Name;
		}
	}

	//default colors:
	public Color ErrColor
	{ get { return Color.DeepPink; } }
	public Color WarColor
	{ get { return Color.Yellow; } }
	public Color OutColor
	{ get { return Color.Coral; } }
	public Color DBEColor
	{ get { return Color.Firebrick; } }
	public Color ExcColor
	{ get { return Color.Red; } }

	// result mail color
	public static string GoodTT { get { return "#98FF98"; } }
	public static string BadTT { get { return "#C24641"; } }
	public static string IgnoreTT { get { return "#FFDB58"; } }

	//visualization
	protected void UpdateControls(System.Web.UI.ControlCollection WorkControls)
	{
		int total = WorkControls.Count;
		for (int i = 0; i < total; i++)
		{
			System.Web.UI.Control c = WorkControls[i] as System.Web.UI.Control;
			System.Web.UI.WebControls.Button b = WorkControls[i] as System.Web.UI.WebControls.Button;
			if (b != null && string.IsNullOrEmpty(b.CssClass))
			{
				b.CssClass = "button";
			}
			if (c != null)
			{
				UpdateControls(c.Controls);
			}
		}
	}
	protected void MPage_Load(object sender, EventArgs e)
	{
//		var link = new HtmlLink();
//		link.Attributes.Add("type", "text/css");
//		link.Attributes.Add("rel", "stylesheet");
//		link.Href = ResolveClientUrl("CSS/BST.css");
//		Header.Controls.Add(link);
//		UpdateControls(Page.Controls);
	}
	public string LoginErrorMsg { get; set; }
	private string UserKey
	{
		get
		{
			return "A77B0234-6914-491c-AFEE-F11E5BE30DFA";
		}
	}
	private string PassKey
	{
		get
		{
			return "BB6178D3-78D2-4a87-869E-7ACB7C02E1A1";
		}
	}
	public string LoginPage
	{
		get { return "Login.aspx"; }
	}
	protected CbstHelper()
	{
		PreInit += new EventHandler(InitSecurityCheck);
		PreRenderComplete += new EventHandler(MPage_Load);
	}
	private void InitSecurityCheck(object sender, EventArgs e)
	{
		SecurityPage.Static_Page_PreInit();
	}
	public bool Login(string sUserName, string Password, bool bStoreUserPass)
	{
		bool bRes = false;
		Application.Lock();
		try
		{
			bRes = FLogin(sUserName, Password, bStoreUserPass);
		}
		finally
		{
			Application.UnLock();
		}
		return bRes;
	}
	public static bool IsConnected
	{
		get { return CurrentContext.Valid; }
	}
	public static string GitUser
	{
		get { return CurrentContext.UserLogin().Split('@')[0]; }
	}
	public bool IsUserGuest
	{
		get { return Session["IsUserGuest"] != null; }
		set { if (value) Session["IsUserGuest"] = value; }
	}
	public bool IsUserActive
	{
		get { return Session["IsUserActive"] != null; }
		set { if (value) Session["IsUserActive"] = value; }
	}
	private bool FLogin(string UserName, string Password, bool bStoreUserPass)
	{
		bool bdomain = UserName.Contains("@");
		if (bdomain)
		{
			bool valid = false;
			string dispUserName = UserName;
			using (PrincipalContext context = new PrincipalContext(ContextType.Domain, "mps"))
			{
				valid = context.ValidateCredentials(UserName, Password);
				if (valid)
				{
					var usr = UserPrincipal.FindByIdentity(context, UserName);
					if (usr != null)
						dispUserName = usr.GivenName + " " + usr.Surname;
				}
			}
			if (!valid)
			{
				LoginErrorMsg = "Domain user was not validated by domain controller.";
				return false;
			}
			object id = GetValue(string.Format("SELECT ID FROM PERSONS P WHERE UPPER(P.USER_LOGIN) = UPPER('{0}')", UserName));
			if (id == null)
			{
				SQLExecute(string.Format("INSERT INTO PERSONS (USER_NAME, USER_LOGIN, USER_PASS, IS_ADMIN, IS_GUEST) VALUES ('{0}', '{1}', '', 0, 0)", dispUserName, UserName));
			}
		}
		using (DataSet ds = GetDataSet(string.Format("SELECT USER_LOGIN, USER_PASS, IS_ADMIN, IS_GUEST, USER_NAME, ID FROM PERSONS WHERE UPPER(USER_LOGIN) = '{0}'", UserName.ToUpper())))
		{
			foreach (DataRow rowCur in ds.Tables[0].Rows)
			{
				bool passvalid = bdomain ? true : rowCur[1].ToString().ToUpper() == Password.ToUpper();
				if (rowCur[0].ToString().ToUpper() == UserName.ToUpper() && passvalid)
				{
					if (bStoreUserPass)
					{
						HttpCookie cookieUser = new HttpCookie(UserKey);
						HttpCookie cookiePass = new HttpCookie(PassKey);
						cookieUser.Value = Encrypt(UserName);
						cookiePass.Value = Encrypt(Password);
						cookiePass.Expires = cookieUser.Expires = DateTime.Now.AddYears(1);
						Response.Cookies.Add(cookiePass);
						Response.Cookies.Add(cookieUser);
					}

					IsUserGuest = rowCur.Field<System.Boolean>(3);

					if (!IsInternalIP() && !CurrentContext.Admin)
						LoginErrorMsg = "Only Administrators can login using external IP";
					else
						IsUserActive = true;

					return true;
				}
			}
			LoginErrorMsg = @"User name or password is invalid";
		}
		return false;
	}
	public static string Decrypt(string cipherString/*, bool useHashing = true*/)
	{
		bool useHashing = true;
		//http://www.codeproject.com/KB/cs/Cryptography.aspx
		byte[] keyArray;
		byte[] toEncryptArray = Convert.FromBase64String(cipherString);

		System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
		string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));

		if (useHashing)
		{
			MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
			keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
			hashmd5.Clear();
		}
		else
		{
			keyArray = UTF8Encoding.UTF8.GetBytes(key);
		}

		TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
		tdes.Key = keyArray;
		tdes.Mode = CipherMode.ECB;
		tdes.Padding = PaddingMode.PKCS7;

		ICryptoTransform cTransform = tdes.CreateDecryptor();
		byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
		tdes.Clear();
		return UTF8Encoding.UTF8.GetString(resultArray);
	}
	public static string Encrypt(string toEncrypt/*, bool useHashing*/)
	{
		bool useHashing = true;
		//http://www.codeproject.com/KB/cs/Cryptography.aspx
		byte[] keyArray;
		byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

		AppSettingsReader settingsReader = new AppSettingsReader();

		string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
		if (useHashing)
		{
			MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
			keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
			hashmd5.Clear();
		}
		else
			keyArray = UTF8Encoding.UTF8.GetBytes(key);

		TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
		tdes.Key = keyArray;
		tdes.Mode = CipherMode.ECB;

		tdes.Padding = PaddingMode.PKCS7;

		ICryptoTransform cTransform = tdes.CreateEncryptor();
		byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0,
		  toEncryptArray.Length);
		tdes.Clear();
		return Convert.ToBase64String(resultArray, 0, resultArray.Length);
	}  //database
	protected bool IsInternalIP()
	{
		string UserIP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
		if (string.IsNullOrEmpty(UserIP))
			UserIP = Request.ServerVariables["REMOTE_ADDR"];

		return (UserIP == "127.0.0.1" /*localhost*/) || UserIP.StartsWith("192.168.0");
	}

	//database
	public static string ConnString
	{
		get
		{
			string strServer = System.Configuration.ConfigurationManager.AppSettings["BST_DB"];
			return @"Provider=sqloledb;Data Source=" + strServer + @";Initial Catalog=BST_STATISTICS;Trusted_Connection=False;User ID=sa;Password=prosuite";
		}
	}
	public static OleDbConnection NewConnection
	{
		get { return new OleDbConnection(ConnString); }
	}
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
	public static DataTable GetDataTable(string strSQL)
	{
		using (DataSet ds = new DataSet())
		{
			using (OleDbConnection conn = NewConnection)
			{
				conn.Open();
				using (OleDbDataAdapter adapter = new OleDbDataAdapter(strSQL, conn))
				{
					adapter.Fill(ds);
				}
			}
			return ds.Tables[0];
		}
	}
	public static DataSet GetDataSet(string strSQL)
	{
		DataSet ds = new DataSet();
		using (OleDbConnection conn = NewConnection)
		{
			conn.Open();
			using (OleDbDataAdapter adapter = new OleDbDataAdapter(strSQL, conn))
				adapter.Fill(ds);
		}
		return ds;
	}
	public static object GetValue(string strSQL)
	{
		using (DataSet ds = GetDataSet(strSQL))
		{
			if (ds.Tables.Count == 0)
				return null;
			if (ds.Tables[0].Rows.Count == 0)
				return null;
			if (ds.Tables[0].Columns.Count == 0)
				return null;
			return ds.Tables[0].Rows[0][0];
		}
	}
	public static DataRow GetValues(string strSQL)
	{
		using (DataSet ds = GetDataSet(strSQL))
		{
			if (ds.Tables.Count == 0)
				return null;
			if (ds.Tables[0].Rows.Count == 0)
				return null;
			if (ds.Tables[0].Columns.Count == 0)
				return null;
			return ds.Tables[0].Rows[0];
		}
	}
	protected string _GetLastRepID()
	{
		DataSet ds = GetDataSet("SELECT MAX(TESTRUNS.ID) FROM TESTRUNS");
		string strRes = ds.Tables[0].Rows[0].Field<Int32>(0).ToString();
		ds.Dispose();
		return strRes;
	}
	public static List<string> GetLastLog()
	{
		DataSet ds = GetDataSet(@"
        SELECT TOP(100)
	        B.[DATETIME]
	        ,P.USER_LOGIN
	        ,B.[TEXT]
        FROM
            [BSTLOG] B
        LEFT JOIN PERSONS P ON P.ID = B.USERID
        ORDER BY B.ID DESC
        ");
		List<string> ls = new List<string>();
		foreach (DataRow dr in ds.Tables[0].Rows)
		{
			ls.Add(Convert.ToDateTime(dr["DATETIME"]).ToString("HH:mm") + " " + dr["USER_LOGIN"] + ": " + dr["TEXT"]);
		}
		return ls;
	}
	public static void FeedLog(string str)
	{
		str.Trim();
		if (string.IsNullOrEmpty(str))
			return;

		str = str.Replace('\'', '\"');
		string sql = string.Format("INSERT INTO BSTLOG ([TEXT],[USERID]) VALUES ('{0}', (SELECT P.ID FROM PERSONS P WHERE P.USER_LOGIN = '{1}'))", str, CurrentContext.UserLogin());
		SQLExecute(sql);
	}
	protected double GetRepDelay()
	{
		return 1.0 / 24.0 / 60.0; //1 min
	}
	protected void _UpdateAppRepInfo()
	{
		double dCDate = Convert.ToDouble(DateTime.Now.ToOADate());
		Application.Lock();
		try
		{
			Application["REPDATE"] = dCDate.ToString();
			Application["REPID"] = _GetLastRepID();
		}
		catch (System.Exception /*ex*/)
		{

		}
		finally
		{
			Application.UnLock();
		}
	}
	public string GetLastRepID()
	{
		double dCDate = Convert.ToDouble(DateTime.Now.ToOADate());
		string strDate = Application["REPDATE"] as string;
		if (string.IsNullOrEmpty(strDate))
		{
			_UpdateAppRepInfo();
		}
		else
		{
			double dOldDate = Convert.ToDouble(strDate);
			if (Math.Abs((dCDate - dOldDate)) > GetRepDelay())
			{
				_UpdateAppRepInfo();
			}
		}
		return Application["REPID"] as string;
	}
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
	public DataSet GetTableDataSet(string strTableAndCondition)
	{
		DataSet ds = new DataSet();

		using (OleDbConnection conn = NewConnection)
		{
			conn.Open();
			using (OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM " + strTableAndCondition, conn))
				adapter.Fill(ds);
		}

		return ds;
	}
	public string GetMSAccessDate(DateTime dt)
	{
		CultureInfo cultures = CultureInfo.CreateSpecificCulture("en-US");
		return "'" + dt.ToString("yyyy/MM/dd", cultures) + "'";
	}
	public static string ParseChildBatches(string strInput)
	{
		string srtFullBatchParsed = "";
		//char[] BatchSeparatorsOne = {'\r\n'};
		string strSearch = "ChildBatches=";
		string[] BatchArray = strInput.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
		int intBatchIndex = -1;
		string[] BatchListArray;
		char[] BatchSeparators = { ';', ',' };
		foreach (string strRead in BatchArray)
		{
			intBatchIndex = strRead.ToUpper().IndexOf(strSearch.ToUpper());
			if (intBatchIndex >= 0)
			{
				BatchListArray = strRead.Replace(strSearch, "").Replace("\"", "").Replace("'", "''").Split(BatchSeparators);
				foreach (string BatchName in BatchListArray)
				{
					//get full batch from db, recursively!!!
					DataSet ds = GetDataSet("SELECT BATCH_DATA FROM BATCHES WHERE BATCH_NAME = '" + BatchName + "'");
					if (ds.Tables[0].Rows.Count > 0)
					{
						string ChildString = ds.Tables[0].Rows[0][0].ToString();
						srtFullBatchParsed += ParseChildBatches(ChildString);
					}
					//srtFullBatchParsed += Environment.NewLine;
				}
			}
			else
			{
				srtFullBatchParsed += strRead;
				srtFullBatchParsed += Environment.NewLine;
			}
		}
		//srtFullBatchParsed += Environment.NewLine;
		string[] FinalBatch = srtFullBatchParsed.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
		srtFullBatchParsed = "";
		foreach (string strFinal in FinalBatch)
			if (strFinal.Length != 0)
			{
				srtFullBatchParsed += strFinal;
				srtFullBatchParsed += Environment.NewLine;
			}
		return srtFullBatchParsed;
	}
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
	public static void SQLExecute(string strSQL)
	{
		using (OleDbConnection conn = NewConnection)
		{
			conn.Open();
			using (OleDbCommand cmd = new OleDbCommand(strSQL, conn))
			{
				cmd.ExecuteNonQuery();
			}
		}
	}
	public static void IgnoreTestRun(string ids)
	{
		string[] sids = ids.Split(',');
		foreach (string id in sids)
		{
			TestRun tr = new TestRun(id) { USERID = CurrentContext.UserID.ToString(), COMMENT = "ignored", IGNORE = true.ToString() };
			tr.Store();
		}
		CbstHelper.FeedLog("Following tests where marked as ignored: " + ids);
	}
	public static void VerifyTestRun(string ids)
	{
		string[] sids = ids.Split(',');
		foreach (string id in sids)
		{
			TestRun tr = new TestRun(id) { USERID = CurrentContext.UserID.ToString(), COMMENT = "verified", VERIFIED_USER_ID = CurrentContext.UserID.ToString() };
			tr.Store();
		}
		CbstHelper.FeedLog("Following tests where marked as verified: " + ids);
	}
	public static void IgnoreRequest(string id, string str1or0)
	{
		bool ignore = str1or0 == "1";
		TestRequest tr = new TestRequest(id) { USERID = CurrentContext.UserID.ToString(), IGNORE = ignore ? "true" : "" };
		tr.Store();

		if (!ignore)
			return;

		string emal = tr.PROGABB;
		string ttid = tr.TTID;
		string comm = tr.COMMENT;

		Version v = new Version(tr.VERSIONID);
		string vers = v.VERSION;

		//----update TestTrack DB
		string tasknumber = Regex.Match(ttid.ToUpper(), "TT[0-9]+").Value.Replace("TT", "");
		if (!String.IsNullOrEmpty(tasknumber))
		{
			DefectConnector.UpdateDefect(tasknumber, "", CurrentContext.User.PHONE);
		}
		//----update end

		string body = string.Format(@"
		Your request to test version ({0}) was processed by {1}<br>
		Your version: <a href='http://{2}/runs.aspx?R.RequestID={3}'>{0}</a> will be <b>IGNORED</b><br>
		{4}<br>
		{5}<br><br>
		Person responsible: <b>{1}</b><br><br>
		Best regards, {1}
		", vers, CurrentContext.UserName(), Settings.CurrentSettings.BSTADDRESS, id, ttid, comm);
		AddEmail(
				emal
				, string.Format("Your request to test version({0}) was processed by {1}", vers, CurrentContext.UserName())
				, body
				, IgnoreTT);
	}
	public static void UntestRequest(string id)
	{
		TestRequest tr = new TestRequest(id) { USERID = CurrentContext.UserID.ToString(), TESTED = "" };
		tr.Store();
	}
	//helpers
	public System.Web.UI.Control GetPostBackControl(Page page)
	{
		System.Web.UI.Control control = null;
		string ctrlname = page.Request.Params.Get(postEventSourceID);
		if (!string.IsNullOrEmpty(ctrlname))
		{
			control = page.FindControl(ctrlname);
		}
		else
		{
			foreach (string ctl in page.Request.Form)
			{
				System.Web.UI.Control mycontrol = page.FindControl(ctl);
				if (mycontrol is System.Web.UI.WebControls.Button)
				{
					control = mycontrol;
					// This gives you ID of which button caused postback                        
					break;
				}
			}
		}
		return control;
	}
	public string GitDir
	{
		get
		{
			//if run in dev studio
			if (System.Diagnostics.Debugger.IsAttached || string.IsNullOrEmpty(Request.ServerVariables["SERVER_SOFTWARE"]))
			{
				return "S:\\";
			}
			//return Server.MapPath("~") + "\\GIT\\";
			return Directory.GetParent(Server.MapPath("~").TrimEnd('\\')).FullName + "\\GIT\\";
		}

	}
	public string GitDirDev
	{
		get
		{
			//if run in dev studio
			if (System.Diagnostics.Debugger.IsAttached || string.IsNullOrEmpty(Request.ServerVariables["SERVER_SOFTWARE"]))
			{
				return "D:\\V8\\";
			}
			//return Server.MapPath("~") + "\\GITV8\\";
			return Directory.GetParent(Server.MapPath("~").TrimEnd('\\')).FullName + "\\GITV8\\";
		}

	}
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
	public string GitExecute(string strCommand, string strRepo)
	{
		ProcessStartInfo gitInfo = new ProcessStartInfo();
		gitInfo.CreateNoWindow = true;
		gitInfo.RedirectStandardError = true;
		gitInfo.RedirectStandardOutput = true;
		gitInfo.UseShellExecute = false;
		//if run in dev studio
		if (System.Diagnostics.Debugger.IsAttached || string.IsNullOrEmpty(Request.ServerVariables["SERVER_SOFTWARE"]))
		{
			gitInfo.FileName = "git.exe";
		}
		else
		{
			gitInfo.FileName = @"c:\Program Files (x86)\Git\bin\git.exe";
		}
		//

		Process gitProcess = new Process();
		gitInfo.Arguments = strCommand; // such as "fetch orign"
		gitInfo.WorkingDirectory = strRepo;

		gitProcess.StartInfo = gitInfo;
		gitProcess.Start();

		//string stderr_str = gitProcess.StandardError.ReadToEnd();  // pick up STDERR
		string stdout_str = gitProcess.StandardOutput.ReadToEnd(); // pick up STDOUT

		//if (stderr_str != "") { strGit_Err += stderr_str + "<br/>"; }
		//if (stdout_str != "") { strGit_Out += stdout_str + "<br/>"; }

		gitProcess.WaitForExit();
		gitProcess.Close();
		return stdout_str;
	}
	public string[] DB_PutDependentFilesTT(string strTTIDs, string TestRunIDs)
	{
		string[] arrTTID = strTTIDs.Split(',');
		string strFilter = "";
		foreach (string strTT in arrTTID)
		{
			if (string.IsNullOrEmpty(strTT))
				continue;
			strFilter += " --grep=" + strTT;
		}
		GitExecute("pull", GitDirDev);
		string strRes = GitExecute("log " + strFilter + " --pretty=format:\"\" --name-only", GitDirDev);
		string[] arrfiles = strRes.Split('\n');

		DB_PutDependentFilesFromTT(TestRunIDs, arrfiles);

		return arrfiles;
	}
	public void DB_PutDependentFilesFromTT(string TestRunIDs, string[] arrfiles)
	{

		string[] arrTestRunIDs = TestRunIDs.Split(',');

		foreach (string TestRunID in arrTestRunIDs)
		{
			if (string.IsNullOrEmpty(TestRunID)) continue;

			// Check it reqest or release
			DataSet ds = GetDataSet("SELECT TR.ProgAbb FROM TESTREQUESTS TR WHERE TR.ID = (SELECT R.RequestID FROM TESTRUNS R WHERE R.ID =" + TestRunID + ")");
			String strDeveloperName = null;
			if (ds.Tables[0].Rows.Count > 0) strDeveloperName = ds.Tables[0].Rows[0][0].ToString();

			if ((String.IsNullOrEmpty(strDeveloperName)) || (strDeveloperName.ToUpper() == "ADMIN")) // it release
			{

				foreach (string strItem in arrfiles)
				{
					if (string.IsNullOrEmpty(strItem)) continue;

					// Check file exist in CODEFILES table
					ds = GetDataSet("SELECT ID FROM CODEFILES WHERE CODEFILENAME = '" + strItem + "'");

					// if file not exist, create in CODEFILES table
					if (ds.Tables[0].Rows.Count < 1) SQLExecute("INSERT INTO CODEFILES (CODEFILENAME) VALUES ('" + strItem + "')");

					// Get file Id from CODEFILES table
					ds = GetDataSet("SELECT ID FROM CODEFILES WHERE CODEFILENAME = '" + strItem + "'");
					int iFileID = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

					// Add link to file id in CODELINKS table
					ds = GetDataSet("SELECT ID FROM CODELINKS WHERE TEST_RUN_ID = " + TestRunID + " AND CODE_FILE_ID =" + iFileID.ToString());
					if (ds.Tables[0].Rows.Count < 1) SQLExecute("INSERT INTO CODELINKS (TEST_RUN_ID, CODE_FILE_ID) VALUES (" + TestRunID + "," + iFileID.ToString() + ")");
				}
			}
			else // FOR REQEST
			{
				string SQL_Add_Files = @"INSERT INTO CODELINKS (TEST_RUN_ID, CODE_FILE_ID) 
              SELECT $TestRunID, RL.FileID FROM REQUESTLINKS RL 
              where RL.RequestID IN (select TR.RequestID from TESTRUNS TR where TR.ID = $TestRunID) 
              AND 1 NOT IN (SELECT 1 FROM CODELINKS CL where CL.TEST_RUN_ID  = $TestRunID and CL.CODE_FILE_ID = RL.FileID)";
				SQLExecute(SQL_Add_Files.Replace("$TestRunID", TestRunID));
			}

		}
	}
	public static string GetTTfromText(string strText)
	{
		string[] arrTrash = Regex.Split(strText, "TT\\d+");
		foreach (string strTrash in arrTrash)
		{
			if (string.IsNullOrEmpty(strTrash))
				continue;
			strText = strText.Replace(strTrash, ",");
		}

		if (strText == ",") return "";
		if (strText.Length >= 1) if (strText[0] == ',') strText = strText.Remove(0, 1);
		if (strText.Length >= 1) if (strText[strText.Length - 1] == ',') strText = strText.Remove(strText.Length - 1, 1);
		return strText;
	}
	public static void AddEmail(string to, string subject, string body, string color)
	{
		SQLExecute(string.Format("INSERT INTO EMAILS([TO], [SUBJECT], [BODY], [COLOR]) VALUES('{0}', '{1}', '{2}', '{3}')", to, subject.Replace('\'', '\"'), body.Replace('\'', '\"'), color));
	}
	public static string SendEmailToBstTeam(string strProgrammer, string Subject, string Body, string strColor)
	{
		MailMessage mail = new MailMessage();
		if (!strProgrammer.Contains("@"))
			strProgrammer += "@" + Settings.CurrentSettings.TEAMDOMAIN;
		mail.To.Add(new MailAddress(strProgrammer));
		mail.To.Add(new MailAddress("BST@" + Settings.CurrentSettings.TEAMDOMAIN));
		mail.From = new MailAddress("bst_tester@" + Settings.CurrentSettings.TEAMDOMAIN);
		mail.Subject = Subject;
		mail.IsBodyHtml = true;

		string body = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
		body += "<HTML><HEAD><META http-equiv=Content-Type content=\"text/html; charset=iso-8859-1\">";
		body += "</HEAD><BODY bgcolor='" + strColor + "'>" + Body + " </BODY></HTML>";

		System.Net.Mime.ContentType mimeType = new System.Net.Mime.ContentType("text/html");
		AlternateView alternate = AlternateView.CreateAlternateViewFromString(body, mimeType);
		mail.AlternateViews.Add(alternate);

		SmtpClient smtp = new SmtpClient();
		smtp.Host = Settings.CurrentSettings.SMTPHOST;
		smtp.Port = int.Parse(Settings.CurrentSettings.SMTPPORT);
		smtp.EnableSsl = bool.Parse(Settings.CurrentSettings.SMTPENABLESSL);
		smtp.Timeout = int.Parse(Settings.CurrentSettings.SMTPTIMEOUT);
		smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
		smtp.UseDefaultCredentials = false;
		smtp.Credentials = new NetworkCredential(Settings.CurrentSettings.CREDENTIALS1, Settings.CurrentSettings.CREDENTIALS2);

		string strError = "";
		try
		{
			smtp.Send(mail);
		}
		catch (Exception e)
		{
			strError = e.Message;
		}

		return strError;
	}
	protected Boolean isProgramerVersion(String strVersion)
	{

		while ((strVersion.Length > 0) && (strVersion[strVersion.Length - 1] == ' '))
		{
			strVersion = strVersion.Remove(strVersion.Length - 1, 1);
		}

		char chLast = strVersion[strVersion.Length - 1];

		switch (chLast)
		{
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			case '0':

				return false;
		}
		return true;
	}
	protected ImageButton GetPostNewImageButton(CommandEventHandler ButtonClick, String strOnClientClick)
	{
		ImageButton btn = new ImageButton();
		// if (ButtonClick != null) btn.Command += ButtonClick;
		if (strOnClientClick != "") btn.OnClientClick = strOnClientClick;
		// btn.CssClass = "btn";
		return btn;
	}
	protected void UpdatePostNewImageButton(System.Web.UI.Control ctrl, string StrUrl, string strHost, string strCommandLine, String ToolTip)
	{
		WebControl btn = ctrl as WebControl;
		btn.Attributes["commandline"] = strCommandLine;
		btn.Attributes["commandhost"] = strHost;
		if (btn is ImageButton)
		{
			(btn as ImageButton).ImageUrl = StrUrl;
			if (ToolTip != "") (btn as ImageButton).ToolTip = ToolTip;
		}
	}
}