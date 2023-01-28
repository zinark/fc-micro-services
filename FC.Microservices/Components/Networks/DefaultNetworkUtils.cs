using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using static FCMicroservices.Components.Networks.INetworkUtils;

namespace FCMicroservices.Components.Networks;

public class DefaultNetworkUtils : INetworkUtils
{
    public object MachineInfo()
    {
        var hostname = Dns.GetHostEntry("").HostName; //System.Net.Dns.GetHostName();
        var machinename = Environment.MachineName;
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


    public string MachineName()
    {
        var hostname = Dns.GetHostEntry("").HostName; //System.Net.Dns.GetHostName();
        return hostname;
    }

    public object Ports()
    {
        var Ports = new List<Port>();

        try
        {
            using (var p = new Process())
            {
                var ps = new ProcessStartInfo();
                ps.Arguments = "-a -n -o";
                ps.FileName = "netstat";
                ps.UseShellExecute = false;
                ps.WindowStyle = ProcessWindowStyle.Hidden;
                ps.RedirectStandardInput = true;
                ps.RedirectStandardOutput = true;
                ps.RedirectStandardError = true;

                p.StartInfo = ps;
                p.Start();

                var stdOutput = p.StandardOutput;
                var stdError = p.StandardError;

                var content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                var exitStatus = p.ExitCode.ToString();

                if (exitStatus != "0")
                {
                }

                var rows = Regex.Split(content, "\r\n");
                foreach (var row in rows)
                {
                    var tokens = Regex.Split(row, "\\s+");
                    if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                    {
                        var localAddress = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1");
                        Ports.Add(new Port
                        {
                            Protocol = localAddress.Contains("1.1.1.1")
                                ? string.Format("{0}v6", tokens[1])
                                : string.Format("{0}v4", tokens[1]),
                            PortNo = localAddress.Split(':')[1],
                            Process = tokens[1] == "UDP"
                                ? GetProcessName(Convert.ToInt16(tokens[4]))
                                : GetProcessName(Convert.ToInt16(tokens[5]))
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

    private object GetMachineIps(IPInterfaceProperties ips)
    {
        var result = new List<string>();
        foreach (var dns in ips.DnsAddresses) result.Add(dns.ToString());

        return string.Join(", ", result);
    }

    public static string GetProcessName(int pid)
    {
        string procName;
        try
        {
            procName = Process.GetProcessById(pid).ProcessName;
        }
        catch (Exception)
        {
            procName = "-";
        }

        return procName;
    }
}