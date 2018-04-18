using System;

public class Batch : IdBasedObject
{
	static readonly string _btable = "BATCHES";
	static readonly string _bname = "BATCH_NAME";
	static readonly string _bdata = "BATCH_DATA";
	
	public string BATCH_DATA
	{
		get { return this[_bdata].ToString(); }
		set { this[_bdata] = value; }
	}
	public string BATCH_NAME
	{
		get { return this[_bname].ToString(); }
		set { this[_bname] = value; }
	}
	public static int AddBatch(string name)
	{
		return AddObject(_btable, new string[] {_bname, _bdata}, new string[] {name, "put your commands here..."});
	}
	public static void DeleteBatch(string id)
	{
		DeleteObject(_btable, id);
		return;
	}
	public Batch(string id, string name = "")
		: base(_btable, new string[] { "ID", _bdata, _bname },
			!string.IsNullOrEmpty(name) ?
				string.Format("(SELECT [ID] FROM [{0}] WHERE [{1}] = '{2}')", _btable, _bname, name.ToUpper())
				: id
			)
	{
	}
}