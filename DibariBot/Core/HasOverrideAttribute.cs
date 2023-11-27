using Microsoft.Extensions.DependencyInjection;

namespace DibariBot;

/// <summary>
/// Succeeds if the calling user has an override enabled.
/// </summary>
public class HasOverrideAttribute : PreconditionAttribute
{
    public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        var config = services.GetRequiredService<BotConfig>();
        var overrideService = services.GetRequiredService<OverrideTrackerService>();

        return config.ManagerUserIds.Contains(context.User.Id) && await overrideService.HasOverride(context.User.Id) ? 
            PreconditionResult.FromSuccess() : 
            PreconditionResult.FromError("No permission.");
    }
}