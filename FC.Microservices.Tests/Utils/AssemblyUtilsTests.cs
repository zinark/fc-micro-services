using FCMicroservices.Components.EnterpriseBUS;
using FCMicroservices.Utils;

namespace Ticimax.Core.Microservice.Utils.Tests;

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