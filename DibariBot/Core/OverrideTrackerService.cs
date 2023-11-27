using Microsoft.Extensions.DependencyInjection;

namespace DibariBot;

// officially the stupidest service to exist.
// 100% could be extension methods on ICacheProvider, but that felt wrong.
/// <remarks>None of the functions check or care if the user is allowed to have an override. Make sure to check with BotConfig before using these.</remarks>
[Inject(ServiceLifetime.Singleton)]
public class OverrideTrackerService
{
    private readonly ICacheProvider cacheProvider;

    public OverrideTrackerService(ICacheProvider cacheProvider)
    {
        this.cacheProvider = cacheProvider;
    }

    public async ValueTask SetOverride(ulong id) => await cacheProvider.SetAsync(GetKeyName(id), true, new CacheValueSettings(TimeSpan.FromMinutes(15)));

    public async ValueTask ClearOverride(ulong id) => await cacheProvider.RemoveAsync(GetKeyName(id));

    public async ValueTask<bool> HasOverride(ulong id) => (await cacheProvider.GetAsync<bool>(GetKeyName(id))).IsSpecified;

    private static string GetKeyName(ulong id) => $"override-{id}";
}
