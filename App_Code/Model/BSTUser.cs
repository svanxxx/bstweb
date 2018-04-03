public class BSTUser : IdBasedObject
{
	public string USER_NAME
	{
		get { return this["USER_NAME"].ToString(); }
		set { this["USER_NAME"] = value; }
	}
    public string ID
    {
        get { return this["ID"].ToString(); }
        set { this["ID"] = value; }
    }
    public BSTUser(string id, string name = "")
		: base("PERSONS", new string[] { "ID", "USER_NAME", "USER_LOGIN", "USER_PASS", "IS_ADMIN", "IS_GUEST" },
            string.IsNullOrEmpty(id) ? 
            string.Format("(SELECT [ID] FROM [PERSONS] WHERE [USER_LOGIN] = '{0}')", name)
            : id)
	{
	}
}