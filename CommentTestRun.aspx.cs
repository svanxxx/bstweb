using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Text.RegularExpressions;
using BSTStatics;

public partial class CommentTestRun : CbstHelper
{
	string SQL_Command = @"
	SELECT CASELIST.CNAME AS CASE_NAME,
	TESTS.TEST_NAME AS TEST,
	DBTYPES.DBTYPE AS DB,
	VER_LEFT.TEST_EXCEPTIONS AS EX1,
	VER_LEFT.TEST_DBERRORS AS DBE1,
	VER_LEFT.TEST_OUTPUTERRORS AS OE1,
	VER_LEFT.TEST_WARNINGS AS W1,
	VER_LEFT.TEST_ERRORS AS ERR1,
	VER_LEFT.TEST_DURATION AS DUR1,
	VER_LEFT.DOCLINK AS LINK1,
	VER_RIGHT.TEST_DBERRORS AS DBE2,
	VER_RIGHT.TEST_EXCEPTIONS AS EX2,
	VER_RIGHT.TEST_OUTPUTERRORS AS OE2,
	VER_RIGHT.TEST_WARNINGS AS W2,
	VER_RIGHT.TEST_ERRORS AS ERR2,
	VER_RIGHT.TEST_DURATION AS DUR2,
	VER_RIGHT.DOCLINK AS LINK2,
	VER_LEFT.RUN_HASH AS HASH1,
	VER_RIGHT.RUN_HASH AS HASH2,
	VER_LEFT.ID AS TEST_RUN_ID1,
	VER_RIGHT.ID AS TEST_RUN_ID2,
	VER_LEFT.TEST_RUN_DATE AS ENDDT,
	(select PP.USER_LOGIN FROM  PERSONS PP WHERE PP.ID = VER_LEFT.USERID) + ISNULL(': ' + VER_LEFT.TTID,'') + ISNULL(': ' + VER_LEFT.COMMENT,'') AS COMMENT,
	(select COUNT(P.TESTCASE_ID) FROM PCS P where P.TESTCASE_ID = VER_LEFT.TEST_CASE_ID) AS RUNNOW,
	CASE 
		WHEN (
				VER_LEFT.RUN_HASH IS NOT NULL
				AND VER_RIGHT.RUN_HASH IS NOT NULL
				AND VER_LEFT.RUN_HASH != VER_RIGHT.RUN_HASH
				)
			THEN 0
		WHEN (
				VER_LEFT.RUN_HASH IS NOT NULL
				AND VER_RIGHT.RUN_HASH IS NOT NULL
				AND VER_LEFT.RUN_HASH = VER_RIGHT.RUN_HASH
				AND
				(VER_LEFT.HOUSTON = 1
				OR VER_RIGHT.HOUSTON = 1)
				)
			THEN 1
		WHEN (
				VER_LEFT.RUN_HASH IS NOT NULL
				AND VER_RIGHT.RUN_HASH IS NULL
				AND VER_LEFT.HOUSTON = 1
				)
			THEN 2
		WHEN (
				VER_LEFT.RUN_HASH IS NULL
				AND VER_RIGHT.RUN_HASH IS NOT NULL
				AND VER_RIGHT.HOUSTON = 1
				)
			THEN 3
			WHEN (
				VER_LEFT.RUN_HASH IS NOT NULL
				AND VER_RIGHT.RUN_HASH IS NOT NULL
				AND VER_LEFT.RUN_HASH = VER_RIGHT.RUN_HASH
				AND VER_RIGHT.HOUSTON = 0
				AND VER_LEFT.HOUSTON = 0
				)
			THEN 4
		WHEN (
				VER_LEFT.RUN_HASH IS NOT NULL
				AND VER_RIGHT.RUN_HASH IS NULL
				AND VER_LEFT.HOUSTON = 0
				)
			THEN 5
		WHEN (
				VER_LEFT.RUN_HASH IS NULL
				AND VER_RIGHT.RUN_HASH IS NOT NULL
				AND VER_RIGHT.HOUSTON = 0
				)
			THEN 6
		ELSE 7
		END AS ORD,
			CASE 
		WHEN (
				VER_LEFT.RUN_HASH IS NOT NULL
				AND VER_RIGHT.RUN_HASH IS NOT NULL
				AND VER_LEFT.RUN_HASH != VER_RIGHT.RUN_HASH
				)
			THEN 0
		WHEN (
				VER_LEFT.RUN_HASH IS NOT NULL
				AND VER_RIGHT.RUN_HASH IS NOT NULL
				AND VER_LEFT.RUN_HASH = VER_RIGHT.RUN_HASH
				)
			THEN 1
		WHEN (
				VER_LEFT.RUN_HASH IS NOT NULL
				AND VER_RIGHT.RUN_HASH IS NULL
				)
			THEN 2
		WHEN (
				VER_LEFT.RUN_HASH IS NULL
				AND VER_RIGHT.RUN_HASH IS NOT NULL
				)
			THEN 3
		ELSE 4
		END AS ORD_PROG,
(SELECT (select PP.USER_LOGIN FROM PERSONS PP WHERE PP.ID = TR.USERID) + ISNULL(': ' + TR.TTID,'') + ISNULL(': ' + TR.COMMENT,'') FROM TESTRUNS TR WHERE TR.ID = 
 (SELECT  top 1 TX.ID  FROM TESTRUNS TX WHERE (TX.TTID IS NOT NULL OR  TX.COMMENT IS NOT NULL )
AND TX.RUN_HASH = VER_LEFT.RUN_HASH AND TX.DBTYPE_ID = VER_LEFT.DBTYPE_ID
 order by id  desc
) )		  as LASTCOMMENT
FROM (
	(
		SELECT DISTINCT TEST_ID TESTID,
			TEST_CASE_ID CASEID,
			DBTYPE_ID DBTID,
			(
				SELECT TEST_CASE_NAME
				FROM TEST_CASES
				WHERE TEST_CASES.ID = TESTRUNS.TEST_CASE_ID
				) CNAME
		FROM TESTRUNS
		WHERE DBTYPE_ID IN (
				2,
				3
				)
			AND TEST_CASE_ID IS NOT NULL
		) AS CASELIST
		JOIN TESTS ON TESTS.ID = CASELIST.TESTID
		JOIN DBTYPES ON DBTYPES.ID = CASELIST.DBTID
		 LEFT OUTER JOIN (
		SELECT TEST_DURATION,
			TEST_ERRORS,
			TEST_EXCEPTIONS,
			TEST_WARNINGS,
			TEST_OUTPUTERRORS,
			TEST_CASE_ID,
			TESTRUNS.DBTYPE_ID,
			TEST_FIPVERSIONID,
			TESTRUNS.RUN_HASH,
			TEST_DBERRORS,
			DOCLINK,
            ID,
			TEST_RUN_DATE,
			row_number() OVER (
				PARTITION BY test_case_id,
				TESTRUNS.DBTYPE_ID ORDER BY RequestID desc, test_case_id,
					TESTRUNS.DBTYPE_ID,
					ID  DESC
				) AS RN,
			CASE
			WHEN
			TEST_EXCEPTIONS > 0
			OR TEST_DBERRORS > 0
			OR TEST_ERRORS > 0
			OR TEST_OUTPUTERRORS > 0
			THEN 1
			ELSE 0
			END AS Houston,
			TESTRUNS.TTID,
			TESTRUNS.COMMENT,
			TESTRUNS.USERID
		FROM TESTRUNS
		WHERE TEST_FIPVERSIONID = $Version_1
                AND IGNORE IS NULL $RequestID
		) AS VER_LEFT ON VER_LEFT.TEST_CASE_ID = CASELIST.CASEID
		AND VER_LEFT.DBTYPE_ID = CASELIST.DBTID
		AND VER_LEFT.rn = 1
	)
LEFT OUTER JOIN (
	SELECT TEST_DURATION,
		TEST_ERRORS,
		TEST_EXCEPTIONS,
		TEST_WARNINGS,
		TEST_OUTPUTERRORS,
		TEST_CASE_ID,
		DBTYPE_ID,
		TEST_FIPVERSIONID,
		RUN_HASH,
		TEST_DBERRORS,
		DOCLINK,
        ID,
		TEST_RUN_DATE,
		row_number() OVER (
			PARTITION BY test_case_id,
			DBTYPE_ID ORDER BY RequestID desc, test_case_id,
				DBTYPE_ID,
				ID  DESC
			) AS RN,
			CASE
			WHEN
			TEST_EXCEPTIONS > 0
			OR TEST_DBERRORS > 0
			OR TEST_ERRORS > 0
			OR TEST_OUTPUTERRORS > 0
			THEN 1
			ELSE 0
			END AS Houston
	FROM TESTRUNS
	WHERE TEST_FIPVERSIONID = $Version_2
            AND IGNORE IS NULL
	) AS VER_RIGHT ON VER_RIGHT.TEST_CASE_ID = CASELIST.CASEID
	AND VER_RIGHT.DBTYPE_ID = CASELIST.DBTID
	AND VER_RIGHT.rn = 1
$Filter
ORDER BY $order,
	EX1 DESC,
	DBE1 DESC,
	ERR1 DESC,
	OE1 DESC,
	W1 DESC
";

