using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Components : CbstHelper
{
	public Components()
	{
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		BstMenuControl1.SelItem = "Components";
	}
	 protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
	 {
		 if (e.Row.RowType == DataControlRowType.DataRow)
		 {
			 e.Row.Cells[1].Text = (e.Row.DataItemIndex + 1).ToString();
			 DataRowView drv = (DataRowView)e.Row.DataItem;
			 if (!Convert.ToBoolean(drv["HASTEST"]))
				 e.Row.BackColor = System.Drawing.Color.LightGray;
			 if ((drv["COMPONENTNAME"].ToString() == "!!! This is newly added record"))
				 e.Row.BackColor = System.Drawing.Color.Red;
		 }
		 if (!string.IsNullOrEmpty(e.Row.Cells[6].Text) && e.Row.Cells[6].Text != "&nbsp;")
		 {
			 HyperLink hr = new HyperLink();
			 hr.Text = "Statistics";
			 hr.NavigateUrl = "index.aspx" + e.Row.Cells[6].Text;
			 hr.NavigateUrl = hr.NavigateUrl.Replace("amp;", "");
			 e.Row.Cells[6].Controls.Add(hr);
		 }
	 }
	 protected void Button1_Click(object sender, EventArgs e)
	 {
		 SqlDataSource3.InsertParameters["COMPONENTNAME"].DefaultValue = "!!! This is newly added record";
		 SqlDataSource3.InsertParameters["HASTEST"].DefaultValue = "false";
		 SqlDataSource3.Insert();
	 }
}
