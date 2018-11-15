using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Controls_BstFooterControl : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
		 LabelCell.Text = "Resource Engineering Systems. Quality Control Page. Copyright: " + DateTime.Now.Year.ToString();
		 DateCell.Text = DateTime.Today.ToShortDateString();
		 TableRow tr = new TableRow();
		 TableCell tc = new TableCell();
		 tc.Font.CopyFrom(UserTable.Font);
		 tr.Cells.Add(tc);

		 tc = new TableCell();
		 tc.Font.CopyFrom(UserTable.Font);
		 tc.Text = Session.SessionID;
		 tc.HorizontalAlign = HorizontalAlign.Right;
		 tr.Cells.Add(tc);

		 UserTable.Rows.Add(tr);
    }
}
