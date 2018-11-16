using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;

public class Expression
{
	public Expression(string s)
	{
		val = s;
	}
	public string val { get; set; }
	public override string ToString()
	{
		return val;
	}
}
static class OleDbTypeMap
{
	private static readonly Dictionary<Type, OleDbType> TypeMap = new Dictionary<Type, OleDbType> {
		  {typeof(string), OleDbType.VarChar },
		  {typeof(long), OleDbType.BigInt },
		  {typeof(byte[]), OleDbType.Binary },
		  {typeof(bool), OleDbType.Boolean },
		  {typeof(decimal), OleDbType.Decimal },
		  {typeof(DateTime), OleDbType.Date },
		  {typeof(TimeSpan), OleDbType.DBTime },
		  {typeof(double), OleDbType.Double },
		  {typeof(Exception),OleDbType.Error },
		  {typeof(Guid), OleDbType.Guid },
		  {typeof(int), OleDbType.Integer },
		  {typeof(float), OleDbType.Single },
		  {typeof(short), OleDbType.SmallInt },
		  {typeof(sbyte), OleDbType.TinyInt },
		  {typeof(ulong), OleDbType.UnsignedBigInt },
		  {typeof(uint), OleDbType.UnsignedInt },
		  {typeof(ushort), OleDbType.UnsignedSmallInt },
		  {typeof(byte), OleDbType.UnsignedTinyInt }
	 };
	public static OleDbType GetType(Type type)
	{
		return TypeMap[type];
	}
}
public class IdBasedObject
{
	public static string defDateFormat = "MM-dd-yyyy";
	public static string defDateTimeFormat = "MM-dd-yyyy HH:mm:ss";
	protected string _fldid;
	protected string _id;
	string _table;
	string _view;
	protected List<string> _columns;
	bool[] _modified;
	DataRow _values;

