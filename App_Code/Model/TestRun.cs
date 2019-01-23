using System;
using System.Collections.Generic;

public class TestRun : IdBasedObject
{
	static readonly string _comm = "COMMENT";
	static readonly string _usr = "USERID";
	static readonly string _ign = "IGNORE";
	static readonly string _vus = "VERIFIED_USER_ID";
	static readonly string _ttid = "TTID";
	static readonly string _doc = "DOCLINK";
	static string[] _allCols = new string[] { _comm, _usr, _ign, _vus, _doc, _ttid };
	static readonly string _Tabl = "TESTRUNS";

	public string TTID
	{
		get { return this[_ttid].ToString(); }
		set { this[_ttid] = value; }
	}
	public string COMMENT
	{
		get { return this[_comm].ToString(); }
		set
		{
			string oldcomm = COMMENT;
			string newcomment = "";
			if ((string.IsNullOrEmpty(value) && string.IsNullOrEmpty(oldcomm)) || "empty" == value)
			{
				newcomment = "";
				USERID = "";
			}
			else
			{
				BSTUser bu = new BSTUser(USERID);
				string UserName = bu.USER_NAME;
				newcomment = value + string.Format("<br>by: {0} at {1}<br>", UserName, DateTime.Now.ToString()) + oldcomm;
				newcomment = newcomment.Replace("'", "\"");
			}
			this[_comm] = newcomment;
		}
	}
	public string USERID
	{
		get { return this[_usr].ToString(); }
		set { this[_usr] = value; }
	}
	public string IGNORE
	{
		get { return this[_ign].ToString(); }
		set { this[_ign] = value; }
	}
	public string VERIFIED_USER_ID
	{
		get { return this[_vus].ToString(); }
		set { this[_vus] = value; }
	}
	public string DOCLINK
	{
		get { return this[_doc].ToString(); }
		set { this[_doc] = value; }
	}
	public string TestURL
	{
		get
		{
			return string.Format("ViewLog.aspx?log={0}&RUNID={1}", DOCLINK, _id);
		}
	}
	public void AddTTID(string ttid)
	{
		ttid = ttid.ToUpper();
		string[] arr = TTID.ToUpper().Split(',');
		List<string> res = new List<string>();
		foreach (string tt in arr)
		{
			if (tt != ttid)
			{
				res.Add(tt);
			}
		}
		res.Add(ttid);
		TTID = string.Join(",", res);
	}
	public TestRun(string id)
		: base(_Tabl, _allCols, id)
	{
	}
	public static void CommentTestRuns(string ids, string comment)
	{
		string[] sids = ids.Split(',');
		foreach (string id in sids)
		{
			TestRun tr = new TestRun(id) { USERID = CurrentContext.UserID.ToString(), COMMENT = comment };
			tr.Store();
		}
		CbstHelper.FeedLog("Following tests where commented: " + ids);
	}
}