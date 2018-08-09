using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Specialized;
using System.Configuration;
using System.Web.UI.HtmlControls;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
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
		var link = new HtmlLink();
		link.Attributes.Add("type", "text/css");
		link.Attributes.Add("rel", "stylesheet");
		link.Href = ResolveClientUrl("CSS/BST.css");
		Header.Controls.Add(link);
		UpdateControls(Page.Controls);
	}
	public string ReplaceTT(string strTT)
	{
		return Regex.Replace(strTT, "TT\\d+", TTEvaluator);
	}
	private static string TTEvaluator(Match match)
	{
		string res = match.Groups[0].Value;
		string id = Convert.ToInt32(res.Replace("TT", "")).ToString();
		return string.Format("<a href='http://{0}/taskmanagerbeta/showtask.aspx?ttid={1}'>{2}</a>", BSTStat.mainName, id, res);
	}

	//security
	private string m_LoginwerrMsg;
	public string LoginErrorMsg
	{
		get { return m_LoginwerrMsg; }
		set { m_LoginwerrMsg = value; }
	}
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
		if (IsUserActive)
			return;

		string strURL = Request.ServerVariables["URL"];
		if (strURL.Contains(LoginPage))
			return;

		HttpCookie cookieUser = Request.Cookies[UserKey];
		HttpCookie cookiePass = Request.Cookies[PassKey];
		string strUser = cookieUser == null ? "" : cookieUser.Value;
		string strPass = cookiePass == null ? "" : cookiePass.Value;

		if (string.IsNullOrEmpty(strUser) || string.IsNullOrEmpty(strPass))
		{
			Response.Redirect(ResolveClientUrl(LoginPage) + "?" + BSTStat.returnurl + "=" + strURL, false);
			Context.ApplicationInstance.CompleteRequest();
			return;
		}
		strUser = Decrypt(strUser);
		strPass = Decrypt(strPass);
		if (!Login(strUser, strPass, true))
		{
			Response.Redirect(ResolveClientUrl(LoginPage) + "?" + BSTStat.returnurl + "=" + strURL, false);
			Context.ApplicationInstance.CompleteRequest();
			return;
		}
	}
	public bool Login(string sUserName, string Password, bool bStoreUserPass)
	{
		BackgroundWorker.Init();
		bool bRes = false;
		Application.Lock();
		try
		{
			bRes = FLogin(sUserName, Password, bStoreUserPass);
			UserName = sUserName;
		}
		finally
		{
			Application.UnLock();
		}
		return bRes;
	}
	public static bool IsConnected
	{
		get { return HttpContext.Current.Session["UserName"] != null; }
	}
	public static string GitUser
	{
		get { return UserName.Split('@')[0]; }
	}
	public static string UserName
	{
		get { return HttpContext.Current.Session["UserName"].ToString(); }
		set { HttpContext.Current.Session["UserName"] = value; }
	}
	public static string UserID
	{
		get { return HttpContext.Current.Session["UserID"].ToString(); }
		set { HttpContext.Current.Session["UserID"] = value; }
	}
	public string UserPass
	{
		get { return Session["UserPass"].ToString(); }
		set { Session["UserPass"] = value; }
	}
	public string UserLabel
	{
		get { return Session["UserLabel"].ToString(); }
		set { Session["UserLabel"] = value; }
	}
	public bool IsUserAdmin
	{
		get { return Session["IsUserAdmin"] != null; }
		set { if (value) Session["IsUserAdmin"] = value; }
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

					IsUserAdmin = rowCur.Field<System.Boolean>(2);
					IsUserGuest = rowCur.Field<System.Boolean>(3);
					UserLabel = rowCur[4].ToString();
					UserID = rowCur[5].ToString();

					if (!IsInternalIP() && !IsUserAdmin)
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
		string sql = string.Format("INSERT INTO BSTLOG ([TEXT],[USERID]) VALUES ('{0}', (SELECT P.ID FROM PERSONS P WHERE P.USER_LOGIN = '{1}'))", str, UserName);
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
	public static void GetCommandsGroups(string text, out string[] Commands, out string[] arrGroup)
	{
		Commands = Regex.Split(text.Replace("'", "''"), "\r\n");
		arrGroup = new string[Commands.Length];

		for (int i = 0; i < Commands.Length; i++) // Loop through List with for
		{
			if (Commands[i].IndexOf('{') != -1)
			{
				Commands[i] = Commands[i].Replace("{", "");
				string StrGuid = Guid.NewGuid().ToString();
				int j;

				for (j = i; (j < Commands.Length) && (Commands[j].IndexOf('}') == -1); j++) // Loop through List with for
				{
					arrGroup[j] = StrGuid;
				}

				String strComandPlus = "";

				if (j < Commands.Length)
				{
					arrGroup[j] = StrGuid;

					int index = Commands[j].IndexOf("}");
					if ((index > -1) && (index + 1 < Commands[j].Length))
					{
						strComandPlus = Commands[j].Substring(index + 1, Commands[j].Length - index - 1);
					}
					Commands[j] = Commands[j].Replace("}" + strComandPlus, "");
				}

				if (strComandPlus != "")
					for (int k = i; k <= j; k++)
						if (!(string.IsNullOrEmpty(Commands[k]))) Commands[k] += " " + strComandPlus;
				i = j;
			}
		}
	}
	public static string GetParam(String strCommand, string strSearch)
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
	public static void ExecRequestSQL(string[] Commands, string[] arrGroup, string RequestID, string strUserID, string priority)
	{
		int i = -1;
		int K = -1;
		string strSearch = "PCNAME:";
		String strSetSQL = @"INSERT INTO SCHEDULE (COMMAND, REQUESTID, PCID, USERID, PRIORITY, SEQUENCENUMBER, SEQUENCEGUID, DBTYPE, Y3DV) VALUES ";

		foreach (string strCommand in Commands)
		{
			i++;

			if (string.IsNullOrEmpty(strCommand))
				continue;
			K++;

			String dbtype = GetParam(strCommand, "dbtype:");
			dbtype = (dbtype == "" ? "NULL" : "'" + dbtype + "'");

			String y3dv = GetParam(strCommand, "special:");
			y3dv = (y3dv.Contains("3DV") ? "1" : "NULL");

			// Find PCName Substring
			string sPCName = "NULL";
			int index = strCommand.ToUpper().IndexOf(strSearch.ToUpper());
			if (index > -1)
			{
				int indexEnd = Math.Min(strCommand.IndexOf("\"", index) > 0 ? strCommand.IndexOf("\"", index) : strCommand.Length, strCommand.IndexOf(" ", index) > 0 ? strCommand.IndexOf(" ", index) : strCommand.Length);
				sPCName = strCommand.Substring(index + strSearch.Length, indexEnd - index - strSearch.Length).ToUpper();
			}

			// Get SQL PCName
			String strSQLPCName = "";
			if (sPCName == "NULL") strSQLPCName = sPCName;
			else strSQLPCName = "(select T1.ID from PCS T1 where T1.PCNAME = '" + sPCName + "')";
			String strGuid = (string.IsNullOrEmpty(arrGroup[i]) ? Guid.NewGuid().ToString() : arrGroup[i]);

			strSetSQL += (K != 0 ? "," : "") + "('" + strCommand + "', " + RequestID + "," + strSQLPCName + "," + strUserID + ", " + priority + "," + i.ToString() + ", '" + strGuid + "', " + dbtype + ", " + y3dv + ")";
		}
		if (K != -1)
			SQLExecute(strSetSQL);
	}
	public static void RunBatch4Request(string user, string batchname, string requestGUID, string priority)
	{
		TestRequest tr = new TestRequest("", requestGUID);
        string text = ParseChildBatches(strInput: (new Batch("", batchname)).BATCH_DATA);

        string[] Commands, arrGroup;
        GetCommandsGroups(text, out Commands, out arrGroup);
		ExecRequestSQL(Commands, arrGroup, tr.ID.ToString(), (new BSTUser("", user)).ID, priority);

        tr.REQUEST_PRIORITY = Convert.ToInt32(priority);
        tr.USERID = (new BSTUser("", "bst")).ID;
        tr.Store();
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
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
	public static void SQLExecuteTestTrackDB(string strSQL)
	{
		using (OleDbConnection conn = new OleDbConnection("Provider=sqloledb;Data Source=192.168.0.1;Initial Catalog=tt_res;Trusted_Connection=False;User ID=sa;Password=prosuite"))
		{
			conn.Open();
			using (OleDbCommand cmd = new OleDbCommand(strSQL, conn))
				cmd.ExecuteNonQuery();
		}
	}
	
	public static void IgnoreTestRun(string ids)
	{
		string[] sids = ids.Split(',');
		foreach (string id in sids)
		{
			TestRun tr = new TestRun(id) { USERID = UserID, COMMENT = "ignored", IGNORE = true.ToString() };
			tr.Store();
		}
		CbstHelper.FeedLog("Following tests where marked as ignored: " + ids);
	}
	public static void CommentTestRun(string ids, string comment)
	{
		string[] sids = ids.Split(',');
		foreach (string id in sids)
		{
			TestRun tr = new TestRun(id) { USERID = UserID, COMMENT = comment };
			tr.Store();
		}
		CbstHelper.FeedLog("Following tests where commented: " + ids);
	}
	public static void VerifyTestRun(string ids)
	{
		string[] sids = ids.Split(',');
		foreach (string id in sids)
		{
			TestRun tr = new TestRun(id) { USERID = UserID, COMMENT = "verified", VERIFIED_USER_ID = UserID };
			tr.Store();
		}
		CbstHelper.FeedLog("Following tests where marked as verified: " + ids);
	}
	public static void IgnoreRequest(string id, string str1or0)
	{
		bool ignore = str1or0 == "1";
		TestRequest tr = new TestRequest(id) { USERID = UserID, IGNORE = ignore ? "true" : "" };
		tr.Store();

		if (!ignore)
			return;

		string emal = tr.PROGABB;
		string ttid = tr.TTID;
		string comm = tr.COMMENT;

		Version v = new Version(tr.VERSIONID);
		string vers = v.VERSION;

		string body = string.Format(@"
		Your request to test version ({0}) was processed by {1}<br>
		Your version: <a href='http://{2}/runs.aspx?R.RequestID={3}'>{0}</a> will be <b>IGNORED</b><br>
		{4}<br>
		{5}<br><br>
		Person responsible: <b>{1}</b><br><br>
		Best regards, {1}
		", vers, UserName, BSTStat.newBSTAddress, id, ttid, comm);
		AddEmail(
				emal
				,string.Format("Your request to test version({0}) was processed by {1}", vers, UserName)
				,body
				,IgnoreTT);
	}
	public static void UntestRequest(string id)
	{
		TestRequest tr = new TestRequest(id){USERID = UserID,TESTED = ""};
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
			strProgrammer += "@resnet.com";
		mail.To.Add(new MailAddress(strProgrammer));
		mail.To.Add(new MailAddress("BST@resnet.com"));
		mail.From = new MailAddress("bst_tester@resnet.com");
		mail.Subject = Subject;
		mail.IsBodyHtml = true;

		string body = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
		body += "<HTML><HEAD><META http-equiv=Content-Type content=\"text/html; charset=iso-8859-1\">";
		body += "</HEAD><BODY bgcolor='" + strColor + "'>" + Body + " </BODY></HTML>";

		System.Net.Mime.ContentType mimeType = new System.Net.Mime.ContentType("text/html");
		AlternateView alternate = AlternateView.CreateAlternateViewFromString(body, mimeType);
		mail.AlternateViews.Add(alternate);

		SmtpClient smtp = new SmtpClient();
		smtp.Host = "Smtp.Gmail.com";
		smtp.Port = 587;
		smtp.EnableSsl = true;
		smtp.Timeout = 10000;
		smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
		smtp.UseDefaultCredentials = false;
		smtp.Credentials = new NetworkCredential("resfieldpro@Gmail.com", "mentor2000");

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