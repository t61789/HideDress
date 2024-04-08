using System.Threading.Tasks;
using EFT.UI;
using HideDress.Models;

namespace HideDress
{
    public partial class HideDressPlugin
    {
        private static async void PlayerModelViewShow(Task __result, PlayerModelView __instance)
        {
            await __result;

            HideDressModel.Instance.PlayerModelViewBody = __instance.PlayerBody;
        }
    }
}