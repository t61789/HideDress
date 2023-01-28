using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System;
using System.Reflection;
using System.Threading.Tasks;
using EFT;

namespace SkinHide.Patches
{
    public class PlayerPatch : ModulePatch
    {
        private static readonly bool Is231Up = SkinHidePlugin.GameVersion > new Version("0.12.12.17349");

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("Init", PatchConstants.PrivateFlags);
        }

        [PatchPostfix]
        private async static void PatchPostfix(Task __result, Player __instance)
        {
            await __result;

            bool isYouPlayer;

            if (Is231Up)
            {
                isYouPlayer = __instance.IsYourPlayer;
            }
            else
            {
                isYouPlayer = __instance.Id == 1;
            }

            if (isYouPlayer)
            {
                SkinHidePlugin.Player = __instance.PlayerBody;
            }
            else
            {
                SkinHidePlugin.Bot.Add(__instance.PlayerBody);
            }
        }
    }
}