	public int GetID()
	{
		return Convert.ToInt32(this[_fldid]);
	}
	public void FromAnotherObject(IdBasedObject obj)
	{
		foreach (var c in _columns)
		{
			if (obj._columns.Contains(c) && obj[c].ToString() != this[c].ToString())
			{
				this[c] = obj[c];
			}
		}
	}
	protected static object GetRecdata(string table, string returnfield, string filterfield, object value)
	{
		string slq = string.Format("SELECT {0} FROM {1} WHERE {2} = {3}", returnfield, table, filterfield, value.ToString());
		foreach (DataRow row in DBHelper.GetDataSet(slq).Tables[0].Rows)
		{
			return row[returnfield];
		}
		return null;
	}
	protected DataRowCollection GetRecords(string where, int limit = 0)
	{
		string sql = GetBaseSQLQuery(limit) + where;
		return DBHelper.GetDataSet(sql).Tables[0].Rows;
	}
	protected static List<int> EnumRecords(string table, string returnfield, string[] filterfields = null, object[] values = null)
	{
		List<OleDbParameter> pars = new List<OleDbParameter>();
		string where = "";
		if (filterfields != null && filterfields.Length > 0)
		{
			where = " WHERE ";
			for (int i = 0; i < filterfields.Length; i++)
			{
				if (values[i] is Expression)
				{
					where += string.Format("{0} {1} = {2}", i == 0 ? "" : " AND ", filterfields[i], values[i].ToString());
				}
				else
				{
					where += string.Format("{0} {1} = ?", i == 0 ? "" : " AND ", filterfields[i]);
					OleDbParameter p = new OleDbParameter("@" + filterfields[i], OleDbTypeMap.GetType(values[i].GetType()));
					p.Value = values[i];
					pars.Add(p);
				}
			}
		}
		string slq = string.Format("SELECT {0} FROM {1} {2}", returnfield, table, where);

		List<int> res = new List<int>();
		foreach (DataRow row in DBHelper.GetDataSet(slq, pars.ToArray()).Tables[0].Rows)
		{
			res.Add(Convert.ToInt32(row[returnfield]));
		}
		return res;
	}
	protected static object GetValue(string sql)
	{
		return DBHelper.GetValue(sql);
	}
	protected static void SQLExecute(string sql, object[] vals = null)
	{
		List<OleDbParameter> pars = new List<OleDbParameter>();
		if (vals != null)
		{
			foreach (object o in vals)
			{
				OleDbParameter p = new OleDbParameter("@par" + pars.Count.ToString(), OleDbTypeMap.GetType(o.GetType()));
				p.Value = o;
				pars.Add(p);
			}
		}
		DBHelper.SQLExecute(sql, pars.ToArray());
	}
	protected static void DeleteObject(string table, string id, string pcname = "ID")
	{
		DBHelper.SQLExecute(string.Format("DELETE FROM {0} WHERE {1} = {2}", table, pcname, id));
	}
	protected static int AddObject(string table, string[] columns, object[] values, string pcname = "ID")
	{
		List<OleDbParameter> pars = new List<OleDbParameter>();
		string scols = "";
		for (int i = 0; i < columns.Length; i++)
		{
			scols += columns[i] + (i == columns.Length - 1 ? "" : ",");
		}

		string svals = "";
		for (int i = 0; i < columns.Length; i++)
		{
			string v = "";
			if (values[i] is string)
			{
				v = string.Format("'{0}'", values[i].ToString().Replace("'", "''"));
			}
			else if (values[i] is Expression)
			{
				v = values[i].ToString();
			}
			else if (values[i] is DateTime)
			{
				v = "?";
				OleDbParameter p = new OleDbParameter("@" + columns[i], OleDbType.Date);
				p.Value = values[i];
				pars.Add(p);
			}
			else if (values[i] is Array)//getting binary data
			{
				v = "?";
				OleDbParameter p = new OleDbParameter("@" + columns[i], OleDbType.VarBinary);
				p.Value = values[i];
				pars.Add(p);
			}
			else
			{
				v = values[i].ToString();
			}
			svals += v + (i == columns.Length - 1 ? "" : ",");
		}

		string sql = string.Format("INSERT INTO {0} ({1}) {2} VALUES({3})",
			table,
			scols,
			string.IsNullOrEmpty(pcname) ? "" : string.Format("OUTPUT INSERTED.{0}", pcname),
			svals);
		return Convert.ToInt32(DBHelper.GetValue(sql, pars.ToArray()));
	}
	protected string ViewTable
	{
		get { return string.IsNullOrEmpty(_view) ? _table : _view; }
		set { }
	}
	public IdBasedObject(string table, string[] columns, string id, string pcname = "ID", bool doload = true, string view = "")
	{
		_view = view;
		_fldid = pcname;
		_columns = new List<string>(columns);
		_modified = new bool[_columns.Count];
		_table = table;
		_id = id;
		if (doload)
		{
			Load();
		}
		else
		{
			DataTable t = new DataTable();
			foreach (var c in _columns)
			{
				t.Columns.Add(c);
			}
			_values = t.NewRow();
		}
	}
	virtual protected string OnTransformCol(string col)
	{
		return string.Format("[{0}]", col);
	}
	string GetBaseSQLQuery(int limit = 0)
	{
		string slimit = limit > 0 ? string.Format(" TOP {0}", limit) : "";
		string sql = string.Format("SELECT {0} ", slimit);
		foreach (string col in _columns)
		{
			sql += string.Format("{0}, ", OnTransformCol(col));
		}
		sql = sql.Remove(sql.Length - 2);
		sql += string.Format(" FROM {0} ", ViewTable);
		return sql;
	}
	public void Load()
	{
		Load(GetRecords(string.Format(" WHERE [{0}]={1}", _fldid, _id))[0]);
	}
	public void Load(DataRow row)
	{
		_values = row;
		if (string.IsNullOrEmpty(_id) && !string.IsNullOrEmpty(_fldid))
		{
			_id = _values[_fldid].ToString();
		}
	}
	protected virtual void PreStore() { }
	protected virtual void PostStore() { }
	virtual public void Store()
	{
		PreStore();
		int columnsInQuery = 0;
		string sql = string.Format("UPDATE {0} SET ", _table);
		for (int i = 0; i < _columns.Count; i++)
		{
			string col = _columns[i];
			if (col.ToUpper() == _fldid || !_modified[i])
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
					val = string.Format("'{0}'", o.ToString().Replace("'", "''"));
				}
				else if (t == typeof(bool))
				{
					val = string.Format("{0}", Convert.ToByte(o));
				}
				else if (t == typeof(DateTime))
				{
					val = string.Format("'{0}'", Convert.ToDateTime(o).ToString(DBHelper.SQLDateFormat));
				}
				else
				{
					val = string.Format("{0}", o.ToString());
				}
			}
			if (IsColumnComplex(col))
			{
				OnChangeColumn(col, val);
				OnProcessComplexColumn(col, o);
				continue;
			}
			OnChangeColumn(col, val);
			sql += string.Format("[{0}]={1}, ", col, val);
			columnsInQuery++;
		}
		if (columnsInQuery > 0)
		{
			sql = sql.Remove(sql.Length - 2);
			sql += string.Format(" WHERE [{0}]={1}", _fldid, _id);
			DBHelper.SQLExecute(sql);
		}
		PostStore();
	}
	public bool IsLoaded()
	{
		{
			return _values != null;
		}
	}
	public bool IsModified()
	{
		{
			foreach (var m in _modified)
			{
				if (m)
				{
					return true;
				}
			}
			return false;
		}
	}
	protected bool IsModifiedCol(string col)
	{
		{
			for (int i = 0; i < _modified.Length; i++)
			{
				if (_columns[i] == col)
				{
					return _modified[i];
				}
			}
			return false;
		}
	}
	virtual protected bool IsColumnComplex(string col)
	{
		return string.Format("[{0}]", col) != OnTransformCol(col);
	}
	virtual protected void OnProcessComplexColumn(string col, object val)
	{
		throw new Exception("OnProcessComplexColumn should be implemented!");
	}
	virtual protected void OnChangeColumn(string col, string val)
	{
		return;
	}
	public object this[string columnName]
	{
		get
		{
			if (_values == null)
				return DBNull.Value;
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
				if (IsLoaded())
				{
					value = Convert.ChangeType(value, _values.Table.Columns[columnName].DataType);
				}
				_values[columnName] = value;
			}
		}
	}
	protected string GetAsDate(string column, string defDate = DBHelper.sdefaultDate)
	{
		return this[column] == DBNull.Value ? defDate : Convert.ToDateTime(this[column]).ToString(defDateFormat, CultureInfo.InvariantCulture);
	}
	protected string GetAsDateTime(string column, string defDate = DBHelper.sdefaultDate)
	{
		return this[column] == DBNull.Value ? defDate : Convert.ToDateTime(this[column]).ToString(defDateTimeFormat, CultureInfo.InvariantCulture);
	}
	protected int GetAsInt(string column, int defVal = -1)
	{
		return this[column] == DBNull.Value ? defVal : Convert.ToInt32(this[column]);
	}
	protected void SetAsDate(string column, string value)
	{
		var dt = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
		if (value == DBHelper.sdefaultDate)
		{
			this[column] = DBNull.Value;
		}
		else
		{
			this[column] = dt;
		}
	}
	protected bool GetAsBool(string column, bool defVal = false)
	{
		return this[column] == DBNull.Value ? defVal : Convert.ToBoolean(this[column]);
	}
}