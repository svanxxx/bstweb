using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BSTStatics;

public partial class Activity: PagedOutput
{
	protected void Page_Load(object sender, EventArgs e)
	{
		int start = (CurrentPage - 1) * ShowBy + 1;
		int end = CurrentPage * ShowBy;
		string sql = string.Format(@"
			SELECT * FROM 
			(
			SELECT 
				ROW_NUMBER() OVER (ORDER BY L.ID DESC) [#]
				  ,L.[Datetime]
				  ,L.[TEXT] [Action]
				  ,P.[USER_NAME] [User]
			FROM [BSTLOG] L
			INNER JOIN PERSONS P ON P.ID = L.USERID
			) T
			WHERE T.[#] >= {0} AND T.[#] <= {1}
		", start, end);

		object opages = GetValue(string.Format("SELECT COUNT(*) FROM BSTLOG"));
		int pages = (int)Math.Ceiling((double)Convert.ToInt32(opages) / ShowBy);
		TTable.Attributes["pages"] = pages.ToString();

		int index = start;
		using (DataTable dt = GetDataTable(sql))
		{
			int colcount = dt.Columns.Count;

			TableHeaderRow th = new TableHeaderRow();
			th.TableSection = TableRowSection.TableHeader;
			for (int i = 0; i < colcount; i++)
			{
				th.Cells[th.Cells.Add(new TableHeaderCell())].Text = dt.Columns[i].ToString();
			}
			TTable.Rows.Add(th);
			
			foreach (DataRow dr in dt.Rows)
			{
				TableRow tr = new TableRow();
				for (int i = 0; i < colcount; i++)
				{
					TableCell tc = tr.Cells[tr.Cells.Add(new TableCell())];
					string txt = dr[i].ToString();
					string title = txt;
					if (txt.Length > 100)
					{
						tc.Text = txt.Substring(0, 100);
						tc.Attributes["title"] = txt;
					}
					else
					{
						tc.Text = txt;
					}
				}
				TTable.Rows.Add(tr);
			}
		}
	}
}