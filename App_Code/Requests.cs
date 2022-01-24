using System.Threading.Tasks;
using System.Web.Http;

public class RequestController : ApiController
{
	[HttpGet]
	public async Task<int?> Add(string id, string name, string commands, string batches, string guid, string owner, string version, string comment, string git, int priority)
	{
		//allowed only for local area. until api key is added
		if (Request.RequestUri.Host != "localhost")
		{
			return null;
		}
		return await RequestManager.Add(id, name, commands, batches, guid, owner, version, comment, git, priority);
	}
}
