using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Data;
using System.Text.RegularExpressions;

public partial class CompareVersion : CbstHelper
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
	VER_LEFT.RequestID AS RequestID,
	VER_LEFT.TEST_RUN_COMMAND AS TEST_RUN_COMMAND,
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
			TESTRUNS.USERID,
            RequestID,
            TEST_RUN_COMMAND
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
	string patternVersion = @"\d{4}.\w+.\d+.\d+.\d+.*";

	protected string m_strFilter, m_strVersion1, m_strVersion2;
	protected string strFilter
	{
		get
		{
			if (m_strFilter == null)
			{
				if (string.IsNullOrEmpty(Request.Params["Filter"]))
					m_strFilter = "";
				else
					m_strFilter = Request.Params["Filter"];
			}
			return m_strFilter;
		}
	}
	protected int VersionID(string strV)
	{
		return Convert.ToInt32(GetDataSet("select V.ID from FIPVERSION V where V.VERSION = '" + strV + "'").Tables[0].Rows[0][0]);
	}
	protected string strVersion1
	{
		get
		{
			return Request.Params["Version1"];
		}
	}

	protected string strRequest
	{
		get
		{
			return Request.Params["Request"];
		}
		set
		{
			strRequest = value;
		}
	}

	protected string strVersion2
	{
		get
		{
			return Request.Params["Version2"];
		}
	}

	protected void UpdateRequestsList(string strVersion)
	{
		DropDownListRequests.Visible = true;
		DropDownListRequests.ClearSelection();
		DropDownListRequests.Items.Clear();

		String sql = @"SELECT TESTREQUESTS.ID, TESTREQUESTS.TTID, TESTREQUESTS.Comment, TESTREQUESTS.RequestDateTime FROM TESTREQUESTS LEFT OUTER JOIN FIPVERSION ON TESTREQUESTS.VersionID = FIPVERSION.ID WHERE (FIPVERSION.VERSION = N'$VERSION')";


		DataSet ds = GetDataSet(sql.Replace("$VERSION", strVersion));

		DropDownListRequests.Items.Add(new ListItem("All", ""));
		int i = 0;
		foreach (DataRow rowCur in ds.Tables[0].Rows)
		{
			i++;
			String fff = rowCur[3].ToString() + " " + rowCur[1].ToString() + " (" + rowCur[2].ToString() + ")";
			fff = fff.Substring(0, (fff.Length > 60 ? 60 : fff.Length)) + "...";
			DropDownListRequests.Items.Add(new ListItem(fff, rowCur[0].ToString()));
		}
		if (i < 2) DropDownListRequests.Visible = false;


	}

	protected string GetFilterURL(string strVersion1, string strVersion2, string strRequestID)
	{
		string strTempFilter = "";
		if (strFilter != "") strTempFilter = "&Filter=" + strFilter;

		string RequestID = "";
		if (string.Compare(strVersion1, this.strVersion1) == 0)
			if (!string.IsNullOrEmpty(strRequestID)) RequestID = "&Request=" + strRequestID;

		return CurrentPageName + "?Version1=" + strVersion1 + RequestID + "&Version2=" + strVersion2 + strTempFilter;
	}

	protected string GetFilterURL(string strNewFilter)
	{

		string strAdd = "", strTempFilter = "";

		if (strFilter != "") strAdd = "AND";
		if (strFilter.IndexOf("(" + strNewFilter + ")") != -1)
		{
			strTempFilter = strFilter.Replace("(" + strNewFilter + ")", "");


			if (strTempFilter.IndexOf("AND") == 0) strTempFilter = strTempFilter.Remove(0, 3);
			if (strTempFilter.LastIndexOf("AND") == strTempFilter.Count() - 3) strTempFilter = strTempFilter.Remove(strTempFilter.Count() - 3, 3);
			strTempFilter = strTempFilter.Replace("ANDAND", "AND");
			if (strTempFilter == "AND") strTempFilter = "";
		}
		else strTempFilter = strFilter + strAdd + "(" + strNewFilter + ")";

		if (strTempFilter != "") strTempFilter = "&Filter=" + strTempFilter;

		return CurrentPageName + "?Version1=" + DropDownListVersion1.SelectedItem.Text + "&Version2=" + DropDownListVersion2.SelectedItem.Text + strTempFilter;
	}

	protected string GetSQLFilter()
	{
		if (strFilter != "") return "WHERE " + strFilter;
		else return "";
	}


	string delSpaseVersion(string strVersion)
	{
		while ((strVersion.Length > 0) && (strVersion[strVersion.Length - 1] == ' '))
		{
			strVersion = strVersion.Remove(strVersion.Length - 1, 1);
		}
		return strVersion;
	}

	protected void Page_Init(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            string strVersion1 = Request.Params["Version1"];
            string strVersion2 = Request.Params["Version2"];


            if ((strVersion1 == null) || (strVersion2 == null))
            {
                String tempVersion = "";
               // char chLast = ' ';

                DataSet ds = GetDataSet("SELECT TOP 100 ID, VERSION FROM dbo.FIPVERSION ORDER BY ID DESC");

                foreach (DataRow rowCur in ds.Tables[0].Rows)
                {
                    tempVersion = delSpaseVersion(rowCur[1].ToString());

                    if (!Regex.IsMatch(tempVersion, patternVersion, RegexOptions.IgnoreCase))
                        continue;

                    if (!isProgramerVersion(tempVersion))
                    {
                        if (strVersion1 == null) strVersion1 = tempVersion;
                        else if (strVersion2 == null)
                        {
                            strVersion2 = tempVersion;

                            if (EquelsFipVersion(strVersion1, strVersion2) != 0)
                            {
                                if (EquelsFipVersion(strVersion1, strVersion2) > 0)
                                    strVersion2 = null;
                                else
                                {
                                    strVersion1 = strVersion2;
                                    strVersion2 = null;
                                }
                                continue;
                            }

                            break;
                        }
                    }
                }

                Response.Redirect(GetFilterURL(strVersion1, strVersion2, null), true);
            }
            else
            {
                // String strSQL = "SELECT TOP 50 ID, VERSION FROM dbo.FIPVERSION ORDER BY ID DESC";
                //OleDbDataReader reader = GetDataFromDB(strSQL);
                DropDownListVersion1.ClearSelection();
                DropDownListVersion2.ClearSelection();
 
                DropDownListVersion1.Items.Clear();
                DropDownListVersion2.Items.Clear();

                DataSet ds = GetDataSet("SELECT TOP 100 ID, VERSION FROM dbo.FIPVERSION ORDER BY ID DESC");

                foreach (DataRow rowCur in ds.Tables[0].Rows)
                {
                    string tempVersion = delSpaseVersion(rowCur[1].ToString());
                    if (!Regex.IsMatch(tempVersion, patternVersion, RegexOptions.IgnoreCase))
                        continue;
                    ListItem lst = new ListItem(tempVersion, rowCur[0].ToString());
                    ListItem lst2 = new ListItem(tempVersion, rowCur[0].ToString());
                    DropDownListVersion1.Items.Add(lst);
                    DropDownListVersion2.Items.Add(lst2);
                }

					 int iV1 = VersionID(strVersion1);
					 int iV2 = VersionID(strVersion2);
                String sqlComand = SQL_Command.Replace("$Version_2", iV2.ToString());
                sqlComand = sqlComand.Replace("$Version_1", iV1.ToString());

                if (string.IsNullOrEmpty(strRequest)) sqlComand = sqlComand.Replace("$RequestID", "");
                else sqlComand = sqlComand.Replace("$RequestID", "AND RequestID=" + strRequest);

                if (isProgramerVersion(strVersion1)) sqlComand = sqlComand.Replace("$order", "ORD_PROG");
                else sqlComand = sqlComand.Replace("$order", "ORD");
                sqlComand = sqlComand.Replace("$Filter", GetSQLFilter());
                SqlDataSource1.SelectCommand = sqlComand;

            }

        }
        else
        {
				int iV1 = VersionID(strVersion1);
				int iV2 = VersionID(strVersion2);
            String sqlComand = SQL_Command.Replace("$Version_2", iV2.ToString());
            sqlComand = sqlComand.Replace("$Version_1", iV1.ToString());

            sqlComand = sqlComand.Replace("$Filter", GetSQLFilter());

            if (string.IsNullOrEmpty(strRequest)) sqlComand = sqlComand.Replace("$RequestID", "");
            else sqlComand = sqlComand.Replace("$RequestID", "AND RequestID=" + strRequest);

            if (isProgramerVersion(strVersion1)) sqlComand = sqlComand.Replace("$order", "ORD_PROG");
            else sqlComand = sqlComand.Replace("$order", "ORD");

            SqlDataSource1.SelectCommand = sqlComand;
        }
    }
	protected void Page_Load(object sender, EventArgs e)
	{


		if (!IsPostBack)
		{
			DropDownListVersion1.ClearSelection();
			DropDownListVersion2.ClearSelection();
			//    DropDownListRequests.ClearSelection();

			DropDownListVersion1.Items.FindByText(strVersion1).Selected = true;
			DropDownListVersion2.Items.FindByText(strVersion2).Selected = true;

			UpdateRequestsList(strVersion1);
			if (!string.IsNullOrEmpty(strRequest))
				DropDownListRequests.Items.FindByValue(strRequest).Selected = true;

		}


		if (IsPostBack)
		{
			string strID = Request.Params.Get(postEventSourceID);
			if (strID.ToUpper().Contains("TIMER"))
			{
				if (REPID.Value != GetLastRepID())
				{
					if (string.IsNullOrEmpty(Request.QueryString.ToString()))
						Response.Redirect(CurrentPageName);
					else
						Response.Redirect(CurrentPageName + "?" + Request.QueryString.ToString());//new tests arriving
				}
				else
					Response.End();
			}
			else if (!strID.ToUpper().Contains("GRIDVIEW"))
				return; //other button clicks do not need data loading.
		}

		int iShowBy = 50;
		if (Request.Cookies["showbyCV"] != null)
		{
			try
			{
				iShowBy = Convert.ToInt32(Request.Cookies["showbyCV"].Value);

			}
			catch (System.Exception /*ex*/) { }
		}
		ShowByList.SelectedValue = iShowBy.ToString();
		GridView1.PageSize = iShowBy;

	}

	protected void Compare_Click(object sender, EventArgs e)
	{
		Response.Redirect(GetFilterURL(DropDownListVersion1.SelectedItem.Text, DropDownListVersion2.SelectedItem.Text, DropDownListRequests.SelectedItem.Value), true);
	}

	protected int GetCountErors(DataRowView drv, string strColumn)
	{
		string strTmp = drv[strColumn].ToString().Trim().Replace("&nbsp;", "");
		if ((strTmp != "") && (strTmp != "0")) return 1;
		else return 0;
	}

	protected int isErors(DataRowView drv, string strColumn)
	{
		string strTmp = drv[strColumn].ToString().Trim().Replace("&nbsp;", "");
		if (strTmp != "0") return 1;
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
			double dbl;
			return ConvertDurationToHHMM(double.TryParse(strDuration, out dbl) ? dbl : 0.0);
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

	protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
	{


		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			DataRowView drv = (DataRowView)e.Row.DataItem;

			e.Row.Cells[0].Text = (e.Row.DataItemIndex + 1).ToString();



			int iRow = 1, iRow2, kCompare = 0, kErr = 0;
			HyperLink hr = new HyperLink();
			hr.NavigateUrl = "index.aspx?DateFlag=0&Filter=(TEST_NAME='" + drv["TEST"].ToString().Replace(" ", "_") + "')&GroupFlag=0";
			hr.Text = "<.>";
			e.Row.Cells[iRow].Controls.Add(hr);

			hr = new HyperLink();
			hr.NavigateUrl = GetFilterURL("TESTS.TEST_NAME='" + drv["TEST"].ToString().Replace(" ", "_") + "'");
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


			int kErr1 = 0, kErr2 = 0;
			kErr1 += GetCountErors(drv, "EX1");
			kErr1 += GetCountErors(drv, "DBE1");
			kErr1 += GetCountErors(drv, "ERR1");
			kErr1 += GetCountErors(drv, "OE1");

			kErr2 += GetCountErors(drv, "EX2");
			kErr2 += GetCountErors(drv, "DBE2");
			kErr2 += GetCountErors(drv, "ERR2");
			kErr2 += GetCountErors(drv, "OE2");

			kErr = kErr1 + kErr2;

			iRow++;
			String strComment = drv["COMMENT"].ToString();
			//e.Row.Cells[iRow].Attributes.Add("onClick", "$(this).css('background-color', 'green');"); ondblclick
			e.Row.Cells[iRow].Attributes.Add("onClick", "Add_TT(this,'" + drv["TEST_RUN_ID1"].ToString() + "');");
			e.Row.Cells[iRow].ToolTip = strComment;
			e.Row.Cells[iRow].Text = strComment.Substring(0, strComment.Length > 12 ? 12 : strComment.Length);
			String strComment1 = drv["LASTCOMMENT"].ToString();
			if (e.Row.Cells[iRow].Text.Length < strComment.Length) e.Row.Cells[iRow].Text += "...";
			if (strComment1.Length == 0 || e.Row.Cells[iRow].Text == "")
			{
				if (kErr != 0)
				{
					if ((kErr1 == kErr2 && strComment1.Length == 0) || (kErr1 > 0 && kErr2 == 0 && strComment1.Length == 0))
					{
						e.Row.Cells[3].BackColor = System.Drawing.ColorTranslator.FromHtml("#B22222");
					}
					if (strComment.Length == 0 && (drv["HASH1"].ToString() != drv["HASH2"].ToString() && drv["HASH1"].ToString() != "") && (drv["HASH2"].ToString() != ""))
					{
						if ((kErr1 == 0) && (kErr2 != 0))
						{
							e.Row.Cells[iRow].BackColor = System.Drawing.ColorTranslator.FromHtml("transparent");
						}
						else
						{
							e.Row.Cells[iRow].BackColor = System.Drawing.ColorTranslator.FromHtml("#B22222");
						}
					}
					else
					{
						if (kErr != 0 && e.Row.Cells[iRow].Text != "")
						{
							e.Row.Cells[iRow].BackColor = System.Drawing.ColorTranslator.FromHtml("#B22222");
						}
					}
				}
				else
				{
					if ((drv["HASH1"].ToString() != drv["HASH2"].ToString() && drv["HASH1"].ToString() != "") && (drv["HASH2"].ToString() != ""))
					{
						e.Row.Cells[iRow].BackColor = System.Drawing.ColorTranslator.FromHtml("#B22222");
					}
				}
				if ((kErr1 == 0) && (kErr2 != 0)) // if version1 not have errors and version2 have errors
				{
					if (e.Row.Cells[3].Text == "")
					{
						if (strComment.Length == 0 && strComment1.Length == 0)
						{
							e.Row.Cells[3].BackColor = System.Drawing.ColorTranslator.FromHtml("transparent");
						}
					}
				}
			}
			// e.Row.Cells[iRow].Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.textDecoration='underline';style.backgroundColor = 'white';";
			// e.Row.Cells[iRow].Attributes["onmouseout"] = "this.style.textDecoration='none'; if (style.backgroundColor != 'green') {style.backgroundColor = 'transparent';};";
			e.Row.Cells[iRow].Attributes["onmouseover"] = "ColorCell(this, 'white');";
			//e.Row.Cells[iRow].Attributes["onmouseout"] = "ColorCell(this, 'transparent');";
			if (e.Row.Cells[iRow].BackColor == System.Drawing.ColorTranslator.FromHtml("#B22222"))
			{
				e.Row.Cells[iRow].Attributes["onmouseout"] = "ColorCell(this, '#B22222');";
			}
			else
			{
				e.Row.Cells[iRow].Attributes["onmouseout"] = "ColorCell(this, 'transparent');";
			}

			iRow++;
			String TEST_RUN_COMMAND = drv["TEST_RUN_COMMAND"].ToString();

			Image imw = new Image();
			imw.ImageUrl = "~/Images/Sign_rerun_green.png";
			imw.ToolTip = TEST_RUN_COMMAND;
			//  imw.Attributes["ondblclick"] = "Rerun(this,'" + drv["RequestID"].ToString() + "','" + TEST_RUN_COMMAND.Replace('"', '`') + "','" + UserName + "')";
			imw.Attributes["onClick"] = "ChangeRerun(this,'" + drv["RequestID"].ToString() + "','" + TEST_RUN_COMMAND.Replace('"', '`') + "','" + CurrentContext.UserName() + "','" + drv["TEST_RUN_ID1"].ToString() + "')";
			e.Row.Cells[iRow].Controls.Add(imw);



			iRow++;
			hr = new HyperLink();
			//   hr.NavigateUrl = CurrentPageName + "?Version1=" + DropDownListVersion1.Text + "&Version2=" + DropDownListVersion2.Text + "&Filter=" + drv["TEST"].ToString().Replace(" ", "_");
			hr.NavigateUrl = GetFilterURL("DBTYPES.DBTYPE='" + drv["DB"].ToString() + "'");
			hr.Text = e.Row.Cells[iRow].Text;
			e.Row.Cells[iRow].Controls.Add(hr);


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
				//  e.Row.Cells[iRow].BackColor = System.Drawing.Color.IndianRed;
				//  e.Row.Cells[iRow2].BackColor = System.Drawing.Color.IndianRed;
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



			if (kCompare != 0) // if 2 versions not have erros but hash1 != Hash2
			{
				if (drv["LASTCOMMENT"].ToString().Contains("old issue"))
				{
					e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#D3D3D3");
				}
				else if (drv["HASH1"].ToString() != drv["HASH2"].ToString())
				{
					e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFF1A6");
				}
			}
			else if (kErr != 0) // errors in 2 versions, set color gray
			{
				e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#D3D3D3");
				iRow = 3;
				strComment = drv["LASTCOMMENT"].ToString();
				e.Row.Cells[iRow].ToolTip = strComment;
				e.Row.Cells[iRow].Text = strComment.Substring(0, strComment.Length > 12 ? 12 : strComment.Length);
				if (e.Row.Cells[iRow].Text.Length < strComment.Length) e.Row.Cells[iRow].Text += "...";
				e.Row.Cells[iRow].Text = "<font color='red'>" + e.Row.Cells[iRow].Text + "</font>";
			}


			if ((kErr1 == 0) && (kErr2 != 0)) // if version1 not have errors and version2 have erors then set "GOOD"
			{
				if (e.Row.Cells[3].Text == "")
				{
					Label mylabel = new Label();
					mylabel.Text = "<center>Good</center>";
					mylabel.ForeColor = System.Drawing.Color.Green;
					e.Row.Cells[3].Controls.Add(mylabel);
				}
			}
		}
	}
	protected void ShowByList_SelectedIndexChanged(object sender, EventArgs e)
	{
		HttpCookie cookieShow = new HttpCookie("showbyCV");
		cookieShow.Value = ShowByList.SelectedValue;
		cookieShow.Expires = DateTime.Now.AddYears(1);
		Response.Cookies.Add(cookieShow);
		Response.Redirect(CurrentPageName + "?" + Request.QueryString);//refreshing the page

	}

	int EquelsFipVersion(string strVer1, string strVer2)
	{
		// return:
		// 0 - Version1 = Version2
		// >0 - Version1 > Version2
		// <0 - Version1 < Version2

		strVer1 = GetFipVersion(strVer1);
		strVer2 = GetFipVersion(strVer2);

		return String.Compare(strVer1, strVer2);
	}
	string GetFipVersion(string strVersion)
	{
		int start = strVersion.IndexOf(".");
		int end = strVersion.IndexOf(".", start + 1);

		return strVersion.Substring(start + 1, end - start - 1);
	}
}