using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System.Reflection;
using System.Threading.Tasks;
using EFT;

namespace SkinHide.Patches
{
    public class PlayerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("Init", PatchConstants.PrivateFlags);
        }

        [PatchPostfix]
        private async static void PatchPostfix(Task __result, Player __instance)
        {
            await __result;

            if (__instance.IsYourPlayer)
            {
                SkinHidePlugin.Player = __instance.gameObject.GetComponentInChildren<PlayerBody>();
            }
        }
    }
}
