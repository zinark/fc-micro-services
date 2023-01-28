namespace FCMicroservices.Utils;

public interface INetworkUtils
{
    string MachineName();

    object MachineInfo();

    object Networks();
    object Ports();

    public class Port
    {
        public string Name => string.Format("{0} ({1} port {2})", Process, Protocol, PortNo);

        public string PortNo { get; set; }
        public string Process { get; set; }
        public string Protocol { get; set; }
    }
}