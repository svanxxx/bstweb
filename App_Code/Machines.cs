using System.Collections.Generic;
using System.Web.Services;
using System.Data;
using System.Linq;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class MachinesService : System.Web.Services.WebService
{
	public MachinesService()
	{
	}
	[WebMethod(EnableSession = true, CacheDuration = 15)]
	public List<MachineState> getMachines()
	{
		CurrentContext.Validate();
		return MachineState.EnumUsed();
	}
	[WebMethod(EnableSession = true)]
	public List<MachineState> PauseOnOff(int id)
	{
		CurrentContext.Validate();
		MachineState.PauseOnOff(id, CurrentContext.UserID);
		return MachineState.EnumUsed();
	}
}