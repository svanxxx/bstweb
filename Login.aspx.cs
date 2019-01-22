using System;
using System.Web;

public partial class Login : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			if (Request.QueryString[CurrentContext.retiredURL] != null)
			{
				message.Text = "This user has retired!";
			}
			CurrentContext.User = null;
			return;
		}
		CurrentContext.User = BSTUser.FindUser(usr.Text, pwd.Text);
		if (CurrentContext.Valid)
		{
			if (keeplogged.Checked)
			{
				HttpCookie c = new HttpCookie(
									CurrentContext.ucook, 
									CurrentContext.User.ID.ToString())
				{
					Expires = DateTime.Today.AddYears(1)
				};
				Response.Cookies.Add(c);
			}
			if (Request.QueryString[SecurityPage.returl] != null)
			{
				Response.Redirect(Request.QueryString[SecurityPage.returl].ToString(), false);
			}
			else
			{
				Response.Redirect("~/", false);
			}
			Context.ApplicationInstance.CompleteRequest();
			return;
		}
	}
}
