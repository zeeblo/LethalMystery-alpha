using HarmonyLib;
using UnityEngine;

namespace PTest.Patches
{
    internal class GrabbableObjectPatch
    {

        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.Start))]
        [HarmonyPostfix]
        static void NoWeightPatch(ref Item ___itemProperties)
        {
            if (___itemProperties.name.Contains("YieldSign"))
            {
                ___itemProperties.weight = 0f;
            }
        }




        [HarmonyPatch(typeof(GrabbableObject), "LateUpdate")]
        [HarmonyPostfix]
        static void HideWeaponOnMonitorPatch(ref Transform ___radarIcon)
        {
            if ((___radarIcon != null))
            {
                ___radarIcon.gameObject.SetActive(false);
                //UnityEngine.Object.Destroy(___radarIcon.gameObject);
            }
        }


        /*
        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.GrabItem))]
        [HarmonyPrefix]
        static bool NoPickupPatch()
        {
            return false;
        }
        */
    }
}
