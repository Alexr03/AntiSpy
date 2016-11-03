using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using Rocket.Core;
using Rocket.Unturned.Chat;

namespace AntiSpy
{
    public class Main : RocketPlugin<AntiSpyConfiguration>
    {
        public Main Instance;
        private static Timer cooldownTimer;
        private static Timer deleteUsers;
        public static bool t;

        protected override void Load()
        {
            CheckKey(Configuration.Instance.Key);
            if (t)
            {
                Instance = this;
                Rocket.Core.Logging.Logger.Log("AntiSpy Protector has loaded.");

                cooldownTimer = new System.Timers.Timer();
                cooldownTimer.Elapsed += new ElapsedEventHandler(timerFunc);
                cooldownTimer.Interval = 100000;
                cooldownTimer.Enabled = true;

                deleteUsers = new System.Timers.Timer();
                deleteUsers.Elapsed += new ElapsedEventHandler(timerFunc2);
                deleteUsers.Interval = 100000;
                deleteUsers.Enabled = false;

                U.Events.OnPlayerConnected += OnConnected;
                U.Events.OnPlayerDisconnected += Disconnected;
                Main.cooldownTimer.Enabled = true;
                CommandCheckAnti.ToProcessPic.Clear();
                CommandCheckAnti.ToUpload.Clear();
            }
            else
            {
                Rocket.Core.Logging.Logger.LogError("Key incorrect, unloading...");
                Unload();
            }
        }

        private void timerFunc2(object sender, ElapsedEventArgs e)
        {
            foreach(CSteamID user in CommandCheckAnti.ToUpload)
            {
                UnturnedPlayer p = UnturnedPlayer.FromCSteamID(user);
                UnturnedChat.Say("User: " + p);
                Provider.kick(user, "Using Antispy");
            }
            CommandCheckAnti.ToUpload.Clear();
        }

        private void OnConnected(UnturnedPlayer player)
        {
            player.Player.sendScreenshot(CommandCheckAnti.ID);
        }

        private void timerFunc(object sender, ElapsedEventArgs e)
        {
            ProcessUser();
        }

        protected override void Unload()
        {
            Rocket.Core.Logging.Logger.Log("AntiSpy Protector has Unloaded.");
            U.Events.OnPlayerConnected -= OnConnected;
            U.Events.OnPlayerDisconnected -= Disconnected;
        }

        private void Disconnected(UnturnedPlayer player)
        {
            if (CommandCheckAnti.ToProcessPic.Contains(player))
            {
                CommandCheckAnti.ToProcessPic.Remove(player);
            }
        }

        private void FixedUpdate()
        {
        }

        public static void ProcessUser()
        {
            foreach (UnturnedPlayer v in CommandCheckAnti.ToProcessPic)
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(CommandCheckAnti.directory + "/Spy/" + v.CSteamID + ".jpg");

                if (File.Exists(img.ToString()))
                {
                    if (img.Height == 480 || img.Width == 640)
                    {
                        Rocket.Core.Logging.Logger.Log("User " + v.CharacterName + " is not using fake screenshots");
                        img.Dispose();
                    }
                    else
                    {
                        Rocket.Core.Logging.Logger.Log("User: " + v.CharacterName + " Screenshot matches fake screenshots!");
                        if (!CommandCheckAnti.BlacklistIDs.Contains(v.CSteamID))
                        {
                            CommandCheckAnti.BlacklistIDs.Add(v.CSteamID);
                        }
                        else { }
                        if (!CommandCheckAnti.ToUpload.Contains(v.CSteamID))
                        {
                            CommandCheckAnti.ToUpload.Add(v.CSteamID);
                        }
                        else { }
                        img.Dispose();
                    }
                    img.Dispose();
                    foreach (CSteamID y in CommandCheckAnti.ToUpload)
                    {
                        UnturnedPlayer p = UnturnedPlayer.FromCSteamID(y);
                        R.Commands.Execute(UnturnedPlayer.FromCSteamID((CSteamID)((long)0)), "broadcast " + p.CharacterName + " testing anti-spy protector");
                        CommandCheckAnti.ToUpload.Remove(y);
                        img.Dispose();
                    }
                    deleteUsers.Enabled = true;
                }
                else { Rocket.Core.Logging.Logger.Log("User: " + v.CharacterName + " screenshot does not exist!"); v.Player.sendScreenshot(CommandCheckAnti.ID); CommandCheckAnti.ToProcessPic.Remove(v); }
            }
            CommandCheckAnti.ToProcessPic.Clear();
        }

        public static void CheckKey(string Key)
        {
            bool enabled = false;
            var http = (HttpWebRequest)WebRequest.Create("http://solarsentinels.co.uk/keys.txt");
            var response = http.GetResponse();

            var stream = response.GetResponseStream();
            var sr = new StreamReader(stream);
            var content = sr.ReadToEnd();


            if (content.Contains(Key)) { enabled = true; }
            else { enabled = false; }
            t = enabled;
        }
    }
}
