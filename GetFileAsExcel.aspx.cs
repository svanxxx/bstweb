using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Diagnostics;
using System.Collections;
using BSTStatics;

public partial class GetFileAsExcel : CbstHelper
{
	protected void Page_Load(object sender, EventArgs e)
	{
		string tempPath = System.IO.Path.GetTempPath();
		string strFilePath = Request.Params["Path"];

		if (strFilePath == null) return;

		string str1 = string.Format("http://{0}", Settings.CurrentSettings.BSTADDRESS);
		if (strFilePath.IndexOf(str1) == 0)
		{
			strFilePath = strFilePath.Replace(str1, @"//192.168.0.8/public");
		}

		str1 = string.Format("http://{0}", BSTStat.globalIPAddress);
		if (strFilePath.IndexOf(str1) == 0)
		{
			strFilePath = strFilePath.Replace(str1, @"//192.168.0.8/public");
		}

		str1 = Settings.CurrentSettings.COMPANYSITE;
		if (strFilePath.IndexOf(str1) == 0)
		{
			strFilePath = strFilePath.Replace(str1, @"//192.168.0.8/public");
		}

		string ext = Path.GetExtension(strFilePath).ToUpper();
		if (ext != ".TXT")
		{
			ErrorMessage.Text = "Unsupported file format!";
			ErrorMessage.Visible = true;
			return;
		}
		filetoshow.Text = strFilePath.Replace('/', '\\');
		return;

		if (!System.IO.File.Exists(strFilePath))
		{
			Response.Write("Sory, file <b>" + strFilePath + "</b> not exist.");
			return;
		}


		Response.ClearContent();
		Response.ClearHeaders();

		if (ext == "txt")
		{
			System.IO.StreamReader file = new System.IO.StreamReader(strFilePath);
			string line;
			ArrayList htmlbody = new ArrayList();
			while ((line = file.ReadLine()) != null)
			{
				htmlbody.Add(line);
			}
			file.Close();

			string html;
			html = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n";
			html += "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n";
			html += "<head>\r\n";
			html += "<meta http-equiv=\"Content-Type\" content=\"text/html\" />\r\n";
			html += "<style type=\"text/css\">\r\n";
			html += "TABLE {border-collapse: collapse;}\r\n";
			html += "TH, TD {border: 1px solid #CCC;}\r\n";
			html += "table th {background-color: lightblue;} .hover_Row { background-color: yellow; } .clicked_Row { background-color: lightgreen; }\r\n";
			html += "</style>\r\n";
			html += "<script type=\"text/javascript\" src=\"GetFileAsExcel.js\"></script>";
			html += "</head>\r\n";
			html += "<body>\r\n";
			int iTable = 0;
			bool NeedCloseTable = false;
			int LastNumberOfColumns = -1;

			foreach (string strTemp in htmlbody)
			{
				string[] lsColumn = strTemp.Split('\t');
				// Close/Open Table
				if (lsColumn.Count() != LastNumberOfColumns)
				{
					LastNumberOfColumns = lsColumn.Count();
					if (NeedCloseTable)
					{
						html += "</table>\r\n";
						html += "<script type=\"text/javascript\">highlight_Table_Rows(\"color_table_" + iTable.ToString() + "\", \"hover_Row\", \"clicked_Row\");</script>\r\n";
						iTable++;
						NeedCloseTable = false;
					}
					if (lsColumn.Count() > 1)
					{
						html += "<table id=\"color_table_" + iTable.ToString() + "\">\r\n";
						NeedCloseTable = true;
					}
				}

				// Row body
				if (lsColumn.Count() == 1)
				{
					html += strTemp + "<br />\r\n";
				}
				else
				{
					html += "<tr>\r\n";
					for (int i = 0; i < lsColumn.Count(); i++)
					{
						html += "<td>" + ((lsColumn[i] != string.Empty) ? lsColumn[i] : "&nbsp;") + "</td>\r\n";
					}
					html += "</tr>\r\n";
				}
			}
			if (NeedCloseTable)
			{
				html += "</table>\r\n";
				html += "<script type=\"text/javascript\">highlight_Table_Rows(\"color_table_" + iTable.ToString() + "\", \"hover_Row\", \"clicked_Row\");</script>\r\n";
			}
			html += "</body>";
			html += "</html>";

			Response.Write(html);
		}
		else
		{
			Response.Write("Sory, but file <b>" + strFilePath + "</b> is not text.");
		}
		Response.Flush();
		Response.End();
	}
}