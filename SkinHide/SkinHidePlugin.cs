using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using EFT;
using EFT.Visual;
using SkinHide.Patches;

namespace SkinHide
{
    [BepInPlugin("com.kmyuhkyuk.SkinHide", "kmyuhkyuk-SkinHide", "1.2.0")]
    public class SkinHidePlugin : BaseUnityPlugin
    {
        public static PlayerBody Player;

        public static PlayerBody PlayerModelView;

        public static List <PlayerBody> Bot = new List<PlayerBody>();

        public static ConfigEntry<bool> KeyPlayerSkinHide { get; set; }
        public static ConfigEntry<bool> KeyBotSkinHide { get; set; }

        public static ConfigEntry<KeyboardShortcut> KBSPlayerSkinHide { get; set; }
        public static ConfigEntry<KeyboardShortcut> KBSBotSkinHide { get; set; }

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-SkinHide");

            string SkinHide = "Skin Hide";
            string KBS = "Keyboard Shortcut";

            KeyPlayerSkinHide = Config.Bind<bool>(SkinHide, "玩家服装隐藏 Player Skin Hide", false);
            KeyBotSkinHide = Config.Bind<bool>(SkinHide, "Bot服装隐藏 Bot Skin Hide", false);

            KBSPlayerSkinHide = Config.Bind<KeyboardShortcut>(KBS, "玩家服装隐藏快捷键 Player Skin Hide", new KeyboardShortcut());
            KBSBotSkinHide = Config.Bind<KeyboardShortcut>(KBS, "Bot服装隐藏快捷键 Bot Skin Hide", new KeyboardShortcut());

            new PlayerModelViewPatch().Enable();
            new GamePlayerOwnerPatch().Enable();
            new BotOwnerPatch().Enable();
        }
        void Update()
        {
            if (KBSPlayerSkinHide.Value.IsDown())
            {
                KeyPlayerSkinHide.Value = !KeyPlayerSkinHide.Value;
            }
            if (KBSBotSkinHide.Value.IsDown())
            {
                KeyBotSkinHide.Value = !KeyBotSkinHide.Value;
            }

            //PlayerModelView Skin Hide
            if (PlayerModelView != null)
            {
                Hide(PlayerModelView, KeyPlayerSkinHide.Value);
            }

            //Player Skin Hide
            if (Player != null)
            {
                Hide(Player, KeyPlayerSkinHide.Value);
            }

            //Bot Skin Hide
            if (Bot.Count > 0)
            {
                foreach (PlayerBody bot in Bot)
                {
                    Hide(bot, KeyBotSkinHide.Value);
                }
            }
        }

        void Hide(PlayerBody playerbody, bool hide)
        {
            object slotviews = playerbody.SlotViews;

            IEnumerable<object> slotlist = (IEnumerable<object>)Traverse.Create(slotviews).Field("list_0").GetValue<object>();

            Dress[] dresses = slotlist.Where(x => Traverse.Create(x).Field("Dresses").GetValue<Dress[]>() != null).SelectMany(x => Traverse.Create(x).Field("Dresses").GetValue<Dress[]>()).ToArray();

            GameObject[] dress = dresses.Where(x => x.GetType() == typeof(Dress)).Select(x => x.gameObject).ToArray();

            MeshRenderer[] renderers = dress.SelectMany(x => x.GetComponentsInChildren<MeshRenderer>()).ToArray();

            GameObject[] skindress = dresses.Where(x => x.GetType() == typeof(SkinDress) || x.GetType() == typeof(ArmBandView)).Select(x => x.gameObject).ToArray();

            foreach (GameObject gameobject in skindress)
            {
                gameobject.SetActive(!hide);
            }

            foreach (MeshRenderer renderer in renderers)
            {
                renderer.enabled = !hide;
            }
        }
    }
}