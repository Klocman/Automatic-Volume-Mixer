using Klocman.Binding.Settings;

namespace Avm.Properties
{
    internal sealed partial class Settings
    {
        public Settings()
        {
            Binder = new SettingBinder<Settings>(this);
        }

        public SettingBinder<Settings> Binder { get; }
    }
}
