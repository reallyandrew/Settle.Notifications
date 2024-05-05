using FluentAssertions;
using Settle.Notifications.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settle.Notifications.Core.Tests.Shared;
public class ResultTests
{
    [Fact]
    public void Success_NoValue_ReturnsSuccess_ErrorNone()
    {
        // Arrange


        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().Be(Error.None);
    }
    [Fact]
    public void Success_Value_ReturnsSuccess_ErrorNone()
    {
        // Arrange
        int value = 1;

        // Act
        var result = Result.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().Be(Error.None);
        result.Value.Should().Be(value);
    }
    [Fact]
    public void Success_NotErrorNone_ThrowsException()
    {
        // Arrange

        // Act
        var act= ()=> new Result(true, new Error("NotNone", "Not an error"));

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
    [Fact]
    public void Failure_ErrorNone_ThrowsException()
    {
        // Arrange

        // Act
        var act = () => new Result(false, Error.None);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
    [Fact]
    public void Failure_ReturnsFailedWithError()
    {
        // Arrange
        var error = new Error("Failed", "Failure error");

        // Act
        var result = Result.Failure(error);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
    [Fact]
    public void Failure_ValueAccess_ThrowsException()
    {
        // Arrange
        var error = new Error("Failed", "Failure error");

        // Act
        var result = Result.Failure<int>(error);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        var act = () => result.Value;
        act.Should().Throw<InvalidOperationException>();
    }
    [Fact]
    public void Create_EmptyValue_Failure()
    {
        // Act
        var result = Result.Create<int?>(null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }
}
