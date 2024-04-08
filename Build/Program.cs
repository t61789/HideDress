using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CopyBuildAssembly;

// ReSharper disable ClassNeverInstantiated.Global

namespace Build
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var arg = args.ElementAtOrDefault(0);
            var sha = Copy.GetTipSha(args.ElementAtOrDefault(1));

            const string modPath =
                @"R:\Battlestate Games\Client.0.14.1.2.29197\BepInEx\plugins\kmyuhkyuk-HideDress";

            var versionName = "1.2.7";

            var releaseName = $"{new DirectoryInfo(modPath).Name}-(Release_{versionName}).7z";

            try
            {
                Copy.CopyFolder(arg, "Release", Path.Combine(baseDirectory, "localized"),
                    Path.Combine(modPath, "localized"));

                Copy.CopyAssembly(arg, "Release", baseDirectory, modPath, new[]
                {
                    "HideDress"
                }, sha);

                Copy.GenerateSevenZip(arg, "Release", modPath, releaseName, @"BepInEx\plugins", Array.Empty<string>(),
                    Array.Empty<string>(), new[] { Path.Combine(baseDirectory, "ReadMe.txt") }, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                Console.ReadKey();

                Process.GetCurrentProcess().Kill();
            }
        }
    }
}