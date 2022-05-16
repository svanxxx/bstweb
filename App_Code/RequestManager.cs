using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class RequestManager
{
	static SemaphoreSlim _Lock = new SemaphoreSlim(1, 1);
	public static async Task<int> Add(string id, string name, string commands, string batches, string guid, string owner, string version, string comment, string git, int priority)
	{
		await _Lock.WaitAsync();
		try
		{
			using (var db = new BST_STATISTICSEntities())
			{
				FIPVERSION dbVersion = await db.FIPVERSIONs.FirstOrDefaultAsync(x => x.VERSION == version);
				if (dbVersion == null)
				{
					dbVersion = new FIPVERSION()
					{
						VERSION = version,
						OFFICIAL = 0,
					};
					db.FIPVERSIONs.Add(dbVersion);
					await db.SaveChangesAsync();
				}
				var user = await db.PERSONS.Where(x => x.USER_LOGIN == "bst").Select(x => x.ID).FirstOrDefaultAsync();
				var request = new TESTREQUEST()
				{
					TTID = "TT" + id + " " + name,
					GUID = guid,
					RequestDateTime = DateTime.Now,
					ProgAbb = owner,
					Comment = comment,
					FIPVERSION = dbVersion,
					IGNORE = null,
					GITHASH = git,
					REQUEST_PRIORITY = priority,
					UserID = user,
				};
				db.TESTREQUESTS.Add(request);
				await db.SaveChangesAsync();

				if (batches == null)
				{
					batches = "";
				}
				var LsBatches = batches.ToUpper().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
				var CorBatches = await db.BATCHES.Where(x => LsBatches.Contains(x.BATCH_NAME.ToUpper())).Select(x => x.BATCH_NAME).ToListAsync();
				StartTest(guid, string.Join(",", CorBatches), commands, priority.ToString());
				return request.ID;
			}
		}
		finally
		{
			_Lock.Release();
		}
	}
	public static void StartTest(string guid, string commaseparatedbatches, string commaseparatedcommands, string priority)
	{
		var hasBatches = !string.IsNullOrEmpty(commaseparatedbatches);
		var hasCommand = !string.IsNullOrEmpty(commaseparatedcommands);
		if (!hasBatches && !hasCommand)
		{
			return;
		}
		BSTUser bu = new BSTUser("", "bst");
		if (hasBatches)
		{
			string[] batches = commaseparatedbatches.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string batch in batches)
			{
				TestRequest.RunBatch4Request(bu.LOGIN, batch, guid, priority);
			}
		}
		if (hasCommand)
		{
			TestRequest tr = new TestRequest("", guid);
			string txtcommands = string.Join("\r\n", commaseparatedcommands.Split(','));
			string[] Commands;
			string[] arrGroup;
			TestRequest.GetCommandsGroups(txtcommands, out Commands, out arrGroup);
			Schedule.AddCommands(Commands, arrGroup, tr.ID.ToString(), bu.ID.ToString(), priority);
		}
	}
}