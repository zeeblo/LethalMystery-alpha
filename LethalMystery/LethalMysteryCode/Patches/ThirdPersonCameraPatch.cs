using _3rdPerson.Helper;
using HarmonyLib;


namespace PTest.Patches
{
    [HarmonyPatch(typeof(ThirdPersonCamera))]
    internal class ThirdPersonCameraPatch
    {

        [HarmonyPatch(typeof(ThirdPersonCamera), "Toggle")]
        [HarmonyPrefix]
        private static bool TogglePrefix()
        {
            if (PTestPlugin.monster == true)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
