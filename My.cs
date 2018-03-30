using System.Web;
using System.Data;
using System.Data.OleDb;
using System;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

public partial class CReportParams
{
	public static string GetSrv()
	{
		string strServer = System.Configuration.ConfigurationSettings.AppSettings["BST_DB"];
		return @"Provider=sqloledb;Data Source=" + strServer + @";Initial Catalog=BST_STATISTICS;Trusted_Connection=False;User ID=sa;Password=prosuite";
		//return "192.168.0.1";
	}
	public static string GetConnString()
	{
		return @"Provider=sqloledb;Data Source=" + GetSrv() + @";Initial Catalog=BST_STATISTICS;Trusted_Connection=False;User ID=sa;Password=prosuite";
	}
	public static string GetMaxReportID()
	{
		OleDbConnection conn = new OleDbConnection(GetConnString());
		conn.Open();

		DataSet ds = new DataSet();
		string strSQL = "SELECT MAX(ID) from TESTRUNS";

		OleDbDataAdapter adapter = new OleDbDataAdapter(strSQL, conn);
		adapter.Fill(ds);

		return ds.Tables[0].Rows[0][0].ToString();
		conn.Close();
	}
	public static string GetSTDHeader()
	{
		string strResult = "";
		strResult += "<div style='font-family:Arial;font-size:8pt;'>";
		strResult += "<table border=1 style='border-collapse:collapse;border:none'>";
		strResult += " <TR>";
		strResult += GetSTDLinkItems();
		strResult += " </tr>";
		strResult += "</table>";
		strResult += "</div>";
		return strResult;
	}
	public static string GetSTDFooter()
	{
		string strResult = "";
		strResult += "<div style='border:none;border-bottom:solid windowtext 1.0pt;padding:0cm 0cm 1.0pt 0cm'>";
		strResult += "<p class=MsoNormal><span style='font-family:\"Arial\",\"sans-serif\";color:silver'>&nbsp;</span></p>";
		strResult += "</div>";

		strResult += "<p class=MsoNormal><span style='font-size:8.0pt;font-family:\"Arial\",\"sans-serif\";";
		strResult += "color:silver'>Resource Engineering Systems. Quality Control Page. Created by SVAN©.</span></p>";
		return strResult;
	}
	public static string GetSTDLinkItems()
	{
		string strResult = "";
		strResult += "  <td id=_TAB_INDE_ align=center style='width:3.5cm;background:white'><img src='IMAGES/FAVICON.ICO'/><br/><A HREF=./index.aspx> HOME</A></td>";
		strResult += "  <td id=_TAB_PCHU_ align=center style='width:3.5cm;background:white'><img src='IMAGES/PCS.ICO'/><br/><A HREF=./machines.aspx> HANDLE PCs</A></td>";
		strResult += "  <td align=center style='width:3.5cm;background:white'><img src='IMAGES/DB.ICO'/><br/><A HREF=./demo/>DEMO DBs List</A></td>";
		strResult += "  <td id=_TAB_TSUM_ align=center style='width:3.5cm;background:white'><img src='IMAGES/TESTS.ICO'/><br/><A HREF=./TSummary.aspx>TESTS SUMMARY</A></td>";
		strResult += "  <td id=_TAB_VSUM_ align=center style='width:3.5cm;background:white'><img src='IMAGES/VERSIONS.ICO'/><br/><A HREF=./VSummary.aspx>VERSIONS SUMMARY</A></td>";
		strResult += "  <td id=_TAB_PCSU_ align=center style='width:3.5cm;background:white'><img src='IMAGES/PCS.ICO'/><br/><A HREF=./PCSummary.aspx> PCS SUMMARY</A></td>";
		return strResult;
	}
	public static string ViewTextFile(string strFileName, bool bBR)
	{
		string strResult = "";
		if (System.IO.File.Exists(strFileName))
		{
			System.IO.StreamReader strm = System.IO.File.OpenText(strFileName);
			string strBR = bBR ? "<br/>" : Environment.NewLine;
			while(strm.Peek() != -1)
			{
				strResult = strResult + strm.ReadLine() + strBR;
			}
			strm.Close();
		}
		return strResult;
	}
	public static string GetLogAsCombo(string strPC, string strFileName)
	{
		string strResult = "<OPTION value='Select...'>Select...</OPTION>\n";
		if (!System.IO.File.Exists(strFileName))
			return strResult;
		System.IO.StreamReader strm = System.IO.File.OpenText(strFileName);
		while (strm.Peek() != -1)
		{
			string strLine = strm.ReadLine();
			if (string.IsNullOrEmpty(strLine))
				continue;
			string str1stChar = strLine.Substring(0, 1);
			switch (str1stChar[0])
			{
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					break;
				default:
					continue;
			}
			strResult += "<OPTION value='" + strPC + "'>" + strLine.Substring(23, strLine.Length - 23) + "</OPTION>\n";
		}
		strm.Close();
		return strResult;
	}
	public static string GetMSAccessDate(DateTime dt)
	{
		System.Globalization.CultureInfo cultures = System.Globalization.CultureInfo.InvariantCulture;
		return "'" + dt.ToString("yyyy/MM/dd", cultures) + "'";
	}
	public static string ViewTextFileSorted(string strFileName, bool bBR, bool bAsc)
	{
		string strResult = "";
		if (System.IO.File.Exists(strFileName))
		{
			System.IO.StreamReader strm = System.IO.File.OpenText(strFileName);
			string strBR = bBR ? "<br/>" : Environment.NewLine;
			ArrayList myAL = new ArrayList();
			while (strm.Peek() != -1)
			{
				myAL.Add(strm.ReadLine());
			}

			strm.Close();
			myAL.Sort();
			myAL.Reverse();

			System.Collections.IEnumerator myEnumerator = myAL.GetEnumerator();
			while (myEnumerator.MoveNext())
				strResult = strResult + myEnumerator.Current + strBR;
			return strResult;
		}
		return strResult;
	}
   public static string ViewTextFile1stLine(string strFileName)
   {
      string strResult = "";
      if (System.IO.File.Exists(strFileName))
      {
         System.IO.StreamReader strm = System.IO.File.OpenText(strFileName);
         while (strm.Peek() != -1)
         {
            strResult = strm.ReadLine();
            strm.Close();
            return strResult;
         }
      }
      return strResult;
   }
   public static bool IsPCExists(string strPCName)
   {
      try
      {
         Ping pingSender = new Ping();
         PingReply reply = pingSender.Send(strPCName, 120);
         return reply.Status == IPStatus.Success;
      }
      catch (System.Exception /*ex*/)
      {
         return false;      	
      }
   }
	public static int TextFileNumOfLines(string strFileName)
	{
		int iResult = 0;
		System.IO.StreamReader strm = System.IO.File.OpenText(strFileName); 
		while(strm.Peek() != -1)
		{
			iResult++;
			strm.ReadLine();
		}
		strm.Close();
		return iResult;
	}
}

