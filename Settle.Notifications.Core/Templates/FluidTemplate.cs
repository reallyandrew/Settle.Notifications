using Fluid;
using Settle.Notifications.Core.Shared;

namespace Settle.Notifications.Templates;

internal static class FluidTemplate
{    
    public static Result<string> ParseTemplate(string template, ITemplateModel model)
    {
        var parser = new FluidParser();
        if (parser.TryParse(template, out IFluidTemplate fluidTemplate, out string error))
        {
            TemplateOptions options = new()
            {
                MemberAccessStrategy = new UnsafeMemberAccessStrategy()
            };
            var ctx = new TemplateContext(model, options, true);
            try
            {
                string output = fluidTemplate.Render(ctx);
                return output;
            }
            catch (Exception ex)
            {
                return Result.Failure<string>(ex.Message);
            }
        }
        else
        {
            return Result.Failure<string>($"Template parsing failed: {error}");
        }
    }
}
