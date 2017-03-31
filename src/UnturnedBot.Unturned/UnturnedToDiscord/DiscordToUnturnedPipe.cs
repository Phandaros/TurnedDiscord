using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using UnturnedBot.Unturned.Structures;

namespace UnturnedBot.Unturned.UnturnedToDiscord
{
    class DiscordToUnturnedPipe
    {
        public static List<string> Queue = new List<string>();
        private static int nextTick = Environment.TickCount + Program.configuration.discordMessagesCheckDelay;

        public static StreamWriter writer;
        public static StreamReader reader;

        public static void Initialize()
        {
            var client = new NamedPipeClientStream(".", "DiscordToUnturned", PipeDirection.InOut);
            client.Connect();

            writer = new StreamWriter(client)
            {
                AutoFlush = true
            };

            reader = new StreamReader(client);
        }

        public static void Update()
        {
            if (Environment.TickCount >= nextTick)
            {
                nextTick = Environment.TickCount + Program.configuration.discordMessagesCheckDelay;

                writer.WriteLine("getCommand");

                HandleInput(reader.ReadLine());
                reader.BaseStream.Flush();
            }
        }

        private static void HandleInput(string input)
        {
            if (input == "empty") return;

            Logger.Log("[Discord] Input: " + input);

            switch (input.ToLower())
            {
                case "getplayers":
                    writer.WriteLine("Player Count: " + Provider.clients.Count);
                    break;
                case "addstructures":
                    StructureExample.AddNewStructuresToList();
                    writer.WriteLine(StructureExample.Structures.Count);
                    break;
                case "checkstructures":
                    writer.WriteLine(StructureExample.CheckStructuresHealth());
                    break;
                default:
                    Logger.Log("[Discord] No CMD for " + input);
                    break;
            }
        }
    }
}
