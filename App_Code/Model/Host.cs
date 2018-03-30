using System;

public class BstHost : IdBasedObject
{
	public string VERSION
	{
		get { return this["NAME"].ToString(); }
		set { this["NAME"] = value; }
	}
	public bool INACTIVE
	{
		get { return this["INACTIVE"] == DBNull.Value ? false : Convert.ToBoolean(this["INACTIVE"]); }
		set { this["INACTIVE"] = value; }
	}
	public bool POWERON
	{
		get { return Convert.ToBoolean(this["POWERON"]); }
		set { this["POWERON"] = value; }
	}
	public bool POWEROFF
	{
		get { return Convert.ToBoolean(this["POWEROFF"]); }
		set { this["POWEROFF"] = value; }
	}
	public BstHost(string name)
		: base("HOSTS", new string[] { "NAME", "INACTIVE", "POWERON", "POWEROFF" }, string.Format("(SELECT ID FROM HOSTS WHERE NAME = '{0}')", name.ToUpper()))
	{
	}
}