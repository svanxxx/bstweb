using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using BSTStatics;

public partial class Sequence : CbstHelper
{
	protected string getBackUrl
	{
		get
		{
			return Request.Params["BackUrl"];
		}
	}
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

	string SQLStopTable = @" SELECT SCHEDULE.COMMAND, PCS.PCNAME, SCHEDULE.SEQUENCEGUID, SCHEDULE.REQUESTID, PCS.ID AS ID_PC
                            FROM SCHEDULE LEFT OUTER JOIN PCS ON SCHEDULE.LOCKEDBY = PCS.ID
                            WHERE (SCHEDULE.REQUESTID = $REQUESTID) and (SCHEDULE.LOCKEDBY is not null) ORDER BY PCS.PCNAME, SCHEDULE.SEQUENCENUMBER";

	string SQLCommandGetSchedules = @"
                            SELECT     SCHEDULE.COMMAND, PCS.PCNAME, PCS_Locked.PCNAME AS PC_Locked, SCHEDULE.SEQUENCEGUID, SCHEDULE.SEQUENCENUMBER, 
                            SCHEDULE.REQUESTID
                            FROM         SCHEDULE LEFT OUTER JOIN
                            PCS AS PCS_Locked ON SCHEDULE.LOCKEDBY = PCS_Locked.ID LEFT OUTER JOIN
                            PCS AS PCS ON SCHEDULE.PCID = PCS.ID
                            WHERE     (SCHEDULE.REQUESTID = $RequestID AND SCHEDULE.LOCKEDBY IS NULL)
                            ORDER BY SCHEDULE.SEQUENCENUMBER
                            ";

	string SQLCommandGetFilesByRequestID = @"SELECT CODEFILES.CODEFILENAME, REQUESTLINKS.RequestID
                            FROM REQUESTLINKS INNER JOIN CODEFILES ON REQUESTLINKS.FileID = CODEFILES.ID
                            WHERE (RequestID = $RequestID)";

	string SQLAdditionalCommandsBefore = @"SELECT COMMAND FROM REQUESTADDITIONALCOMMANDS
                            WHERE (RequestID = $RequestID) AND POSITION = 0 ORDER BY SEQUENCENUMBER";

