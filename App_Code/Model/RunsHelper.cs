using System.Collections.Specialized;

public class RunsHelper
{
	public readonly string sqlCounter = "";
	public readonly string sql = "";
	public readonly string REQUESTID = "";
	static readonly string _fEx = "[Ex R.TEST_EXCEPTIONS]";
	static readonly string _fDb = "[Db R.TEST_DBERRORS]";
	static readonly string _fOu = "[Ou R.TEST_OUTPUTERRORS]";
	static readonly string _fEr = "[Er R.TEST_ERRORS]";
	static readonly string _fWa = "[Wr R.TEST_WARNINGS]";
	public static string fEx { get { return _fEx.Replace("]", "").Replace("[", ""); } }
	public static string fDb { get { return _fDb.Replace("]", "").Replace("[", ""); } }
	public static string fOu { get { return _fOu.Replace("]", "").Replace("[", ""); } }
	public static string fEr { get { return _fEr.Replace("]", "").Replace("[", ""); } }
	public static string fWa { get { return _fWa.Replace("]", "").Replace("[", ""); } }
	public RunsHelper(NameValueCollection QueryString, int start, int end)
	{
		string sqlfilter = "";
		string[] keys = QueryString.AllKeys;
		foreach (string key in keys)
		{
			string filterval = QueryString[key];
			if (key.Contains("."))
			{
				if (filterval.ToUpper() == "NULL")
				{
					sqlfilter += string.Format(" AND {0} IS NULL ", key, filterval);
				}
				else if (filterval.Contains("<>"))
				{
					sqlfilter += string.Format(" AND {0}{1} ", key, filterval);
				}
				else
				{
					sqlfilter += string.Format(" AND {0}='{1}' ", key, filterval);
				}
			}
			if (key.ToUpper().Contains("REQUESTID"))
			{
				REQUESTID = filterval;
			}
		}
		if (!sqlfilter.Contains("R.REPEATED"))
		{
			sqlfilter += " AND R.[REPEATED] = 0 ";
		}

		string sqlCols = string.Format(@"
			R.[ID] [ID]
			,R.[RequestID] [RequestID]
			,ROW_NUMBER() OVER (ORDER BY R.[ID] DESC) [#]
			,T.[TEST_NAME] [Test T.TEST_NAME]
			,V.[VERSION] [Version V.VERSION]
			,P.[PCNAME] [Machine P.PCNAME]
			,D.[DBTYPE] [DBType D.DBTYPE]
			,C.[TEST_CASE_NAME] [TestCase C.TEST_CASE_NAME]
			,R.[TEST_EXCEPTIONS] [Ex R.TEST_EXCEPTIONS]
			,R.[TEST_DBERRORS] [Db R.TEST_DBERRORS]
			,R.[TEST_OUTPUTERRORS] [Ou R.TEST_OUTPUTERRORS]
			,R.[TEST_ERRORS] [Er R.TEST_ERRORS]
			,R.[TEST_WARNINGS] [Wr R.TEST_WARNINGS]
			,R.[TEST_DURATION] [Duration]
			,R.[TEST_RUN_DATE] [Time]
			,R.[DOCLINK] [Link]
			,R.[TEST_RUN_COMMAND] [RR]
			,PR2.[USER_LOGIN] [V]
			,(PR.[USER_LOGIN] + ':' + R.[COMMENT]) [Comment]
			,R.[RUN_HASH] [Hash]
			,R.[BRANCH] [Branch]
			,TR.[TASKID] [TASKID]
		", _fEx, _fDb, _fOu, _fEr, _fWa);

		string sqlJoins = @"
			LEFT JOIN [PERSONS] PR on PR.ID = R.[USERID]
			LEFT JOIN [PERSONS] PR2 on PR2.ID = R.[VERIFIED_USER_ID]
			INNER JOIN [FIPVERSION] V ON V.ID = R.[TEST_FIPVERSIONID]
			INNER JOIN [PCS] P ON P.[ID] = R.[TEST_PC_ID]
			INNER JOIN [TEST_CASES] C ON C.[ID] = R.[TEST_CASE_ID]
			INNER JOIN [DBTYPES] D ON D.[ID] = R.[DBTYPE_ID]
			INNER JOIN [TESTS] T ON T.[ID] = C.[TEST_ID]
			INNER JOIN [TESTREQUESTS] TR ON R.[RequestID] = TR.[ID]
		";
		string sqlJoinsCount = sqlJoins;

		string sqlBase = (@"
			SELECT 
			{0}
			FROM [TESTRUNS] R
			{1}
			WHERE R.[IGNORE] IS NULL {2}
		");

		if (!(sqlfilter.Contains(" AND T.") || sqlfilter.Contains(" AND D.") || sqlfilter.Contains(" AND C.") || sqlfilter.Contains(" AND P.") || sqlfilter.Contains(" AND V.")))
		{
			sqlJoinsCount = "";
		}

		sqlCounter = string.Format(sqlBase, "count(*)", sqlJoinsCount, sqlfilter);

		string sqlFull = string.Format(sqlBase, sqlCols, sqlJoins, sqlfilter);
		sql = string.Format(@"
			SELECT T.* FROM 
			(
				{2}
			) T
			WHERE T.[#] >= {0} AND T.[#] <= {1}
		", start, end, sqlFull);
	}
}