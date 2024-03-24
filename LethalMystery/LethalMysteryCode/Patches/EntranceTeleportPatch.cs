using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace PTest.Patches
{
    internal class EntranceTeleportPatch
    {
        public static GameObject SkeldMap;
        public static bool SpawnVents = false;
        public static bool SpawnLights = false;

        [HarmonyPatch(typeof(EntranceTeleport), nameof(EntranceTeleport.TeleportPlayer))]
        [HarmonyPrefix]
        private static bool SkeldDungeon(ref EntranceTeleport __instance)
        {
            if ( PTestPlugin.playerClientID == 0)
            {
                PTestPlugin.customServerMessage.SendAllClients("canEnter");
            }

            /*
             * Mostly just for skeld, the owner needs to be the first one in to spawn the vents & button
             * Why? A series of bad implementations.
            if (PTestPlugin.canEnter.Value != true)
            {
                return false;
            }
            */


            try
            {
                Scene targetScene = SceneManager.GetSceneByName("InitSceneLaunchOptions");

                if (targetScene != null)
                {
                    foreach (GameObject obj in targetScene.GetRootGameObjects())
                    {
                        if (obj.name == "Skeld(Clone)")
                        {
                            SkeldMap = obj;
                            break;
                        }
                    }


                    if (SkeldMap)
                    {
                        GameNetworkManager.Instance.localPlayerController.TeleportPlayer(new Vector3(-8f, -148.6f, 172f));

                        if (SpawnVents == false && PTestPlugin.playerClientID == 0)
                        {
                            SpawnVents = true;
                            //PTestPlugin.SpawnScrapFunc("spwscr metalsheet", "vnt");
                            //PTestPlugin.SpawnScrapFunc("spwscr enginepart", "btn");
                        }
                        if (SpawnLights == false && PTestPlugin.playerClientID == 0)
                        {
                            SpawnLights = true;
                            PTestPlugin.lightsOn.Value = true;
                        }
                        return false;
                    }
                }
            }
            catch (ArgumentException)
            {
                return true;
            }

            if (SkeldMap )
            {
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
