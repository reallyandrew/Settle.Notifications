using FluentAssertions;
using Settle.Notifications.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settle.Notifications.Core.Tests.Primitives;
public class ValueObjectTests
{
    [Fact]
    public void Equals_ShouldBeTrue()
    {
        // arrange
        var emailResult1 = Email.Create("test@test.com");
        var emailResult2 = Email.Create("TEST@TEST.com");

        // act
        var result = emailResult1.Value.Equals(emailResult2.Value);

        // assert
        result.Should().BeTrue();
    }
    [Fact]
    public void Equals_NotAValueObject_ShouldBeFalse()
    {
        // arrange
        var emailResult = Email.Create("test@test.com");
        var other = new Object();

        // act
        var result = emailResult.Value.Equals(other);

        // assert
        result.Should().BeFalse();
    }
    [Theory]
    [InlineData(null, null)]
    [InlineData("email@example.com", "EMAIL@example.com")]
    public void EqualsOperator_ShouldBeTrue(string? email1, string? email2)
    {
        // arrange
        Email? emailAddress1 = null;
        Email? emailAddress2 = null;

        if (!string.IsNullOrWhiteSpace(email1))
        {
            var emailResult1 = Email.Create(email1);
            emailAddress1 = emailResult1.IsSuccess ? emailResult1.Value : null;
        }
        if (!string.IsNullOrWhiteSpace(email2))
        {
            var emailResult2 = Email.Create(email2);
            emailAddress2 = emailResult2.IsSuccess ? emailResult2.Value : null;
        }
        // act
        var result = emailAddress1 == emailAddress2;

        // assert
        result.Should().BeTrue();
    }
    [Theory]
    [InlineData("email@example.com",null)]
    [InlineData(null,"email@example.com")]
    [InlineData("email@example.com","differentemail@example.com")]
    public void NotEqualsOperator_ShouldBeTrue(string? email1, string? email2) {
        // arrange
        Email? emailAddress1=null;
        Email? emailAddress2=null;
            
        if (!string.IsNullOrWhiteSpace(email1))
        {
            var emailResult1= Email.Create(email1);
            emailAddress1 = emailResult1.IsSuccess ? emailResult1.Value : null;
        }
        if (!string.IsNullOrWhiteSpace(email2))
        {
            var emailResult2 = Email.Create(email2);
            emailAddress2 = emailResult2.IsSuccess ? emailResult2.Value : null;
        }
        // act
        var result = emailAddress1!=emailAddress2;

        // assert
        result.Should().BeTrue();
    }
    [Fact]
    public void ValueObject_HashCode_ReturnsValue()
    {
        var emailResult = Email.Create("test@test.com");

        // act
        var result = emailResult.Value.GetHashCode();

        // assert
        result.Should().NotBe(0);
    }
}