	//	string m_strFilter = @"(VER_LEFT.RUN_HASH IS NOT NULL
	//				AND VER_RIGHT.RUN_HASH IS NOT NULL
	//				AND VER_LEFT.RUN_HASH != VER_RIGHT.RUN_HASH) OR (VER_LEFT.RUN_HASH IS NOT NULL
	//				AND VER_RIGHT.RUN_HASH IS NULL)";

	protected string strFiPVersion, strFiPVersionBase, strTTID, strDT, strProgrammer, strComment;
	string m_IDs;
	protected string RequestID
	{
		get { return m_IDs; }
		set { m_IDs = value; }
	}
	protected string getBackUrl
	{
		get
		{
			return Request.Params["BackUrl"];
		}
	}
	protected bool CheckCorrectParam()
	{
		UserMess.Visible = false;
		string strRunID = Request.Params["RequestID"];

		if (string.IsNullOrEmpty(strRunID))
		{
			UserMess.Visible = true;
			UserMess.Text = "No Request ID is provided - invalid URL";
			//CommentButton.Visible = false;
			ClearButton.Visible = false;
			CMT.Visible = false;
			Label2.Visible = false;
			return false;
		}


		RequestID = strRunID;
		return true;
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!CheckCorrectParam())
			return;

	}
	protected void Page_Init(object sender, EventArgs e)
	{
		if (!CheckCorrectParam())
			return;
		GetProgrammerInfo();

		string strVersion1 = VersionID(strFiPVersion).ToString();
		string strVersion2 = VersionID(strFiPVersionBase).ToString();


		String sqlComand = SQL_Command.Replace("$Version_2", strVersion2);
		sqlComand = sqlComand.Replace("$Version_1", strVersion1);
		sqlComand = sqlComand.Replace("$Filter", GetSQLFilter());
		if (string.IsNullOrEmpty(RequestID)) sqlComand = sqlComand.Replace("$RequestID", "");
		else sqlComand = sqlComand.Replace("$RequestID", "AND RequestID=" + RequestID);

		sqlComand = sqlComand.Replace("$order", "ORD_PROG");

		SqlDataSource1.SelectCommand = sqlComand;
	}
	protected int VersionID(string strV)
	{
		return Convert.ToInt32(GetDataSet("select V.ID from FIPVERSION V where V.VERSION = '" + strV + "'").Tables[0].Rows[0][0]);
	}
	protected string GetSQLFilter()
	{
		return "";
		//		if (m_strFilter != "") return "WHERE " + m_strFilter;
		//		else return "";
	}
	protected void GetProgrammerInfo()
	{
		TestRequest tr = new TestRequest(RequestID);

		strTTID = tr.TTID;
		strDT = tr.REQUESTDATETIME;
		strProgrammer = tr.PROGABB;
		strComment = tr.COMMENT;

		Version v = new Version(tr.VERSIONID);
		strFiPVersion = v.VERSION;

		int DotCount = 0;
		int len = 0;
		foreach (var c in strFiPVersion)
		{
			if (DotCount == 4)
			{
				int CharCode = c - 48;
				if (CharCode < 0 || CharCode > 9)
					break;
			}
			if (c == '.')
				DotCount++;
			len++;
		}

		strFiPVersionBase = strFiPVersion.Substring(0, len);
		//check if base version exists. if no - use max admin version.
		DataSet DS = GetDataSet(@"select V.ID from FIPVERSION V where V.VERSION = '" + strFiPVersionBase + "'");
		if (DS.Tables[0].Rows.Count == 0)
		{
			DS = GetDataSet(@"select TOP 1 V.VERSION from TESTREQUESTS T, FIPVERSION V where T.ProgAbb = 'Admin' and T.VersionID = V.ID order by T.ID DESC");
			strFiPVersionBase = DS.Tables[0].Rows[0][0].ToString();
		}
	}
	protected void CommentButton_Click(object sender, EventArgs e)
	{
		if ((TestOK.Checked == false) && (TestFailed.Checked == false))
		{
			LabelError.Text = "<b> [Please choose result] </b>";
			return;
		}

		string strPutColor = GoodTT;
		string strText = TestOK.Text;

		if (TestFailed.Checked) { strPutColor = BadTT; strText = TestFailed.Text; }

		LabelError.Text = "";

		String strCommentTest = CMT.Text;
		strCommentTest = strCommentTest.Replace("\r\n", "<br>");

		ArrayList list = GetCaseNameList();
		string[] arrfiles = GetFilesList(RequestID);
		string TestRunIDs = string.Empty;
		string TestsInMail = string.Empty;

		foreach (CheckBox cb in list)
		{
			if (!String.IsNullOrEmpty(cb.Attributes["TEST_RUN_ID"]))
			{
				if (TestRunIDs == string.Empty)
					TestRunIDs = cb.Attributes["TEST_RUN_ID"];
				else
					TestRunIDs += "," + cb.Attributes["TEST_RUN_ID"];

				TestsInMail += string.Format("<a href='http://{0}/", Settings.CurrentSettings.BSTADDRESS) + cb.Attributes["TEST_LINK"] + "'>" + cb.Attributes["CASE_NAME"] + "</a><br>";
			}
		}

		if (TestsInMail != string.Empty)
			TestsInMail = "<br>Tests which were affected by your task:<br>" + TestsInMail + "<br>";

		DB_PutDependentFilesFromTT(TestRunIDs, arrfiles);

		TestRequest tr = new TestRequest(RequestID) { USERID = UserID, TESTED = strPutColor == GoodTT ? "true" : "false" };
		tr.Store();

		//----update TestTrack DB
		string tasknumber = Regex.Match(strTTID.ToUpper(), "TT[0-9]+").Value.Replace("TT", "");
		if (!String.IsNullOrEmpty(tasknumber))
		{
			if (TestFailed.Checked)
			{
				SQLExecuteTestTrackDB("UPDATE DEFECTS set idDisposit = 4, REFERENCE = 'R' + ISNULL(REFERENCE, '') where DefectNum = " + tasknumber);
			}
			else
			{
				SQLExecuteTestTrackDB("UPDATE DEFECTS set idDisposit = 10, REFERENCE = 'P' + ISNULL(REFERENCE, '') where DefectNum = " + tasknumber);
			}
		}
		//----update end

		string body = string.Format(@"
			Your version: ({0} <a href='http://{1}/runs.aspx?R.RequestID={2}'>{3}</a>)
			<br>{4}
			<br>{5}
			<br>
			<br><b>{6}</b>
			<br>
			<br>Result:
			<br><a href='http://{1}/CompareVersion.aspx?Version1={3}&Version2={7}'>Compare Version</a>
			<br>
			<br>{8}
			<br>{9}
			<br>
			<br>Best regards, <b>{10}</b>"
		, strDT, Settings.CurrentSettings.BSTADDRESS, RequestID, strFiPVersion, strTTID, strComment, strText, strFiPVersionBase, strCommentTest, TestsInMail, UserName);
		AddEmail(
			strProgrammer
			, string.Format("Your request for version ({0}) was tested. Request info: ({1})", strFiPVersion, strTTID)
			, body
			, strPutColor);

		LabelError.Visible = false;
		if (getBackUrl != null) Response.Redirect(getBackUrl);
	}
	protected void ClearButton_Click(object sender, EventArgs e)
	{
		CMT.Text = "";
	}
	protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			e.Row.Cells[0].Controls.Add(GetNewCheckBox());
			e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
		}
		else
		{
			CheckBox cb = new CheckBox();
			cb.Checked = true;
			cb.CheckedChanged += new EventHandler(this.CheckBoxAll_CheckedChanged);
			cb.AutoPostBack = true;
			e.Row.Cells[0].Controls.Add(cb);
		}
	}
	protected void CheckBoxAll_CheckedChanged(object sender, EventArgs e)
	{
		CheckBox cb = (CheckBox)sender;

		foreach (CheckBox myCheckBox in listBox)
		{
			myCheckBox.Checked = cb.Checked;
		}
	}
	protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			DataRowView drv = (DataRowView)e.Row.DataItem;

			string hash1 = drv["HASH1"].ToString();
			string hash2 = drv["HASH2"].ToString();
			if (string.IsNullOrEmpty(hash1) || hash1.Equals(hash2))
				e.Row.Visible = false;

			e.Row.Cells[1].Text = (e.Row.DataItemIndex + 1).ToString();

			if (e.Row.Visible)
				UpdatePostNewCheckBox(e.Row.Cells[0].Controls[0], drv["CASE_NAME"].ToString(),
					  "ViewLog.aspx?log=" + drv["Link1"].ToString() + "&RUNID=" + drv["TEST_RUN_ID1"].ToString(),
					  drv["TEST_RUN_ID1"].ToString());

			int iRow = 2, iRow2, kCompare = 0, kErr = 0;

			HyperLink hr = new HyperLink();
			hr.NavigateUrl = "index.aspx?DateFlag=0&Filter=(TEST_NAME='" + drv["TEST"].ToString().Replace(" ", "_") + "')&GroupFlag=0";
			hr.Text = e.Row.Cells[iRow].Text;
			e.Row.Cells[iRow].Controls.Add(hr);

			iRow++;
			hr = new HyperLink();
			hr.NavigateUrl = "index.aspx?DateFlag=0&Filter=(TEST_CASE_NAME='" + drv["CASE_NAME"].ToString().Replace(" ", "_") + "')&GroupFlag=0";
			hr.Text = e.Row.Cells[iRow].Text;
			e.Row.Cells[iRow].Controls.Add(hr);

			if (drv["RUNNOW"].ToString() != "0")
			{
				Image Circle = new Image();
				Circle.ImageUrl = "~/IMAGES/Circle.GIF";
				e.Row.Cells[iRow].Controls.Add(Circle);
			}

			iRow++;
			String strComment = drv["COMMENT"].ToString();

			e.Row.Cells[iRow].Attributes.Add("onClick", "Add_TT(this,'" + drv["TEST_RUN_ID1"].ToString() + "');");
			e.Row.Cells[iRow].ToolTip = strComment;
			e.Row.Cells[iRow].Text = strComment.Substring(0, strComment.Length > 12 ? 12 : strComment.Length);
			if (e.Row.Cells[iRow].Text.Length < strComment.Length) e.Row.Cells[iRow].Text += "...";
			e.Row.Cells[iRow].Attributes["onmouseover"] = "ColorCell(this, 'white');";
			e.Row.Cells[iRow].Attributes["onmouseout"] = "ColorCell(this, 'transparent');";

			//EX1 and EX2
			iRow = 6;
			iRow2 = 14;
			if ((drv["EX1"].ToString() != drv["EX2"].ToString()) && (GetCountErors(drv, "EX1", "EX2") == 2)) kCompare++;
			if (GetCountErors(drv, "EX1") > 0) e.Row.Cells[iRow].BackColor = ExcColor;
			if (GetCountErors(drv, "EX2") > 0) e.Row.Cells[iRow2].BackColor = ExcColor;


			//DB
			iRow++;
			iRow2++;
			if ((drv["DBE1"].ToString() != drv["DBE2"].ToString()) && (GetCountErors(drv, "DBE1", "DBE2") == 2)) kCompare++;
			if (GetCountErors(drv, "DBE1") > 0) e.Row.Cells[iRow].BackColor = DBEColor;
			if (GetCountErors(drv, "DBE2") > 0) e.Row.Cells[iRow2].BackColor = DBEColor;

			//ERR1
			iRow++;
			iRow2++;

			if ((drv["ERR1"].ToString() != drv["ERR2"].ToString()) && (GetCountErors(drv, "ERR1", "ERR2") == 2)) kCompare++;
			if (GetCountErors(drv, "ERR1") > 0) e.Row.Cells[iRow].BackColor = ErrColor;
			if (GetCountErors(drv, "ERR2") > 0) e.Row.Cells[iRow2].BackColor = ErrColor;


			//OE1
			iRow++;
			iRow2++;

			if ((drv["OE1"].ToString() != drv["OE2"].ToString()) && (GetCountErors(drv, "OE1", "OE2") == 2)) kCompare++;
			if (GetCountErors(drv, "OE1") > 0) e.Row.Cells[iRow].BackColor = OutColor;
			if (GetCountErors(drv, "OE2") > 0) e.Row.Cells[iRow2].BackColor = OutColor;


			//W1
			iRow++;
			iRow2++;
			if ((drv["W1"].ToString() != drv["W2"].ToString()) && (GetCountErors(drv, "W1", "W2") == 2)) kCompare++;
			if (GetCountErors(drv, "W1") > 0) e.Row.Cells[iRow].BackColor = WarColor;
			if (GetCountErors(drv, "W2") > 0) e.Row.Cells[iRow2].BackColor = WarColor;


			// Duration
			iRow++;
			iRow2++;

			if (e.Row.Cells[iRow].Text != "") e.Row.Cells[iRow].Text = ConvertDuration(e.Row.Cells[iRow].Text);
			if (e.Row.Cells[iRow2].Text != "") e.Row.Cells[iRow2].Text = ConvertDuration(e.Row.Cells[iRow2].Text);

			//HASH1
			iRow += 1;
			iRow2 = iRow + 1;
			if ((drv["HASH1"].ToString() != drv["HASH2"].ToString()) && ((drv["HASH1"].ToString() != "") && (drv["HASH2"].ToString() != "")))
			{
				kCompare++;
			}



			hr = new HyperLink();
			hr.NavigateUrl = "ViewLog.aspx?log=" + drv["Link1"].ToString() + "&RUNID=" + drv["TEST_RUN_ID1"].ToString();

			hr.Text = "Link1";//e.Row.Cells[iRow].Text;
			e.Row.Cells[iRow].Controls.Add(hr);

			hr = new HyperLink();
			hr.NavigateUrl = "ViewLog.aspx?log=" + drv["Link2"].ToString() + "&RUNID=" + drv["TEST_RUN_ID2"].ToString();
			hr.Text = "Link2";//e.Row.Cells[iRow2].Text;
			e.Row.Cells[iRow2].Controls.Add(hr);

			int kErr1 = 0, kErr2 = 0;
			kErr1 += isErors(drv, "EX1");
			kErr1 += isErors(drv, "DBE1");
			kErr1 += isErors(drv, "ERR1");
			kErr1 += isErors(drv, "OE1");

			kErr2 += GetCountErors(drv, "EX2");
			kErr2 += GetCountErors(drv, "DBE2");
			kErr2 += GetCountErors(drv, "ERR2");
			kErr2 += GetCountErors(drv, "OE2");

			kErr = kErr1 + kErr2;

			if (kCompare != 0) // if 2 versions not have erros but hash1 != Hash2
			{
				if (drv["HASH1"].ToString() != drv["HASH2"].ToString()) e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFF1A6");
			}
			else if (kErr != 0) // errors in 2 versions, set color gray
			{
				e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#D3D3D3");
				iRow = 4;
				strComment = drv["LASTCOMMENT"].ToString();
				e.Row.Cells[iRow].ToolTip = strComment;
				e.Row.Cells[iRow].Text = strComment.Substring(0, strComment.Length > 12 ? 12 : strComment.Length);
				if (e.Row.Cells[iRow].Text.Length < strComment.Length) e.Row.Cells[iRow].Text += "...";
				e.Row.Cells[iRow].Text = "<font color='red'>" + e.Row.Cells[iRow].Text + "</font>";
			}


			if ((kErr1 == 0) && (kErr2 != 0)) // if version1 not have errors and version2 have erors then set "GOOD"
				if (e.Row.Cells[3].Text == "")
				{
					Label mylabel = new Label();
					mylabel.Text = "<center>Good</center>";
					mylabel.ForeColor = System.Drawing.Color.Green;
					e.Row.Cells[3].Controls.Add(mylabel);
				}

		}
	}
	protected int GetCountErors(DataRowView drv, string strColumn)
	{
		string strTmp = drv[strColumn].ToString().Trim().Replace("&nbsp;", "");
		if ((strTmp != "") && (strTmp != "0")) return 1;
		else return 0;
	}
	protected int GetCountErors(DataRowView drv, string strColumn1, string strColumn2)
	{
		return GetCountErors(drv, strColumn1) + GetCountErors(drv, strColumn2);
	}
	protected string ConvertDuration(String strDuration)
	{
		try
		{
			double dval = 0;
			if (double.TryParse(strDuration, out dval))
				return ConvertDurationToHHMM(Convert.ToDouble(strDuration));
			else
				return "00:00";
		}
		catch (System.Exception /*ex*/)
		{
			return strDuration;
		}

	}
	protected string ConvertDurationToHHMM(double dDuration)
	{
		int iHrsTotal = Convert.ToInt32(Math.Floor(dDuration));
		int iMinTotal = Convert.ToInt32(Math.Floor(60 * (dDuration - iHrsTotal)));
		return iHrsTotal.ToString("00") + ":" + iMinTotal.ToString("00");
	}
	protected int isErors(DataRowView drv, string strColumn)
	{
		string strTmp = drv[strColumn].ToString().Trim().Replace("&nbsp;", "");
		if (strTmp != "0") return 1;
		else return 0;
	}
	protected ArrayList GetCaseNameList()
	{
		ArrayList list = new ArrayList();

		foreach (CheckBox myCheckBox in listBox)
		{
			if (myCheckBox.Checked)
			{
				list.Add(myCheckBox);
			}
		}

		return list;
	}
	List<CheckBox> listBox = new List<CheckBox>();
	protected CheckBox GetNewCheckBox()
	{
		CheckBox cb = new CheckBox();
		cb.Checked = true;
		listBox.Add(cb);
		return cb;
	}
	protected void UpdatePostNewCheckBox(Control ctrl, string CASE_NAME, string TEST_LINK, string TEST_RUN_ID)
	{
		WebControl btn = ctrl as WebControl;
		btn.Attributes["CASE_NAME"] = CASE_NAME;
		btn.Attributes["TEST_LINK"] = TEST_LINK;
		btn.Attributes["TEST_RUN_ID"] = TEST_RUN_ID;
	}
	protected string[] GetFilesList(string RequestID)
	{

		DataSet dsFiles = GetDataSet("select C.CODEFILENAME from REQUESTLINKS R JOIN CODEFILES C ON C.ID = R.FileID where R.RequestID = " + RequestID);

		string[] arrfiles = new string[dsFiles.Tables[0].Rows.Count];

		for (int iRow = 0; iRow < dsFiles.Tables[0].Rows.Count; iRow++)
		{
			arrfiles[iRow] += dsFiles.Tables[0].Rows[iRow][0].ToString();
		}

		return arrfiles;
	}
	protected void RadioButton_CheckedChanged(object sender, EventArgs e)
	{
		if (TestOK.Checked)
		{
			TestOK.Font.Bold = true;
			TestFailed.Font.Bold = false;
			TestOK.BackColor = System.Drawing.ColorTranslator.FromHtml("#98FF98");
			TestFailed.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
			TestFailed.ForeColor = System.Drawing.ColorTranslator.FromHtml("#000000");
			ImageButtonRun.ImageUrl = "IMAGES/Send mail.png";
		}
		else
		{
			TestFailed.Font.Bold = true;
			TestOK.Font.Bold = false;
			ImageButtonRun.ImageUrl = "IMAGES/Send_mail_Err.png";

			TestOK.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
			TestFailed.BackColor = System.Drawing.ColorTranslator.FromHtml("#C24641");
			TestFailed.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");


		}


	}
}
