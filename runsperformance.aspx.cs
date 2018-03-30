using System;
using System.Data;

public partial class RunsPerformance : CbstHelper
{
	public readonly Channel _channel = new Channel(5, 95);
	protected void Page_Load(object sender, EventArgs e)
	{
		RunsHelper rh = new RunsHelper(Request.QueryString, 1, 10000);
		string sql = string.Format("select * from ({0}) TT", rh.sql);
		using (DataTable dt = GetDataTable(sql))
		{
			foreach (DataRow dr in dt.Rows)
			{
				float x = (float)Convert.ToDateTime(dr["Time"]).ToOADate();
				float y = Convert.ToSingle(dr["Duration"]);
				string css = "";
				if (Convert.ToInt32(dr[RunsHelper.fEx]) > 0)
				{
					css = "data-ex";
				}
				else if (Convert.ToInt32(dr[RunsHelper.fDb]) > 0)
				{
					css = "data-db";
				}
				else if (Convert.ToInt32(dr[RunsHelper.fOu]) > 0)
				{
					css = "data-ou";
				}
				else if (Convert.ToInt32(dr[RunsHelper.fEr]) > 0)
				{
					css = "data-er";
				}
				_channel.Add(x, y, dr["ID"].ToString(), css);
			}
		}
	}
}