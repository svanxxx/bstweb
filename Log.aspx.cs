using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BSTStatics;

public partial class Log : PagedOutput
{
	protected void Page_Load(object sender, EventArgs e)
	{
		object t = Request.QueryString["thoster"];
		if (t == null)
		{
			t = DBHelper.GetValue("SELECT TOP 1 P.PCNAME FROM PCS P WHERE P.UNUSED = 0 ORDER BY P.PCNAME");
		}
		string sThoster = t.ToString();

		object d = Request.QueryString["date"];
		if (d == null)
		{
			d = DateTime.Today;
		}
		else
		{
			d = DateTime.ParseExact(d.ToString(), BSTStat.defDateFormat, CultureInfo.InvariantCulture);

		}
		DateTime dat = Convert.ToDateTime(d);
		string sDate = dat.ToString(BSTStat.SQLDateFormat);

		object opages = DBHelper.GetValue(string.Format("SELECT COUNT(*) FROM LOGS WHERE PC_ID = (SELECT PCS.ID FROM PCS WHERE PCS.PCNAME = '{0}') AND CONVERT(DATE, ACTION_DATE) = '{1}'", sThoster, sDate));
		int pages = (int)Math.Ceiling((double)Convert.ToInt32(opages) / ShowBy);
		TTable.Attributes["pages"] = pages.ToString();

		int start = (CurrentPage - 1) * ShowBy + 1;
		int end = CurrentPage * ShowBy;
		string sql = string.Format(@"
			SELECT * FROM 
			(
			SELECT 
				ROW_NUMBER() OVER (ORDER BY L.[ACTION_DATE]) [#]
				,L.[ACTION_DATE] [DATETIME]
				,P.[USER_NAME] [USER]
				,L.[ACTION_NAME] [ACTION]
				,L.[ACTION_COMMENT] [ACTION COMMENT]
			FROM [LOGS] L
				INNER JOIN [PERSONS] P ON P.ID = L.USER_ID
				INNER JOIN [PCS] PC ON PC.ID = L.PC_ID
			WHERE 
				CONVERT(DATE, L.ACTION_DATE) = CONVERT(DATE, '{3}')
				AND PC.PCNAME = '{2}'
			) T
			WHERE T.[#] >= {0} AND T.[#] <= {1}
			ORDER BY T.[#]
		", start, end, sThoster, sDate);
		int index = start;
		using (DataTable dt = DBHelper.GetDataTable(sql))
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
					if (dr[i] is DateTime)
					{
						txt = Convert.ToDateTime(dr[i]).ToString("HH:mm");
					}
					
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
		bool setsel = false;
		using (DataTable dt = DBHelper.GetDataTable("SELECT P.PCNAME FROM PCS P WHERE P.UNUSED = 0 ORDER BY P.PCNAME"))
		{
			foreach (DataRow dr in dt.Rows)
			{
				ListItem li = new ListItem(dr[0].ToString());
				if (!setsel && char.IsNumber(li.Text[li.Text.Length - 1]))
				{
					li.Selected = setsel = true;
				}
				thoster.Items.Add(li);
			}
		}
	}
}