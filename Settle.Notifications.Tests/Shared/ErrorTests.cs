using FluentAssertions;
using Settle.Notifications.Core.Shared;

namespace Settle.Notifications.Core.Tests.Shared;
public class ErrorTests
{
    [Fact]
    public void Error_SameError_Equals_ShouldBeTrue()
    {
        // arrange
        var error1 = Error.NullValue;
        var error2 = Error.NullValue;

        // act
        var result = error1 == error2;

        // assert
        result.Should().BeTrue();
    }
    [Fact]
    public void Error_ErrorNull_Equals_ShouldBeFalse()
    {
        // arrange
        var error1 = Error.None;
        Error? error2 = null;

        // act
        var result = error1.Equals(error2);

        // assert
        result.Should().BeFalse();
    }
    [Fact]
    public void Error_ErrorNotAnError_Equals_ShouldBeFalse()
    {
        // arrange
        var error1 = Error.None;
        object error2 = new();

        // act
        var result = error1.Equals(error2);

        // assert
        result.Should().BeFalse();
    }
    [Fact]
    public void Error_ErrorNotAnError_Equals_ShouldBeTrue()
    {
        // arrange
        var error1 = Error.NullValue;
        object error2 = Error.NullValue;

        // act
        var result = error1.Equals(error2);

        // assert
        result.Should().BeTrue();
    }
    [Fact]
    public void Error_BothNull_Equals_ShouldBeTrue()
    {
        // arrange
        Error? error1 = null;
        Error? error2 = null;

        // act
        var result = error1 == error2;

        // assert
        result.Should().BeTrue();
    }
    [Fact]
    public void Error_ToString_ReturnsCode()
    {
        // arrange
        var error = Error.NullValue;

        // act
        var result = error.ToString();

        // assert
        result.Should().Be(error.Code);
    }
    [Fact]
    public void Error_OneSideNull_ShouldBeFalse()
    {
        // arrange
        Error? error1=Error.NullValue;
        Error? error2 = null;

        // act
        var result1=error1 == error2;
        var result2 = error2 == error1;

        // assert
        result1.Should().BeFalse();
        result2.Should().BeFalse();
    }
    [Fact]
    public void Error_GetHashCode_ReturnsValue()
    {
        // Arrange
        var error = Error.NullValue;

        // Act
        var result = error.GetHashCode();

        // Assert
        result.Should().NotBe(0);
    }
}
