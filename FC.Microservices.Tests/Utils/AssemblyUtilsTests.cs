using FCMicroservices.Components.EnterpriseBUS;
using FCMicroservices.Utils;

namespace FCMicroservices.Tests.Utils;

[TestClass]
public partial class AssemblyUtilsTests
{
    [TestMethod]
    public void SearchTypesTest()
    {
        Assert.Fail();
    }

    [TestMethod]
    public void HasAttributeTest()
    {
        Assert.IsTrue(typeof(FooCommand).HasAttribute<CommandAttribute>());
        Assert.IsTrue(typeof(FooQuery).HasAttribute<QueryAttribute>());
    }
}