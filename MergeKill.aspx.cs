using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MergeKill : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        string target_name = "Merge";
        System.Diagnostics.Process[] local_procs = System.Diagnostics.Process.GetProcesses();
        try
        {
            System.Diagnostics.Process target_proc = local_procs.First(p => p.ProcessName == target_name);
            System.TimeSpan diff = DateTime.Now - target_proc.StartTime;
             target_proc.Kill();

             Label1.Text = "Merge process was killed. It worked " + diff.TotalMinutes +" min.";
        }
        catch (InvalidOperationException)
        {
            Label1.Text = "Error!";
        }
    }
}