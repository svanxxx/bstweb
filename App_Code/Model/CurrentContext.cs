using System.Web;

public static class CurrentContext
{
	public static string ucook = "userid";
	public static string retiredURL = "retired";
	static string _id = "userid";
	static string _us = "currentuser";
	public static bool Valid
	{
		get
		{
			return User != null;
		}
	}
	public static bool Admin
	{
		get
		{
			return User != null && User.ISADMIN;
		}
	}
	public static int UserID
	{
		get
		{
			return User != null ? User.ID : -1;
		}
	}
	public static string UserName()
	{
		if (!Valid)
		{
			return "";
		}
		return User.USER_NAME;
	}
	public static string UserLogin()
	{
		if (!Valid)
		{
			return "";
		}
		return User.LOGIN;
	}
	static object _lockobject = new object();
	public static BSTUser User
	{
		set
		{
			lock (_lockobject)
			{
				if (value == null)
				{
					HttpContext.Current.Session.Remove(_id);
					HttpContext.Current.Session.Remove(_us);
					return;
				}
				HttpContext.Current.Session[_id] = value.ID;
				HttpContext.Current.Session[_us] = value;
			}
		}
		get
		{
			if (HttpContext.Current.Session == null)
			{
				return null;
			}
			object ous = HttpContext.Current.Session[_us];
			if (ous == null)
			{
				HttpRequest r = HttpContext.Current.Request;
				if (r != null && r.Params["susername"] != null && r.Params["suserpass"] != null)
				{
					ous = BSTUser.FindUser(r.Params["susername"].ToString(), r.Params["suserpass"].ToString());
				}
				else
				{
					HttpCookie cookie = r.Cookies.Get(ucook);
					if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
					{
						int id = -1;
						if (int.TryParse(cookie.Value, out id))
						{
							ous = BSTUser.FindUserbyID(id);
						}
					}
				}
				HttpContext.Current.Session[_us] = ous;
			}
			return (ous as BSTUser);
		}
	}
	public static string CurrentPageName
	{
		get
		{
			System.IO.FileInfo oInfo = new System.IO.FileInfo(HttpContext.Current.Request.Url.AbsolutePath);
			return oInfo.Name;
		}
	}
}