using FluentAssertions;
using Settle.Notifications.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settle.Notifications.Core.Tests.ValueObjects;
public class UKMobilePhoneNumberTests
{
    [Theory]
    [InlineData("07700900900")]
    [InlineData("+447700900900")]
    public void Create_ValidData_ReturnsSuccess(string phoneNumber)
    {
        // Arrange

        // Act
        var result=UKMobilePhoneNumber.Create(phoneNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(phoneNumber);
    }
    [Fact]
    public void Create_EmptyValue_ReturnsFailure()
    {
        // Arrange
        string phoneNumber = string.Empty;

        // Act
        var result = UKMobilePhoneNumber.Create(phoneNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PhoneNumberErrors.Empty);
    }
    [Theory]
    [InlineData("02700900900")]
    [InlineData("+442700900900")]
    [InlineData("07700xxx900")]
    [InlineData("+447700xxx900")]
    [InlineData("0770090090")]
    [InlineData("+44770090090")]
    [InlineData("077009009000")]
    [InlineData("+4477009009000")]
    public void Create_InvalidData_ReturnsFailure(string phoneNumber)
    {
        // Arrange

        // Act
        var result = UKMobilePhoneNumber.Create(phoneNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PhoneNumberErrors.Invalid);
    }
}
