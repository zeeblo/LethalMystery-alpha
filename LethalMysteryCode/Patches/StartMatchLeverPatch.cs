using HarmonyLib;


namespace PTest.Patches
{
    internal class StartMatchLeverPatch
    {
        public static bool continueStatement;



        [HarmonyPatch(typeof(StartMatchLever), "Start")]
        [HarmonyPrefix]
        private static void StartPatch()
        {
            continueStatement = true;
        }



        [HarmonyPatch(typeof(StartMatchLever), nameof(StartMatchLever.EndGame))]
        [HarmonyPrefix]
        private static bool EndGamePatch()
        {
            if (PTestPlugin.playerClientID == 0)
            {
                PTestPlugin.customServerMessage.SendAllClients("endgame");
            }
            return true;
        }




        [HarmonyPatch(typeof(StartMatchLever), "Update")]
        [HarmonyPostfix] // used to be prefix
        private static void UpdatePatch(StartMatchLever __instance)
        {

            // Disable Lever
            if (PTestPlugin.gameStarted.Value)
            {
                __instance.triggerScript.interactable = false;
                __instance.triggerScript.hoverTip = "[Find the Monster!]";
            }

            // Game ended
            if (PTestPlugin.gameEnded == true && continueStatement)
            {
                PTestPlugin.Instance.logger.LogInfo(">>> Reached END_GAME");
                continueStatement = false;
                __instance.EndGame();
                //PTestPlugin.DefaultValues();
            }

        }
    }
}
