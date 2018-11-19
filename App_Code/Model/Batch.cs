using System;
using System.Collections.Generic;
using System.Data;

public class Batch : IdBasedObject
{
	static readonly string _Tabl = "BATCHES";
	static readonly string _bname = "BATCH_NAME";
	static readonly string _bdata = "BATCH_DATA";
	static readonly string _pid = "ID";

	public int ID
	{
		get { return GetAsInt(_pid); }
		set { }
	}
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
		return AddObject(_Tabl, new string[] { _bname, _bdata }, new string[] { name, "put your commands here..." });
	}
	public static void DeleteBatch(string id)
	{
		DeleteObject(_Tabl, id);
		return;
	}
	public Batch(string id, string name = "")
		: base(_Tabl, new string[] { _pid, _bdata, _bname },
			!string.IsNullOrEmpty(name) ?
				string.Format("(SELECT [ID] FROM [{0}] WHERE [{1}] = '{2}')", _Tabl, _bname, name.ToUpper())
				: id
			)
	{
	}

	static object _lockobj = new object();
	static List<string> _Batches = new List<string>();
	public static List<string> Enum()
	{
		lock(_lockobj)
		{
			if (_Batches.Count < 1)
			{
				foreach (DataRow r in DBHelper.GetRows(string.Format("SELECT {0} FROM {1} ORDER BY {0}", _bname, _Tabl)))
				{
					_Batches.Add(r[0].ToString());
				}
			}
			return new List<string>(_Batches);
		}
	}
	public static Batch Find(string name)
	{
		object o = GetRecdata(_Tabl, _pid, _bname, name);
		if (o == DBNull.Value)
		{
			return null;
		}
		return new Batch(o.ToString());
	}
}