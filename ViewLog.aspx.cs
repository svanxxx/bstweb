using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using BSTStatics;

public partial class SetTestLog : CbstHelper
{

	public string NetworkFormat(string strPath)
	{
		string strReturn = strPath.Replace(string.Format("http://{0}/BST/", BSTStat.mainAddress), "\\\\192.168.0.8\\Public\\BST\\");
		return strReturn;
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		string urlLog = Request.Params["log"];

		if (urlLog == null)
		{
			Response.Clear();
			Response.Write("Error: We need one path for log");
			Response.End();
			return;
		}

		String text = System.IO.File.ReadAllText(NetworkFormat(urlLog));
		Response.Clear();

		string output = text.Replace("http://mps.resnet.com/bst/web/", "").
			Replace(BSTStat.mainAddress, BSTStat.newBSTAddress);
		Response.Write(output);
		Response.End();
	}
}