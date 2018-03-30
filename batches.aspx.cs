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
	protected void Page_Load(object sender, EventArgs e)
	{
		CalcPages("SELECT COUNT(*) FROM [BATCHES]", TTable);

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
			WHERE T.[#] >= {0} AND T.[#] <= {1}
		", ShowFrom, ShowTo);

		int startcol = 1;
		using (DataTable dt = GetDataTable(sql))
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