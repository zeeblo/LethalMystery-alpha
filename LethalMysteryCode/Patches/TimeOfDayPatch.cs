using HarmonyLib;



namespace PTest.Patches
{
    [HarmonyPatch(typeof(TimeOfDay))]
    internal class TimeOfDayPatch
    {
        public static bool winVar;
        public static int taskNum = 130;


        [HarmonyPatch(typeof(TimeOfDay), "Awake")]
        [HarmonyPostfix]
        static void WinPatch()
        {
            winVar = false;
        }



        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void UpdatePatch(ref float ___globalTimeSpeedMultiplier)
        {
            ___globalTimeSpeedMultiplier = 0f;
        }



        [HarmonyPatch(typeof(TimeOfDay), "Update")]
        [HarmonyPostfix]
        static void DisplayTaskPatch(ref int ___profitQuota, ref int ___quotaFulfilled)
        {
            if (!PTestPlugin.gameStarted.Value) return;

            if (PTestPlugin.inMeeting.Value)
            {
                ___quotaFulfilled = PTestPlugin.userfinishedTasks;
                StartOfRound.Instance.profitQuotaMonitorText.text = $"PROFIT QUOTA:\n${___quotaFulfilled} / ${___profitQuota}";
            }
            else
            {
                ___profitQuota = taskNum;
            }

            // Task win
            if (PTestPlugin.userfinishedTasks >= taskNum && PTestPlugin.gameEnded == false)
            {
                winVar = true;
            }
        }

    }
}
