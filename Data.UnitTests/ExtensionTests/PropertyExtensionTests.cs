using System;
using Data.Extensions;
using Moq;
using NUnit.Framework;

namespace Data.UnitTests.ExtensionTests
{
    [TestFixture]
    public class PropertyExtensionTests
    {
        #region SetProperty

        [Test]
        public void SetProperty_WhenTypeObjectIsNull_ReturnsTypeObjectUnchanged()
        {
            PropertyClass testClass = null;
            var result = testClass.SetProperty(a => a.Writable, It.IsAny<string>());
            Assert.AreEqual(testClass, result);
        }

        [Test]
        public void SetProperty_WhenLambdaFunctionIsNotAMemberExpression_ThrowsArgumentException()
        {
            var testClass = new PropertyClass();
            Assert.Catch<ArgumentException>(() => testClass.SetProperty(a => a.ToString(), It.IsAny<string>()));
        }

        [Test]
        public void SetProperty_WhenPassingThroughAValueOfDifferentType_ThrowsInvalidOperationException()
        {
            var testClass = new PropertyClass();
            var exception = Assert.Catch<InvalidOperationException>(() => testClass.SetProperty(a => a.Writable, 3));
            Assert.AreEqual("Value type does not match the Property type.", exception.Message);
        }

        [Test]
        public void SetProperty_WhenTryingToSetAReadonlyProperty_ThrowsInvalidOperationException()
        {
            var testClass = new PropertyClass();
            var exception = Assert.Catch<InvalidOperationException>(() => testClass.SetProperty(a => a.Readonly, It.IsAny<string>()));
            Assert.AreEqual("Cannot set a restricted property.", exception.Message);
        }

        [Test]
        public void SetProperty_ChangesPropertyValue()
        {
            var value = "test";
            var testClass = new PropertyClass();
            testClass.SetProperty(a => a.Writable, value);
            Assert.AreEqual(value, testClass.Writable);
        }

        [Test]
        public void SetProperty_WhenSettingPropertyWithNonPublicAccessModifiers_ChangesPropertyValue()
        {
            var value = "test";
            var testClass = new PropertyClass();
            testClass.SetProperty(a => a.PrivateWritable, value);
            Assert.AreEqual(value, testClass.PrivateWritable);
        }

        [Test]
        public void SetProperty_WhenPassingNullAsValueForNonNullableProperty_SetPropertyToDefaultValue()
        {
            var testClass = new PropertyClass()
            {
                NonNullableInt = 1
            };
            testClass.SetProperty(a => a.NonNullableInt, null);
            Assert.AreEqual(default(int), testClass.NonNullableInt);
        }

        #endregion SetProperty

        #region TrySetProperty

        [Test]
        public void TrySetProperty_WhenClassObjectIsNull_ReturnsUnchangedObject()
        {
            PropertyClass testClass = null;
            var result = testClass.TrySetProperty(nameof(PropertyClass.Writable), It.IsAny<string>());
            Assert.AreEqual(testClass, result);
        }

        [Test]
        public void TrySetProperty_WhenClassDoesNotHaveProperty_ReturnsUnchangedObject()
        {
            var testClass = new PropertyClass();
            var result = testClass.TrySetProperty("Random", It.IsAny<string>());
            Assert.AreEqual(testClass, result);
        }

        [Test]
        public void TrySetProperty_WhenTryingToSetAReadonlyProperty_ThrowsInvalidOperationException()
        {
            var testClass = new PropertyClass();
            var exception = Assert.Catch<InvalidOperationException>(() => testClass.TrySetProperty(nameof(PropertyClass.Readonly), It.IsAny<string>()));
            Assert.AreEqual("Cannot set a restricted property.", exception.Message);
        }

        [Test]
        public void TrySetProperty_WhenValueTypeDoesNotMatchProperty_ThrowsInvalidOperationException()
        {
            var testClass = new PropertyClass();
            var exception = Assert.Catch<InvalidOperationException>(() => testClass.TrySetProperty(nameof(PropertyClass.Writable), 0));
            Assert.AreEqual("Property type is not assignable from the Value type.", exception.Message);
        }

        [Test]
        public void TrySetProperty_ChangesPropertyValue()
        {
            var value = "test";
            var testClass = new PropertyClass();
            testClass.TrySetProperty(nameof(PropertyClass.Writable), value);
            Assert.AreEqual(value, testClass.Writable);
        }

        [Test]
        public void TrySetProperty_WhenSettingPropertyWithNonPublicAccessModifiers_ChangesPropertyValue()
        {
            var value = "test";
            var testClass = new PropertyClass();
            testClass.TrySetProperty(nameof(PropertyClass.PrivateWritable), value);
            Assert.AreEqual(value, testClass.PrivateWritable);
        }

        [Test]
        public void TrySetProperty_WhenPassingNullAsValueForNonNullableProperty_SetPropertyToDefaultValue()
        {
            var testClass = new PropertyClass()
            {
                NonNullableInt = 1
            };
            testClass.TrySetProperty(nameof(PropertyClass.NonNullableInt), null);
            Assert.AreEqual(default(int), testClass.NonNullableInt);
        }

        [Test]
        public void TrySetProperty_WhenSettingANullableProperty_DoesNotThrowException()
        {
            var testClass = new PropertyClass();
            Assert.DoesNotThrow(() => testClass.TrySetProperty(nameof(PropertyClass.DateTime), DateTime.UtcNow));
        }
        
        [Test]
        public void TrySetProperty_WhenSettingANonNullableProperty_DoesNotThrowException()
        {
            var testClass = new PropertyClass();
            Assert.DoesNotThrow(() => testClass.TrySetProperty(nameof(PropertyClass.DateTime2), DateTime.UtcNow));
        }

        #endregion TrySetProperty

        private class PropertyClass
        {
            public PropertyClass()
            {
                Writable = nameof(Writable);
                Readonly = nameof(Readonly);
                PrivateWritable = nameof(PrivateWritable);
            }

            public string Writable { get; set; }
            public string Readonly { get; }
            public string PrivateWritable { get; private set; }
            public DateTime? DateTime { get; set; }
            public DateTime DateTime2 { get; set; }
            public int NonNullableInt { get; set; }
        }
    }
}