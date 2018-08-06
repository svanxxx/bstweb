using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using BSTStatics;

public partial class Merge : System.Web.UI.Page
{
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

        if (strFormat == ".TXT" || strFormat == "LAS" || strFormat == ".CMG" || strFormat == ".HTML" || strFormat == ".LOG" || strFormat == ".FPX" || strFormat == ".XML" || strFormat == ".FPA" || strFormat == ".DBS" || strFormat == ".IPT")
			Response.Redirect(string.Format("compare.aspx?file1={0}&file2={1}", files[0], files[1]));

		if (strFormat == ".PNG" || strFormat == ".JPG" || strFormat == ".BMP")
			Response.Redirect(string.Format("compareimg.aspx?file1={0}&file2={1}", files[0], files[1]));

		Response.Clear();
		Response.Write("Error: unsupported file format");
		Response.End();
	}
}