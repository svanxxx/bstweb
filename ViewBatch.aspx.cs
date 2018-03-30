using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class ViewBatch : CbstHelper
{
    protected bool bCompactMode
    {
        get
        {
            if (string.IsNullOrEmpty(Request.Params["CompactMode"]) || Request.Params["CompactMode"] == "On")
                return true;
            return false;
        }
    }
    private int CountCompactMode = 10;
    protected void Page_Load(object sender, EventArgs e)
    {
        string strParam = (bCompactMode) ? "?CompactMode=Off" : "?CompactMode=On";
        CompactMode.NavigateUrl = CurrentPageName + strParam;
        if (bCompactMode)
            CompactMode.BackColor = System.Drawing.Color.Red;
    }
	 protected void AddButton_Click(object sender, EventArgs e)
	 {
		 SqlDataSource1.InsertParameters["BATCH_NAME"].DefaultValue = "New batch";
		 SqlDataSource1.InsertParameters["BATCH_DATA"].DefaultValue = "put your tests here";
		 SqlDataSource1.Insert();
	 }
	 protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
	 {
		 int[] iIndex = { 3, 5 };
		 string[] strControl = { "DataDridLabel", "PCDridLabel" };

		 if (e.Row.RowType == DataControlRowType.DataRow)
		 {
			if (e.Row.RowState != DataControlRowState.Edit && e.Row.RowState.ToString() != "Alternate, Edit")
			{
				for (int i = 0; i < iIndex.Length; i++)
				{
					if (e.Row.Cells[iIndex[i]].FindControl(strControl[i]) != null)
					{
						Label lb = e.Row.Cells[iIndex[i]].FindControl(strControl[i]) as Label;
						if (lb != null)
						{
							lb.Text = lb.Text.Replace("\r\n", "<br/>");
                            if (bCompactMode)
                            {
                                int iBR = 0;
                                int iPos = 0;
                                while (iBR < CountCompactMode)
                                {
                                    iPos = lb.Text.IndexOf("<br/>", iPos);
                                    if (iPos == -1)
                                        break;
                                    iPos += 5;
                                    iBR++;
                                }
                                if (iBR == CountCompactMode)
                                {
                                    if (lb.Text.Length > iPos)
                                    {
                                        lb.Text = lb.Text.Remove(iPos);
                                        lb.Text += "&nbsp;&nbsp;&nbsp;.&nbsp;&nbsp;.&nbsp;&nbsp;.";
                                    }
                                }
                            }
						}
					}
				}
			}
			else if (e.Row.Cells[3].FindControl("DataTextBox") != null)
			{
                e.Row.Focus();
				for (int i = 0; i < iIndex.Length; i++)
				{
					TextBox lb = e.Row.Cells[iIndex[i]].FindControl(strControl[i]) as TextBox;
					if (lb != null)
					{
						string[] arr = Regex.Split(lb.Text, "\r\n");
						if (arr.Length > 1)
						{
							lb.Rows = arr.Length;
						}
					}
				}
			}
		 }
	 }
}
