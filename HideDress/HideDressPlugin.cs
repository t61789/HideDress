using System.Collections.Generic;
using System.Linq;
using BepInEx;
using EFT;
using EFT.Visual;
using HideDress.Models;
using static EFTApi.EFTHelpers;

namespace HideDress
{
    [BepInPlugin("com.kmyuhkyuk.HideDress", "kmyuhkyuk-HideDress", "1.2.7")]
    [BepInDependency("com.kmyuhkyuk.EFTApi", "1.2.0")]
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

            if (settingsModel.KeyPlayerHideDressShortcut.Value.IsDown())
            {
                settingsModel.KeyPlayerHideDress.Value = !settingsModel.KeyPlayerHideDress.Value;
            }

            if (settingsModel.KeyOtherPlayerHideDressShortcut.Value.IsDown())
            {
                settingsModel.KeyOtherPlayerHideDress.Value = !settingsModel.KeyOtherPlayerHideDress.Value;
            }

            if (hideDressModel.PlayerModelViewBody != null)
            {
                EnabledPartDress(hideDressModel.PlayerModelViewBody, settingsModel.KeyPlayerHideDressPart.Value, !settingsModel.KeyPlayerHideDress.Value);
            }

            if (player != null)
            {
                EnabledPartDress(player.PlayerBody, settingsModel.KeyPlayerHideDressPart.Value, !settingsModel.KeyPlayerHideDress.Value);
            }

            if (world != null)
            {
                foreach (var otherPlayer in _GameWorldHelper.AllOtherPlayer)
                {
                    EnabledPartDress(otherPlayer.PlayerBody, settingsModel.KeyOtherPlayerHideDressPart.Value, !settingsModel.KeyOtherPlayerHideDress.Value);
                }
            }
        }

        private static void EnabledPartDress(PlayerBody playerBody, HideDressModel.DressPart part, bool enabled)
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

            EnabledDress(dressList.Where(x => x.GetType() == typeof(Dress)), part == HideDressModel.DressPart.SkinDress || enabled);
            EnabledSkinDress(dressList.Where(x => x is SkinDress || x is ArmBandView), part == HideDressModel.DressPart.Dress || enabled);
        }

        private static void EnabledDress(IEnumerable<Dress> dressEnumerable, bool enabled)
        {
            foreach (var dress in dressEnumerable)
            {
                foreach (var renderer in ReflectionModel.Instance.RefRenderers.GetValue(dress))
                {
                    renderer.enabled = enabled;
                }
            }
        }

        private static void EnabledSkinDress(IEnumerable<Dress> skinDressEnumerable, bool enabled)
        {
            foreach (var skinDress in skinDressEnumerable)
            {
                skinDress.gameObject.SetActive(enabled);
            }
        }
    }
}