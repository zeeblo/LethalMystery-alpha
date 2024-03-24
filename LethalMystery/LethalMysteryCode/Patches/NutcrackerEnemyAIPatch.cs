using HarmonyLib;


namespace PTest.Patches
{
    [HarmonyPatch(typeof(NutcrackerEnemyAI))]
    internal class NutcrackerEnemyAIPatch
    {
        [HarmonyPatch(typeof(NutcrackerEnemyAI), nameof(NutcrackerEnemyAI.Update))]
        [HarmonyPrefix]
        private static bool UpdatePatch(ref bool ___isEnemyDead)
        {
            // TODO:  Once game starts, only run this once
            ___isEnemyDead = true;
            return true;
        }
    }
}
