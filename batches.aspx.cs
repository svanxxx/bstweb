using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BSTStatics;

public partial class Batches : PagedOutput
{
	int BatchID
	{
		get
		{
			object o = Request.QueryString["id"];
			return o == null ? -1 : Convert.ToInt32(o);
		}
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		string filter = BatchID == -1 ? "" : ("ID = " + BatchID.ToString());
		CalcPages(string.Format("SELECT COUNT(*) FROM [BATCHES] {0}", string.IsNullOrEmpty(filter) ? "" : " WHERE " + filter), TTable);

		string sql = string.Format(@"
			SELECT T.* FROM 
			(
			SELECT R.[ID]
					,ROW_NUMBER() OVER (ORDER BY R.[ID] DESC) [#]
					,R.[BATCH_NAME]
					,R.[BATCH_DATA]
					,R.[BATCH_COMM]
					,R.[PC_NAME]
			  FROM [BATCHES] R
			) T
			WHERE T.[#] >= {0} AND T.[#] <= {1} {2}
		", ShowFrom, ShowTo, string.IsNullOrEmpty(filter) ? "" : " AND " + filter);

		int startcol = 1;
		using (DataTable dt = DBHelper.GetDataTable(sql))
		{
			int colcount = dt.Columns.Count;
			TableHeaderRow th = new TableHeaderRow();
			th.TableSection = TableRowSection.TableHeader;
			for (int i = startcol; i < colcount; i++)
			{
				th.Cells[th.Cells.Add(new TableHeaderCell())].Text = dt.Columns[i].ToString();
			}
			TTable.Rows.Add(th);

			foreach (DataRow dr in dt.Rows)
			{
				TableRow tr = new TableRow();
				tr.Attributes["rowid"] = dr[0].ToString();
				for (int i = startcol; i < colcount; i++)
				{
					TableCell tc = tr.Cells[tr.Cells.Add(new TableCell())];
					tc.Text = dr[i].ToString();
				}
				TTable.Rows.Add(tr);
			}
		}
	}
}