using Rocket.Core.Plugins;
using SDG.Unturned;
using Rocket.Unturned.Player;
using Steamworks;
using UnturnedBot.Unturned.Structures;
using System.ServiceModel;
using System;
using System.Xml;
using System.Threading;
using System.IO.Pipes;
using System.IO;
using System.ServiceModel.Description;
using System.IdentityModel.Selectors;
using System.Net;
using System.Collections.Specialized;
using UnturnedBot.Unturned.WebUtils;

namespace UnturnedBot.Unturned
{
    class Program : RocketPlugin<ProgramConfiguration>/*, MainService.IMainServiceCallback*/
    {
        public static Program instance;
        public static ProgramConfiguration configuration;

        public void SendNotification(string notification)
        {
            Logger.Log("[Notification] " + notification);
        }

        protected override void Load()
        {
            base.Load();

            Logger.Log("[UnturnedToDiscord] Loading...");
            instance = this;
            configuration = Configuration.Instance;

            StructureExample.SetupTimers();

            //UnturnedBotPipe.StartClient();
            var response = WebClientUtils.Request("getname/phandaros");
            Console.WriteLine("Response: " + response);

            Provider.onServerDisconnected += PlayerDisconnectCallback;
            Provider.onServerConnected += PlayerConnectCallbackAsync;

            Logger.Log("[UnturnedToDiscord] Loaded.");
        }
        protected override void Unload()
        {
            Destroy(this);

        }

        private void PlayerConnectCallbackAsync(CSteamID steamID)
        {
            var player = UnturnedPlayer.FromCSteamID(steamID);
            Logger.Log("[PlayerConnected] " + player.DisplayName);
            //UnturnedToDiscordPipe.NotifyPlayerConnected(player.DisplayName);
        }

        private void PlayerDisconnectCallback(CSteamID steamID)
        {
            var player = UnturnedPlayer.FromCSteamID(steamID);

            Logger.Log("[PlayerDisconnected] " + player.DisplayName);
            //UnturnedToDiscordPipe.NotifyPlayerDisconnected(player.DisplayName);
        }
    }
}
