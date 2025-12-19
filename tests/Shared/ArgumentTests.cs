using System;
using System.Collections.Generic;
using NUnit.Framework;
using OpenAI;

namespace OpenAI.Tests.Shared
{
    public class ArgumentTests
    {
        [Test]
        public void AssertNotNull_AllowsNonNullReference()
        {
            Argument.AssertNotNull(new object(), nameof(AssertNotNull_AllowsNonNullReference));
        }

        [Test]
        public void AssertNotNull_ThrowsOnNullReference()
        {
            Assert.Throws<ArgumentNullException>(() => Argument.AssertNotNull<object>(null, "value"));
        }

        [Test]
        public void AssertNotNull_AllowsNullableStructWithValue()
        {
            int? value = 5;
            Argument.AssertNotNull(value, nameof(value));
        }

        [Test]
        public void AssertNotNull_ThrowsOnNullableStructWithoutValue()
        {
            int? value = null;
            Assert.Throws<ArgumentNullException>(() => Argument.AssertNotNull(value, nameof(value)));
        }

        [Test]
        public void AssertNotNullOrEmpty_AllowsNonEmptyString()
        {
            Argument.AssertNotNullOrEmpty("ok", "value");
        }

        [Test]
        public void AssertNotNullOrEmpty_ThrowsOnNullString()
        {
            Assert.Throws<ArgumentNullException>(() => Argument.AssertNotNullOrEmpty((string)null, "value"));
        }

        [Test]
        public void AssertNotNullOrEmpty_ThrowsOnEmptyString()
        {
            Assert.Throws<ArgumentException>(() => Argument.AssertNotNullOrEmpty(string.Empty, "value"));
        }

        [Test]
        public void AssertNotNullOrEmpty_AllowsNonEmptyEnumerable()
        {
            Argument.AssertNotNullOrEmpty(new[] { 1 }, "value");
        }

        [Test]
        public void AssertNotNullOrEmpty_ThrowsOnNullEnumerable()
        {
            Assert.Throws<ArgumentNullException>(() => Argument.AssertNotNullOrEmpty((IEnumerable<int>)null, "value"));
        }

        [Test]
        public void AssertNotNullOrEmpty_ThrowsOnEmptyEnumerable()
        {
            Assert.Throws<ArgumentException>(() => Argument.AssertNotNullOrEmpty(Array.Empty<int>(), "value"));
        }
    }
}
