using System;
using System.Collections.Generic;

public partial class Merge : System.Web.UI.Page
{
	static List<string> _txtFormats = new List<string>(new string[] { ".TXT", ".LAS", ".CMG", ".HTML", ".LOG", ".FPX", ".XML", ".FPA", ".IPT", ".HTML", ".HTM" });
	static List<string> _imgFormats = new List<string>(new string[] { ".PNG", ".JPG", ".BMP" });
	public List<String> GetParam(string strParam)
	{
		int iPos, i, k;
		List<String> lstReturn = new List<String>();
		while (strParam.IndexOf('"') != -1)
		{
			iPos = strParam.IndexOf('"');
			k = 1;
			for (i = iPos + 1; (i < strParam.Length) && (strParam[i] != '"'); i++) { k++; }

			if (k != 1)
			{
				lstReturn.Add(strParam.Substring(iPos + 1, k - 1));
			}
			strParam = strParam.Remove(iPos, k + 1);
		}

		return lstReturn;
	}
	public string GetFormat(string strPath)
	{
		string file = Server.UrlDecode(strPath);
		return System.IO.Path.GetExtension(file.Replace("\"", "")).ToUpper();
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		string[] files = Request.QueryString.ToString().Split('&');
		if (files.Length != 2)
		{
			Response.Clear();
			Response.Write("Error: We need two path, like Merge.aspx?\"Path1\"&\"Path2\"");
			Response.End();
			return;
		}

		string strFormat = GetFormat(files[0]);

		if (_txtFormats.Contains(strFormat))
		{
			Response.Redirect(string.Format("compare.aspx?file1={0}&file2={1}", files[0], files[1]));
		}

		if (_imgFormats.Contains(strFormat))
		{
			Response.Redirect(string.Format("compareimg.aspx?file1={0}&file2={1}", files[0], files[1]));
		}

		Response.Clear();
		Response.Write("Error: unsupported file format");
		Response.End();
	}
}