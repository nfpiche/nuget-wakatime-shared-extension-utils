﻿using System;
using System.Net;
using System.Text.RegularExpressions;

namespace WakaTime.Shared.ExtensionUtils
{
    internal static class Constants
    {
        internal const string PluginName = "visualstudio-wakatime";

        internal static string PluginVersion =
            $"{WakaTime.CoreAssembly.Version.Major}.{WakaTime.CoreAssembly.Version.Minor}.{WakaTime.CoreAssembly.Version.Build}";
       
        internal const string CliUrl = "https://github.com/wakatime/wakatime/archive/master.zip";
        internal const string CliFolder = @"wakatime-master\wakatime\cli.py";

        internal static Func<string> LatestWakaTimeCliVersion = () =>
        {
            var regex = new Regex(@"(__version_info__ = )(\(( ?\'[0-9]+\'\,?){3}\))");

            if (!ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls12))
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            }
            var client = new WebClient { Proxy = new Proxy().Get() };

            try
            {
                var about = client.DownloadString("https://raw.githubusercontent.com/wakatime/wakatime/master/wakatime/__about__.py");
                var match = regex.Match(about);

                if (match.Success)
                {
                    var grp1 = match.Groups[2];
                    var regexVersion = new Regex("([0-9]+)");
                    var match2 = regexVersion.Matches(grp1.Value);
                    return $"{match2[0].Value}.{match2[1].Value}.{match2[2].Value}";
                }

                Logger.Warning("Couldn't auto resolve wakatime cli version");
            }
            catch (Exception ex)
            {
                Logger.Error("Exception when checking current wakatime cli version: ", ex);
            }

            return string.Empty;
        };
    }
}