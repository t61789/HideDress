using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System.Reflection;
using EFT;

namespace SkinHide.Patches
{
    public class BotOwnerPatch : ModulePatch
    {

        protected override MethodBase GetTargetMethod()
        {
            return typeof(BotOwner).GetMethod("method_4", PatchConstants.PrivateFlags);
        }

        [PatchPostfix]
        private static void PatchPostfix(BotOwner __instance)
        {
            SkinHidePlugin.Bot.Add(__instance.gameObject);
        }
    }
}
