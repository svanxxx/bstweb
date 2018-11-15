using System;
using System.Linq;
using System.Web;

public class SecurityPage : System.Web.UI.Page
{
	public static string returl = "ReturnUrl";
	public static string loginpage = "login.aspx";

	static void CheckRetired()
	{
		if (CurrentContext.Valid && CurrentContext.User.RETIRED)
		{
			HttpContext.Current.Response.Redirect(string.Format("{0}?{1}=1", loginpage, CurrentContext.retiredURL), false);
			HttpContext.Current.ApplicationInstance.CompleteRequest();
		}
	}
	static public void Static_Page_PreInit()
	{
		if (HttpContext.Current.Request.Url.Segments.Last().ToUpper() == loginpage.ToUpper())
		{
			return;
		}
		if (CurrentContext.Valid)
		{
			CheckRetired();
			return;
		}
		else
		{
			HttpContext.Current.Response.Redirect(loginpage + "?" + returl + "=" + HttpContext.Current.Request.Url.PathAndQuery, false);
			HttpContext.Current.ApplicationInstance.CompleteRequest();
		}
		CheckRetired();
		return;
	}
	protected void Page_PreInit(object sender, EventArgs e)
	{
		Static_Page_PreInit();
	}

	public static string GetPageOgName()
	{
		string ttid = GetPageTTID();
		if (!string.IsNullOrEmpty(ttid))
		{
			return "TT" + ttid;
		}
		return "";
	}
	public static string GetPageOgImage()
	{
		return
			HttpContext.Current.Request.Url.Scheme +
			"://" +
			HttpContext.Current.Request.Url.Host +
			HttpContext.Current.Request.ApplicationPath +
			"/images/gear.jpeg";
	}
	static string GetPageTTID()
	{
		if (HttpContext.Current == null || HttpContext.Current.Request == null)
		{
			return "";
		}
		object o = HttpContext.Current.Request.QueryString[SecurityPage.returl];
		if (o == null)
		{
			return "";
		}
		string findstr = "ttid=";
		string s = o.ToString();
		int ind = s.IndexOf(findstr);
		if (ind < 0)
		{
			return "";
		}
		return s.Substring(ind + findstr.Length);
	}
	public static string GetPageOgTitle()
	{
		return "";
	}
}