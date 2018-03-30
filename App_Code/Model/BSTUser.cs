public class BSTUser : IdBasedObject
{
	public string USER_NAME
	{
		get { return this["USER_NAME"].ToString(); }
		set { this["USER_NAME"] = value; }
	}
	public BSTUser(string id)
		: base("PERSONS", new string[] { "USER_NAME", "USER_LOGIN", "USER_PASS", "IS_ADMIN", "IS_GUEST" }, id)
	{
	}
}