using System;
using System.Text.RegularExpressions;

public class TestRequest : IdBasedObject
{
	static string _testerid = "TESTERID";
	static string[] _allCols = new string[] { "ID", "TTID", "GUID", "REQUESTDATETIME", "USERID", "PROGABB", "COMMENT", "VERSIONID", "IGNORE", "TESTED", "GITHASH", "REQUEST_PRIORITY", _testerid };

	public string USERID
	{
		get { return this["USERID"].ToString(); }
		set { this["USERID"] = value; }
	}
	public string TESTED
	{
		get { return this["TESTED"].ToString(); }
		set { this["TESTED"] = value; }
	}
	public string IGNORE
	{
		get { return this["IGNORE"].ToString(); }
		set { this["IGNORE"] = value; }
	}
	public string PROGABB
	{
		get { return this["PROGABB"].ToString(); }
		set { this["PROGABB"] = value; }
	}
	public string TTID
	{
		get { return this["TTID"].ToString(); }
		set { this["TTID"] = value; }
	}
	public string COMMENT
	{
		get { return this["COMMENT"].ToString(); }
		set { this["COMMENT"] = value; }
	}
	public string VERSIONID
	{
		get { return this["VERSIONID"].ToString(); }
		set { this["VERSIONID"] = value; }
	}
	public string REQUESTDATETIME
	{
		get { return this["REQUESTDATETIME"].ToString(); }
		set { this["REQUESTDATETIME"] = value; }
	}
	public int TESTER
	{
		get { return this[_testerid] == DBNull.Value ? -1 : Convert.ToInt32(this[_testerid]); }
		set
		{
			if (value == -1)
			{
				this[_testerid] = DBNull.Value;
			}
			else
			{
				this[_testerid] = value;
			}
		}
	}
	public int ID
	{
		get { return Convert.ToInt32(this["ID"]); }
		set { this["ID"] = value; }
	}
	public int REQUEST_PRIORITY
	{
		get { return Convert.ToInt32(this["REQUEST_PRIORITY"]); }
		set { this["REQUEST_PRIORITY"] = value; }
	}

	public TestRequest(string id, string guid = "")
	  : base("TESTREQUESTS", _allCols,
		  string.IsNullOrEmpty(id) ?
		  string.Format("(SELECT ID FROM [TESTREQUESTS] WHERE [GUID] = '{0}')", guid)
		  : id)
	{
	}
	public static string ParseChildBatches(string strInput)
	{
		string srtFullBatchParsed = "";
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
					Batch b = Batch.Find(BatchName);
					if (b != null)
					{
						string ChildString = b.BATCH_DATA;
						srtFullBatchParsed += ParseChildBatches(ChildString);
					}
				}
			}
			else
			{
				srtFullBatchParsed += strRead;
				srtFullBatchParsed += Environment.NewLine;
			}
		}
		string[] FinalBatch = srtFullBatchParsed.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
		srtFullBatchParsed = "";
		foreach (string strFinal in FinalBatch)
		{
			if (strFinal.Length != 0)
			{
				srtFullBatchParsed += strFinal;
				srtFullBatchParsed += Environment.NewLine;
			}
		}
		return srtFullBatchParsed;
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

				string strComandPlus = "";

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
	public static void RunBatch4Request(string user, string batchname, string requestGUID, string priority)
	{
		TestRequest tr = new TestRequest("", requestGUID);
		string text = ParseChildBatches(strInput: (new Batch("", batchname)).BATCH_DATA);

		string[] Commands, arrGroup;
		GetCommandsGroups(text, out Commands, out arrGroup);
		Schedule.ExecRequestSQL(Commands, arrGroup, tr.ID.ToString(), (new BSTUser("", user)).ID.ToString(), priority);

		tr.REQUEST_PRIORITY = Convert.ToInt32(priority);
		tr.USERID = (new BSTUser("", "bst")).ID.ToString();
		tr.Store();
	}

}