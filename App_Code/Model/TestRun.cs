using System;

public class TestRun : IdBasedObject
{
	public string COMMENT
	{
		get { return this["COMMENT"].ToString(); }
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
			this["COMMENT"] = newcomment;
		}
	}
	public string USERID
	{
		get { return this["USERID"].ToString(); }
		set { this["USERID"] = value; }
	}
	public string IGNORE
	{
		get { return this["IGNORE"].ToString(); }
		set { this["IGNORE"] = value; }
	}
	public string VERIFIED_USER_ID
	{
		get { return this["VERIFIED_USER_ID"].ToString(); }
		set { this["VERIFIED_USER_ID"] = value; }
	}
	public string DOCLINK
	{
		get { return this["DOCLINK"].ToString(); }
		set { this["DOCLINK"] = value; }
	}
	public string TestURL
	{
		get
		{
			return string.Format("ViewLog.aspx?log={0}&RUNID={1}", DOCLINK, _id);
		}
	}
	public TestRun(string id)
		: base("TESTRUNS", new string[] { "COMMENT", "USERID", "IGNORE", "VERIFIED_USER_ID", "DOCLINK" }, id)
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