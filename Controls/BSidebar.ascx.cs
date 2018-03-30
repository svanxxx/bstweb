using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using BSTStatics;

public partial class Controls_WebUserControl : System.Web.UI.UserControl
{
	protected long m_lIndex;
	public Controls_WebUserControl()
	{
		m_lIndex = 0;
	}
	protected void ReloadPanel()
	{
		Int32 iIndex = 0;
		for (Int32 i = 0; i < Panel1.Controls.Count; i++)
		{
			Panel pnl = Panel1.Controls[i] as Panel;
			if (pnl == null)
				continue;

			for (int j = 0; j < pnl.Controls.Count; j++)
			{
				Control ctrl = pnl.Controls[j];
				Button btn = ctrl as Button;
				if (btn != null)
				{
					btn.Click += new EventHandler(ButtonClick);
					btn.ID = iIndex.ToString();
				}
				else
				{
					ctrl.Visible = m_lIndex == iIndex;
				}
			}
			iIndex++;
		}
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		ReloadPanel();
	}
	protected void ButtonClick(object sender, EventArgs e)
	{
		Button btn = (sender as Button);
		m_lIndex = Convert.ToInt32(btn.ID);
		ReloadPanel();
		UpdatePanel1.Update();
	}
}
