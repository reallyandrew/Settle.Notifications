using FluentAssertions;
using Settle.Notifications.Core.ValueObjects;

namespace Settle.Notifications.Core.Tests.ValueObjects;
public class EmailTests
{
    [Theory]
    [InlineData("simple@example.com")]
    [InlineData("SIMPLE@example.com")]
    [InlineData("very.common@example.com")]
    [InlineData("Very.Common@Example.Com")]
    [InlineData("user+mailbox@example.com")]
    [InlineData("customer/department=shipping@example.com")]
    [InlineData("$A12345@example.com")]
    [InlineData("user@123.123.123.123")]
    [InlineData("用户@例子.广告")]
    [InlineData("user@xn--fsq.com")]
    public void CreateEmail(string email)
    {
        // arrange

        // act
        var emailResult = Email.Create(email);

        // assert
        emailResult.Should().NotBeNull();
        emailResult.IsSuccess.Should().BeTrue();
        emailResult.Value.Value.Should().Be(email.ToLowerInvariant());
    }
    [Fact]
    public void CreateEmail_EmptyEmail_ReturnsFailure()
    {
        // Arrange
        var email = string.Empty;
        // Assert

        var emailResult = Email.Create(email);

        // assert
        emailResult.Should().NotBeNull();
        emailResult.IsFailure.Should().BeTrue();
        emailResult.Error.Code.Should().Be("Email.Empty");
    }
    [Theory]
    [InlineData("plainaddress")]
    [InlineData("missingatsign.com")]
    public void CreateEmail_InvalidEmail_ReturnsFailure(string email)
    {
        // arrange

        // act
        var emailResult = Email.Create(email);

        // assert
        emailResult.Should().NotBeNull();
        emailResult.IsFailure.Should().BeTrue();
        emailResult.Error.Code.Should().Be("Email.Invalid");
    }
}
