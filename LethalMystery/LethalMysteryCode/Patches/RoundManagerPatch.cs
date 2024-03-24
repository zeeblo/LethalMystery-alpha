using HarmonyLib;
using LethalNetworkAPI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace PTest.Patches
{
    internal class RoundManagerPatch
    {

        private static List<string> tempColors = new List<string>();
        public static string tempWin;
        public static bool roundLoaded = false;
        public static bool subEventsOnce = false;

        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.LoadNewLevel))]
        [HarmonyPrefix]
        private static bool LoadNewLevelPatch(ref RoundManager __instance)
        {
            PTestPlugin.customServerMessage.SendAllClients("roundedBool"); // since the clients cant detect when this method runs

            //PTestPlugin.player = __instance.localPlayerController;


            if (PTestPlugin.playerClientID == 0)
            {
                PTestPlugin.currentRound = __instance;
                ChatCommands.ChatCommands.ProcessCommand("infammo");

                // Update PlayerList
                ulong[] clientList = StartOfRound.Instance.ClientPlayerList.Keys.ToArray();
                foreach (ulong item in clientList)
                {
                    if (PTestPlugin.playerList.Contains(item)) continue;
                    PTestPlugin.playerList.Add(item);
                }

                string color = "";
                foreach (ulong item in clientList)
                {
                    if (PTestPlugin.defaultPlayerList.Contains(item)) continue;
                    PTestPlugin.defaultPlayerList.Add(item);

                    color = $"!!{item}".Replace("!!0", "Red").Replace("!!1", "Blue").Replace("!!2", "Green").Replace("!!3", "Pink").Replace("!!4", "Orange").Replace("!!5", "Yellow").Replace("!!6", "Black").Replace("!!7", "White").Replace("!!8", "Purple").Replace("!!9", "Cyan"); ;
                    tempColors.Add(color.ToLower());

                }
                PTestPlugin.playerColors.Value = tempColors.ToArray();
            }


            return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(RoundManager), nameof(RoundManager.GenerateNewFloor))]
        static void SubscribeToHandler()
        {
            if (subEventsOnce == false)
            {
                PTestPlugin.CustomNetworkEvents();
                PTestPlugin.Instance.logger.LogInfo(">>> Subscribed to custom events");
                subEventsOnce = true;
            }
        }

        /// <summary>
        /// Remove all Monsters from the game
        /// </summary>
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.LoadNewLevel))]
        [HarmonyPrefix]
        private static bool NoEnemiesPatch(ref SelectableLevel newLevel)
        {
            foreach (SpawnableEnemyWithRarity enemy in newLevel.Enemies)
            {
                enemy.rarity = 0;
            }
            foreach (SpawnableEnemyWithRarity outsideEnemy in newLevel.OutsideEnemies)
            {
                outsideEnemy.rarity = 0;
            }
            foreach (SpawnableEnemyWithRarity daytimeEnemy in newLevel.DaytimeEnemies)
            {
                daytimeEnemy.rarity = 0;
            }
            return true;
        }







        /*
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.LoadNewLevel))]
        [HarmonyPrefix]
        private static bool NoShovelsPatch(ref SelectableLevel newLevel)
        {
            foreach (SpawnableItemWithRarity item in newLevel.spawnableScrap)
            {
                if (item.spawnableItem.itemName.ToLower().Contains("shovel") || item.spawnableItem.itemName.ToLower().Contains("yield"))
                {
                    item.rarity = 0;
                }
            }
            return true;
        }
        */




        private static void RoundEndChecks()
        {

            // Task win
            if (TimeOfDayPatch.winVar && PTestPlugin.gameEnded == false)
            {
                PTestPlugin.customServerMessage.SendAllClients("taskWin");
            }


            // Monsters win
            if ( (PTestPlugin.listOfCrew.ToArray().Length <= 0 && PTestPlugin.gameEnded == false && PTestPlugin.gameStarted.Value == true && PTestPlugin.roundBegin == true) || tempWin == "monsterWin" || PTestPlugin.monsterWin.Value == true)
            {
                if (PTestPlugin.inMeeting.Value != true)
                {
                    tempWin = "monsterWin";
                    PTestPlugin.Instance.logger.LogInfo(">>> Above SendAllClients (monster)");
                    PTestPlugin.customServerMessage.SendAllClients("monsterWin");
                }
            }

            // Crew win
            else if ((PTestPlugin.listOfMonsters.ToArray().Length <= 0 && PTestPlugin.gameEnded == false && PTestPlugin.gameStarted.Value == true && PTestPlugin.roundBegin == true) || tempWin == "crewWin" || PTestPlugin.crewWin.Value == true)
            {
                if (PTestPlugin.inMeeting.Value != true)
                {
                    tempWin = "crewWin";
                    PTestPlugin.Instance.logger.LogInfo(">>> Above SendAllClients (crew)");
                    PTestPlugin.customServerMessage.SendAllClients("crewWin");
                }
            }

        }


        [HarmonyPatch(typeof(RoundManager), "Update")]
        [HarmonyPrefix]
        private static bool UpdatePatch()
        {

            // Win/Lose
            if (PTestPlugin.playerClientID == 0 && PTestPlugin.inMeeting.Value != true)
            {
                //PTestPlugin.Instance.logger.LogInfo(">>> Above roundCheck Method");
                RoundEndChecks();
            }
            return true;
        }


    }
}
