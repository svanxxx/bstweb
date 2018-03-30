public class Version : IdBasedObject
{
	public string VERSION
	{
		get { return this["VERSION"].ToString(); }
		set { this["VERSION"] = value; }
	}
	public Version(string id)
		: base("FIPVERSION", new string[] { "VERSION" }, id)
	{
	}
}