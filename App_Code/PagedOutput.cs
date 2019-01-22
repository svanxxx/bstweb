using System;
using System.Web;
using System.Web.UI.WebControls;

public class PagedOutput : SecurityPage
{
	public int CurrentPage
	{
		get
		{
			object p = Request.QueryString["page"];
			int page = p == null ? 1 : Convert.ToInt32(p);
			return page;
		}
	}
	private int _DefaultShowBy = 30;
	public void SetDefaultShowBy(int val)
	{
		_DefaultShowBy = val;
	}
	public int ShowBy
	{
		get
		{
			var p = Request.QueryString["showby"];
			int lines = _DefaultShowBy;
			if (p != null)
			{
				lines = Convert.ToInt32(p);
			}
			else
			{
				HttpCookie c = Request.Cookies["showby%40" + CurrentContext.CurrentPageName];
				if (c != null)
				{
					lines = Convert.ToInt32(c.Value);
				}
			}
			return lines;
		}
	}
	private int _Pages = 1;
	public int Pages
	{
		get
		{
			return _Pages;
		}
	}
	public void CalcPages(string sql, WebControl c)
	{
		object opages = DBHelper.GetValue(sql);
		_Pages = (int)Math.Ceiling((double)Convert.ToInt32(opages) / ShowBy);
		c.Attributes["pages"] = Pages.ToString();
	}
	public int ShowFrom
	{
		get
		{
			return (CurrentPage - 1) * ShowBy + 1;
		}
	}
	public int ShowTo
	{
		get
		{
			return CurrentPage * ShowBy;
		}
	}
}