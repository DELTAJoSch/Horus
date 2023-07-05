using HorusAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.UnitTests.Helpers
{
    internal class BuilderTest
    {
        private class TestClass
        {
            public int TestValue { get; set; }
        }

        [Test]
        public void Builder_WhenSettingKnownAttribute_AddsAttrribute()
        {
            var testClass = new TestClass();

            var built = new Builder<TestClass>()
                .With(4, nameof(TestClass.TestValue))
                .Build();

            Assert.That(4 == built.TestValue);
        }

        [Test]
        public void Builder_WhenSettingUnknownAttribute_ReturnsArgumentException()
        {
            var testClass = new TestClass();

            Assert.Throws(typeof(ArgumentException), () =>
            {
                var built = new Builder<TestClass>()
                .With(4, "KARL_HEINZ")
                .Build();
            });
        }
    }
}
