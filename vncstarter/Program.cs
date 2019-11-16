using System.IO;

namespace vncstarter
{
	class Program
	{
		static void Main(string[] args)
		{
			string machine = Path.GetFileNameWithoutExtension(string.Join("", args));
			System.Diagnostics.Process.Start(@"C:\Program Files\RealVNC\VNC Viewer\vncviewer.exe", " " + machine.Split('(')[0]);
		}
	}
}
