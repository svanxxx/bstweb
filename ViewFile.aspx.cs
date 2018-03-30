using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

public partial class ViewFile : CbstHelper
{
	protected string m_strFile;
	public ViewFile()
	{
	}
	protected void LoadFile()
	{
		m_strFile = Request.Params["file"];
		FileLabel.Text = m_strFile;
		if (string.IsNullOrEmpty(m_strFile))
		{
			FileTextBox.ReadOnly = true;
			SaveButton.Enabled = false;
		}
		else
		{
			FileTextBox.ReadOnly = false;
			FileTextBox.Text = ViewTextFile(m_strFile, false);
			SaveButton.Enabled = true;
		}
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(Request.Params["file"]) || !Request.Params["file"].Contains("SEQUENCE.TXT"))
			DataGridView.Visible = false;

		if (GetPostBackControl(this) == DataGridView)
			return;

		LoadFile();
	}
	protected void SaveButton_Click(object sender, ImageClickEventArgs e)
	{
		if (string.IsNullOrEmpty(m_strFile) || !File.Exists(m_strFile))
			return;
		StreamWriter swFile = new StreamWriter(m_strFile, false);
		string strData = Request.Form["FileTextBox"];
		if (string.IsNullOrEmpty(strData))
			return;

		bool bChanged = true;
		string strNewData = null;
		while (bChanged)
		{
			strNewData = strData.Replace("\n\n", "\n");
			bChanged = strNewData != strData;
			if (bChanged)
			{
				strData = strNewData;
			}
		}

		if (string.IsNullOrEmpty(strNewData))
			return;
	
		swFile.Write(strNewData);
		
		swFile.Close();
		
		FileTextBox.Text = Request.Form["FileTextBox"];

		FeedLog(DateTime.Now.ToString("HH:mm: ") + UserLabel + ":" + m_strFile + " - OK");

		Response.Redirect(ResolveClientUrl("~\\machines.aspx"));
	}
	protected void RefreshButton_Click(object sender, ImageClickEventArgs e)
	{
		LoadFile();
	}
	protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
	{
		if (e.CommandName == "add")
		{
			int index = Convert.ToInt32(e.CommandArgument);
			GridViewRow row = DataGridView.Rows[index];
			DataSet ds = new DataSet();

			using (SqlConnection conn = new SqlConnection(SqlDataSource1.ConnectionString))
			{
				conn.Open();
				string strSQL = "SELECT BATCH_DATA FROM BATCHES WHERE ID = " + row.Cells[3].Text;
				using (SqlDataAdapter adapter = new SqlDataAdapter(strSQL, conn))
					adapter.Fill(ds);
			}

			FileTextBox.Text += ds.Tables[0].Rows[0][0].ToString();
			FileTextBox.Text += Environment.NewLine;
		}
	}
}
