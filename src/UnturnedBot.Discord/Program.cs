using System;
using UnturnedBot.Discord.Discord;

namespace UnturnedBot.Discord
{
    class Program
    {
        public static Program instance = new Program();
        public static void Start()
        {
            DiscordBot.Start();
            HttpServer.ServerMain.Start();
        }

        public void SendNotification(string notification)
        {
            Utils.Logger.Log("[Notification] " + notification);
        }
    }
}
