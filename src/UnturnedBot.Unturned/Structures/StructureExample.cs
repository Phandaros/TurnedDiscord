using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace UnturnedBot.Unturned.Structures
{
    class StructureExample
    {
        public static Dictionary<StructureData, ushort> Structures = new Dictionary<StructureData, ushort>();
        public static void SetupTimers()
        {
            Timer checkStructures = new Timer(Program.configuration.checkForDamagedStructuresDelay);
            checkStructures.Elapsed += (e, a) =>
            {
                AddNewStructuresToList();
                CheckStructuresHealth();
            };
            checkStructures.Start();
            Logger.Log("[StructureManager] Timers setup");
        }
        public static void AddNewStructuresToList()
        {
            StructureRegion structureRegion;
            StructureData sData = null;
            int transformCount = 0;

            for (int k = 0; k < StructureManager.regions.GetLength(0); k++)
            {
                for (int l = 0; l < StructureManager.regions.GetLength(1); l++)
                {
                    structureRegion = StructureManager.regions[k, l];
                    transformCount = structureRegion.structures.Count;
                    for (int i = 0; i < transformCount; i++)
                    {
                        sData = structureRegion.structures[i];

                        if (!Structures.ContainsKey(sData))
                            Structures.Add(sData, sData.structure.health);

                        //Logger.Log("[StructuresHealthManager] Found structure from: (SteamID)" + sData.owner);
                    }
                }
            }
        }
        public static string CheckStructuresHealth()
        {
            var updatedStructures = new List<StructureData>();
            string msg = "Damaged Structures: ";
            foreach (var item in Structures)
            {
                var sData = item.Key;
                if (item.Value > sData.structure.health)
                {
                    msg += "Found an damaged structure at " + sData.point + '\n';
                    //UnturnedToDiscord.UnturnedToDiscordPipe.NotifyBuildDamaged(sData.owner);
                    updatedStructures.Add(sData);
                }
            }
            if (updatedStructures.Any())
            {
                foreach (var structure in updatedStructures)
                    UpdateHealthInList(structure);

                Logger.Log(msg);
            }
            return msg;
        }
        public static void UpdateHealthInList(StructureData structure)
        {
            var msg = "[Updating health] OldHealth: " + Structures[structure];
            Structures[structure] = structure.structure.health;
            Logger.Log(msg + " | NewHealth: " + Structures[structure]);
        }

    }
}