	string SQLAdditionalCommandsAfter = @"SELECT COMMAND FROM REQUESTADDITIONALCOMMANDS
                            WHERE (RequestID = $RequestID) AND POSITION = 1 ORDER BY SEQUENCENUMBER";

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
	protected String SubString(String str1, String str2)
	{
		str1 = " " + str1;
		String strString = "";
		for (int i = 1; (i <= Math.Min(str1.Length, str2.Length)) && (str1[str1.Length - i] == str2[str2.Length - i]); i++)
		{
			strString = str1[str1.Length - i] + strString;
		}

		int index = strString.IndexOf(" ");
		if (index != -1) strString = strString.Remove(0, index + 1);
		return strString;
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		string RequestID = Request.Params["RequestID"];
		if (string.IsNullOrEmpty(RequestID))
		{
			ImageButtonRun.Visible = false;
		}

		if (!IsPostBack)
		{
			if (!string.IsNullOrEmpty(RequestID))
			{
				// update stop table

				TableRow tRow;
				TableCell tCell;

				DataSet DS = GetDataSet(SQLStopTable.Replace("$REQUESTID", RequestID));

				string strOldGuid = "";
				string strOldThoster = "";
				string strOldComand = "";
				string strThosterID = "";

				int iCount = 0;

				foreach (DataRow rowCur in DS.Tables[0].Rows)
				{
					iCount++;
					if ((rowCur[2].ToString() != strOldGuid) && (strOldGuid != ""))
					{
						tRow = new TableRow();
						tRow.ID = "TR_" + strOldGuid;
						TableStop.Rows.Add(tRow);

						tCell = new TableCell();
						tCell.Text = strOldComand; //rowCur[0].ToString();
						tRow.Cells.Add(tCell);

						tCell = new TableCell();
						tCell.Text = strOldThoster;// rowCur[1].ToString();
						tRow.Cells.Add(tCell);

						tCell = new TableCell();
						tCell.Text = "<input type='button' onClick=\"StopSequence('" + tRow.ClientID + "', '" + strOldGuid + "', '" + strThosterID + "')\"> Stop </input>";// strOldGuid; // rowCur[2].ToString();
						tRow.Cells.Add(tCell);

						strOldComand = rowCur[0].ToString() + "<br>";
						strOldThoster = rowCur[1].ToString() + "<br>";
						strOldGuid = rowCur[2].ToString();
						strThosterID = rowCur[4].ToString();
					}
					else
					{
						strOldComand += rowCur[0].ToString() + "<br>";
						strOldThoster += rowCur[1].ToString() + "<br>";
						strOldGuid = rowCur[2].ToString();
						strThosterID = rowCur[4].ToString();
					}


					if (iCount == DS.Tables[0].Rows.Count)
					{
						tRow = new TableRow();
						tRow.ID = "TR_" + strOldGuid;
						TableStop.Rows.Add(tRow);

						tCell = new TableCell();
						tCell.Text = strOldComand; //rowCur[0].ToString();
						tRow.Cells.Add(tCell);

						tCell = new TableCell();
						tCell.Text = strOldThoster;// rowCur[1].ToString();
						tRow.Cells.Add(tCell);

						tCell = new TableCell();
						tCell.Text = "<input type='button' onClick=\"StopSequence('" + tRow.ClientID + "', '" + strOldGuid + "', '" + strThosterID + "')\"> Stop </input>";// strOldGuid; // rowCur[2].ToString();
						tRow.Cells.Add(tCell);

					}

				}

				if (iCount == 0) TableStop.Visible = false;

				// get version and change Priority:
				DS = GetDataSet(@" SELECT TESTREQUESTS.TTID, TESTREQUESTS.RequestDateTime, PERSONS.USER_LOGIN, 
                                   TESTREQUESTS.ProgAbb, FIPVERSION.VERSION, TESTREQUESTS.Comment, TESTREQUESTS.UserID, 
                                   TESTREQUESTS.ID, TESTREQUESTS.IGNORE, TESTREQUESTS.REQUEST_PRIORITY  
                                   FROM TESTREQUESTS INNER JOIN FIPVERSION ON TESTREQUESTS.VersionID = FIPVERSION.ID LEFT OUTER JOIN PERSONS ON TESTREQUESTS.UserID = PERSONS.ID 
                                where TESTREQUESTS.ID = " + RequestID);

				string strFiPVersion = DS.Tables[0].Rows[0][4].ToString();
				int iPriority = Convert.IsDBNull(DS.Tables[0].Rows[0][9]) ?
									 (
									 isProgramerVersion(strFiPVersion) ? 4 : 3
									 )
									 : Convert.ToInt32(DS.Tables[0].Rows[0][9]);

				PriorityList.ClearSelection();
				PriorityList.Items.FindByValue(iPriority.ToString()).Selected = true;

				// Get Files By Request ID
				string strFiles = "";
				using (DataSet ds = GetDataSet(SQLCommandGetFilesByRequestID.Replace("$RequestID", RequestID)))
				{
					foreach (DataRow rowCur in ds.Tables[0].Rows) strFiles += rowCur[0].ToString() + "\n";
				}

				TextAreaMessage.Text = strFiles != "" ? strFiles : "NO FILES";
				string[] arrfiles = strFiles != "" ? strFiles.Split('\n') : "NO FILES\n".Split('\n');
				lstFile = getSqlFilterByFiles(arrfiles);
				string SQL_CommandByFilter = SQL_Command_1.Replace("$Filter", "and (" + lstFile + ")");

				//SqlDataSource2.SelectCommand = SQL_CommandByFilter;

				List<String> arrCommands = new List<String>();
				List<String> arrGroup = new List<String>();
				List<String> arrStart = new List<String>();
				List<String> arrEnd = new List<String>();

				List<String> arrRUN = new List<String>();


				FileTextBox.Text = "";
				using (DataSet ds = GetDataSet(SQLCommandGetSchedules.Replace("$RequestID", RequestID)))
				{

					foreach (DataRow rowCur in ds.Tables[0].Rows)
					{
						arrCommands.Add(rowCur["COMMAND"].ToString());
						arrGroup.Add(rowCur["SEQUENCEGUID"].ToString());
						arrRUN.Add(rowCur["PC_Locked"].ToString());
						arrStart.Add("");
						arrEnd.Add("");
					}
				}


				String strSubString;

				for (int i = 0; i < arrCommands.Count - 1; i++) // Loop through List with for
				{
					if (arrGroup[i + 1] == arrGroup[i])
					{
						int j;
						strSubString = arrCommands[i];
						for (j = i + 1; (j < arrCommands.Count) && (arrGroup[j] == arrGroup[i]); j++)
						{
							strSubString = SubString(strSubString, arrCommands[j]);
						}
						arrStart[i] = "{\n";

						if (strSubString.Length > 2)
						{

							for (j = i; (j <= arrCommands.Count - 1) && (arrGroup[j] == arrGroup[i]); j++)
							{
								arrCommands[j] = arrCommands[j].Replace(strSubString, "");
							}

							arrEnd[j - 1] = "\n}" + strSubString.Replace("  ", " ");

						}
						else arrEnd[j - 1] = "\n}";
						i = j - 1;


					}

				}



				for (int i = 0; i < arrCommands.Count; i++) // Loop through List with for
				{
					FileTextBox.Text += arrStart[i] + arrCommands[i] + arrEnd[i] + "\n";
				}
				//fill additional commands textboxes
				TextBoxBefore.Text = "";
				DataSet addDS = GetDataSet(SQLAdditionalCommandsBefore.Replace("$RequestID", RequestID));
				foreach (DataRow rowCur in addDS.Tables[0].Rows)
					TextBoxBefore.Text += rowCur[0].ToString() + "\r\n";
				TextBoxAfter.Text = "";
				addDS = GetDataSet(SQLAdditionalCommandsAfter.Replace("$RequestID", RequestID));
				foreach (DataRow rowCur in addDS.Tables[0].Rows)
					TextBoxAfter.Text += rowCur[0].ToString() + "\r\n";
			}
		}
		else
		{

			string SQL_CommandByFilter = SQL_Command_1.Replace("$Filter", "and (" + lstFile + ")");
			//SqlDataSource2.SelectCommand = SQL_CommandByFilter;
		}

	}
	protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
	{
		if (e.CommandName == "add")
		{
			int index = Convert.ToInt32(e.CommandArgument);
			GridViewRow row = DataGridView.Rows[index];

			DataSet ds = new DataSet();
			using (SqlConnection conn = new SqlConnection(SqlDataSource1.ConnectionString))
			{
				conn.Open();
				string strSQL = "SELECT BATCH_DATA FROM BATCHES WHERE ID = " + row.Cells[3].Text;
				using (SqlDataAdapter adapter = new SqlDataAdapter(strSQL, conn))
					adapter.Fill(ds);
			}
			string srtFullBatch = ds.Tables[0].Rows[0][0].ToString();
			FileTextBox.Text += ParseChildBatches(srtFullBatch);
			FileTextBox.Text = FileTextBox.Text.ToUpper();
            List<string> lsSequence = new List<string>(FileTextBox.Text.Split('\n'));
            Boolean isGroupOfTests = false;
            Boolean isDBTypeMSSQL = false;
            Boolean isDBTypeORACLE = false;
            for (int i = 0; i < lsSequence.Count; i++) // Remove spaces between tests and after test name
            {
                if (lsSequence[i] == "\r")
                {
                    lsSequence.RemoveAt(i);
                    i--;
                    continue;
                }
                for (int j = 0; j < lsSequence[i].Length; j++)
                {
                    if (lsSequence[i][j] == ' ' && lsSequence[i][j + 1] == ' ')
                    {
                        lsSequence[i] = lsSequence[i].Remove(j + 1, 1);
                        j--;
                    }
                }
                while (lsSequence[i].EndsWith(" \r"))
                {
                    lsSequence[i] = lsSequence[i].Substring(0, lsSequence[i].Length - 2);
                    lsSequence[i] += "\r";
                }
                if (UseLowerCaseForUsernamesAndPasswords.Checked)
                {
                    string[] commands = { "USER:", "PASS:" };
                    string command;
                    int pos;
                    for (int j = 0; j < commands.Length; j++)
                    {
                        command = commands[j];
                        pos = lsSequence[i].IndexOf(command);
                        if (pos >= 0)
                        {
                            for (int k = pos + command.Length; k < lsSequence[i].Length; k++)
                            {
                                command = command.Insert(command.Length, lsSequence[i][k].ToString().ToLower());
                                if (lsSequence[i][k] == '\"')
                                {
                                    lsSequence[i] = lsSequence[i].Remove(pos, command.Length);
                                    lsSequence[i] = lsSequence[i].Insert(pos, command);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
			if (RemoveIdenticalTests.Checked)
            {
                for (int i = 0; i < lsSequence.Count; i++)
                {
                    for (int j = i + 1; j < lsSequence.Count; j++)
                    {
                        if (lsSequence[j].IndexOf("{") >= 0) // Ignore tests in batches
                        {
                            isGroupOfTests = true;
                            continue;
                        }
                        else if (lsSequence[j].IndexOf("}") >= 0)
                        {
                            isGroupOfTests = false;
                            continue;
                        }
                        if (!isGroupOfTests && lsSequence[i] == lsSequence[j]) // Remove duplicate test, if it's not in batch
                        {
                            lsSequence.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
            if (RemoveIdenticalGroupsOfTests.Checked)
            {
                for (int i = 0; i < lsSequence.Count; i++)
                {
                    if (lsSequence[i].IndexOf("{") >= 0)
                    {
                        isGroupOfTests = true;
                        isDBTypeMSSQL = false;
                        isDBTypeORACLE = false;
                        List<string> lsGroupOfTests = new List<string>();
                        while (isGroupOfTests)
                        {
                            i++;
                            if (lsSequence[i].IndexOf("}") >= 0)
                            {
                                if (lsSequence[i].IndexOf("DBTYPE:MSSQL") >= 0)
                                {
                                    isDBTypeMSSQL = true;
                                }
                                else if (lsSequence[i].IndexOf("DBTYPE:ORACLE") >= 0)
                                {
                                    isDBTypeORACLE = true;
                                }
                                isGroupOfTests = false;
                                break;
                            }
                            lsGroupOfTests.Add(lsSequence[i]);
                        }
                        for (int j = i + 1; j < lsSequence.Count; j++)
                        {
                            if (lsSequence[j].IndexOf("{") >= 0)
                            {
                                int equalElements = 0;
                                j++;
                                while (true)
                                {
                                    if (lsGroupOfTests.IndexOf(lsSequence[j]) >= 0)
                                    {
                                        equalElements++;
                                    }
                                    j++;
                                    if (lsSequence[j].IndexOf("}") >= 0)
                                    {
                                        if ((lsSequence[j].IndexOf("DBTYPE:MSSQL") >= 0 && isDBTypeMSSQL) || (lsSequence[j].IndexOf("DBTYPE:MSSQL") < 0 && !isDBTypeMSSQL))
                                        {
                                            if ((lsSequence[j].IndexOf("DBTYPE:ORACLE") >= 0 && isDBTypeORACLE) || (lsSequence[j].IndexOf("DBTYPE:ORACLE") < 0 && !isDBTypeORACLE))
                                            {
                                                if (equalElements == lsGroupOfTests.Count)
                                                {
                                                    lsSequence.RemoveRange(j - equalElements - 1, equalElements + 2);
                                                    j -= equalElements + 2;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            FileTextBox.Text = String.Join("\n", lsSequence.ToArray());
		}
	}


	protected void ImageButton_Run(object sender, ImageClickEventArgs e)
	{
		string errorMail = "";

		if (!CurrentContext.Admin)
		{
			ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Access denied')", true);
			return;
		}

        string RequestID = Request.Params["RequestID"];
		if (string.IsNullOrEmpty(RequestID))
			RequestID = "NULL";
        else
        {
            FeedLog("Test request has been changed. Task: " + GetValue("SELECT T.TTID FROM TESTREQUESTS T WHERE ID = " + RequestID));
        }

        string strPRIORITY = PriorityList.SelectedItem.Value;

		string[] Commands;
		string[] arrGroup;
		GetCommandsGroups(FileTextBox.Text, out Commands, out arrGroup);

		SQLExecute("DELETE FROM REQUESTADDITIONALCOMMANDS WHERE REQUESTID = " + RequestID);
		SQLExecute("DELETE FROM SCHEDULE WHERE REQUESTID = " + RequestID + " AND LOCKEDBY is NULL");


		//save before and after commands
		string[] commandsBefore = TextBoxBefore.Text.Replace("'", "''").Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
		string[] commandsAfter = TextBoxAfter.Text.Replace("'", "''").Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

		string AdditionalCommandsSQL = "INSERT INTO REQUESTADDITIONALCOMMANDS (COMMAND, REQUESTID, POSITION, SEQUENCENUMBER) VALUES";
		int addNumber = 0;
		foreach (string command in commandsBefore)
		{
			if (command.Trim() != "")
			{
				addNumber++;
				AdditionalCommandsSQL += (addNumber != 1 ? "," : "") + " ( '" + command + "'," + RequestID + "," + 0 + "," + addNumber.ToString() + ") ";
			}
		}
		foreach (string command in commandsAfter)
		{
			if (command.Trim() != "")
			{
				addNumber++;
				AdditionalCommandsSQL += (addNumber != 1 ? "," : "") + " ( '" + command + "'," + RequestID + "," + 1 + "," + addNumber.ToString() + ") ";
			}
		}
		if (addNumber != 0)
			SQLExecute(AdditionalCommandsSQL);

		// get user id

		DataSet DS = GetDataSet(@"select T2.ID from PERSONS T2 where T2.USER_LOGIN = '" + CurrentContext.UserLogin() + "'");
		string strUserID = DS.Tables[0].Rows[0][0].ToString();
		ExecRequestSQL(Commands, arrGroup, RequestID, strUserID, strPRIORITY);

		ImageButtonRun.Enabled = false;
		FileTextBox.Enabled = false;

		//set request priority
		if (RequestID != "NULL")
		{
			SQLExecute(string.Format("update TESTREQUESTS set REQUEST_PRIORITY = {0} where ID = {1}", strPRIORITY, RequestID));
		}
		//set testing person and send email to programmer
		if (RequestID != "NULL")
		{
			TestRequest r = new TestRequest(RequestID);
			Version v = new Version(r.VERSIONID);

			if (r.USERID == "")
			{
				r.USERID = CurrentContext.UserID.ToString();
				r.Store();

				string body = string.Format(@"
				Your request for version ({0}) test was processed by: {1}
				<br>Your version: ({2} <a href='http://{3}/runs.aspx?R.RequestID={4}'>{0}</a>)
				<br>{5}
				<br>{6}
				<br>
				<br>Your version will be tested as soon as possible.
				<br>Person responsible: <b>{1}</b>
				<br>Best regards, <b>{1}</b>
				",
				v.VERSION, CurrentContext.UserName(), r.REQUESTDATETIME, Settings.CurrentSettings.BSTADDRESS, RequestID, r.TTID, r.COMMENT);

				AddEmail(
					r.PROGABB
					,string.Format("Your request for version ({0}) test was processed by: {1}", v.VERSION, CurrentContext.UserName())
					,body
					,"#FEFCFF");
			}
		}

		LabelError.Visible = false;
		if (errorMail != "")
		{
			LabelError.Text = errorMail;
			LabelError.Visible = true;
		}
		else if (getBackUrl != null)
			Response.Redirect(Server.UrlDecode(getBackUrl));
	}
	protected void ImageButton_StopRequest(object sender, ImageClickEventArgs e)
	{
		string RequestID = Request.Params["RequestID"];
		SQLExecute("DELETE FROM SCHEDULE WHERE REQUESTID = " + RequestID);
		SQLExecute("update PCS set ACTIONFLAG = 2 where REQUEST_ID = " + RequestID);
		Response.Redirect(Request.RawUrl);
	}
}