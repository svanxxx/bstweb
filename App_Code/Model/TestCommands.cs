public class TestCommands : IdBasedObject
{
	public string CMD
	{
		get { return this["CMD"].ToString(); }
		set { this["CMD"] = value.Replace("'", "\""); }
	}
	private static string id
	{
		get
		{
			return CbstHelper.GetValue("SELECT TOP 1 [ID] FROM [TESTS_CMD]").ToString();
		}
	}
	public TestCommands()
		: base("TESTS_CMD", new string[] { "CMD" }, id)
	{
	}
}