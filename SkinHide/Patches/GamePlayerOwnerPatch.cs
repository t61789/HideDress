using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System.Reflection;
using EFT;

namespace SkinHide.Patches
{
    public class GamePlayerOwnerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GamePlayerOwner).GetMethod("method_6", PatchConstants.PrivateFlags);
        }

        [PatchPostfix]
        private static void PatchPostfix(GamePlayerOwner __instance)
        {
            SkinHidePlugin.Player = __instance.gameObject.GetComponentInChildren<PlayerBody>();
        }
    }
}
