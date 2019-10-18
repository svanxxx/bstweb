using System.Collections.Generic;
using System.Data;
using System.Web;

public class RawSettings
{
	static object _lockobject = new object();
	static RawSettings _CurrentSettings = null;
	public static Settings CurrentSettings
	{
		get { return new Settings(CurrentRawSettings); }
		set { CurrentRawSettings = null; }
	}
	public static RawSettings CurrentRawSettings
	{
		set
		{
			lock (_lockobject)
			{
				_CurrentSettings = value;
			}
		}
		get
		{
			lock (_lockobject)
			{
				if (_CurrentSettings == null)
				{
					_CurrentSettings = new RawSettings(true);
				}
				return new RawSettings(_CurrentSettings);
			}
		}
	}

	static readonly string _Tabl = "[SETTINGS]";
	protected virtual string ProcessValue(string val)
	{
		return val;
	}
	string GetVal(string key)
	{
		string val = values.ContainsKey(key) ? values[key] : "";
		return ProcessValue(val);
	}

	public string TASKSSERVICE
	{
		get { return GetVal("TASKSSERVICE"); }
		set { values["TASKSSERVICE"] = value; }
	}
	public string DEFECTLINK
	{
		get { return GetVal("DEFECTLINK"); }
		set { values["DEFECTLINK"] = value; }
	}
	public string SMTPHOST
	{
		get { return GetVal("smtp.Host"); }
		set { values["smtp.Host"] = value; }
	}
	public string SMTPPORT
	{
		get { return GetVal("smtp.Port"); }
		set { values["smtp.Port"] = value; }
	}
	public string SMTPENABLESSL
	{
		get { return GetVal("smtp.EnableSsl"); }
		set { values["smtp.EnableSsl"] = value; }
	}
	public string SMTPTIMEOUT
	{
		get { return GetVal("smtp.Timeout"); }
		set { values["smtp.Timeout"] = value; }
	}
	public string CREDENTIALS1
	{
		get { return GetVal("Credentials1"); }
		set { values["Credentials1"] = value; }
	}
	public string CREDENTIALS2
	{
		get { return GetVal("Credentials2"); }
		set { values["Credentials2"] = value; }
	}
	public string ANGULARCDN
	{
		get { return GetVal("ANGULARCDN"); }
		set { values["ANGULARCDN"] = value; }
	}
	public string TEAMDOMAIN
	{
		get { return GetVal("TEAMDOMAIN"); }
		set { values["TEAMDOMAIN"] = value; }
	}
	public string BOOTCSSCDN
	{
		get { return GetVal("BOOTCSSCDN"); }
		set { values["BOOTCSSCDN"] = value; }
	}
	public string BOOTSTRAPCDN
	{
		get { return GetVal("BOOTSTRAPCDN"); }
		set { values["BOOTSTRAPCDN"] = value; }
	}
	public string MPSCDN
	{
		get { return GetVal("MPSCDN"); }
		set { values["MPSCDN"] = value; }
	}
	public string JQUERYCDN
	{
		get { return GetVal("JQUERYCDN"); }
		set { values["JQUERYCDN"] = value; }
	}
	public string JQUERYUICDN
	{
		get { return GetVal("JQUERYUICDN"); }
		set { values["JQUERYUICDN"] = value; }
	}
	public string COMPANYSITE
	{
		get { return GetVal("COMPANYSITE"); }
		set { values["COMPANYSITE"] = value; }
	}
	public string COMPANYNAME
	{
		get { return GetVal("COMPANYNAME"); }
		set { values["COMPANYNAME"] = value; }
	}
	public string BSTADDRESS
	{
		get { return GetVal("BSTADDRESS"); }
		set { values["BSTADDRESS"] = value; }
	}
	public string BSTSHARE
	{
		get { return GetVal("BSTSHARE"); }
		set { values["BSTSHARE"] = value; }
	}
	public string WORKGIT
	{
		get
		{
			if (System.Diagnostics.Debugger.IsAttached)
			{
				return "S:\\";
			}
			return GetVal("WORKGIT");
		}
		set { values["WORKGIT"] = value; }
	}
	public string ROOTGIT
	{
		get { return GetVal("ROOTGIT"); }
		set { values["ROOTGIT"] = value; }
	}
	public string MERGE_IMAGE_FORMATS
	{
		get { return GetVal("MERGE_IMAGE_FORMATS"); }
		set { values["MERGE_IMAGE_FORMATS"] = value; }
	}
	public string MERGE_TEXT_FORMATS
	{
		get { return GetVal("MERGE_TEXT_FORMATS"); }
		set { values["MERGE_TEXT_FORMATS"] = value; }
	}
	public string USERIMGURL
	{
		get { return GetVal("USERIMGURL"); }
		set { values["USERIMGURL"] = value; }
	}
	public string CDNFONTAWESOME
	{
		get { return GetVal("CDNFONTAWESOME"); }
		set { values["CDNFONTAWESOME"] = value; }
	}
	Dictionary<string, string> values = new Dictionary<string, string>();
	void LoadData()
	{
		foreach (DataRow dr in DBHelper.GetRows(string.Format("SELECT * FROM {0}", _Tabl)))
		{
			values[dr["NAME"].ToString()] = dr["VALUE"].ToString();
		}
	}
	public RawSettings(bool loaddata)
	{
		if (loaddata)
		{
			LoadData();
		}
	}
	public RawSettings()
	{
	}
	public RawSettings(RawSettings o)
	{
		values = new Dictionary<string, string>(o.values);
	}
	public void Store()
	{
		foreach (string key in values.Keys)
		{
			DBHelper.SQLExecute(string.Format("INSERT INTO {0} ([NAME], [VALUE]) SELECT '{1}', '{2}' WHERE NOT EXISTS (SELECT * FROM {0} WHERE NAME = '{1}')", _Tabl, key, values[key]));
			DBHelper.SQLExecute(string.Format("UPDATE {0} SET [VALUE]='{2}' WHERE [NAME] = '{1}'", _Tabl, key, values[key]));
		}
		CurrentRawSettings = null;
	}
}
public class Settings : RawSettings
{
	public Settings(bool loaddata) : base(loaddata)
	{
	}
	public Settings() : base()
	{
	}
	public Settings(RawSettings o) : base(o)
	{
	}
	protected override string ProcessValue(string val)
	{
		if (val.ToUpper().StartsWith("HTTPS://") && !HttpContext.Current.Request.IsSecureConnection)
		{
			return "http" + val.Substring(5);
		}
		return val;
	}
}