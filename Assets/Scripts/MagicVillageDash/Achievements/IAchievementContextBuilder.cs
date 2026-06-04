using ErccDev.Foundation.Core.Achievements;

namespace MagicVillageDash.Achievements
{
    /// <summary>Builds the live <see cref="IAchievementContext"/> from the scene's game services.</summary>
    public interface IAchievementContextBuilder
    {
        IAchievementContext Build();
    }
}
