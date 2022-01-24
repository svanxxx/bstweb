using System;
using System.Data;
using System.Threading;

public class BackgroundWorker
{
	static BackgroundWorker gWorker;
	public static void Init()
	{
		if (gWorker == null)
		{
			gWorker = new BackgroundWorker();
		}
	}
	public BackgroundWorker()
	{
		Thread obj = new Thread(() =>
		{
			Thread.Sleep(10000);
			while (true)
			{
				Thread.CurrentThread.IsBackground = true;
				using (DataSet ds = CbstHelper.GetDataSet(@"
					SELECT[ID]
					,[TO]
					,[SUBJECT]
					,[BODY]
					,[COLOR]
					FROM [EMAILS]
				"))
				{
					foreach (DataRow r in ds.Tables[0].Rows)
					{
						try
						{
							string res = CbstHelper.SendEmailToBstTeam(r[1].ToString(), r[2].ToString(), r[3].ToString(), r[4].ToString());
							Thread.Sleep(1000);
							if (string.IsNullOrEmpty(res))
							{
								CbstHelper.SQLExecute("DELETE FROM [EMAILS] WHERE [ID] =" + r[0].ToString());
							}
						}
						catch (Exception)
						{

						}
					}
				}
				Thread.Sleep(1000);
			}
		});
		obj.Start();
	}
}