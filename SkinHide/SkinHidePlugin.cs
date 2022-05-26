using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using SkinHide.Patches;
using EFT;
using EFT.Visual;
using System.Collections.Generic;

namespace SkinHide
{
    [BepInPlugin("com.kmyuhkyuk.SkinHide", "kmyuhkyuk-SkinHide", "1.1.0")]
    public class SkinHidePlugin : BaseUnityPlugin
    {
        public static GameObject Player;

        public static GameObject PlayerModelView;

        public SkinDress[] PlayerMVSkinDress;

        public Dress[] PlayerMVDress;

        public SkinDress[] PlayerSkinDress;

        public Dress[] PlayerDress;

        public HashSet<GameObject> PlayerSkinGameObject = new HashSet<GameObject>();

        public HashSet<GameObject> PlayerDressGameObject = new HashSet<GameObject>();

        public static List <GameObject> Bot = new List<GameObject>();

        public HashSet<GameObject> BotSkinGameObject = new HashSet<GameObject>();

        public HashSet<GameObject> BotDressGameObject = new HashSet<GameObject>();

        public static ConfigEntry<bool> KeyPlayerSkinHide { get; set; }
        public static ConfigEntry<bool> KeyBotSkinHide { get; set; }

        public static ConfigEntry<bool> KeyBotSkinHideShutDown { get; set; }

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-SkinHide");

            string SkinHide = "Skin Hide";
            KeyPlayerSkinHide = Config.Bind<bool>(SkinHide, "玩家服装隐藏 Player Skin Hide", false);
            KeyBotSkinHide = Config.Bind<bool>(SkinHide, "Bot服装隐藏 Bot Skin Hide", false);
            KeyBotSkinHideShutDown = Config.Bind<bool>(SkinHide, "Bot服装隐藏功能关闭 Bot Skin Hide Function Shut Down", false, "Many Bot corpse will cause lag, turn the switch off Bot Skin Scan.");

