using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class PCSummary : CbstHelper
{
	public PCSummary()
	{
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		BstMenuControl1.SelItem = "PCs Summary";
		//SqlDataSource1.ConnectionString = ConnString;
	}
	protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
            DataRowView drv = (DataRowView)e.Row.DataItem;
            Boolean bUNUSED = drv["UNUSED"].ToString().ToUpper() == "TRUE" ? true : false;
            Boolean bPHYSICAL = drv["PHYSICAL"].ToString().ToUpper() == "TRUE" ? true : false;
            Boolean bNO3DV = drv["NO3DV"].ToString().ToUpper() == "TRUE" ? true : false;
			if (bUNUSED) e.Row.BackColor = System.Drawing.Color.Gray;
		}
              
	}

    protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox checkbox = (CheckBox)sender;
        GridViewRow row = (GridViewRow)checkbox.NamingContainer;
        SQLExecute("UPDATE PCS SET UNUSED = CASE UNUSED WHEN 1 THEN 0 ELSE 1 END WHERE PCNAME = '" + row.Cells[2].Text + "'");
        Response.Redirect(CurrentPageName);
    }
    protected void CheckBox2_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox checkbox = (CheckBox)sender;
        GridViewRow row = (GridViewRow)checkbox.NamingContainer;
        SQLExecute("UPDATE PCS SET PHYSICAL = CASE PHYSICAL WHEN 1 THEN 0 ELSE 1 END WHERE PCNAME = '" + row.Cells[2].Text + "'");
        Response.Redirect(CurrentPageName);
    }
    protected void CheckBox3_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox checkbox = (CheckBox)sender;
        GridViewRow row = (GridViewRow)checkbox.NamingContainer;
        SQLExecute("UPDATE PCS SET NO3DV = CASE NO3DV WHEN 1 THEN 0 ELSE 1 END WHERE PCNAME = '" + row.Cells[2].Text + "'");
        Response.Redirect(CurrentPageName);
    }
}
