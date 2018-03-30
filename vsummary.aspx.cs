using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BSTStatics;

public partial class VSummary : PagedOutput
{
	protected void Page_Load(object sender, EventArgs e)
	{
		SetDefaultShowBy(20);
		int start = (CurrentPage - 1) * ShowBy + 1;
		int end = CurrentPage * ShowBy;
		string sql = string.Format(@"
			SELECT
			(SELECT V.[VERSION] FROM [FIPVERSION] V WHERE V.ID = T.TEST_FIPVERSIONID) [VERSION]
			,SUM(T.TEST_DURATION) Duration
			,SUM(T.TEST_DBERRORS) DBERRORS
			,SUM(T.TEST_ERRORS) ERRORS
			,SUM(T.TEST_EXCEPTIONS) EXCEPTIONS
			,SUM(T.TEST_WARNINGS) WARNINGS
			,SUM(T.TEST_OUTPUTERRORS) OUTPUTERRORS
			FROM [TESTRUNS] T
			WHERE T.TEST_FIPVERSIONID IN (SELECT VV.ID FROM (SELECT V.ID, ROW_NUMBER() OVER (ORDER BY ID DESC) ROWNUM FROM [FIPVERSION] V) VV WHERE VV.ROWNUM >= {0} AND VV.ROWNUM <= {1})
			GROUP BY T.TEST_FIPVERSIONID
			ORDER BY T.TEST_FIPVERSIONID DESC
			", start, end);

		object opages = GetValue(string.Format("SELECT COUNT(*) FROM FIPVERSION"));
		int pages = (int)Math.Ceiling((double)Convert.ToInt32(opages) / ShowBy);
		TTable.Attributes["pages"] = pages.ToString();

		int index = start;
		using (DataTable dt = GetDataTable(sql))
		{
			int colcount = dt.Columns.Count;

			TableHeaderRow th = new TableHeaderRow();
			th.TableSection = TableRowSection.TableHeader;
			th.Cells[th.Cells.Add(new TableHeaderCell())].Text = "#";
			for (int i = 0; i < colcount; i++)
			{
				th.Cells[th.Cells.Add(new TableHeaderCell())].Text = dt.Columns[i].ToString();
			}
			TTable.Rows.Add(th);

			foreach (DataRow dr in dt.Rows)
			{
				TableRow tr = new TableRow();
				tr.CssClass = "versionrow";
				tr.Cells[tr.Cells.Add(new TableCell())].Text = (index++).ToString();
				for (int i = 0; i < colcount; i++)
				{
					string colname = th.Cells[i + 1].Text;
					if (colname == "Duration")
					{
						double time = Convert.ToDouble(dr[i]);
						double hours = Math.Floor(time);
						double minutes = (time - hours) * 60.0;
						tr.Cells[tr.Cells.Add(new TableCell())].Text = string.Format("{0}:{1}", hours.ToString("00"), minutes.ToString("00"));
					}
					else
					{
						TableCell c = tr.Cells[tr.Cells.Add(new TableCell())];
						c.Text = dr[i].ToString();
						if (c.Text != "0")
						{
							if (colname == "DBERRORS")
							{
								c.CssClass = "bstdberror";
							}
							else if (colname == "ERRORS")
							{
								c.CssClass = "bsterror";
							}
							else if (colname == "EXCEPTIONS")
							{
								c.CssClass = "bstexception";
							}
							else if (colname == "WARNINGS")
							{
								c.CssClass = "bstwarning";
							}
							else if (colname == "OUTPUTERRORS")
							{
								c.CssClass = "bstoutputerror";
							}
						}
					}
				}
				TTable.Rows.Add(tr);
			}
		}
	}
}