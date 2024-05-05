using FluentAssertions;
using TestApp;

namespace Settle.Notifications.Templates.Tests;

public class FluidTemplateTests
{
    [Fact]
    public void TemplateParsesCorrectly()
    {
        // Arrange
        var template = @"
<p>Here's my template</p>
<ul>
<li>{{ Name }}</li>
<li>{{ Description }}</li>
</ul>";
        var model = new TestTemplateModel("Test user", "Description");
        var modelTemplate = @"
<p>Here's my template</p>
<ul>
<li>Test user</li>
<li>Description</li>
</ul>";

        // Act
        var result = FluidTemplate.ParseTemplate(template, model);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(modelTemplate);
    }
    [Fact]
    public void TemplateTryParse_IsFalse_ReturnsFailure()
    {
        // Arrange
        var template = @"
<p>Here's my template</p>
{% if Name %}
<ul>
<li>{{ Name }}</li>
<li>{{ Description }}</li>
</ul>";
        var model = new TestTemplateModel("Test user", "Description");
        
        // Act
        var result = FluidTemplate.ParseTemplate(template, model);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
    [Fact]
    public void TemplateRender_ThrowsException_ReturnsFailure()
    {
        // Arrange
        var template = @"
<p>Here's my template</p>
{{ 0|modulo }}
<ul>
<li>{{ Name }}</li>
<li>{{ Description }}</li>
</ul>";
        var model = new TestTemplateModel("Test user", "Description");
        

        // Act
        var result = FluidTemplate.ParseTemplate(template, model);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}