using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Timers;

namespace AntiSpy
{
    class CommandCheckAnti : IRocketCommand
    {
        public static string directory = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName;
        public static string directory2 = Directory.GetParent(directory).ToString();
        public static string root = Directory.GetParent(directory2).ToString();
        public static List<CSteamID> BlacklistIDs = new List<CSteamID>();
        public static List<UnturnedPlayer> ToProcessPic = new List<UnturnedPlayer>();
        public static List<CSteamID> ToUpload = new List<CSteamID>();
        public static CSteamID ID = (CSteamID)0;
        public static IRocketPlayer f { get; }

        public string Help
        {
            get { return ""; }
        }

        public string Name
        {
            get { return "checkanti"; }
        }

        public string Syntax
        {
            get { return "/checkanti"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Both; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "anti.spy" };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            IRocketPlayer f = caller;
            foreach (SteamPlayer victim in Provider.clients)
            {
                UnturnedPlayer user = UnturnedPlayer.FromSteamPlayer(victim);
                user.Player.sendScreenshot(ID);
                if (File.Exists(directory + "/Spy/" + user.CSteamID + ".jpg")) { ToProcessPic.Add(user); }
                else
                {
                    user.Player.sendScreenshot(ID);
                }
                Main.ProcessUser();
            }
        }
    }
}
