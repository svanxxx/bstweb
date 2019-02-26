using System.Web.Services;

public partial class PutFiles : SecurityPage
{
	[WebMethod]
	public static string GetListFiles(string lstFiles)
	{
		string[] fls = lstFiles.Split('?');

		ListOfChanges changes = new ListOfChanges();
		FileChange c = null;
		foreach (string f in fls)
		{
			string ff = f.Replace("\"", "").Trim();
			if (string.IsNullOrEmpty(ff))
			{
				continue;
			}
			if (c == null)
			{
				c = new FileChange() { NEW = ff };
				changes.Add(c);
			} else
			{
				c.BST = ff;
				c = null;
			}
		}
		return ChangesContainer.Add(changes);
	}
}