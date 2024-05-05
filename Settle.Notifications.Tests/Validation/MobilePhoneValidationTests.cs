using FluentAssertions;
using Settle.Notifications.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settle.Notifications.Core.Tests.Validation;
public class MobilePhoneValidationTests
{
    private const string _ukFormatMobileNumber = "07700900900";
    private const string _intFormatUkMobileNumber = "+447700900900";

    [Fact]
    public void UKMobilePhone_UKSTD_StartsWith07()
    {
        // Arrange
        var phoneNumber = _ukFormatMobileNumber;
        // Act
        var isValid=MobilePhoneValidation.IsValidUkNumber(phoneNumber);
        // Assert
        isValid.Should().BeTrue();
    }
    [Fact]
    public void UKMobilePhone_IntFormat_StartsWithplus447()
    {
        // Arrange
        var phoneNumber = _intFormatUkMobileNumber;
        // Act
        var isValid = MobilePhoneValidation.IsValidUkNumber(phoneNumber);

        // Assert
        isValid.Should().BeTrue();
    }
    [Fact]
    public void UKMobilePhone_UKSTD_StartsWith07_invalidnumber()
    {
        // Arrange
        var phoneNumber = "02079460000";
        // Act
        var isValid = MobilePhoneValidation.IsValidUkNumber(phoneNumber);
        // Assert
        isValid.Should().BeFalse();
    }
    [Fact]
    public void UKMobilePhone_IntFormat_StartsWithplus447_invalidNumber()
    {
        // Arrange
        var phoneNumber = "+442079460000";
        // Act
        var isValid = MobilePhoneValidation.IsValidUkNumber(phoneNumber);

        // Assert
        isValid.Should().BeFalse();
    }
    [Fact]
    public void UKMobilePhone_UKSTD_ContainsNonNumericCharacters()
    {
        // Arrange
        var phoneNumber = "07700xxx900";
        // Act
        var isValid = MobilePhoneValidation.IsValidUkNumber(phoneNumber);
        // Assert
        isValid.Should().BeFalse();
    }
    [Fact]
    public void UKMobilePhone_IntFormat_ContainsNonNumericCharacters()
    {
        // Arrange
        var phoneNumber = "+447700xxx900";
        // Act
        var isValid = MobilePhoneValidation.IsValidUkNumber(phoneNumber);

        // Assert
        isValid.Should().BeFalse();
    }
    [Fact]
    public void UKMobilePhone_UKSTD_TooShort_ReturnsFalse()
    {
        // Arrange
        var phoneNumber = "0770090090";
        // Act
        var isValid = MobilePhoneValidation.IsValidUkNumber(phoneNumber);
        // Assert
        isValid.Should().BeFalse();
    }
    [Fact]
    public void UKMobilePhone_IntFormat_TooShort_ReturnsFalse()
    {
        // Arrange
        var phoneNumber = "+44770090090";
        // Act
        var isValid = MobilePhoneValidation.IsValidUkNumber(phoneNumber);

        // Assert
        isValid.Should().BeFalse();
    }
}
