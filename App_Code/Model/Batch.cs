using System;

public class Batch: IdBasedObject
{
	public string BATCH_DATA
	{
		get { return this["BATCH_DATA"].ToString(); }
		set { this["BATCH_DATA"] = value; }
	}
	public string BATCH_NAME
	{
		get { return this["BATCH_NAME"].ToString(); }
		set { this["BATCH_NAME"] = value; }
	}
	public Batch(string name)
		: base("BATCHES", new string[] { "ID", "BATCH_DATA", "BATCH_NAME" }, string.Format("(SELECT [ID] FROM [BATCHES] WHERE [BATCH_NAME] = '{0}')", name.ToUpper()))
	{
	}
}