using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using EFT;
using EFT.InventoryLogic;
using EFT.Visual;
using HideDress.Attributes;
using HideDress.Models;
using System.Linq;
using UnityEngine;
using static EFTApi.EFTHelpers;

namespace HideDress
{
    [BepInPlugin("com.kmyuhkyuk.HideDress", "HideDress", "1.3.1")]
    [BepInDependency("com.kmyuhkyuk.EFTApi", "1.2.1")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/654-hide-dress")]
    public partial class HideDressPlugin : BaseUnityPlugin
    {
        private static readonly List<HideConfig> _hideConfigs = new List<HideConfig>();
        
        private struct HideConfig
        {
            public PlayerBody body;
            public bool enableHide;
            public bool hideBackpack;
            public bool hideArmorVest;
            public bool hideTacticalVest;
            public bool hideHeadWear;
            public bool hideEarPiece;
            public bool hideEyeWear;
            public bool hideArmBand;
            public bool hideFaceCover;
        }

        private float _preUpdateTime = -999;

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
            var settingsModel = SettingsModel.Instance;
            if (Time.time - _preUpdateTime < settingsModel.UpdateInterval.Value)
            {
                return;
            }
            _preUpdateTime = Time.time;
            
            if (settingsModel.SwitchHidingShortcut.Value.IsDown())
            {
                settingsModel.EnableHiding.Value = !settingsModel.EnableHiding.Value;
            }

            GradAllHideConfigs(_hideConfigs, settingsModel.EnableHiding.Value);
            StartHiding(_hideConfigs);
        }
        
        private void GradAllHideConfigs(List<HideConfig> hideConfigs, bool enableHide)
        {
            var specifiedPlayerNickNames = SettingsModel.Instance.SpecifiedOtherNickName.Value.Split(',')
                .Select(x => x.Trim()).Where(x => x != "").ToHashSet();
            
            hideConfigs.Clear();
            Add(HideDressModel.Instance.PlayerModelViewBody, true, enableHide);
            Add(_PlayerHelper.Player?.PlayerBody, true, enableHide);
            if (_GameWorldHelper.GameWorld != null)
            {
                foreach (var otherPlayer in _GameWorldHelper.AllOtherPlayer)
                {
                    if (!otherPlayer)
                    {
                        continue;
                    }
                    
                    var curEnableHide = enableHide;
                    if (specifiedPlayerNickNames.Count != 0 &&
                        !specifiedPlayerNickNames.Contains(otherPlayer.Profile.Nickname))
                    {
                        curEnableHide = false;
                    }
                    Add(otherPlayer?.PlayerBody, false, curEnableHide);
                }
            }

            return;

            void Add(PlayerBody playerBody, bool isPlayer, bool hide)
            {
                if (playerBody == null)
                {
                    return;
                }
                
                hideConfigs.Add(CreateHideConfig(playerBody, isPlayer, hide));
            }
        }

        private void StartHiding(List<HideConfig> hideConfigs)
        {
            foreach (var hideConfig in hideConfigs)
            {
                var reflectionModel = ReflectionModel.Instance;
                var slotViews = reflectionModel.RefSlotViews.GetValue(hideConfig.body);
                var slotDic = (Dictionary<EquipmentSlot, PlayerBody.GClass1875>)reflectionModel.RefSlotDic.GetValue(slotViews);
                foreach (var (equipmentSlot, slot) in slotDic)
                {
                    var dresses = reflectionModel.RefDresses.GetValue(slot);
                    if (dresses == null)
                    {
                        continue;
                    }

                    foreach (var dress in dresses)
                    {
                        SetEnable(dress, equipmentSlot, hideConfig);
                    }
                }
            }
        }

        private void SetEnable(Dress dress, EquipmentSlot slot, in HideConfig hideConfig)
        {
            var hide = slot == EquipmentSlot.Backpack && hideConfig.hideBackpack;
            hide = hide || (slot == EquipmentSlot.ArmorVest && hideConfig.hideArmorVest);
            hide = hide || (slot == EquipmentSlot.TacticalVest && hideConfig.hideTacticalVest);
            hide = hide || (slot == EquipmentSlot.Headwear && hideConfig.hideHeadWear);
            hide = hide || (slot == EquipmentSlot.Earpiece && hideConfig.hideEarPiece);
            hide = hide || (slot == EquipmentSlot.Eyewear && hideConfig.hideEyeWear);
            hide = hide || (slot == EquipmentSlot.ArmBand && hideConfig.hideArmBand);
            hide = hide || (slot == EquipmentSlot.FaceCover && hideConfig.hideFaceCover);

            hide = hide && hideConfig.enableHide;
            
            var dressType = dress.GetType();
            if (dressType == typeof(SkinDress) || dressType == typeof(ArmBandView))
            {
                dress.gameObject.SetActive(!hide);
            }
            else if (dressType == typeof(Dress))
            {
                foreach (var renderer in ReflectionModel.Instance.RefRenderers.GetValue(dress))
                {
                    renderer.enabled = !hide;
                } 
            }
        }

        private static HideConfig CreateHideConfig(PlayerBody body, bool isPlayer, bool enableHide)
        {
            var result = isPlayer ? 
                TakeHideConfigFromPlayer(SettingsModel.Instance) : 
                TakeHideConfigFromOther(SettingsModel.Instance);
            result.body = body;
            result.enableHide = enableHide;
            return result;
        }

        private static HideConfig TakeHideConfigFromPlayer(SettingsModel settingsModel)
        {
            return new HideConfig
            {
                hideBackpack = settingsModel.PlayerHideBackpack.Value,
                hideArmorVest = settingsModel.PlayerHideArmorVest.Value,
                hideTacticalVest = settingsModel.PlayerHideTacticalVest.Value,
                hideHeadWear = settingsModel.PlayerHideHeadWear.Value,
                hideEarPiece = settingsModel.PlayerHideEarPiece.Value,
                hideEyeWear = settingsModel.PlayerHideEyeWear.Value,
                hideArmBand = settingsModel.PlayerHideArmBand.Value,
                hideFaceCover = settingsModel.PlayerHideFaceCover.Value
            };
        }
        
        private static HideConfig TakeHideConfigFromOther(SettingsModel settingsModel)
        {
            return new HideConfig
            {
                hideBackpack = settingsModel.OtherHideBackpack.Value,
                hideArmorVest = settingsModel.OtherHideArmorVest.Value,
                hideTacticalVest = settingsModel.OtherHideTacticalVest.Value,
                hideHeadWear = settingsModel.OtherHideHeadWear.Value,
                hideEarPiece = settingsModel.OtherHideEarPiece.Value,
                hideEyeWear = settingsModel.OtherHideEyeWear.Value,
                hideArmBand = settingsModel.OtherHideArmBand.Value,
                hideFaceCover = settingsModel.OtherHideFaceCover.Value
            };
        }
    }
}