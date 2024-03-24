using HarmonyLib;
using LethalNetworkAPI;
using UnityEngine.SceneManagement;
using UnityEngine;


namespace PTest.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    internal class EnemyAIPatch
    {
        public static bool localErase = false;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void UpdatePostfix(ref EnemyAI __instance)
        {
            if (PlayerControllerBPatch.turnedImp)
            {
                __instance.destination = PTestPlugin.playerClientID.GetPlayerController().transform.position;
                __instance.moveTowardsDestination = true;
                __instance.agent.SetDestination(PTestPlugin.playerClientID.GetPlayerController().transform.position);
                //__instance.SetMovingTowardsTargetPlayer(PTestPlugin.playerClientID.GetPlayerController());

            }

            if ((PTestPlugin.killMonster.Value != null) && PTestPlugin.killMonster.Value.Contains("true/") && PTestPlugin.playerClientID == 0)
            {
                Scene targetScene = SceneManager.GetSceneByName("SampleSceneRelay");
                if (targetScene != null)
                {
                    foreach (GameObject obj in targetScene.GetRootGameObjects())
                    {
                        if (obj.name.ToLower().Contains(PTestPlugin.killMonster.Value.Replace("true/", "")))
                        {
                            UnityEngine.Object.Destroy(obj);
                        }
                    }
                }

                PTestPlugin.killMonster.Value = "false";
            }
        }

        // PlayerIsTargetable | GetClosestPlayer | SetDestinationToPosition

        // AvoidClosestPlayer |  LookAtFlowermanTrigger

        /*
        [HarmonyPatch("SetDestinationToPosition")]
        [HarmonyPrefix]
        private static bool SetDestinationToPositionPrefix()
        {
            if (PTestPlugin.playerClientID.GetPlayerController().name == "Player")
            {
                return true;
            }
            else
            {
                return true;
            }
        }
        */

        /*
        [HarmonyPatch("LookAtFlowermanTrigger")]
        [HarmonyPrefix]
        private static bool LookAtFlowermanTriggerPrefix()
        {
            return false;
        }


        [HarmonyPatch("AvoidClosestPlayer")]
        [HarmonyPrefix]
        private static bool AvoidClosestPlayerPrefix()
        {
            return false;
        }
        */


        [HarmonyPatch("PlayerIsTargetable")]
        [HarmonyPrefix]
        private static bool PlayerIsTargetablePrefix()
        {
            if (PlayerControllerBPatch.turnedImp)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        [HarmonyPatch("GetClosestPlayer")]
        [HarmonyPrefix]
        private static bool GetClosestPlayerPrefix()
        {
            if (PlayerControllerBPatch.turnedImp)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        /*
        [HarmonyPatch("SetMovingTowardsTargetPlayer")]
        [HarmonyPrefix]
        private static bool SetMovingTowardsTargetPlayerPrefix(ref EnemyAI __instance)
        {
            PTestPlugin.Instance.logger.LogInfo($">>>TargetName:  {__instance.targetPlayer.name}");
            if (PTestPlugin.playerClientID.GetPlayerController().name == __instance.targetPlayer.name)
            {
                PTestPlugin.Instance.logger.LogInfo($">>>TargetName if-reached ");
                return false;
            }
            else
            {
                return true;
            }

        }
        */



    }
}