using System.Diagnostics.CodeAnalysis;
using BepInEx.Configuration;

namespace HideDress.Models
{
    internal class SettingsModel
    {
        public static SettingsModel Instance { get; private set; }

        public readonly ConfigEntry<bool> KeyPlayerHideDress;
        public readonly ConfigEntry<bool> KeyOtherPlayerHideDress;

        public readonly ConfigEntry<HideDressModel.DressPart> KeyPlayerHideDressPart;
        public readonly ConfigEntry<HideDressModel.DressPart> KeyOtherPlayerHideDressPart;

        public readonly ConfigEntry<KeyboardShortcut> KeyPlayerHideDressShortcut;
        public readonly ConfigEntry<KeyboardShortcut> KeyOtherPlayerHideDressShortcut;

        [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
        private SettingsModel(ConfigFile configFile)
        {
            const string hideDressSettings = "Hide Dress Settings";
            const string hideDressPartSettings = "Hide Dress Part Settings";
            const string shortcutSettings = "Keyboard Shortcut Settings";

            KeyPlayerHideDress = configFile.Bind<bool>(hideDressSettings, "Hide Player Dress", false);
            KeyOtherPlayerHideDress = configFile.Bind<bool>(hideDressSettings, "Hide Other Player Dress", false);

            KeyPlayerHideDressPart =
                configFile.Bind<HideDressModel.DressPart>(hideDressPartSettings, "Player",
                    HideDressModel.DressPart.Both);
            KeyOtherPlayerHideDressPart =
                configFile.Bind<HideDressModel.DressPart>(hideDressPartSettings, "Other Player",
                    HideDressModel.DressPart.Both);

            KeyPlayerHideDressShortcut =
                configFile.Bind<KeyboardShortcut>(shortcutSettings, "Hide Player Dress",
                    KeyboardShortcut.Empty);
            KeyOtherPlayerHideDressShortcut =
                configFile.Bind<KeyboardShortcut>(shortcutSettings, "Hide Other Player Dress", KeyboardShortcut.Empty);
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