using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Master : System.Web.UI.MasterPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!CbstHelper.IsConnected)
		{
			return;
		}
		Control c = FindControl("logina");
		LiteralControl l = (LiteralControl)c.Controls[0];
		l.Text = l.Text.Replace("-Login-", CbstHelper.UserName);
	}
}