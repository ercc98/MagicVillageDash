using System;
using System.Collections.Generic;
using ErccDev.Foundation.Core.Achievements;

namespace MagicVillageDash.Achievements
{
    /// <summary>
    /// Service locator handed to conditions and rewards so they can reach the game's
    /// systems (score, coins, distance...) without the Foundation knowing about them.
    /// Mirrors Foundation's TutorialContext.
    /// </summary>
    public sealed class AchievementContext : IAchievementContext
    {
        private readonly Dictionary<Type, object> _services = new();

        public AchievementContext Add<T>(T service) where T : class
        {
            if (service == null) return this;          // skip nulls, keep the chain clean
            _services[typeof(T)] = service;
            return this;
        }

        public bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var obj) && obj is T cast)
            {
                service = cast;
                return true;
            }

            service = null;
            return false;
        }

        public T Get<T>() where T : class => TryGet<T>(out T service) ? service : null;
    }
}