            new PlayerModelViewPatch().Enable();
            new GamePlayerOwnerPatch().Enable();
            new BotOwnerPatch().Enable();
        }
        void Update()
        {
            //PlayerModelView Skin Hide
            if (PlayerModelView != null && KeyPlayerSkinHide.Value)
            {
                //Get PlayerModelView all SkinDress and Dress
                PlayerMVSkinDress = PlayerModelView.GetComponentsInChildren<SkinDress>();
                PlayerMVDress = PlayerModelView.GetComponentsInChildren<Dress>();

                //False SkinDress and Dress GameObject
                if (PlayerMVSkinDress != null)
                {
                    foreach (SkinDress skindress in PlayerMVSkinDress)
                    {
                        skindress.gameObject.SetActive(false);
                    }
                }
                if (PlayerMVDress != null)
                {
                    foreach (Dress dress in PlayerMVDress)
                    {
                        dress.gameObject.SetActive(false);
                    }
                }
            }
            //Player Skin Hide
            if (Player != null)
            {
                //Get Player all SkinDress and Dress
                PlayerSkinDress = Player.transform.Find("Player/Mesh").gameObject.GetComponentsInChildren<SkinDress>();
                PlayerDress = Player.transform.Find("Player/Root_Joint").gameObject.GetComponentsInChildren<Dress>();

                //False SkinDress GameObject
                if (PlayerSkinDress != null)
                {
                    foreach (SkinDress skindress in PlayerSkinDress)
                    {
                        PlayerSkinGameObject.Add(skindress.gameObject);
                    }
                }
                if (PlayerDress != null)
                {
                    foreach (Dress dress in PlayerDress)
                    {
                        PlayerDressGameObject.Add(dress.gameObject);
                    }   
                }

                //Hide Dress GameObject
                if (PlayerDressGameObject != null)
                {
                    List<GameObject> Loot = new List<GameObject>();

                    foreach (GameObject dress in PlayerDressGameObject)
                    {
                        MeshRenderer[] MeshRenderer = dress.GetComponentsInChildren<MeshRenderer>();

                        //Loot False Hide
                        if (dress != null && dress.GetComponentInParent<GamePlayerOwner>() != null)
                        {
                            foreach (MeshRenderer mesh in MeshRenderer)
                            {
                                mesh.enabled = !KeyPlayerSkinHide.Value;
                            }
                        }
                        else
                        {
                            foreach (MeshRenderer mesh in MeshRenderer)
                            {
                                mesh.enabled = true; 
                            }
                            Loot.Add(dress);
                        }
                    }

                    if (Loot != null)
                    {
                        PlayerDressGameObject.ExceptWith(Loot);
                    }
                }

                //False or true SkinDress and Dress GameObject
                if (PlayerSkinGameObject != null)
                {
                    List<GameObject> Loot = new List<GameObject>();

                    foreach (GameObject skin in PlayerSkinGameObject)
                    {
                        if (skin != null && skin.GetComponentInParent<GamePlayerOwner>() != null)
                        {
                            skin.SetActive(!KeyPlayerSkinHide.Value);
                        }
                        else
                        {
                            skin.SetActive(true);

                            Loot.Add(skin);
                        }
                    }

                    //Loot from List Remove 
                    if (Loot != null)
                    {
                        PlayerSkinGameObject.ExceptWith(Loot);
                    }
                }
            }
            else
            {
                //Quit Raid Clear GameObject List
                PlayerSkinGameObject.Clear();
                PlayerDressGameObject.Clear();
            }

            //Clear List null Bot
            Bot.RemoveAll(x => x == null);
            //Bot Skin Hide
            if (Bot != null && !KeyBotSkinHideShutDown.Value)
            {
                //Get Bot all SkinDress and Dress
                foreach (GameObject bot in Bot)
                {
                    SkinDress[] botskindress = bot.transform.Find("Player/Mesh").gameObject.GetComponentsInChildren<SkinDress>();

                    foreach (SkinDress skinDress in botskindress)
                    {
                        BotSkinGameObject.Add(skinDress.gameObject);
                    }

                    Dress[] botDress = bot.transform.Find("Player/Root_Joint").gameObject.GetComponentsInChildren<Dress>();

                    foreach (Dress Dress in botDress)
                    {
                        BotDressGameObject.Add(Dress.gameObject);
                    }
                }

                //Hide Dress GameObject
                if (BotDressGameObject != null)
                {
                    List<GameObject> Loot = new List<GameObject>();

                    foreach (GameObject botdress in BotDressGameObject)
                    {
                        MeshRenderer[] MeshRenderer = botdress.GetComponentsInChildren<MeshRenderer>();

                        //Loot False Hide
                        if (botdress.GetComponentInParent<BotOwner>() != null)
                        {
                            foreach (MeshRenderer botmesh in MeshRenderer)
                            {
                                botmesh.enabled = !KeyBotSkinHide.Value;
                            }
                        }
                        else
                        {
                            foreach (MeshRenderer botmesh in MeshRenderer)
                            {
                                botmesh.enabled = true;  
                            }
                            Loot.Add(botdress);
                        }
                    }

                    //Loot from List Remove 
                    if (Loot != null)
                    {
                        BotDressGameObject.ExceptWith(Loot);
                    }
                }

                //False or true SkinDress GameObject
                if (BotSkinGameObject != null)
                {
                    List<GameObject> Loot = new List<GameObject>();

                    foreach (GameObject botskin in BotSkinGameObject)
                    {
                        if (botskin.GetComponentInParent<BotOwner>() != null)
                        {
                            botskin.SetActive(!KeyBotSkinHide.Value);
                        }
                        else
                        {
                            botskin.SetActive(true);

                            Loot.Add(botskin);
                        }
                    }

                    //Loot from List Remove 
                    if (Loot != null)
                    {
                        BotSkinGameObject.ExceptWith(Loot);
                    }
                }
            }
            else
            {
                //Quit Raid Clear GameObject List
                BotSkinGameObject.Clear();
                BotDressGameObject.Clear();
            }
        }

    }
}