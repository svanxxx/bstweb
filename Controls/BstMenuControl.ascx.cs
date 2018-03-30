using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;

public partial class BstMenu : UserControl
{
	private string m_selItem = "";
	public virtual string SelItem
	{
		get
		{
			return m_selItem;
		}
		set
		{
			m_selItem = value;
		}
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		Int32 iCount = 13;
		while (HeaderTable.Rows[0].Cells.Count > 0)
		{
			HeaderTable.Rows[0].Cells.RemoveAt(0);
            HeaderTable.BackImageUrl = "../IMAGES/tabselectedreverse.JPG";
		}
		for (Int32 iInd = 0; iInd < iCount; iInd++)
		{
			TableCell tc = new TableCell();
			tc.Width = new Unit(100.0 / iCount, UnitType.Percentage);
			tc.Wrap = false;

			Panel pnl = new Panel();
			pnl.HorizontalAlign = HorizontalAlign.Center;
			pnl.Wrap = false;
			pnl.Width = new Unit(100.0, UnitType.Percentage);
			pnl.Height = new Unit(20, UnitType.Pixel);

			HyperLink lbtn = new HyperLink();
			lbtn.Font.Bold = true;
			lbtn.ForeColor = System.Drawing.Color.Yellow;

			switch (iInd)
			{
				case 0:
					lbtn.Text = "BST Home";
					lbtn.NavigateUrl = "~/index.aspx";
					break;
				case 1:
					lbtn.Text = "Handle PCs";
					lbtn.NavigateUrl = "~/machines.aspx";
					break;
				case 2:
					lbtn.Text = "DEMO DBs";
					lbtn.NavigateUrl = "~/Demo.aspx";
					break;
				case 3:
					lbtn.Text = "Tests Summary";
					lbtn.NavigateUrl = "~/TSummary.aspx";
					break;
                case 4:
                    //lbtn.Attributes.Add("class", "VersionsSummary");
                    lbtn.Text = "Versions Summary";
                    lbtn.NavigateUrl = "~/VSummary.aspx";
                    break;
                case 5:
                    lbtn.Attributes.Add("class", "TestRequests");
                    lbtn.Text = "Requests";
                    string strUnRequestCount = Application["UntestedRequestCount"] as string;
                    string NotAnswerRequestCount = Application["NotAnswerRequestCount"] as string;
                   
                    if (strUnRequestCount != "0")
                        {
                            lbtn.Text += " (" + strUnRequestCount + ")";
                        }
                    else if (NotAnswerRequestCount != "0")
                        {
                            lbtn.BackColor = System.Drawing.Color.Blue;
                            lbtn.Text += " (" + NotAnswerRequestCount + ")";
                        }
                    
                    lbtn.NavigateUrl = "~/requests.aspx";
                    break;
                case 6:
                    lbtn.Text = "View Batch";
                    lbtn.NavigateUrl = "~/ViewBatch.aspx";
                    break;
                case 7:
					lbtn.Text = "PCs Summary";
					lbtn.NavigateUrl = "~/PCSummary.aspx";
					break;
				case 8:
					lbtn.Text = "Components";
					lbtn.NavigateUrl = "~/Components.aspx";
					break;
                case 9:
                    lbtn.Text = "Documents";
                    lbtn.NavigateUrl = "~/Documents.aspx";
                    break;
                case 10:
                    lbtn.Text = "Files Statistics";
                    lbtn.NavigateUrl = "~/FilesStatistics.aspx";
                    break;
                case 11:
                    lbtn.Text = "Compare Version";
                    lbtn.NavigateUrl = "~/CompareVersion.aspx";
                    break;
                case 12:
                    lbtn.Text = "Host Control";
                    lbtn.NavigateUrl = "~/hosts.aspx";
                    break;
            }
			if (lbtn.Text == SelItem)
				pnl.BackImageUrl = "../IMAGES/tabselected.JPG";

			lbtn.Width = new Unit(100.0, UnitType.Percentage);
			lbtn.Height = new Unit(100.0, UnitType.Percentage);
			pnl.Controls.Add(lbtn);

			tc.Controls.Add(pnl);

			HeaderTable.Rows[0].Cells.Add(tc);
		}
	}

}
