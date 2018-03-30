using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.JScript;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Web.Services;
using System.Data;
using System.Text.RegularExpressions;
using System.Security;

public partial class PutFiles : CbstHelper
{
	string strGit_Err = "", strGit_Out = "";
	public string lstFile
	{
		set
		{
			OriginalData.Value = value;
		}
		get
		{
			return OriginalData.Value;
		}
	}

	DateTime timeNow;
	string strTime = "";

	int m_ID;
	protected int TestID
	{
		get { return m_ID; }
		set { m_ID = value; }
	}

	public List<String> lstParams { get; set; }

	void StartTimer()
	{
		strTime = "";
		timeNow = DateTime.Now;
	}

	void StopTimer(string strText)
	{
		DateTime timeNew = DateTime.Now;
		TimeSpan sp = timeNew - timeNow;
		strTime += strText + " - <b>" + sp.Seconds.ToString() + " sec. </b> <br/>";
		timeNow = DateTime.Now;
	}

	public string GetGitWorkDir(string strPath)
	{
		string strReturn = strPath.Replace(@"s:\", GitDir);
		strReturn = strReturn.Replace(@"S:\", GitDir);
		return strReturn;
	}

	public List<String> GetParam(string strParam)
	{
		int iPos, i, k;
		List<String> lstReturn = new List<String>();

		while (strParam.IndexOf('"') != -1)
		{
			iPos = strParam.IndexOf('"');
			k = 1;
			for (i = iPos + 1; (i < strParam.Length) && (strParam[i] != '"'); i++) { k++; }

			if (k != 1)
			{
				lstReturn.Add(strParam.Substring(iPos + 1, k - 1));
			}
			strParam = strParam.Remove(iPos, k + 1);
		}

		return lstReturn;
	}

	protected void Page_Init(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			string strGuid = Request.Params["Guid"];
			string strTestName = Request.Params["Test_Name"];

			try
			{
				lstFile = HttpContext.Current.Application.Get(strGuid).ToString();
			}
			catch (Exception /*Ecx*/)
			{
				Response.Clear();
				Response.Write("<h2>This page is outdated, please click 'Back' and try again.</h2>");
				Response.End();
			}

			HttpContext.Current.Application.Remove(strGuid);
			lstParams = GetParam(GlobalObject.unescape(lstFile));
			Branch.Text = GetGitBranchName();
			UserMess.Text = UserLabel + " (" + GitUser + "@resnet.com)";
			LabelFile.Text = "File :";
			if (lstParams.Count > 1 * 2) { LabelFile.Text = "Files(" + (lstParams.Count / 2).ToString() + "):"; }

			lstFilesCheckBox.Items.Clear();

			for (int i = 0; i < lstParams.Count; i = i + 2)
			{
				lstFilesCheckBox.Items.Add(lstParams[i] + "<br/>&emsp;<small><font color='green'> " + lstParams[i + 1] + "</font></small>");
			}

			for (int j = 0; j < lstFilesCheckBox.Items.Count; j++)
			{
				lstFilesCheckBox.Items[j].Selected = true;
			}

			if (strTestName != null)
			{
				TextAreaMessage.Text = strTestName + ", etalon update ";
			}
			TextAreaMessage.Focus();
		}
	}

	protected bool CheckCorrectParam()
	{
		UserMess.Visible = false;
		string strRunID = Request.Params["RUNID"];
		Int32 iID;
		if (string.IsNullOrEmpty(strRunID) || !Int32.TryParse(strRunID, out iID))
		{
			TestID = 0;
			return false;
		}
		TestID = iID;
		return true;
	}

	protected void Page_Load(object sender, EventArgs e)
	{

		if (IsUserGuest)
		{
			Response.Clear();
			Response.Write("<h2>You have no permission to use this page.</h2>");
			Response.End();
		}

		if (IsPostBack)
		{
			CheckCorrectParam();
			lstParams = GetParam(GlobalObject.unescape(lstFile));
		}
	}

	string SetGitCommand(string strCommand)
	{
		string stdout_str = string.Empty;
		using (Process gitProcess = new Process())
		{
			ProcessStartInfo gitInfo = new ProcessStartInfo();
			gitInfo.CreateNoWindow = true;
			gitInfo.RedirectStandardError = true;
			gitInfo.RedirectStandardOutput = true;
			gitInfo.UseShellExecute = false;
			gitInfo.FileName = @"c:\Program Files (x86)\Git\bin\git.exe";
			gitInfo.Arguments = strCommand; // such as "fetch origin"
			gitInfo.WorkingDirectory = GitDir;
			stdout_str = strCommand + "<br/>";

			gitProcess.StartInfo = gitInfo;

			gitProcess.Start();
			string stderr_str = gitProcess.StandardError.ReadToEnd();  // pick up STDERR
			stdout_str += gitProcess.StandardOutput.ReadToEnd() + "<br/>"; // pick up STDOUT
			if (stderr_str.Contains("Switched to branch") || stderr_str.Contains("Already on") || stderr_str.Contains("Everything up-to-date") || stderr_str.Contains("To //192.168.0.8/bst"))
			{
				stdout_str += stderr_str + "<br/>";
				stderr_str = "";
			}

			if (stderr_str != "")
				strGit_Err += stderr_str + "<br/>";
			if (stdout_str != "")
				strGit_Out += stdout_str;
			gitProcess.WaitForExit();
		}
		StopTimer(strCommand);
		return stdout_str;
	}

	private static readonly Object objGitLock = new Object();
	protected void Button_Click(object sender, EventArgs e)
	{
		StartTimer();

		if (TextAreaMessage.Text == "")
		{
			LabelErrorMessage.Text = "Please write Message <br/>";
			return;
		}
		else
			LabelErrorMessage.Text = "<br/>";

		strGit_Err = "";
		strGit_Out = "";

		List<string> lstNeedPathFile = new List<string>();
		List<string> lstNeedPathNewFile = new List<string>();
		int j = 0;

		for (int i = 0; i < lstFilesCheckBox.Items.Count; i++)
		{
			if (lstFilesCheckBox.Items[i].Selected == true)
			{
				lstNeedPathFile.Add(lstParams[j]);
				lstNeedPathNewFile.Add(lstParams[j + 1]);
			}
			j += 2;
		}

		if (lstNeedPathFile.Count < 1)
		{
			LabelErrorFile.Text = "Please select file or files";
			return;
		}
		else
			LabelErrorFile.Text = "";

		lock (objGitLock)
		{
			SetGitCommand("reset --hard");
			SetGitCommand("fetch --all");
			String branchName = GetGitBranchName();
			SwitchGitBranch(branchName);
			SetGitCommand("pull origin");
			for (int i = 0; i < lstNeedPathFile.Count; i++)
			{
				if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(GetGitWorkDir(lstNeedPathFile[i]))))
				{
					System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(GetGitWorkDir(lstNeedPathFile[i])));
				}
				System.IO.File.Copy(lstNeedPathNewFile[i], GetGitWorkDir(lstNeedPathFile[i]), true);
				SetGitCommand("add -f \"" + GetGitWorkDir(lstNeedPathFile[i]) + "\""); //to add new files
			}
			StopTimer("Copy file(s)");
			SetGitCommand("commit --all --message=\"" + "WEB: " + TextAreaMessage.Text + "\" --author=\"" + GitUser + "\"");
			SetGitCommand("push origin refs/heads/" + branchName + ":refs/heads/" + branchName);
			SetGitCommand("reset --hard");
			SwitchGitBranch("master");
		}

		// Find and send list of files affects the test

		string strTTids = GetTTfromText(TextAreaMessage.Text);
		string strNew_TTids = "";

		//Find TTs affects the test
		string strOld_TTids = GetValue(string.Format("SELECT TTID FROM TESTRUNS WHERE ID = {0}", TestID)).ToString();

		// Check the uniqueness of the new TTs
		string[] arrTTID = strTTids.Split(',');
		foreach (string strTT in arrTTID)
		{
			if (string.IsNullOrEmpty(strTT))
				continue;
			strOld_TTids = strOld_TTids.Replace(strTT, "");
		}

		strNew_TTids = GetTTfromText(strOld_TTids + "," + strTTids);

		// Delete list of files affects the test
		SQLExecute("DELETE FROM CODELINKS WHERE TEST_RUN_ID = " + TestID.ToString());

		// Update TTs list
		String strTTs = strNew_TTids == "" ? "NULL" : "'" + strNew_TTids + "'";
		String strSQLExecute = "UPDATE TESTRUNS SET TTID=" + strTTs + ", USERID = (select T2.ID from PERSONS T2 where T2.USER_LOGIN = '" + UserName + "')  WHERE ID = " + TestID.ToString();
		SQLExecute(strSQLExecute);

		CommentTestRun(TestID.ToString(), TextAreaMessage.Text);

		if ((TestID != 0) && (strTTids != ""))
		{

			// Update list of files affects the test
			string[] arrfiles = DB_PutDependentFilesTT(strNew_TTids, TestID.ToString());

			// Print list of files affects the test
			LabelDependentFiles.Text += "<b>List of files affects the test (TT : " + strNew_TTids + ") :</b><br/>";
			foreach (string strItem in arrfiles)
			{
				if (string.IsNullOrEmpty(strItem))
					continue;
				LabelDependentFiles.Text += strItem + "<br/>";
			}
		}

		TextAreaMessage.Visible = false;
		Button1.Visible = false;
		lstFilesCheckBox.Enabled = false;
		LabelMessage.Visible = false;
		LabelMsg.Text = "<h2>File(s) have been uploaded on Git</h2></br>";

		if ((strGit_Out != "") || (strTime != ""))
		{
			LabelInformation.Text = "<h3>Information : </h3><br/>" + strTime + "<br/>" + strGit_Out;
		}
		if (strGit_Err != "")
		{
			LabelError.Visible = true;
			LabelError.Text = "<h3>Error : </h3><br/>" + strGit_Err;
		}
		else
			LabelError.Visible = false;

        FeedLog("Etalon files have been committed to git repository: " + TextAreaMessage.Text);
	}

	void SwitchGitBranch(string branchName)
	{
		//check current branch and switch if needed
		string currentBranch = SetGitCommand("rev-parse --abbrev-ref HEAD");
		Regex rgx = new Regex("[^a-zA-Z0-9]");
		currentBranch = rgx.Replace(currentBranch, "");
		if (!(currentBranch.Trim() == branchName))
			SetGitCommand("branch --track " + branchName + " origin/" + branchName);
		SetGitCommand("checkout " + branchName); //switch branch if branch differ
	}

	string GetGitBranchName()
	{
		string gitBranch = Request.Params["gitBranch"];
		if (gitBranch == "")
			gitBranch = "master";
		return gitBranch;
	}


	[WebMethod]
	public static string GetListFiles(string lstFiles)
	{
		Guid gd = Guid.NewGuid();
		HttpContext.Current.Application[gd.ToString()] = lstFiles;
		return gd.ToString();
	}

}