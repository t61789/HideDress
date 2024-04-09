using System;
using EFT;

namespace HideDress.Models
{
    internal class HideDressModel
    {
        private static readonly Lazy<HideDressModel> Lazy = new Lazy<HideDressModel>(() => new HideDressModel());

        public static HideDressModel Instance => Lazy.Value;

        public PlayerBody PlayerModelViewBody;

        public enum DressPart
        {
            Both,
            Dress,
            SkinDress,
            None
        }

        private HideDressModel()
        {
        }
    }
}