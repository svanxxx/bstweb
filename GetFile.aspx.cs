using System;
using System.IO;
using System.Diagnostics;

public partial class GetFile : CbstHelper
{
	private string ReturnExtension(string fileExtension)
	{
		switch (fileExtension)
		{
			case "txt":
			case "las": return "text/plain";
			case "pdf": return "application/pdf";
			case "swf": return "application/x-shockwave-flash";
			case "gif": return "image/gif";
			case "jpeg": return "image/jpg";
			case "jpg": return "image/jpg";
			case "png": return "image/png";
			case "mp4": return "video/mp4";
			case "mpeg": return "video/mpeg";
			case "mov": return "video/quicktime";
			case "wmv":
			case "avi": return "video/x-ms-wmv";
			default: return "application/octet-stream";
		}
	}
	public string LastName(string strPath)
	{
		int i;
		if (strPath[strPath.Length - 1] == '\\')
		{
			strPath = strPath.Remove(strPath.Length - 1);
		}
		for (i = strPath.Length - 1; (i > 1) && strPath[i] != '\\'; i--) ;
		string strReturn = strPath.Substring(i + 1, strPath.Length - i - 1);
		return strReturn;
	}
	public string GetFormat(string strPath)
	{
		int i;
		for (i = strPath.Length - 1; (i > 1) && strPath[i] != '.'; i--) ;
		string strReturn = strPath.Substring(i + 1, strPath.Length - i - 1);
		return strReturn.ToLower();
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		string tempPath = Path.GetTempPath();
		string strFilePath = Request.Params["Path"];
		strFilePath.Trim('\"');
		Boolean bDirectory = false;

		if (string.IsNullOrEmpty(strFilePath))
		{
			Response.Write("No file to show");
			return;
		}

		if (strFilePath[strFilePath.Length - 1] == '\\')
		{
			if (!Directory.Exists(strFilePath))
			{
				Response.Write("Sorry, directory <b>" + strFilePath + "</b> not exist.");
				return;
			}
			bDirectory = true;

			// set path to rar file
			string strPathRarFile = tempPath + LastName(strFilePath) + "_" + String.Format("{0:yyyy_dd_MM__HH_mm_ss_ff}", System.DateTime.Now) + ".rar";

			// create process
			ProcessStartInfo pWinRar = new ProcessStartInfo();
			pWinRar.CreateNoWindow = true;
			pWinRar.RedirectStandardError = true;
			pWinRar.RedirectStandardOutput = true;
			pWinRar.UseShellExecute = false;
			pWinRar.FileName = @"C:\Program Files\WinRAR\WinRAR.exe";

			Process gitProcess = new Process();
			// C:\Program Files\WinRAR\WinRAR.exe" a -ep "f:\cost.rar" "s:\Tracker\TRFs\"
			pWinRar.Arguments = " a -ep \"" + strPathRarFile + "\" \"" + strFilePath + "\"";
			pWinRar.WorkingDirectory = tempPath;

			gitProcess.StartInfo = pWinRar;
			gitProcess.Start();

			string stderr_str = gitProcess.StandardError.ReadToEnd();  // pick up STDERR
			string stdout_str = gitProcess.StandardOutput.ReadToEnd(); // pick up STDOUT

			gitProcess.WaitForExit();
			gitProcess.Close();

			strFilePath = strPathRarFile;
		}
		if (!File.Exists(strFilePath))
		{
			Response.Write("Sory, file <b>" + strFilePath + "</b> not exist.");
			return;
		}
		FileInfo file_Info = new FileInfo(strFilePath);
		long lFileSize = file_Info.Length;

		Response.ClearContent();
		Response.ClearHeaders();

		Response.ContentType = ReturnExtension(GetFormat(strFilePath));
		Response.AddHeader("Content-Length", lFileSize.ToString());
		Response.AddHeader("Content-Disposition", "filename=" + '"' + LastName(strFilePath) + '"');

		using (var fs = new FileStream(strFilePath, FileMode.Open, FileAccess.Read))
		{
			Response.BufferOutput = false;   // to prevent buffering
			byte[] buffer = new byte[1024 * 1024];
			int bytesRead = 0;
			while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
			{
				Response.OutputStream.Write(buffer, 0, bytesRead);
			}
		}
		Response.Flush();

		// delete rar file, if exist
		if (bDirectory) File.Delete(strFilePath);

		Response.End();

	}
}