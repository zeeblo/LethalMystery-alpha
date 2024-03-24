using GameNetcodeStuff;
using HarmonyLib;
using LethalNetworkAPI;


namespace PTest.Patches
{

    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        //public static GameObject SkeldMap;
        //public static bool CDErrorCheck = false;

        [HarmonyPatch(typeof(StartOfRound), "Update")]
        [HarmonyPostfix]
        private static void UpdatePatch(ref StartOfRound __instance)
        {

            if (RoundManagerPatch.roundLoaded)
            {

                PTestPlugin.playerClientID = (ulong)__instance.thisClientPlayerId;
                PTestPlugin.playerName = PTestPlugin.playerClientID.GetPlayerController().name.ToLower();

                // PTestPlugin.Instance.logger.LogInfo(">>> ROUNDED IS NOW TRUE");
            }

            if (__instance.allPlayerScripts[0].playerClientId == 0)
            {
                for (int i = 0; i < __instance.allPlayerScripts.Length; i++)
                {
                    //PTestPlugin.Instance.logger.LogInfo($">>> ID: {__instance.allPlayerScripts[i].playerClientId} || Name: {__instance.allPlayerScripts[i].name} || Status: {__instance.allPlayerScripts[i].isPlayerDead}");
                    //PTestPlugin.Instance.logger.LogInfo($">>> PTestPlugin.listOfCrew.Contains($\"{{__instance.allPlayerScripts[i].playerClientId}}\")");
                    string colorName = $"{__instance.allPlayerScripts[i].playerClientId}".Replace("0", "red").Replace("1", "blue").Replace("2", "green").Replace("3", "pink").Replace("4", "orange").Replace("5", "yellow").Replace("6", "black").Replace("7", "white").Replace("8", "purple").Replace("9", "cyan");
                    if (__instance.allPlayerScripts[i].isPlayerDead && (PTestPlugin.listOfCrew.Contains($"{colorName}") || PTestPlugin.listOfMonsters.Contains($"{colorName}")))
                    {
                        //string colorName = $"{__instance.allPlayerScripts[i].playerClientId}".Replace("0", "red").Replace("1", "blue").Replace("2", "green").Replace("3", "pink").Replace("4", "orange").Replace("5", "yellow").Replace("6", "black").Replace("7", "white").Replace("8", "purple").Replace("9", "cyan");
                        foreach (string c in PTestPlugin.listOfCrew)
                        {
                            if (c == colorName)
                            {
                                PTestPlugin.customClientMessage.SendServer($"killed/crew/{colorName}");
                                break;
                            }
                        }

                        foreach (string c in PTestPlugin.listOfMonsters)
                        {
                            if (c == colorName)
                            {
                                PTestPlugin.customClientMessage.SendServer($"killed/monster/{colorName}");
                                break;
                            }
                        }
                    }

                }
            }
        }

        /*
        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.UpdatePlayerVoiceEffects))]
        [HarmonyPrefix]
        private static bool MutePlayers()
        {
            if (PTestPlugin.gameStarted.Value && PTestPlugin.muted.Value)
            {
                if (PTestPlugin.inMeeting.Value == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }
        */


        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        private static void StartAudioPatch(ref StartOfRound __instance)
        {
            // PTestPlugin.SoundFX[0]; Body reported
            //__instance.shipIntroSpeechSFX = PTestPlugin.SoundFX[0];
        }

        //GameObject.Find("HangarShip").gameObject.transform.Find("ShipRails")?.gameObject;

        /*
        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.StartGame))]
        [HarmonyPostfix]
        private static void StartPatch()
        {
            PTestPlugin.SpawnScrapFunc("spwscr metalsheet", "vnt");
        }
        */


    }


}
