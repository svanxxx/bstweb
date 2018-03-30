using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections;
using System.Data;

public partial class Demo : CbstHelper
{
	protected string DemoFolders
	{
		get
		{
			string strPath = ConfigurationSettings.AppSettings["DemoFolders"];
			if (string.IsNullOrEmpty(strPath))
			{
				Response.Write("Invalid application setting");
				Response.End();
			}
			return strPath;
		}
	}
	protected string DemoFoldersPrefix
	{
		get
		{
			string strPath = ConfigurationSettings.AppSettings["DemoFolderPrefix"];
			if (string.IsNullOrEmpty(strPath))
			{
				Response.Write("Invalid application setting");
				Response.End();
			}
			return strPath;
		}
	}
	protected string GetVersion(string strFolder)
	{
		return strFolder.Replace(DemoFoldersPrefix, "");
	}
	protected int PageNum()
	{
		return Request.QueryString["page"] == null ? 1 : Convert.ToInt32(Request.QueryString["page"].ToString());
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		const int row_in_page = 30;
		int iPage = PageNum();
		DirectoryInfo di = new DirectoryInfo(DemoFolders);
		ArrayList al = new ArrayList();

		//loop through all sub directories
		for (int i = 1; i <= di.GetDirectories().Length / row_in_page + 1; i++)
		{
			if (iPage == i)
			{
				Label lt = new Label();
				lt.Text = i.ToString();
				PagesPanel.Controls.Add(lt);
			}
			else
			{
				HyperLink hr = new HyperLink();
				hr.Text = i.ToString();
				hr.NavigateUrl = CurrentPageName + "?page=" + i.ToString();
				PagesPanel.Controls.Add(hr);
			}

			Literal lb = new Literal();
			lb.Text = "&nbsp";
			PagesPanel.Controls.Add(lb);
		}
		
		foreach (DirectoryInfo sub in di.GetDirectories())
		{
			if (sub.Name.Contains(DemoFoldersPrefix))
				al.Add(sub.Name);
		}
		al.Sort();
		al.Reverse();

		int iCondition = iPage * row_in_page >= al.Count ? al.Count - 1 : iPage * row_in_page - 1;
		int iStart = (iPage - 1) * row_in_page;
		for (int i = iStart; i <= iCondition; i++)
		{
			string str = al[i].ToString();
			TableRow tr = new TableRow();
			tr.BackColor = i % 2 == 0 ? System.Drawing.Color.LightGray : tr.BackColor;
			for (int k = 0; k < DemoTable.Rows[0].Cells.Count; ++k)
			{
				TableCell tCell = new TableCell();
				tr.Cells.Add(tCell);
				tCell.VerticalAlign = VerticalAlign.Top;

				Uri demoFolder = new Uri(DemoFolders);
				if (k == 0)
				{
					HyperLink hl = new HyperLink();
					hl.Text = str;
					hl.NavigateUrl = "http://" + Request.Url.Host + demoFolder.AbsolutePath + str + "/";
					tCell.Controls.Add(hl);
				}
				else
				{
					bool hasFailures = false;
					String final_file = GetFinalFile(str, k - 1, out hasFailures);
					if (final_file == String.Empty)
					{
						tCell.Text = "did not finish";
					}
					else
					{
						HyperLink hl = new HyperLink();
						hl.Text = hasFailures ? "failed" : "passed";
						hl.NavigateUrl = "http://" + Request.Url.Host + demoFolder.AbsolutePath + "/" + str + "/" + final_file;
						if (hasFailures)
						{
							Image imbtn = new Image();
							imbtn.ImageUrl = "~/IMAGES/Error.ico";
							//imbtn.PostBackUrl = hl.NavigateUrl;
							tCell.Controls.Add(imbtn);
						}
						tCell.Controls.Add(hl);
					}
				}
			}

			DemoTable.Rows.Add(tr);
		}
	}

	private string GetFinalFile(string _version, int _type, out bool _has_failures)
	{
		String required_file = String.Empty;
		String file_key = String.Empty;
		_has_failures = false;
		
		switch (_type)
		{
			case 0:
				file_key = "BAK";
				break;
			case 1:
				file_key = "DMP";
				break;
		}

		DirectoryInfo files = new DirectoryInfo(DemoFolders + _version + "\\");
		foreach (FileInfo file in files.GetFiles())
		{
			if (!file.Name.ToUpper().Contains(file_key))
				continue;

			if (file.Name.ToUpper().Contains("FAIL"))
			{
				_has_failures = true;
			}

			if (file.Name.ToUpper().Contains(file_key) && file.Name.ToUpper().Contains("FINAL"))
				required_file = file.Name;
		}

		return required_file;
	}

	private Table GetFilesTable(String _version, int _type, out bool _has_failures)
	{
		bool bHasFailures = false;
		_has_failures = bHasFailures;
		String file_key = String.Empty;

		switch (_type)
		{
			case 0:
				file_key = "FIP";
				break;
			case 1:
				file_key = "BAK";
				break;
			case 2:
				file_key = "DMP";
				break;
		}

		Table inner_table = new Table();
		DirectoryInfo files = new DirectoryInfo(DemoFolders + _version + "\\");
		foreach (FileInfo file in files.GetFiles())
		{
			bHasFailures = false;
			if (!file.Name.ToUpper().Contains(file_key))
				continue;

			if (file.Name.ToUpper().Contains("FAIL"))
			{
				bHasFailures = true;
				_has_failures = true;
			}

			TableRow row = new TableRow();
			inner_table.Rows.Add(row);
			TableCell inner_cell = new TableCell();
			row.Cells.Add(inner_cell);

			Uri uri = new Uri(DemoFolders + file.Name);
			string site_name = file.Name;
			HyperLink hl = new HyperLink();
			hl.Text = site_name;
			hl.NavigateUrl = uri.ToString();
			inner_cell.BackColor = bHasFailures ? System.Drawing.Color.Red : inner_cell.BackColor;
			inner_cell.Controls.Add(hl);
		}

		return inner_table;
	}
}
