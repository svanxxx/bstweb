using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BSTStatics;

public partial class Runs : PagedOutput
{
	protected void Page_Load(object sender, EventArgs e)
	{
		int start = (CurrentPage - 1) * ShowBy + 1;
		int end = CurrentPage * ShowBy;

		RunsHelper rh = new RunsHelper(Request.QueryString, start, end);
		if (!string.IsNullOrEmpty(rh.REQUESTID))
		{
			TestRequest tr = new TestRequest(rh.REQUESTID);
			requestinfo.InnerHtml = string.Format("<strong><a href='requests.aspx?PROGABB={0}'>{0}</a>:</strong>{1}", tr.PROGABB, ReplaceTT(tr.TTID));
			requestinfo.Visible = true;
		}

		object opages = GetValue(rh.sqlCounter);

		int pages = (int)Math.Ceiling((double)Convert.ToInt32(opages) / ShowBy);
		TTable.Attributes["pages"] = pages.ToString();

		int numOfInternalCols = 2;

		int index = start;
		using (DataTable dt = GetDataTable(rh.sql))
		{
			int colcount = dt.Columns.Count;

			TableHeaderRow th = new TableHeaderRow();
			th.TableSection = TableRowSection.TableHeader;
			for (int i = numOfInternalCols; i < colcount; i++)
			{
				string captionfilter = dt.Columns[i].ToString();
				Type t = dt.Columns[i].DataType;
				string[] dat = captionfilter.Split(' ');

				TableCell tc = th.Cells[th.Cells.Add(new TableHeaderCell())];
				tc.Text = dat[0];
				if (dat.Length > 1)
				{
					tc.Attributes["filter"] = dat[1];
					if (t == typeof(string))
					{
						tc.Attributes["filtertype"] = "string";
					}
					else if (t == typeof(int))
					{
						tc.Attributes["filtertype"] = "int";
					}
				}
			}
			TTable.Rows.Add(th);

			foreach (DataRow dr in dt.Rows)
			{
				TableRow tr = new TableRow();
				tr.Attributes["runid"] = dr[0].ToString();
				tr.Attributes["requestid"] = dr[1].ToString();
				for (int i = numOfInternalCols; i < colcount; i++)
				{
					TableCell tc = tr.Cells[tr.Cells.Add(new TableCell())];
					string txt = "";
					if (dr[i] is DateTime)
					{
						txt = Convert.ToDateTime(dr[i]).ToString("dd.MM HH:mm", CultureInfo.InvariantCulture);
						tc.Attributes["title"] = Convert.ToDateTime(dr[i]).ToString();
					}
					else if (dt.Columns[i].Caption == "Duration")
					{
						double time = Convert.ToDouble(dr[i]);
						double hours = Math.Floor(time);
						double minutes = Math.Round(60.0 * (time - hours), 0);
						txt = hours.ToString("00") + ":" + minutes.ToString("00");
					}
					else
					{
						txt = dr[i].ToString();
					}

					if (dt.Columns[i].Caption == "Comment")
					{
						string cellval = txt.Split(new string[] { "<br>" }, StringSplitOptions.None)[0];
						if (cellval.Length > 40)
							cellval = cellval.Substring(0, 40) + "...";
						else if (!string.IsNullOrEmpty(cellval) && txt.IndexOf("<br>") > -1)
							cellval += "...";
						tc.Text = cellval;
						tc.Attributes["title"] = txt.Replace("<br>", "\n");
					}
					else
					{
						tc.Text = txt;
					}

					string colname = th.Cells[i - numOfInternalCols].Text;
					if (colname == "Ex" && tc.Text != "0")
					{
						tc.CssClass = "bstexception";
					}
					else if (colname == "Db" && tc.Text != "0")
					{
						tc.CssClass = "bstdberror";
					}
					else if (colname == "Ou" && tc.Text != "0")
					{
						tc.CssClass = "bstoutputerror";
					}
					else if (colname == "Wr" && tc.Text != "0")
					{
						tc.CssClass = "bstwarning";
					}
					else if (colname == "Er" && tc.Text != "0")
					{
						tc.CssClass = "bsterror";
					}
					else if (colname == "Link")
					{
						tc.Text = "<a href='ViewLog.aspx?log=" + txt + "&RUNID=" + dr[0].ToString() + "'>Link</a>";
					}
					else if (colname == "RR")
					{
						tc.ToolTip = txt;
						tc.Text = "<img src='images/Sign_rerun_green.png'></img>";
						tc.Attributes["onclick"] = "rerun(this)";
					}
					else if (colname == "V" && !string.IsNullOrEmpty(txt))
					{
						tc.ToolTip = txt;
						tc.Text = "<img class='imgpers' src='images/persons/" + txt + ".jpg'></img>";
					}

					if (string.IsNullOrEmpty(tr.CssClass) && !string.IsNullOrEmpty(tc.CssClass))
					{
						tr.CssClass = tc.CssClass;
					}
				}
				TTable.Rows.Add(tr);
			}
		}
	}
}