﻿using System;
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
				var request = new TESTREQUEST()
				{
					TTID = "TT" + id + " " + name,
					GUID = guid,
					RequestDateTime = System.DateTime.Now,
					ProgAbb = owner,
					Comment = comment,
					FIPVERSION = dbVersion,
					IGNORE = null,
					GITHASH = git,
					REQUEST_PRIORITY = priority,
				};
				db.TESTREQUESTS.Add(request);
				await db.SaveChangesAsync();

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
		BSTUser bu = new BSTUser("", "bst");
		string[] batches = commaseparatedbatches.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string batch in batches)
		{
			TestRequest.RunBatch4Request(bu.LOGIN, batch, guid, priority);
		}
		if (!string.IsNullOrEmpty(commaseparatedcommands))
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