using FluentAssertions;
using Settle.Notifications.Core.Validation;

namespace Settle.Notifications.Core.Tests.Validation;
public class EmailValidationTests
{
    [Theory]
    [InlineData("simple@example.com")]
    [InlineData("very.common@example.com")]
    [InlineData("user+mailbox@example.com")]
    [InlineData("customer/department=shipping@example.com")]
    [InlineData("$A12345@example.com")]
    [InlineData("user@123.123.123.123")]
    [InlineData("用户@例子.广告")]
    [InlineData("user@xn--fsq.com")]
    public void Email_IsValid(string email)
    {
        // Arrange

        // Act
        var result = email.IsValidEmail();

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("plainaddress")]
    [InlineData("missingatsign.com")]
    [InlineData("a\"b(c)d,e:f;g<h>i[j\\k]l@example.com")]
    [InlineData("just\"not\"right@example.com")]
    [InlineData("this is\"not\\allowed@example.com")]
    [InlineData("this\\ still\\\"not\\\\allowed@example.com")]
    [InlineData("john.doe@example..com")]
    [InlineData("john.doe@example@example.com")]
    [InlineData(".email@example.com")]
    [InlineData("email.@example.com")]
    [InlineData("em..ail@example.com")]
    [InlineData("user@[192.168.1.1")]
    [InlineData("user@192.168.1.1]")]
    [InlineData("email@example")]
    [InlineData("@missinglocalpart.com")]
    [InlineData("email@-example.com")]
    public void Email_InvalidEmail(string email)
    {
        // Arrange

        // Act
        var result = email.IsValidEmail();

        // Assert
        result.Should().BeFalse();
    }
    [Fact]
    public void Email_RegExTimeout_ReturnsFalse()
    {
        // arrange
        var localPart = new string('a', 64);
        var domain = new string('b', 187) + ".c";
        var email = $"{localPart}@{domain}";

        // act
        var result = email.IsValidEmailTest();

        // assert
        result.Should().BeFalse();
    }
}
