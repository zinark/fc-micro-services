using FCMicroservices.Components.BUS;

namespace Ticimax.Core.Microservice.Utils.Tests;

public partial class AssemblyUtilsTests
{
    [Command]
    public class FooCommand
    {
        public int X { get; set; }
    }
}