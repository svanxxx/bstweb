using System.Collections.Generic;
using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class MachinesService : System.Web.Services.WebService
{
	public MachinesService()
	{
	}
	[WebMethod(EnableSession = true)]
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
	[WebMethod(EnableSession = true)]
	public List<MachineState> ChangeStatus(int id, MachineState.MachineStatus status)
	{
		CurrentContext.Validate();
		MachineState ms = new MachineState(id);
		ms.UpdateMachineStatus(status);
		return MachineState.EnumUsed();
	}

	[WebMethod(EnableSession = true)]
	public List<HostState> getHosts()
	{
		CurrentContext.Validate();
		return HostState.EnumUsed();
	}
	[WebMethod(EnableSession = true)]
	public List<HostState> startStopHost(int id, bool start)
	{
		CurrentContext.Validate();
		HostState.StartStopHost(id, start);
		return HostState.EnumUsed();
	}
	[WebMethod(EnableSession = true)]
	public List<HostState> OnlineHost(int id)
	{
		CurrentContext.Validate();
		HostState.OnlineHost(id);
		return HostState.EnumUsed();
	}
	[WebMethod(EnableSession = true)]
	public List<OfflineHost> DeleteHost(int id)
	{
		CurrentContext.Validate();
		HostState.DeleteHost(id);
		return OfflineHost.Enum();
	}
}