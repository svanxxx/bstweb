using System;
using System.Collections.Generic;
using System.Data;
using BSTStatics;

public class IdBasedObject
{
	protected string _id;
	string _table;
	List<string> _columns;
	bool[] _modified;
	DataRow _values;

	protected static void DeleteObject(string table, string id)
	{
		CbstHelper.SQLExecute(string.Format("DELETE FROM {0} WHERE ID = {1}", table, id));
	}
	protected static int AddObject(string table, string[] columns, object[] values)
	{
		string scols = "";
		for(int i = 0; i < columns.Length; i++)
		{
			scols += columns[i] + (i == columns.Length - 1 ? "" : ",");
		}

		string svals = "";
		for (int i = 0; i < columns.Length; i++)
		{
			string v = "";
			if (values[i] is string)
			{
				v = string.Format("'{0}'", values[i].ToString());
			}
			else
			{
				v = values[i].ToString();
			}
			svals += v + (i == columns.Length - 1 ? "" : ",");
		}

		string sql = string.Format("INSERT INTO {0} ({1}) OUTPUT INSERTED.ID VALUES({2})", table, scols, svals);
		return Convert.ToInt32(CbstHelper.GetValue(sql));
	}
	public IdBasedObject(string table, string[] columns, string id)
	{
		_columns = new List<string>(columns);
		_modified = new bool[_columns.Count];
		_table = table;
		_id = id;
		Load();
	}
	public void Load()
	{
		string sql = "SELECT ";
		foreach (string col in _columns)
		{
			sql += string.Format("[{0}], ", col);
		}
		sql = sql.Remove(sql.Length - 2);
		sql += string.Format("FROM [{0}] WHERE [ID]={1}", _table, _id);
		_values = CbstHelper.GetValues(sql);
	}
	public void Store()
	{
		string sql = string.Format("UPDATE [{0}] SET ", _table);
		for (int i = 0; i < _columns.Count; i++)
		{
			string col = _columns[i];
			if (col.ToUpper() == "ID" || !_modified[i])
			{
				continue;
			}
			object o = _values[col];
			Type t = _values.Table.Columns[col].DataType;
			string val = "NULL";
			if (o != DBNull.Value)
			{
				if (t == typeof(string))
				{
					val = string.Format("'{0}'", o.ToString());
				}
				else if (t == typeof(bool))
				{
					val = string.Format("{0}", Convert.ToByte(o));
				}
				else if (t == typeof(DateTime))
				{
					val = string.Format("'{0}'", Convert.ToDateTime(o).ToString(BSTStat.SQLDateFormat));
				}
				else
				{
					val = string.Format("{0}", o.ToString());
				}
			}
			sql += string.Format("[{0}]={1}, ", col, val);
		}
		sql = sql.Remove(sql.Length - 2);
		sql += string.Format(" WHERE [ID]={0}", _id);
		CbstHelper.SQLExecute(sql);
	}
	public object this[string columnName]
	{
		get
		{
			return _values[columnName];
		}
		set
		{
			_modified[_columns.FindIndex(x => x.ToUpper() == columnName.ToUpper())] = true;
			if (value.ToString() == "")
			{
				_values[columnName] = DBNull.Value;
			}
			else
			{
				value = Convert.ChangeType(value, _values.Table.Columns[columnName].DataType);
				_values[columnName] = value;
			}
			
		}
	}
}