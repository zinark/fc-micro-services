using FCMicroservices.Extensions;
using Shouldly;

namespace FCMicroservices.Tests.Components.Extensions;

[TestClass]
public class HashExtensionsTests
{
    [TestMethod]
    public void test_as_hash()
    {
        "{0} special message {1} param"
            .AsHash()
            .ShouldBe ("F0956CB0E846152A01EE03511F39DB2C64AFA6C7150AA841E59F013B1F69B32C");
    }
}