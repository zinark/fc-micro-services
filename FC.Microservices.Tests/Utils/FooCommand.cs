using FCMicroservices.Components.EnterpriseBUS;

namespace FCMicroservices.Tests.Utils;

public partial class AssemblyUtilsTests
{
    [Command]
    public class FooCommand
    {
        public int X { get; set; }
    }
}