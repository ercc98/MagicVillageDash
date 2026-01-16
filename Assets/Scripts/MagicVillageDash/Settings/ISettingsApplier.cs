using ErccDev.Foundation.Data;

namespace MagicVillageDash.Settings
{
    public interface ISettingsApplier
    {
        void Apply(SettingsData settings);
    }
}