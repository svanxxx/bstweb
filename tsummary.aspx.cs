using System;
using System.Data;
using System.Web.UI.WebControls;

public partial class TSummary : CbstHelper
{
	protected void Page_Load(object sender, EventArgs e)
	{
		string sql = @"
			SELECT T.[ID]
				  ,T.[TEST_NAME]
				  ,T.[TEST_GUID]
				  ,T.[TT_IDS]
			FROM [TESTS] T
			ORDER BY T.TEST_NAME
		";
		int index = 1;
		using (DataTable dt = GetDataTable(sql))
		{
			int colcount = dt.Columns.Count;

			TableHeaderRow th = new TableHeaderRow();
			th.TableSection = TableRowSection.TableHeader;
			th.Cells[th.Cells.Add(new TableHeaderCell())].Text = "#";
			for (int i = 1; i < colcount; i++)
			{
				TableCell tc = th.Cells[th.Cells.Add(new TableHeaderCell())];
				tc.Text = dt.Columns[i].ToString();
			}
			TTable.Rows.Add(th);

			foreach (DataRow dr in dt.Rows)
			{
				TableRow tr = new TableRow();
				tr.Attributes["testid"] = dr["ID"].ToString();
				tr.Cells[tr.Cells.Add(new TableCell())].Text = (index++).ToString();
				for (int i = 1; i < colcount; i++)
				{
					tr.Cells[tr.Cells.Add(new TableCell())].Text = dr[i].ToString();
				}
				TTable.Rows.Add(tr);
			}
		}
		sql = @"
			SELECT TOP 100 V.[VERSION]
				FROM [FIPVERSION] V
				ORDER BY V.[ID] DESC
		";
		bool setsel = false;
		using (DataTable dt = GetDataTable(sql))
		{
			foreach (DataRow dr in dt.Rows)
			{
				ListItem li = new ListItem(dr[0].ToString());
				if (!setsel && li.Text.ToUpper().EndsWith("ADMIN"))
				{
					li.Selected = setsel = true;
				}
				version.Items.Add(li);
			}
		}
	}
}