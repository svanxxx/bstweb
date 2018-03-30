using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using BSTStatics;

public partial class Login : CbstHelper
{
	protected void LoginControl_LoggingIn(object sender, LoginCancelEventArgs e)
	{
		if (Login(LoginControl.UserName, LoginControl.Password, LoginControl.RememberMeSet))
		{
			string strRedirect = Request.QueryString[BSTStat.returnurl];
			if (!string.IsNullOrEmpty(strRedirect))
			{

				Response.Redirect(ResolveClientUrl(strRedirect));
			}
			else
			{
				Response.Redirect(ResolveClientUrl("~\\VSummary.aspx"));
			}
		}
		e.Cancel = true;
		LoginControl.InstructionText = LoginErrorMsg;
		LoginControl.InstructionTextStyle.ForeColor = System.Drawing.Color.RosyBrown;
	}
}
