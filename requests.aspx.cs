using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BSTStatics;

public partial class Requests : PagedOutput
{
	public bool ShowAll
	{
		get
		{
			object p = Request.QueryString["showall"];
			bool all = p == null ? false : (Convert.ToInt32(p) > 0);
			return all;
		}
	}
	public string PRogAbb
	{
		get
		{
			object p = Request.QueryString["PROGABB"];
			string pa = p == null ? "" : p.ToString();
			return pa;
		}
	}
	public string Bst
	{
		get
		{
			object p = Request.QueryString["BST"];
			string pa = p == null ? "" : p.ToString();
			return pa;
		}
	}
	void AddFilterProp(ref string oldfilter, string newfilter, string where)
	{
		if (!string.IsNullOrEmpty(newfilter))
		{
			if (string.IsNullOrEmpty(oldfilter))
			{
				oldfilter = " WHERE ";
			}
			else
			{
				oldfilter += " AND ";
			}
			oldfilter += string.Format(where, newfilter);
		}
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		SetDefaultShowBy(20);

		string strFilter = "WHERE R.[IGNORE] IS NULL AND R.[TESTED] IS NULL";
		if (ShowAll)
		{
			strFilter = "";
		}
		AddFilterProp(ref strFilter, PRogAbb, " R.[PROGABB]='{0}'");
		AddFilterProp(ref strFilter, Bst, " R.[USERID] = (SELECT ID FROM PERSONS WHERE USER_LOGIN='{0}')");

		string basesql = string.Format(@"
			SELECT 
			R.[ID] [ID]
			,R.[GUID] [GUID]
			,ROW_NUMBER() OVER (ORDER BY R.[ID] DESC) [#]
			,R.[REQUESTDATETIME] [DATE]
			,R.[TTID] [TTID]
			,(SELECT COUNT(S.[REQUESTID]) FROM [SCHEDULE] S WHERE S.[REQUESTID] = R.[ID]) [N]
			,'' [F]
			,'' [C]
			,(SELECT V.[VERSION] FROM [FIPVERSION] V WHERE V.[ID] = R.[VERSIONID]) [VERSION]
			,R.[PROGABB] [PROGABB]
			,(SELECT P.[USER_LOGIN] FROM [PERSONS] P WHERE P.[ID] = R.[USERID]) [BST]
			,'' [SCHED]
			,R.[COMMENT] [COMMENT]
			,R.[IGNORE] [IGNORE]
			,R.[TESTED] [TESTED]
			,(SELECT P.[USER_LOGIN] FROM [PERSONS] P WHERE P.[ID] = R.[TESTERID]) [TESTER]
			FROM 
			[TESTREQUESTS] R
			{0}
		", strFilter);

		CalcPages(string.Format("SELECT COUNT(*) FROM [TESTREQUESTS] R {0}", strFilter), TTable);

		string sql = string.Format(@"
			SELECT T.* FROM 
			(
				{0}
			) T
			WHERE T.[#] >= {1} AND T.[#] <= {2}
		", basesql, ShowFrom, ShowTo);

		int index = ShowFrom;
		using (DataTable dt = DBHelper.GetDataTable(sql))
		{
			int colcount = dt.Columns.Count;
			TableHeaderRow th = new TableHeaderRow();
			th.TableSection = TableRowSection.TableHeader;
			for (int i = 2; i < colcount; i++)
			{
				th.Cells[th.Cells.Add(new TableHeaderCell())].Text = dt.Columns[i].ToString();
			}
			TTable.Rows.Add(th);

			foreach (DataRow dr in dt.Rows)
			{
				TableRow tr = new TableRow();
				tr.Attributes["guid"] = dr["GUID"].ToString();
				tr.Attributes["requestid"] = dr["ID"].ToString();
				for (int i = 2; i < colcount; i++)
				{
					TableCell c = tr.Cells[tr.Cells.Add(new TableCell())];
					if (dr[i] is DateTime)
					{
						DateTime d = Convert.ToDateTime(dr[i]);
						c.Text = d.ToString("dd.MM HH:mm", CultureInfo.InvariantCulture);
						c.Attributes["title"] = d.ToString();
					}
					else
					{
						c.Text = dr[i].ToString();
						int limit = 100;
						if (dt.Columns[i].ToString() == "COMMENT")
						{
							limit = 45;
						}

						if (c.Text.Length > limit)
						{
							int m = (int)Math.Ceiling((double)c.Text.Length / limit);
							string t = c.Text;
							c.Text = "";
							for (int iline = 0; iline < m; iline++)
							{
								c.Text += t.Substring(iline * limit, Math.Min(limit, t.Length - iline * limit)) + ((iline < m - 1) ? "<br>" : "");
							}
							c.Attributes["title"] = t;
						}

						if (dt.Columns[i].ToString() == "N")
						{
							if (c.Text != "0")
								c.CssClass = "requestprogress";
						}
					}
				}
				TTable.Rows.Add(tr);
			}
		}
	}
}