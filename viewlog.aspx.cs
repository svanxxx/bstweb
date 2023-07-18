using BSTStatics;
using System;
using System.IO;

public partial class ViewLog : CbstHelper
{

	public string NetworkFormat(string strPath)
	{
		string strReturn = strPath.Replace(
													string.Format("http://{0}/BST/", BSTStat.globalIPAddress),
													string.Format("\\\\{0}\\Public\\BST\\", BSTStat.networkBSTMachine));
		return strReturn;
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		string urlLog = Request.Params["log"];

		if (urlLog == null)
		{
			Response.Clear();
			Response.Write("Error: no file path found in URL.");
			Response.End();
			return;
		}

		string filename = NetworkFormat(urlLog);
		FileInfo fi = new FileInfo(filename);
		if (!fi.Exists)
		{
			Response.Clear();
			Response.Write(string.Format("Error: file not found: {0}", filename));
			Response.End();
			return;
		}

		string text = File.ReadAllText(filename);
		Response.Clear();

		string output = text.Replace("http://mps.efieldpro.com/bst/web/", "").
			Replace(BSTStat.globalIPAddress, Settings.CurrentSettings.BSTADDRESS).
			Replace("192.168.0.8", BSTStat.networkBSTMachine);
		Response.Write(output);
		Response.End();
	}
}