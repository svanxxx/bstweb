<%@ WebHandler Language="C#" Class="BstSignal" %>

using System;
using System.Web;
using System.Collections.Specialized;
using System.Threading;

public class BstSignal : EventsProducerListener, IHttpHandler
{
	public void ProcessRequest(HttpContext context)
	{
		_Response = context.Response;
		_Request = context.Request;

		if (_Request.RawUrl == null)
			return;

		string[] pars = _Request.RawUrl.Split('?');
		if (pars.Length < 2)
			return;

		NameValueCollection cols = HttpUtility.ParseQueryString(pars[1]);
		string events = cols["events"];
		if (string.IsNullOrEmpty(events))
			return;

		foreach (string ev in events.Split(','))
		{
			if (!string.IsNullOrEmpty(ev))
				_eventsState[ev] = "";
		}

		_Response.ContentType = "text/event-stream";
		_Response.CacheControl = "no-cache";
		_Response.Flush();

		while (eventsProducer.IsConnected(this))
		{
			if (!_Response.IsClientConnected)//do self check also to skip timeout in eventsProducer thread to disconnect.
			{
				eventsProducer.FinishContract(this);
				return;
			}
			Thread.Sleep(1000);
		}
	}
	public bool IsReusable
	{
		get
		{
			return false;
		}
	}
}