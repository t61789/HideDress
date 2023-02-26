using BepInEx;
using BepInEx.Configuration;
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using EFT;
using EFT.Visual;
using SkinHide.Patches;
using SkinHide.Utils;
using EFT.InventoryLogic;

namespace SkinHide
{
    [BepInPlugin("com.kmyuhkyuk.SkinHide", "kmyuhkyuk-SkinHide", "1.2.7")]
    public class SkinHidePlugin : BaseUnityPlugin
    {
        internal static PlayerBody Player;

        internal static PlayerBody PlayerModelView;

        internal static List<PlayerBody> Bot = new List<PlayerBody>();

        private readonly SettingsData SettingsDatas = new SettingsData();

        private readonly ReflectionData ReflectionDatas = new ReflectionData();

        private bool PMVHideCache;

        private bool PlayerHideCache;

        private bool BotHideCache;

        internal static Version GameVersion { get; private set; }

        public enum Part
        {
            All,
            Dress,
            SkinDress
        }

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-SkinHide");

            FileVersionInfo exeInfo = Process.GetCurrentProcess().MainModule.FileVersionInfo;

            GameVersion = new Version(exeInfo.FileMajorPart, exeInfo.ProductMinorPart, exeInfo.ProductBuildPart, exeInfo.FilePrivatePart);

            const string skinHideSettings = "Skin Hide Settings";
            const string skinHidePartSettings = "隐藏部分设置 Skin Hide Part Settings";
            const string kbsSettings = "快捷键设置 Keyboard Shortcut Settings";

            SettingsDatas.KeyPlayerSkinHide = Config.Bind<bool>(skinHideSettings, "玩家服装隐藏 Player Skin Hide", false);
            SettingsDatas.KeyBotSkinHide = Config.Bind<bool>(skinHideSettings, "Bot服装隐藏 Bot Skin Hide", false);

            SettingsDatas.KeyPlayerSkinHidePart = Config.Bind<Part>(skinHidePartSettings, "Player", Part.All);
            SettingsDatas.KeyBotSkinHidePart = Config.Bind<Part>(skinHidePartSettings, "Bot", Part.All);

            SettingsDatas.KBSPlayerSkinHide = Config.Bind<KeyboardShortcut>(kbsSettings, "玩家服装隐藏快捷键 Player Skin Hide", KeyboardShortcut.Empty);
            SettingsDatas.KBSBotSkinHide = Config.Bind<KeyboardShortcut>(kbsSettings, "Bot服装隐藏快捷键 Bot Skin Hide", KeyboardShortcut.Empty);

            new PlayerModelViewPatch().Enable();
            new PlayerPatch().Enable();

            ReflectionDatas.RefSlotViews = RefHelp.FieldRef<PlayerBody, object>.Create("SlotViews");
            ReflectionDatas.RefSlotList = RefHelp.FieldRef<object, IEnumerable<object>>.Create(ReflectionDatas.RefSlotViews.FieldType, "list_0");
            ReflectionDatas.RefDresses = RefHelp.FieldRef<object, Dress[]>.Create(ReflectionDatas.RefSlotList.FieldType.GetGenericArguments()[0], "Dresses");
            ReflectionDatas.RefRenderers = RefHelp.FieldRef<Dress, Renderer[]>.Create("Renderers");
        }

        void Update()
        {
            if (SettingsDatas.KBSPlayerSkinHide.Value.IsDown())
            {
                SettingsDatas.KeyPlayerSkinHide.Value = !SettingsDatas.KeyPlayerSkinHide.Value;
            }
            if (SettingsDatas.KBSBotSkinHide.Value.IsDown())
            {
                SettingsDatas.KeyBotSkinHide.Value = !SettingsDatas.KeyBotSkinHide.Value;
            }

            //PlayerModelView Skin Hide
            if (PlayerModelView != null)
            {
                if (SettingsDatas.KeyPlayerSkinHide.Value)
                {
                    Hide(PlayerModelView, SettingsDatas.KeyPlayerSkinHidePart.Value, true);

                    PMVHideCache = true;
                }
                else if (!SettingsDatas.KeyPlayerSkinHide.Value && PMVHideCache)
                {
                    Hide(PlayerModelView, Part.All, false);

                    PMVHideCache = false;
                }
            }

            //Player Skin Hide
            if (Player != null)
            {
                if (SettingsDatas.KeyPlayerSkinHide.Value)
                {
                    Hide(Player, SettingsDatas.KeyPlayerSkinHidePart.Value, true);

                    PlayerHideCache = true;
                }
                else if (!SettingsDatas.KeyPlayerSkinHide.Value && PlayerHideCache)
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
            if (Bot.Count > 0)
            {
                if (SettingsDatas.KeyBotSkinHide.Value)
                {
                    foreach (PlayerBody bot in Bot)
                    {
                        Hide(bot, SettingsDatas.KeyBotSkinHidePart.Value, true);
                    }

                    BotHideCache = true;
                }
                else if (!SettingsDatas.KeyBotSkinHide.Value && BotHideCache)
                {
                    foreach (PlayerBody bot in Bot)
                    {
                        Hide(bot, Part.All, false);
                    }

                    BotHideCache = false;
                }
            }
        }

        void Hide(PlayerBody playerbody, Part part, bool hide)
        {
            object slotViews = ReflectionDatas.RefSlotViews.GetValue(playerbody);

            IEnumerable<object> slotList = ReflectionDatas.RefSlotList.GetValue(slotViews);

            IEnumerable<Dress> dresses = slotList.SelectMany(x => ReflectionDatas.RefDresses.GetValue(x)).Where(x => x != null);

            IEnumerable<Dress> dress = dresses.Where(x => x.GetType() == typeof(Dress));

            IEnumerable<Renderer> renDress = dress.SelectMany(x => ReflectionDatas.RefRenderers.GetValue(x));

            IEnumerable<GameObject> skinDress = dresses.Where(x => x.GetType() == typeof(SkinDress) || x.GetType() == typeof(ArmBandView)).Select(x => x.gameObject);

            switch (part)
            {
                case Part.All:
                    foreach (GameObject gameobject in skinDress)
                    {
                        gameobject.SetActive(!hide);
                    }

                    foreach (MeshRenderer renderer in renDress)
                    {
                        renderer.enabled = !hide;
                    }
                    break;
                case Part.Dress:
                    foreach (MeshRenderer renderer in renDress)
                    {
                        renderer.enabled = !hide;
                    }
                    break;
                case Part.SkinDress:
                    foreach (GameObject gameobject in skinDress)
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
            public RefHelp.FieldRef<Dress, Renderer[]> RefRenderers;
        }
    }
}