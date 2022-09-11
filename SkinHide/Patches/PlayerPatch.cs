using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System.Reflection;
using System.Threading.Tasks;
using Comfort.Common;
using EFT;

namespace SkinHide.Patches
{
    public class PlayerPatch : ModulePatch
    {
        private static bool? Is231Up;

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("Init", PatchConstants.PrivateFlags);
        }

        [PatchPostfix]
        private async static void PatchPostfix(Task __result, Player __instance)
        {
            await __result;

            if (!Is231Up.HasValue)
            {
                Is231Up = typeof(Player).GetProperty("IsYourPlayer").GetSetMethod() == null;
            }

            bool isyouplayer;

            if ((bool)Is231Up)
            {
                isyouplayer = __instance.IsYourPlayer;
            }
            else
            {
                isyouplayer = Singleton<GameWorld>.Instance.AllPlayers[0] == __instance;
            }

            if (isyouplayer)
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
