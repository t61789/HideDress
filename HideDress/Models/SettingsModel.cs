using System.Diagnostics.CodeAnalysis;
using BepInEx.Configuration;

namespace HideDress.Models
{
    internal class SettingsModel
    {
        public static SettingsModel Instance { get; private set; }

        public readonly ConfigEntry<bool> KeyUpdatePlayerHideDress;
        public readonly ConfigEntry<bool> KeyUpdateOtherPlayerHideDress;

        public readonly ConfigEntry<HideDressModel.DressPart> KeyPlayerHideDressPart;
        public readonly ConfigEntry<HideDressModel.DressPart> KeyOtherPlayerHideDressPart;

        public readonly ConfigEntry<KeyboardShortcut> KeyUpdatePlayerHideDressShortcut;
        public readonly ConfigEntry<KeyboardShortcut> KeyUpdateOtherPlayerHideDressShortcut;

        [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
        private SettingsModel(ConfigFile configFile)
        {
            const string hideDressSettings = "Hide Dress Settings";
            const string hideDressPartSettings = "Hide Dress Part Settings";
            const string shortcutSettings = "Keyboard Shortcut Settings";

            KeyUpdatePlayerHideDress = configFile.Bind<bool>(hideDressSettings, "Update Hide Player Dress", false);
            KeyUpdateOtherPlayerHideDress =
                configFile.Bind<bool>(hideDressSettings, "Update Hide Other Player Dress", false);

            KeyPlayerHideDressPart =
                configFile.Bind<HideDressModel.DressPart>(hideDressPartSettings, "Player",
                    HideDressModel.DressPart.Both);
            KeyOtherPlayerHideDressPart =
                configFile.Bind<HideDressModel.DressPart>(hideDressPartSettings, "Other Player",
                    HideDressModel.DressPart.Both);

            KeyUpdatePlayerHideDressShortcut =
                configFile.Bind<KeyboardShortcut>(shortcutSettings, "Update Hide Player Dress",
                    KeyboardShortcut.Empty);
            KeyUpdateOtherPlayerHideDressShortcut =
                configFile.Bind<KeyboardShortcut>(shortcutSettings, "Update Hide Other Player Dress",
                    KeyboardShortcut.Empty);
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static SettingsModel Create(ConfigFile configFile)
        {
            if (Instance != null)
                return Instance;

            return Instance = new SettingsModel(configFile);
        }
    }
}