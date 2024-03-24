using HarmonyLib;

namespace PTest.Patches
{

    [HarmonyPatch(typeof(Shovel))]
    internal class ShovelPatch
    {
        public static bool monsterHitPlayer = false;

        [HarmonyPatch(typeof(Shovel), nameof(Shovel.HitShovel))]
        [HarmonyPrefix]
        private static bool InstantKill(ref int ___shovelHitForce)
        {
            if (PTestPlugin.monster && monsterHitPlayer == false)
            {
                ___shovelHitForce = 9999;
                monsterHitPlayer = true;
                return true;
            }
            else if (PTestPlugin.monster)
            {
                ___shovelHitForce = 1;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
