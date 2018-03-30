using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class FilesStatistics : CbstHelper
{

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

    string SQL_Command_1 = @"select 
 CODEFILES.CODEFILENAME
,TEST_CASES.TEST_CASE_NAME
,COUNT(CODELINKS.ID) as FAILURES
from
CODEFILES, TEST_CASES, TESTRUNS, CODELINKS
where
CODEFILES.ID = CODELINKS.CODE_FILE_ID
and CODELINKS.TEST_RUN_ID = TESTRUNS.ID
and TESTRUNS.TEST_CASE_ID = TEST_CASES.ID
$Filter
group by
CODEFILES.CODEFILENAME,
TEST_CASES.TEST_CASE_NAME
order by FAILURES DESC
";

string SQLCommandGetFilesByRequestID = @"SELECT CODEFILES.CODEFILENAME, REQUESTLINKS.RequestID
FROM REQUESTLINKS INNER JOIN CODEFILES ON REQUESTLINKS.FileID = CODEFILES.ID
WHERE (RequestID = $RequestID)";

    protected void Page_Load(object sender, EventArgs e)
    {
        string SQL_Command = "";
        if (IsPostBack)
        {
            if (lstFile != "")
            {

                SQL_Command = SQL_Command_1.Replace("$Filter", "and (" + lstFile + ")");
                btn_Filter_Off.Visible = true;
                btn_Filter.Visible = false;
            }
            else
            {
                    SQL_Command = SQL_Command_1.Replace("$Filter", "");
                    btn_Filter_Off.Visible = false;
                    btn_Filter.Visible = true;
                
            }
            SqlDataSource1.SelectCommand = SQL_Command;
        }
        else
        {
            string RequestID = Request.Params["RequestID"];

             if (!string.IsNullOrEmpty(RequestID))
                   {
                    string strFiles = "";
                    using (DataSet ds = GetDataSet(SQLCommandGetFilesByRequestID.Replace("$RequestID", RequestID)))
                    {
                     foreach (DataRow rowCur in ds.Tables[0].Rows) strFiles += rowCur[0].ToString() + "\n";
                    }

                    TextAreaMessage.Text = strFiles != ""? strFiles:"NO FILES";
                    string[] arrfiles = strFiles != "" ? strFiles.Split('\n') : "NO FILES\n".Split('\n');
                    lstFile = getSqlFilterByFiles(arrfiles);
                    string SQL_CommandByFilter = SQL_Command_1.Replace("$Filter", "and (" + lstFile + ")");
                   
                     btn_Filter_Off.Visible = true;
                     btn_Filter.Visible = false;
                     TextAreaMessage.ReadOnly = true;
                     SqlDataSource1.SelectCommand = SQL_CommandByFilter;
                     }
                     else
                     {
                      SQL_Command = SQL_Command_1.Replace("$Filter", "");
                      btn_Filter_Off.Visible = false;
                      btn_Filter.Visible = true;
                      SqlDataSource1.SelectCommand = SQL_Command;
                     }
        }

    }

    protected string getSqlFilterByFiles(string[] arrfiles)
    {
        string strFilter = "";
        int i = 0;
        foreach (string strItem in arrfiles)
        {
            if (string.IsNullOrEmpty(strItem))
                continue;
            if (i != 0) strFilter += " OR ";
            strFilter += "CODEFILES.CODEFILENAME LIKE '%" + strItem + "%'";
            i++;
        }
        return strFilter;
    }

    protected void btn_Filter_Click(object sender, EventArgs e)
    {
        if (TextAreaMessage.Text == "") return;
     
        string strFiles = TextAreaMessage.Text;
        strFiles = strFiles.Replace("\r", "");
        string[] arrfiles = strFiles.Split('\n');

         lstFile = getSqlFilterByFiles(arrfiles);

         string  SQL_Command = SQL_Command_1.Replace("$Filter", "and (" + lstFile + ")");

         btn_Filter_Off.Visible = true;
         btn_Filter.Visible = false;
         TextAreaMessage.ReadOnly = true;

        SqlDataSource1.SelectCommand = SQL_Command;
 
    }
    protected void btn_Filter_Off_Click(object sender, EventArgs e)
    {
        lstFile = "";
        string SQL_Command = SQL_Command_1.Replace("$Filter", "");
        btn_Filter_Off.Visible = false;
        btn_Filter.Visible = true;
        TextAreaMessage.ReadOnly = false;
        SqlDataSource1.SelectCommand = SQL_Command;
    }
}
