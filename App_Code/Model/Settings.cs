using System.Collections.Generic;
using System.Data;

public class Settings
{
	public string SMTPHOST
	{
		get { return values["smtp.Host"]; }
		set { values["smtp.Host"] = value; }
	}
	public string SMTPPORT
	{
		get { return values["smtp.Port"]; }
		set { values["smtp.Port"] = value; }
	}
	public string SMTPENABLESSL
	{
		get { return values["smtp.EnableSsl"]; }
		set { values["smtp.EnableSsl"] = value; }
	}
	public string SMTPTIMEOUT
	{
		get { return values["smtp.Timeout"]; }
		set { values["smtp.Timeout"] = value; }
	}
	public string CREDENTIALS1
	{
		get { return values["Credentials1"]; }
		set { values["Credentials1"] = value; }
	}
	public string CREDENTIALS2
	{
		get { return values["Credentials2"]; }
		set { values["Credentials2"] = value; }
	}
	Dictionary<string, string> values = new Dictionary<string, string>();
	public Settings()
	{
		foreach (DataRow dr in DBHelper.GetRows("SELECT * FROM [SETTINGS]"))
		{
			values[dr["NAME"].ToString()] = dr["VALUE"].ToString();
		}
	}
}