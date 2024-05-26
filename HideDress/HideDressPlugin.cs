using System;
using System.Collections.Generic;
using BepInEx;
using EFT;
using EFT.Visual;
using HideDress.Attributes;
using HideDress.Models;
using static EFTApi.EFTHelpers;

namespace HideDress
{
    [BepInPlugin("com.kmyuhkyuk.HideDress", "HideDress", "1.3.0")]
    [BepInDependency("com.kmyuhkyuk.EFTApi", "1.2.1")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/654-hide-dress")]
    public partial class HideDressPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);
        }

        private void Start()
        {
            ReflectionModel.Instance.PlayerModelViewShow.Add(this, nameof(PlayerModelViewShow));
        }

        private void Update()
        {
            var player = _PlayerHelper.Player;
            var world = _GameWorldHelper.GameWorld;
            var settingsModel = SettingsModel.Instance;
            var hideDressModel = HideDressModel.Instance;

            if (settingsModel.KeyUpdatePlayerHideDressShortcut.Value.IsDown())
            {
                settingsModel.KeyUpdatePlayerHideDress.Value = !settingsModel.KeyUpdatePlayerHideDress.Value;
            }

            if (settingsModel.KeyUpdateOtherPlayerHideDressShortcut.Value.IsDown())
            {
                settingsModel.KeyUpdateOtherPlayerHideDress.Value = !settingsModel.KeyUpdateOtherPlayerHideDress.Value;
            }

            if (settingsModel.KeyUpdatePlayerHideDress.Value && hideDressModel.PlayerModelViewBody)
            {
                EnabledPartDress(hideDressModel.PlayerModelViewBody, settingsModel.KeyPlayerHideDressPart.Value);
            }

            if (settingsModel.KeyUpdatePlayerHideDress.Value && player)
            {
                EnabledPartDress(player.PlayerBody, settingsModel.KeyPlayerHideDressPart.Value);
            }

            if (settingsModel.KeyUpdateOtherPlayerHideDress.Value && world)
            {
                foreach (var otherPlayer in _GameWorldHelper.AllOtherPlayer)
                {
                    if (!otherPlayer)
                        continue;

                    EnabledPartDress(otherPlayer.PlayerBody, settingsModel.KeyOtherPlayerHideDressPart.Value);
                }
            }
        }

        private static void EnabledPartDress(PlayerBody playerBody, HideDressModel.DressPart dressPart)
        {
            var reflectionModel = ReflectionModel.Instance;

            var slotViews = reflectionModel.RefSlotViews.GetValue(playerBody);

            var slotList = reflectionModel.RefSlotList.GetValue(slotViews);

            var dressList = new List<Dress>();
            foreach (var slot in slotList)
            {
                var dresses = reflectionModel.RefDresses.GetValue(slot);

                if (dresses == null)
                    continue;

                foreach (var dress in dresses)
                {
                    dressList.Add(dress);
                }
            }

            switch (dressPart)
            {
                case HideDressModel.DressPart.Both:
                    EnabledDress(dressList, false);
                    EnabledSkinDress(dressList, false);
                    break;
                case HideDressModel.DressPart.Dress:
                    EnabledDress(dressList, false);
                    EnabledSkinDress(dressList, true);
                    break;
                case HideDressModel.DressPart.SkinDress:
                    EnabledDress(dressList, true);
                    EnabledSkinDress(dressList, false);
                    break;
                case HideDressModel.DressPart.None:
                    EnabledDress(dressList, true);
                    EnabledSkinDress(dressList, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dressPart), dressPart, null);
            }
        }

        private static void EnabledDress(IEnumerable<Dress> dressEnumerable, bool enabled)
        {
            foreach (var dress in dressEnumerable)
            {
                var dressType = dress.GetType();

                if (dressType != typeof(Dress))
                    continue;

                foreach (var renderer in ReflectionModel.Instance.RefRenderers.GetValue(dress))
                {
                    renderer.enabled = enabled;
                }
            }
        }

        private static void EnabledSkinDress(IEnumerable<Dress> dressEnumerable, bool enabled)
        {
            foreach (var dress in dressEnumerable)
            {
                var dressType = dress.GetType();

                if (dressType != typeof(SkinDress) && dressType != typeof(ArmBandView))
                    continue;

                dress.gameObject.SetActive(enabled);
            }
        }
    }
}