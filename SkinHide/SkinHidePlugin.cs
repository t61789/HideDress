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
    [BepInPlugin("com.kmyuhkyuk.SkinHide", "kmyuhkyuk-SkinHide", "1.2.1")]
    public class SkinHidePlugin : BaseUnityPlugin
    {
        public static PlayerBody Player;

        public static PlayerBody PlayerModelView;

        public static List <PlayerBody> Bot = new List<PlayerBody>();

        public enum Part
        {
            All,
            Dress,
            SkinDress
        }

        public static ConfigEntry<bool> KeyPlayerSkinHide { get; set; }
        public static ConfigEntry<bool> KeyBotSkinHide { get; set; }

        public static ConfigEntry<Part> KeyPlayerSkinHidePart { get; set; }
        public static ConfigEntry<Part> KeyBotSkinHidePart { get; set; }

        public static ConfigEntry<KeyboardShortcut> KBSPlayerSkinHide { get; set; }
        public static ConfigEntry<KeyboardShortcut> KBSBotSkinHide { get; set; }

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-SkinHide");

            string SkinHide = "Skin Hide";
            string SkinHidePart = "隐藏部分 Skin Hide Part";
            string KBS = "快捷键 Keyboard Shortcut";

            KeyPlayerSkinHide = Config.Bind<bool>(SkinHide, "玩家服装隐藏 Player Skin Hide", false);
            KeyBotSkinHide = Config.Bind<bool>(SkinHide, "Bot服装隐藏 Bot Skin Hide", false);

            KeyPlayerSkinHidePart = Config.Bind<Part>(SkinHidePart, "Player", Part.All);
            KeyBotSkinHidePart = Config.Bind<Part>(SkinHidePart, "Bot", Part.All);

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
                Hide(PlayerModelView, KeyPlayerSkinHide.Value, KeyPlayerSkinHidePart.Value);
            }

            //Player Skin Hide
            if (Player != null)
            {
                Hide(Player, KeyPlayerSkinHide.Value, KeyPlayerSkinHidePart.Value);
            }

            //Bot Skin Hide
            Bot.RemoveAll(x => x == null);
            if (Bot.Count > 0)
            {
                foreach (PlayerBody bot in Bot)
                {
                    Hide(bot, KeyBotSkinHide.Value, KeyBotSkinHidePart.Value);
                }
            }
        }

        void Hide(PlayerBody playerbody, bool hide, Part part)
        {
            object slotviews = Traverse.Create(playerbody).Field("SlotViews").GetValue<object>();

            IEnumerable<object> slotlist = (IEnumerable<object>)Traverse.Create(slotviews).Field("list_0").GetValue<object>();

            Dress[] dresses = slotlist.Where(x => Traverse.Create(x).Field("Dresses").GetValue<Dress[]>() != null).SelectMany(x => Traverse.Create(x).Field("Dresses").GetValue<Dress[]>()).ToArray();

            GameObject[] dress = dresses.Where(x => x.GetType() == typeof(Dress)).Select(x => x.gameObject).ToArray();

            MeshRenderer[] renderers = dress.SelectMany(x => x.GetComponentsInChildren<MeshRenderer>()).ToArray();

            GameObject[] skindress = dresses.Where(x => x.GetType() == typeof(SkinDress) || x.GetType() == typeof(ArmBandView)).Select(x => x.gameObject).ToArray();

            switch (part)
            {
                case Part.All:
                    foreach (GameObject gameobject in skindress)
                    {
                        gameobject.SetActive(!hide);
                    }

                    foreach (MeshRenderer renderer in renderers)
                    {
                        renderer.enabled = !hide;
                    }
                    break;
                case Part.Dress:
                    foreach (MeshRenderer renderer in renderers)
                    {
                        renderer.enabled = !hide;
                    }
                    break;
                case Part.SkinDress:
                    foreach (GameObject gameobject in skindress)
                    {
                        gameobject.SetActive(!hide);
                    }
                    break;
            }
        }
    }
}