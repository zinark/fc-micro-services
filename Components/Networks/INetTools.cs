namespace fc.micro.services.Components.Networks;

public interface INetTools
{
    public class Port
    {
        public string Name
        {
            get
            {
                return string.Format("{0} ({1} port {2})", Process, Protocol, PortNo);
            }
        }
        public string PortNo { get; set; }
        public string Process { get; set; }
        public string Protocol { get; set; }
    }

    string MachineName();

    object MachineInfo();

    object Networks();
    object Ports();
}
