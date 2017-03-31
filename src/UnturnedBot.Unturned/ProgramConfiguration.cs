using Rocket.API;
using System.Xml.Serialization;

namespace UnturnedBot.Unturned
{
    public class ProgramConfiguration : IRocketPluginConfiguration
    {
        [XmlElement("CheckDelayForDiscordMessages")]
        public int discordMessagesCheckDelay;
        [XmlElement("CheckDelayForDamagedStructures")]
        public int checkForDamagedStructuresDelay;
        public void LoadDefaults()
        {
            discordMessagesCheckDelay = 1000;
            checkForDamagedStructuresDelay = 3000;
        }
    }
}
