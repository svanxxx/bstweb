public static class Log
{
	public static void FeedLog(string message)
	{
		message.Trim();
		if (string.IsNullOrEmpty(message))
			return;

		using (var db = new BST_STATISTICSEntities())
		{
			BSTLOG bl = new BSTLOG() { TEXT = message, USERID = CurrentContext.UserID };
			db.BSTLOGs.Add(bl);
			db.SaveChanges();
		}
	}
}