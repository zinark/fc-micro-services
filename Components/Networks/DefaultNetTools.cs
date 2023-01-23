using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

using static FCMicroservices.Components.Networks.INetTools;

namespace FCMicroservices.Components.Networks;

public class DefaultNetTools : INetTools
{
    public object MachineInfo()
    {
        string hostname = System.Net.Dns.GetHostEntry("").HostName;//System.Net.Dns.GetHostName();
        string machinename = Environment.MachineName;
        return new
        {
            hostname,
            machinename
        };
    }

    public object Networks()
    {
        var networks = NetworkInterface.GetAllNetworkInterfaces()
        .Select(x => new
        {
            x.Name,
            Ips = GetMachineIps(x.GetIPProperties())
        });

        return networks;
    }

    private object GetMachineIps(IPInterfaceProperties ips)
    {
        var result = new List<string>();
        foreach (var dns in ips.DnsAddresses)
        {
            result.Add(dns.ToString());
        }

        return string.Join(", ", result);
    }


    public string MachineName()
    {
        string hostname = System.Net.Dns.GetHostEntry("").HostName;//System.Net.Dns.GetHostName();
        return hostname;
    }

    public object Ports()
    {
        var Ports = new List<Port>();

        try
        {
            using (Process p = new Process())
            {
                ProcessStartInfo ps = new ProcessStartInfo();
                ps.Arguments = "-a -n -o";
                ps.FileName = "netstat";
                ps.UseShellExecute = false;
                ps.WindowStyle = ProcessWindowStyle.Hidden;
                ps.RedirectStandardInput = true;
                ps.RedirectStandardOutput = true;
                ps.RedirectStandardError = true;

                p.StartInfo = ps;
                p.Start();

                StreamReader stdOutput = p.StandardOutput;
                StreamReader stdError = p.StandardError;

                string content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                string exitStatus = p.ExitCode.ToString();

                if (exitStatus != "0")
                {
                }

                string[] rows = Regex.Split(content, "\r\n");
                foreach (string row in rows)
                {
                    string[] tokens = Regex.Split(row, "\\s+");
                    if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                    {
                        string localAddress = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1");
                        Ports.Add(new Port
                        {
                            Protocol = localAddress.Contains("1.1.1.1") ? string.Format("{0}v6", tokens[1]) : string.Format("{0}v4", tokens[1]),
                            PortNo = localAddress.Split(':')[1],
                            Process = tokens[1] == "UDP" ? GetProcessName(Convert.ToInt16(tokens[4])) : GetProcessName(Convert.ToInt16(tokens[5]))
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return Ports;
    }
    public static string GetProcessName(int pid)
    {
        string procName;
        try { procName = Process.GetProcessById(pid).ProcessName; }
        catch (Exception) { procName = "-"; }
        return procName;
    }

}
