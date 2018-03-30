using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BSTStatics;

public partial class Commands : CbstHelper
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			LoadText();
		}
		else
		{
			TestCommands tc = new TestCommands() { CMD = commandstext.InnerText };
			tc.Store();
			LoadText();
		}
	}
	protected void LoadText()
	{
		TestCommands tc = new TestCommands();
		commandstext.InnerText = tc.CMD;
	}
}