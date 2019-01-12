using Pluralsight.OutsideInTDD.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Pluralsight.OutsideInTDD
{
    public class UnitTests
    {
        //Example of an ice breaker test. Test to get us started.
        [Fact]
        public void SutIsIteratorOfClaims()
        {
            var sut = new SimpleWebToken();
            Assert.IsAssignableFrom<IEnumerable<Claim>>(sut);
        }

        [Fact]
        public void SutYieldsInjectedClaims()
        {
            var expected = new[]
            {
                new Claim("foo", "bar"),
                new Claim("baz", "qux"),
                new Claim("quux", "corge")
            };

            var sut = new SimpleWebToken(expected);
            Assert.True(expected.SequenceEqual(sut));
            Assert.True(
                expected.Cast<object>().SequenceEqual(
                ((IEnumerable<object>)sut).OfType<object>()));
        }

        [Theory]
        [InlineData(new string[0], "")]
        [InlineData(new[] { "foo|bar" }, "foo=bar")]
        [InlineData(new[] { "foo|bar", "baz|qux" }, "foo=bar&baz=qux")]
        public void ToStringReturnsCorrectResult(string[] keysAndValues, string expected)
        {
            var claims = keysAndValues.Select(k => k.Split('|'))
                                      .Select(k => new Claim(k[0], k[1]))
                                      .ToArray();
            var sut = new SimpleWebToken(claims);
            var actual = sut.ToString();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("foo")]
        [InlineData("    ")]
        public void TryParseInvalidStringReturnsFalse(string invalidString)
        {
            SimpleWebToken dummy;
            var isValid = SimpleWebToken.TryParse(invalidString, out dummy);
            Assert.False(isValid);
        }

        [Theory]
        [InlineData(new object[] { new string[0] })]
        [InlineData(new object[] { new[] { "foo|bar" } })]
        [InlineData(new object[] { new[] { "foo|bar", "baz|qux" } })]
        //[InlineData(new[] { "foo|bar", "baz|qux" }, "foo=bar&baz=qux")]
        public void TryParseValidStringReturnsTrue(string[] keysAndValues)
        {
            var expected = keysAndValues.Select(k => k.Split('|'))
                                      .Select(k => new Claim(k[0], k[1]))
                                      .ToArray();
            var tokenString = new SimpleWebToken(expected).ToString();
            SimpleWebToken actual;
            var isValid = SimpleWebToken.TryParse(tokenString, out actual);
            Assert.True(isValid);
            Assert.True(expected.SequenceEqual(actual, new ClaimComparer()));
        }
    }
}
