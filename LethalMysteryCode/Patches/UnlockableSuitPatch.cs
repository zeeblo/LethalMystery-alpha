using HarmonyLib;



namespace PTest.Patches
{
    [HarmonyPatch(typeof(UnlockableSuit))]
    internal class UnlockableSuitPatch
    {
        public static bool localUnlockUpdate = false;
        /*
         * Yellow : 37
         * White : 36
         * Red : 35
         * Purple : 34
         * Pink : 33
         * Orange : 32
         * Magenta : 31
         * Lime : 30
         * Light Blue : 29
         * Green : 28
         * Brown : 27
         * Blue : 26
         * Black : 25
         */
        [HarmonyPatch(typeof(UnlockableSuit), "SwitchSuitToThis")]
        [HarmonyPrefix]
        private static bool SwitchSuitToThisPatch()
        {
            return false;
        }


        /*
        [HarmonyPatch(typeof(UnlockableSuit), "Update")]
        [HarmonyPostfix]
        private static void UpdatePatch()
        {
            if (localUnlockUpdate == false)
            {
                if (PTestPlugin.playerClientID == 0)
                {
                    PTestPlugin.customClientMessage.SendServer($"switchSuit/{0}/35");
                    //UnlockableSuit.SwitchSuitForPlayer(GameNetworkManager.Instance.localPlayerController, 35); // Red
                }
                else if (PTestPlugin.playerClientID == 1)
                {
                    PTestPlugin.customClientMessage.SendServer($"switchSuit/{1}/26");
                    //UnlockableSuit.SwitchSuitForPlayer(GameNetworkManager.Instance.localPlayerController, 26); // Blue
                }
                else if (PTestPlugin.playerClientID == 2)
                {
                    PTestPlugin.customClientMessage.SendServer($"switchSuit/{2}/28");
                    //UnlockableSuit.SwitchSuitForPlayer(GameNetworkManager.Instance.localPlayerController, 28); // Green
                }
                else if (PTestPlugin.playerClientID == 3)
                {
                    UnlockableSuit.SwitchSuitForPlayer(GameNetworkManager.Instance.localPlayerController, 33); // Pink
                }
                else if (PTestPlugin.playerClientID == 4)
                {
                    UnlockableSuit.SwitchSuitForPlayer(GameNetworkManager.Instance.localPlayerController, 32); // Orange
                }
                else if (PTestPlugin.playerClientID == 5)
                {
                    UnlockableSuit.SwitchSuitForPlayer(GameNetworkManager.Instance.localPlayerController, 37); // Yellow
                }
                else if (PTestPlugin.playerClientID == 6)
                {
                    UnlockableSuit.SwitchSuitForPlayer(GameNetworkManager.Instance.localPlayerController, 25); // Black
                }
                else if (PTestPlugin.playerClientID == 7)
                {
                    UnlockableSuit.SwitchSuitForPlayer(GameNetworkManager.Instance.localPlayerController, 36); // White
                }
                else if (PTestPlugin.playerClientID == 8)
                {
                    UnlockableSuit.SwitchSuitForPlayer(GameNetworkManager.Instance.localPlayerController, 34); // Purple
                }
                else if (PTestPlugin.playerClientID == 9) 
                {
                    UnlockableSuit.SwitchSuitForPlayer(GameNetworkManager.Instance.localPlayerController, 29); // Cyan
                }
                localUnlockUpdate = true;
            }
        }
        */
    }
}
