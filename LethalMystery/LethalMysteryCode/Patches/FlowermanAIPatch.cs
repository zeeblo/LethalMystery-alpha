using GameNetcodeStuff;
using HarmonyLib;
using LethalNetworkAPI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PTest.Patches
{

    [HarmonyPatch(typeof(FlowermanAI))]
    internal class FlowermanAIPatch
    {

        // Test teleporting the Bracken gameobject to the globalPlayerImp if a player triggers the func that makes it run away


        [HarmonyPatch(typeof(FlowermanAI), "Update")]
        [HarmonyPostfix]
        private static void UpdatePatch(ref FlowermanAI __instance)
        {
            __instance.evadeStealthTimer = 0f;
            __instance.timesThreatened = 50;
            __instance.angerMeter = 9999f;
            __instance.isInAngerMode = true;
        }

    }
}
