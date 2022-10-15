using BepInEx;
using BepInEx.Configuration;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using EFT;
using EFT.Visual;
using SkinHide.Patches;
using SkinHide.Utils;
using System;

namespace SkinHide
{
    [BepInPlugin("com.kmyuhkyuk.SkinHide", "kmyuhkyuk-SkinHide", "1.2.5")]
    public class SkinHidePlugin : BaseUnityPlugin
    {
        internal static PlayerBody Player;

        internal static PlayerBody PlayerModelView;

        internal static List<PlayerBody> Bot = new List<PlayerBody>();

        private readonly SettingsData settingsdata = new SettingsData();

        private readonly ReflectionData reflectiondata = new ReflectionData();

        private bool PMVHideCache;

        private bool PlayerHideCache;

        private bool BotHideCache;

        public enum Part
        {
            All,
            Dress,
            SkinDress
        }

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-SkinHide");

            string SkinHideSettings = "Skin Hide Settings";
            string SkinHidePartSettings = "隐藏部分设置 Skin Hide Part Settings";
            string KBSSettings = "快捷键设置 Keyboard Shortcut Settings";

            settingsdata.KeyPlayerSkinHide = Config.Bind<bool>(SkinHideSettings, "玩家服装隐藏 Player Skin Hide", false);
            settingsdata.KeyBotSkinHide = Config.Bind<bool>(SkinHideSettings, "Bot服装隐藏 Bot Skin Hide", false);

            settingsdata.KeyPlayerSkinHidePart = Config.Bind<Part>(SkinHidePartSettings, "Player", Part.All);
            settingsdata.KeyBotSkinHidePart = Config.Bind<Part>(SkinHidePartSettings, "Bot", Part.All);

            settingsdata.KBSPlayerSkinHide = Config.Bind<KeyboardShortcut>(KBSSettings, "玩家服装隐藏快捷键 Player Skin Hide", KeyboardShortcut.Empty);
            settingsdata.KBSBotSkinHide = Config.Bind<KeyboardShortcut>(KBSSettings, "Bot服装隐藏快捷键 Bot Skin Hide", KeyboardShortcut.Empty);

            new PlayerModelViewPatch().Enable();
            new PlayerPatch().Enable();

            reflectiondata.RefSlotViews = RefHelp.FieldRef<PlayerBody, object>.Create("SlotViews");
            reflectiondata.RefSlotList = RefHelp.FieldRef<object, IEnumerable<object>>.Create(reflectiondata.RefSlotViews.FieldType, "list_0");
            reflectiondata.RefDresses = RefHelp.FieldRef<object, Dress[]>.Create(reflectiondata.RefSlotList.FieldType.GetGenericArguments()[0], "Dresses");
        }

        void Update()
        {
            if (settingsdata.KBSPlayerSkinHide.Value.IsDown())
            {
                settingsdata.KeyPlayerSkinHide.Value = !settingsdata.KeyPlayerSkinHide.Value;
            }
            if (settingsdata.KBSBotSkinHide.Value.IsDown())
            {
                settingsdata.KeyBotSkinHide.Value = !settingsdata.KeyBotSkinHide.Value;
            }

            //PlayerModelView Skin Hide
            if (PlayerModelView != null && settingsdata.KeyPlayerSkinHide.Value)
            {
                Hide(PlayerModelView, settingsdata.KeyPlayerSkinHidePart.Value, true);

                PMVHideCache = true;
            }
            else if (PlayerModelView != null && !settingsdata.KeyPlayerSkinHide.Value && PMVHideCache)
            {
                Hide(PlayerModelView, Part.All, false);

                PMVHideCache = false;
            }

            //Player Skin Hide
            if (Player != null)
            {
                if (settingsdata.KeyPlayerSkinHide.Value)
                {
                    Hide(Player, settingsdata.KeyPlayerSkinHidePart.Value, true);

                    PlayerHideCache = true;
                }
                else if (!settingsdata.KeyPlayerSkinHide.Value && PlayerHideCache)
                {
                    Hide(Player, Part.All, false);

                    PlayerHideCache = false;
                }
            }
            else
            {
                Bot.Clear();
            }

            //Bot Skin Hide
            if (Bot.Count > 0 && settingsdata.KeyBotSkinHide.Value)
            {
                foreach (PlayerBody bot in Bot)
                {
                    Hide(bot, settingsdata.KeyBotSkinHidePart.Value, true);
                }

                BotHideCache = true;
            }
            else if (Bot.Count > 0 && !settingsdata.KeyBotSkinHide.Value && BotHideCache)
            {
                foreach (PlayerBody bot in Bot)
                {
                    Hide(bot, Part.All, false);
                }

                BotHideCache = false;
            }
        }

        void Hide(PlayerBody playerbody, Part part, bool hide)
        {
            object slotviews = reflectiondata.RefSlotViews.GetValue(playerbody);

            IEnumerable<object> slotlist = reflectiondata.RefSlotList.GetValue(slotviews);

            IEnumerable<Dress> dresses = slotlist.Where(x => reflectiondata.RefDresses.GetValue(x) != null).SelectMany(x => reflectiondata.RefDresses.GetValue(x));

            IEnumerable<GameObject> dress = dresses.Where(x => x.GetType() == typeof(Dress)).Select(x => x.gameObject);

            IEnumerable<MeshRenderer> renderers = dress.SelectMany(x => x.GetComponentsInChildren<MeshRenderer>());

            IEnumerable<GameObject> skindress = dresses.Where(x => x.GetType() == typeof(SkinDress) || x.GetType() == typeof(ArmBandView)).Select(x => x.gameObject);

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

        public class SettingsData
        {
            public ConfigEntry<bool> KeyPlayerSkinHide;
            public ConfigEntry<bool> KeyBotSkinHide;

            public ConfigEntry<Part> KeyPlayerSkinHidePart;
            public ConfigEntry<Part> KeyBotSkinHidePart;

            public ConfigEntry<KeyboardShortcut> KBSPlayerSkinHide;
            public ConfigEntry<KeyboardShortcut> KBSBotSkinHide;
        }

        public class ReflectionData
        {
            public RefHelp.FieldRef<PlayerBody, object> RefSlotViews;
            public RefHelp.FieldRef<object, IEnumerable<object>> RefSlotList;
            public RefHelp.FieldRef<object, Dress[]> RefDresses;
        }
    }
}