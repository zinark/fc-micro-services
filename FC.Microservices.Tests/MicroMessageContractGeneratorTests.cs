using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

namespace FCMicroservices.Tests
{
    [TestClass]
    public class MicroMessageContractGeneratorTests
    {
        [TestMethod]
        public void test_generate()
        {
            var (content, sampleJson) = new MicroMessageContractGenerator().GenerateCode(typeof(Foo));
            Console.WriteLine(content);
            Console.WriteLine(sampleJson);
            Assert.IsNotNull(content);
        }

        [TestMethod]
        public void test_empty()
        {
            var (content, sampleJson) = new MicroMessageContractGenerator().GenerateCode(typeof(EmptyClass));
            Console.WriteLine(content);
            Console.WriteLine(sampleJson);
            Assert.IsNotNull(content);

        }
        public class EmptyClass
        {

        }
        public class Bar
        {
            public string Title { get; set; }
        }
        public class Foo
        {
            public int X { get; set; } = 1;
            public List<Bar> bars { get; set; }
        }
    }
}