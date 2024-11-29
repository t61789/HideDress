using System.Diagnostics.CodeAnalysis;
using BepInEx.Configuration;

namespace HideDress.Models
{
    internal class SettingsModel
    {
        public static SettingsModel Instance { get; private set; }
        
        public readonly ConfigEntry<bool> EnableHiding;
        public readonly ConfigEntry<string> SpecifiedOtherNickName;
        public readonly ConfigEntry<float> UpdateInterval;
        public readonly ConfigEntry<KeyboardShortcut> SwitchHidingShortcut;

        public readonly ConfigEntry<bool> PlayerHideBackpack;
        public readonly ConfigEntry<bool> PlayerHideArmorVest;
        public readonly ConfigEntry<bool> PlayerHideTacticalVest;
        public readonly ConfigEntry<bool> PlayerHideHeadWear;
        public readonly ConfigEntry<bool> PlayerHideEarPiece;
        public readonly ConfigEntry<bool> PlayerHideEyeWear;
        public readonly ConfigEntry<bool> PlayerHideArmBand;
        public readonly ConfigEntry<bool> PlayerHideFaceCover;
        public readonly ConfigEntry<bool> OtherHideBackpack;
        public readonly ConfigEntry<bool> OtherHideArmorVest;
        public readonly ConfigEntry<bool> OtherHideTacticalVest;
        public readonly ConfigEntry<bool> OtherHideHeadWear;
        public readonly ConfigEntry<bool> OtherHideEarPiece;
        public readonly ConfigEntry<bool> OtherHideEyeWear;
        public readonly ConfigEntry<bool> OtherHideArmBand;
        public readonly ConfigEntry<bool> OtherHideFaceCover;

        [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
        private SettingsModel(ConfigFile configFile)
        {
            const string hideSettingsSection = "Hide Settings";
            EnableHiding = configFile.Bind<bool>(
                hideSettingsSection, 
                "Enable Hiding", 
                true);
            SpecifiedOtherNickName = configFile.Bind<string>(
                hideSettingsSection,
                "Only Hide Specified Character By Nick Name (except your player)",
                "");       
            SwitchHidingShortcut =
                configFile.Bind<KeyboardShortcut>(
                    hideSettingsSection, 
                    "Switch Hiding Shortcut",
                    KeyboardShortcut.Empty);
            UpdateInterval = 
                configFile.Bind<float>(
                    hideSettingsSection, 
                    "Update Interval",
                    2);
            
            const string hideDressPartSettingsSection = "Hide Part Settings";
            PlayerHideBackpack = configFile.Bind<bool>(
                hideDressPartSettingsSection,
                "Hide Player Backpack",
                false);       
            PlayerHideArmorVest = configFile.Bind<bool>(
                hideDressPartSettingsSection,
                "Hide Player Armor Vest",
                true);       
            PlayerHideTacticalVest = configFile.Bind<bool>(
                hideDressPartSettingsSection,
                "Hide Player Tactical Vest",
                true);       
            PlayerHideHeadWear = configFile.Bind<bool>(
                hideDressPartSettingsSection,
                "Hide Player Head Wear",
                true);       
            PlayerHideEarPiece = configFile.Bind<bool>(
                hideDressPartSettingsSection,
                "Hide Player Ear Piece",
                false);       
            PlayerHideEyeWear = configFile.Bind<bool>(
                hideDressPartSettingsSection,
                "Hide Player Eye Wear",
                false);       
            PlayerHideArmBand = configFile.Bind<bool>(
                hideDressPartSettingsSection,
                "Hide Player Arm Band",
                false);       
            PlayerHideFaceCover = configFile.Bind<bool>(
                hideDressPartSettingsSection,
                "Hide Player Face Cover",
                true);       
            OtherHideBackpack = configFile.Bind<bool>(
                hideDressPartSettingsSection,
                "Hide Other Backpack",
                false);       
            OtherHideArmorVest = configFile.Bind<bool>(
                hideDressPartSettingsSection,
                "Hide Other Armor Vest",
                true);       
            OtherHideTacticalVest = configFile.Bind<bool>(
                hideDressPartSettingsSection,
                "Hide Other Tactical Vest",
                true);       
            OtherHideHeadWear = configFile.Bind<bool>(
                hideDressPartSettingsSection,
                "Hide Other Head Wear",
                true);       
            OtherHideEarPiece = configFile.Bind<bool>(
                hideDressPartSettingsSection,
                "Hide Other Ear Piece",
                false);       
            OtherHideEyeWear = configFile.Bind<bool>(
                hideDressPartSettingsSection,
                "Hide Other Eye Wear",
                false);       
            OtherHideArmBand = configFile.Bind<bool>(
                hideDressPartSettingsSection,
                "Hide Other Arm Band",
                false);       
            OtherHideFaceCover = configFile.Bind<bool>(
                hideDressPartSettingsSection,
                "Hide Other Face Cover",
                true);       
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