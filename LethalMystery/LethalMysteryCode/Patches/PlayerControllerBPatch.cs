using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using LethalNetworkAPI;



namespace PTest.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {


        /* TODO:
         * Red lights need to be on
        */

        public static bool notsafe = false;
        public static bool vents = false;
        public static bool invisArea = false;
        public static bool turnedImp = false;
        public static bool eraseMonster = false;
        public static int tempMaxValue = 0;
        private static float introCountDown = 15f;
        private static float scrapTimer = 45;
        public static Vector3 plrPos = Vector3.zero;
        public static Vector3 oldPlayerPosition = Vector3.zero;
        public static Vector3 newPlayerPosition = Vector3.zero;
        public static Vector3 oldCamPosition = Vector3.zero;
        public static Vector3 newCamPosition = Vector3.zero;
        public static string theImp;
        private static string wButt = "none";
        private static string aButt = "none";
        private static string sButt = "none";
        private static string dButt = "none";
        private static float wButtPos = 0f;
        private static float aButtPos = 0f;
        private static float sButtPos = 0f;
        private static float dButtPos = 0f;
        public static GameObject plr;
        public static bool invisMon = false;
        public static bool localhudImpWin = false;
        public static bool localhudCrewWin = false;
        public static bool ranSection1votetime = false;
        public static float killCooldown = 15f; // (Initial cooldown when round first starts)
        public static bool canSpawnWeapon = true;
        public static bool resetSucking = false;
        /*
        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnLocalDisconnect))]
        [HarmonyPrefix]
        private static void DisconnectPatch()
        {
            // - If imposter leaves, then crew autowins
            //PTestPlugin.gameStarted.Value = false;
            //PTestPlugin.DefaultValues();
        }
        */



        /*
        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnClientConnect))]
        [HarmonyPrefix]
        private static void ConnectPatch()
        {
            ulong[] clientList = StartOfRound.Instance.ClientPlayerList.Keys.ToArray();
            foreach (ulong item in clientList)
            {
                if (PTestPlugin.playerList.Contains(item)) continue;
                PTestPlugin.playerList.Add(item);
            }

            foreach (ulong item in clientList)
            {
                //UnlockableSuit.SwitchSuitForPlayer(item.GetPlayerController(), PTestPlugin.PlayerIDToSuitID[item]);
                if (PTestPlugin.defaultPlayerList.Contains(item)) continue;
                PTestPlugin.defaultPlayerList.Add(item);
            }

        }
        */


        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DiscardHeldObject))]
        [HarmonyPrefix]
        private static bool DontDropPatch(ref GrabbableObject ___currentlyHeldObjectServer)
        {
            if (___currentlyHeldObjectServer != null)
            {
                if (___currentlyHeldObjectServer.name.ToLower().Contains("gun") || ___currentlyHeldObjectServer.name.ToLower().Contains("yieldsign"))
                {
                    if (PTestPlugin.monster)
                    {
                        return false;
                    }
                    return true;
                }
            }

            //PTestPlugin.GrabItem.Value = ___currentlyHeldObjectServer.transform.gameObject;
            return true;

        }
        


        [HarmonyPatch(typeof(PlayerControllerB), "AllowPlayerDeath")]
        [HarmonyPrefix]
        private static bool NoDamagePatch()
        {
            if (notsafe == true)
            {
                return true;
            }

            if (PTestPlugin.inMeeting.Value || PTestPlugin.gracePeriod.Value)
            {
                return false;
            }
            return true;
        }



        public static void DespawnBodies()
        {

            Scene targetScene = SceneManager.GetSceneByName("SampleSceneRelay");

            if (targetScene != null)
            {
                /*
                foreach (GameObject obj in targetScene.GetRootGameObjects())
                {
                    if (obj.name.Contains("PlayerRagdoll"))
                    {
                        UnityEngine.Object.Destroy(obj);
                    }
                }
                */


                try
                {
                    // Make user invis if INSIDE the ship
                    foreach (GameObject obj in targetScene.GetRootGameObjects())
                    {
                        if (obj.name == "Environment")
                        {

                            GameObject body = obj.transform.Find($"HangarShip/PlayerRagdoll(Clone)").gameObject;
                            UnityEngine.Object.Destroy(body);
                        }
                    }
                }
                catch
                {
                    foreach (GameObject obj in targetScene.GetRootGameObjects())
                    {
                        if (obj.name == "PlayerRagdoll(Clone)")
                        {
                            UnityEngine.Object.Destroy(obj);
                        }
                    }


                }
            }
        }


        [HarmonyPatch(typeof(PlayerControllerB), "BeginGrabObject")]
        [HarmonyPrefix]
        private static bool NoPickupPatch(ref GrabbableObject ___currentlyGrabbingObject, ref RaycastHit ___hit, ref Camera ___gameplayCamera, ref Ray ___interactRay, ref int ___interactableObjectsMask, ref bool ___twoHanded, ref float ___sinkingValue)
        {
            ___interactRay = new Ray(___gameplayCamera.transform.position, ___gameplayCamera.transform.forward);
            if (!Physics.Raycast(___interactRay, out ___hit, 5f, ___interactableObjectsMask) || ___hit.collider.gameObject.layer == 8 || !(___hit.collider.tag == "PhysicsProp") || ___twoHanded || ___sinkingValue > 0.73f)
            {
                PTestPlugin.Instance.logger.LogInfo($">>>Bleee in raycast if-state");
                return true;
            }
            ___currentlyGrabbingObject = ___hit.collider.transform.gameObject.GetComponent<GrabbableObject>();
            PTestPlugin.Instance.logger.LogInfo($">>>(Above)Attempted to pick up: {___currentlyGrabbingObject.name}");

            if (___currentlyGrabbingObject.name.ToLower().Contains("metal") && EntranceTeleportPatch.SkeldMap)
            {
                // Teleport player to other vent
                foreach (Vector3 pos in PTestPlugin.ventPositions.Keys)
                {
                    if (___currentlyGrabbingObject.transform.position == pos && PTestPlugin.monster) // && PTestPlugin.monster
                    {
                        GameNetworkManager.Instance.localPlayerController.TeleportPlayer(PTestPlugin.ventPositions[pos]);
                        //HUDManager.Instance.UIAudio.PlayOneShot(PTestPlugin.SoundFX[2], 1f);
                        break;
                    }
                }
                PTestPlugin.Instance.logger.LogInfo($">>>(Below)Attempted to pick up: {___currentlyGrabbingObject.name}");
                return false;
            }
            else if (___currentlyGrabbingObject.name.ToLower().Contains("engine") && EntranceTeleportPatch.SkeldMap)
            {
                // Teleport back outside
                GameNetworkManager.Instance.localPlayerController.TeleportPlayer(new Vector3(2.11f, 2f, 0.99f));
                return false;
            }
            else if (___currentlyGrabbingObject.name.ToLower().Contains("ragdoll") && PTestPlugin.gameEnded == false)
            {
                // Body Reported (now start meeting)
                ShipAlarmCordPatch.reportedBody = true;
                GameNetworkManager.Instance.localPlayerController.TeleportPlayer(PTestPlugin.playerSpawnPositions[GameNetworkManager.Instance.localPlayerController.playerClientId].position);
                PTestPlugin.customClientMessage.SendServer("bodyReport");

                //DespawnProps();
                return false;
            }
            else
            {
                return true;
            }

        }

        /*
        [HarmonyPatch(typeof(PlayerControllerB), "BeginGrabObject")]
        [HarmonyPrefix]
        private static bool NoPickupPatch(ref RaycastHit ___hit)
        {
            GrabbableObject hitobj = ___hit.collider.transform.gameObject.GetComponent<GrabbableObject>();
            if (hitobj != null && hitobj.name.ToLower().Contains("metal"))
            {
                PTestPlugin.Instance.logger.LogInfo($">>>Attempted to pick up: {hitobj.name}");
                return false;
            }
            else
            {
                return true;
            }
        }
        */



        [HarmonyPatch(typeof(PlayerControllerB), "SetNightVisionEnabled")]
        [HarmonyPrefix]
        private static bool NightVisionPatch(ref PlayerControllerB __instance)
        {
            // From "ToggleableNightVision" mod by kentrosity
            if (PTestPlugin.monster)
            {
                __instance.nightVision.intensity = 7500f;
                __instance.nightVision.range = 100000f;
                __instance.nightVision.shadowStrength = 0f;
                __instance.nightVision.shadows = (LightShadows)0;
                __instance.nightVision.shape = (LightShape)2;
            }
            else
            {
                float num = 1f;
                __instance.nightVision.intensity = 366.9317f * num;
                __instance.nightVision.range = 12f;
                __instance.nightVision.shadowStrength = 1f;
                __instance.nightVision.shadows = (LightShadows)0;
                __instance.nightVision.shape = (LightShape)0;

            }
            return true;
        }






        /*
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayer))]
        [HarmonyPrefix]
        private static bool KillPlayerPatch(PlayerControllerB __instance)
        {
            if (PTestPlugin.gameEnded == false)
            {
                string colorName = $"{__instance.playerClientId}".Replace("0", "red").Replace("1", "blue").Replace("2", "green").Replace("3", "pink").Replace("4", "orange").Replace("5", "yellow").Replace("6", "black").Replace("7", "white").Replace("8", "purple").Replace("9", "cyan");
                foreach (string c in PTestPlugin.listOfCrew)
                {
                    if (c == colorName)
                    {
                        PTestPlugin.customClientMessage.SendServer($"killed/crew/{colorName}");
                        break;
                    }
                }

                foreach (string c in PTestPlugin.listOfMonsters)
                {
                    if (c == colorName)
                    {
                        PTestPlugin.customClientMessage.SendServer($"killed/monster/{colorName}");
                        break;
                    }
                }


                PTestPlugin.customClientMessage.SendServer("cleanCrew");

            }


            //string colorName = $"{__instance.playerClientId}".Replace("0", "red").Replace("1", "blue").Replace("2", "green").Replace("3", "pink").Replace("4", "orange").Replace("5", "yellow").Replace("6", "black").Replace("7", "white").Replace("8", "purple").Replace("9", "cyan");
            //PTestPlugin.listOfCrew.Remove(colorName);
            //PTestPlugin.Instance.logger.LogInfo($">>> colorName Killed: {colorName}");

            return true;

        }
        */


        public static void playerInScene(string id, bool enable)
        {
            Scene targetScene = SceneManager.GetSceneByName("SampleSceneRelay");
            string PlayerString = id == "0" ? "Player" : $"Player ({id})";
            string usernameCanvas = id == "0" ? "PlayerNameCanvas" : "PlayerUsernameCanvas";


            PTestPlugin.Instance.logger.LogInfo($">>>ID: {id} || PlayerString: {PlayerString}");
            try
            {
                // Make user invis if INSIDE the ship
                foreach (GameObject obj in targetScene.GetRootGameObjects())
                {
                    if (obj.name == "Environment")
                    {
                        //PTestPlugin.Instance.logger.LogInfo($">>>env.HangarShip/{PlayerString}/ScavengerModel/LOD1 || {obj.transform.Find($"HangarShip/{PlayerString}/ScavengerModel/LOD1").gameObject}");
                        obj.transform.Find($"HangarShip/{PlayerString}/ScavengerModel/LOD1").gameObject.SetActive(enable); // Lod1 to Lod3 is player model

                        //PTestPlugin.Instance.logger.LogInfo($">>>env.HangarShip/{PlayerString}/ScavengerModel/LOD2 || {obj.transform.Find($"HangarShip/{PlayerString}/ScavengerModel/LOD2").gameObject}");
                        obj.transform.Find($"HangarShip/{PlayerString}/ScavengerModel/LOD2").gameObject.SetActive(enable);

                        //PTestPlugin.Instance.logger.LogInfo($">>>env.HangarShip/{PlayerString}/ScavengerModel/LOD3 || {obj.transform.Find($"HangarShip/{PlayerString}/ScavengerModel/LOD3").gameObject}");
                        obj.transform.Find($"HangarShip/{PlayerString}/ScavengerModel/LOD3").gameObject.SetActive(enable);

                        //PTestPlugin.Instance.logger.LogInfo($">>>env.HangarShip/{PlayerString}/PlayerNameCanvas/Text (TMP) || {obj.transform.Find($"HangarShip/{PlayerString}/PlayerNameCanvas/Text (TMP)").gameObject}");
                        obj.transform.Find($"HangarShip/{PlayerString}/{usernameCanvas}/Text (TMP)")?.gameObject.SetActive(enable);

                        //PTestPlugin.Instance.logger.LogInfo($">>>env.HangarShip/{PlayerString}/ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/BetaBadge || {obj.transform.Find($"HangarShip/{PlayerString}/ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/BetaBadge").gameObject}");
                        obj.transform.Find($"HangarShip/{PlayerString}/ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/BetaBadge").gameObject.SetActive(enable);

                        //PTestPlugin.Instance.logger.LogInfo($">>>env.HangarShip/{PlayerString}/ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/LevelSticker || {obj.transform.Find($"HangarShip/{PlayerString}/ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/LevelSticker").gameObject}");
                        obj.transform.Find($"HangarShip/{PlayerString}/ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/LevelSticker").gameObject.SetActive(enable);

                    }
                }
            }
            catch
            {
                // If the user transforms into a monster OUTSIDE the ship then make them invisible
                foreach (GameObject obj in targetScene.GetRootGameObjects())
                {
                    if (obj.name == "PlayersContainer")
                    {
                        //PTestPlugin.Instance.logger.LogInfo($">>>plr.{PlayerString}/ScavengerModel/LOD1 || {obj.transform.Find($"{PlayerString}/ScavengerModel/LOD1").gameObject}");
                        obj.transform.Find($"{PlayerString}/ScavengerModel/LOD1").gameObject.SetActive(enable);

                        //PTestPlugin.Instance.logger.LogInfo($">>>plr.{PlayerString}/ScavengerModel/LOD2 || {obj.transform.Find($"{PlayerString}/ScavengerModel/LOD2").gameObject}");
                        obj.transform.Find($"{PlayerString}/ScavengerModel/LOD2").gameObject.SetActive(enable);

                        //PTestPlugin.Instance.logger.LogInfo($">>>plr.{PlayerString}/ScavengerModel/LOD3 || {obj.transform.Find($"{PlayerString}/ScavengerModel/LOD3").gameObject}");
                        obj.transform.Find($"{PlayerString}/ScavengerModel/LOD3").gameObject.SetActive(enable);

                        //PTestPlugin.Instance.logger.LogInfo($">>>plr.{PlayerString}/ScavengerModel/Text (TMP) || {obj.transform.Find($"{PlayerString}/ScavengerModel/Text (TMP)").gameObject}");
                        obj.transform.Find($"{PlayerString}/{usernameCanvas}/Text (TMP)")?.gameObject.SetActive(enable);

                        //PTestPlugin.Instance.logger.LogInfo($">>>plr.{PlayerString}/ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/BetaBadge || {obj.transform.Find($"{PlayerString}/ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/BetaBadge").gameObject}");
                        obj.transform.Find($"{PlayerString}/ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/BetaBadge").gameObject.SetActive(enable);


                        //PTestPlugin.Instance.logger.LogInfo($">>>plr.{PlayerString}/ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/LevelSticker || {obj.transform.Find($"{PlayerString}/ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/LevelSticker").gameObject}");
                        obj.transform.Find($"{PlayerString}/ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/LevelSticker").gameObject.SetActive(enable);

                    }
                }
            }
        }



        public static void impControls()
        {

            bool inside = GameNetworkManager.Instance.localPlayerController.isInsideFactory;
            if (PTestPlugin.inMeeting.Value || inside == false) return;


            // If Already Imp | Disable
            if (Keyboard.current.digit8Key.wasPressedThisFrame && PTestPlugin.monster == true && turnedImp == true && EntranceTeleportPatch.SkeldMap == null && PTestPlugin.Gamemode.Value != "normal")
            {
                PTestPlugin.DisableMonsters();
            }

            // Just turned into imp | Enable
            else if (Keyboard.current.digit8Key.wasPressedThisFrame && PTestPlugin.monster == true && turnedImp == false && EntranceTeleportPatch.SkeldMap == null && PTestPlugin.Gamemode.Value != "normal")
            {
                PTestPlugin.EnableMonsters();
            }


        }

        

        private static void ownerCommands()
        {

            if (StartOfRound.Instance.allPlayerScripts[0].playerClientId != 0) return;


            // numpad1 | Make ship leave (Happens automatically on the first round but doesnt every other round)
            if (Keyboard.current.numpad1Key.wasPressedThisFrame)
            {
                PTestPlugin.gameEnded = true;
                StartMatchLeverPatch.continueStatement = true;
            }


            // numpad2 | Kick Player off terminal
            if (Keyboard.current.numpad2Key.wasPressedThisFrame)
            {
                PTestPlugin.customServerMessage.SendAllClients("tpall");
            }

            // numpad3 | Reset Ship and attempt to fix sucking player off
            if (Keyboard.current.numpad3Key.wasPressedThisFrame)
            {
                PTestPlugin.customServerMessage.SendAllClients("resetSucking");
            }
        }


        private static void impCommands()
        {

            if (PTestPlugin.monster != true) return;


            // 8 | Spawn sign
            if (Keyboard.current.digit7Key.wasPressedThisFrame && PTestPlugin.Gamemode.Value == "normal" && canSpawnWeapon == true)
            {
                //PTestPlugin.customClientMessage.SendAllClients("spweapon");
                //canSpawnWeapon = false;
            }
        }




        // Because the spawn positions dont exist serverside, players too high in the sky will die to fall damage when they get teleported back.
        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPostfix]
        private static void NoFallDamagePatch(ref bool ___takingFallDamage)
        {
            ___takingFallDamage = false;
        }


        private static void killCooldownFunc()
        {
            killCooldown -= Time.deltaTime;
            if (killCooldown <= 0f)
            {
                HUDManager.Instance.DisplayTip("Kill Cooldown Reset!", "You can now attack someone", false);
                ShovelPatch.monsterHitPlayer = false;
            }
        }



        public static void turnImpNameRed(string id, bool reset = false)
        {
            Scene targetScene = SceneManager.GetSceneByName("SampleSceneRelay");
            string PlayerString = id == "0" ? "Player" : $"Player ({id})";
            string usernameCanvas = id == "0" ? "PlayerNameCanvas" : "PlayerUsernameCanvas";

            try
            {
                // Make user invis if INSIDE the ship
                foreach (GameObject obj in targetScene.GetRootGameObjects())
                {
                    if (obj.name == "Environment")
                    {
                        GameObject username = obj.transform.Find($"HangarShip/{PlayerString}/{usernameCanvas}/Text (TMP)")?.gameObject;
                        TextMeshProUGUI colorChange = username?.GetComponent<TextMeshProUGUI>();

                        if (username != null)
                        {
                            colorChange.color = Color.red;
                        }

                        if (reset)
                        {
                            colorChange.color = Color.white;
                        }

                    }
                }
            }
            catch
            {
                // If the user transforms into a monster OUTSIDE the ship then make them invisible
                foreach (GameObject obj in targetScene.GetRootGameObjects())
                {
                    if (obj.name == "PlayersContainer")
                    {
                        GameObject username = obj.transform.Find($"{PlayerString}/{usernameCanvas}/Text (TMP)")?.gameObject;
                        TextMeshProUGUI colorChange = username?.GetComponent<TextMeshProUGUI>();

                        if (username != null)
                        {
                            colorChange.color = Color.red;
                            if (reset)
                            {
                                colorChange.color = Color.white;
                            }
                        }
                    }
                }
            }
        }




        [HarmonyPatch(typeof(StartOfRound), "LateUpdate")]
        [HarmonyPrefix]
        private static void LateUpdatePatch(
            ref NetworkObject ___shipAnimatorObject,
            ref ShipLights ___shipRoomLights,
            ref Animator ___shipDoorsAnimator,
            ref AudioClip ___airPressureSFX,
            ref GameObject ___starSphereObject,
            ref bool ___suckingPlayersOutOfShip,
            ref bool ___suckingFurnitureOutOfShip,
            ref UnlockablesList ___unlockablesList,
            ref AudioSource ___shipDoorAudioSource,
            ref AudioClip ___alarmSFX,
            ref AudioClip ___suckedIntoSpaceSFX,
            ref Transform[] ___playerSpawnPositions,
            ref GameObject ___currentPlanetPrefab,
            ref Transform ___planetContainer,
            ref bool ___choseRandomFlyDirForPlayer,
            ref float ___suckingPower,
            ref Transform ___middleOfShipNode,
            ref Transform ___shipDoorNode,
            ref int ___collidersAndRoomMask,
            ref Vector3 ___randomFlyDir,
            ref Transform ___middleOfSpaceNode,
            ref Transform ___moveAwayFromShipNode,
            ref bool ___hangarDoorsClosed,
            ref TextMeshProUGUI ___mapScreenPlayerName
            )
        {


            ownerCommands();
            impCommands();


            if (resetSucking)
            {
                StartOfRound.Instance.ResetShip();
                ___suckingPlayersOutOfShip = false;
                ___choseRandomFlyDirForPlayer = false;
                ___suckingPower = 0f;
                resetSucking = false;
            }

            if (PTestPlugin.gameEnded == true)
            {
                HUDManager.Instance.ShowPlayersFiredScreen(show: false);
            }

            if (PTestPlugin.monsterWin.Value == true && localhudImpWin == false)
            {
                PTestPlugin.Instance.logger.LogInfo(">>>RRRR In hudImp");
                HUDManager.Instance.ShowPlayersFiredScreen(show: true);
                localhudImpWin = true;
            }
            else if (PTestPlugin.crewWin.Value == true && localhudCrewWin == false)
            {
                PTestPlugin.Instance.logger.LogInfo(">>>RRRR In hudCrew");
                HUDManager.Instance.ShowPlayersFiredScreen(show: true);
                localhudCrewWin = true;
            }

            //PTestPlugin.Instance.logger.LogInfo(GameNetworkManager.Instance.localPlayerController.isInsideFactory);


            // Turn Of Screen
            //___mapScreen.SwitchScreenOn(false);
            // (Potentially turn of TV Screen server wide instead of client side): UnityEngine.Object.FindObjectOfType<TVScript>().TurnOffTVServerRpc();

            // Change Name On Screen
            ___mapScreenPlayerName.text = "Body";
            PTestPlugin.playerSpawnPositions = ___playerSpawnPositions;

            // KillCooldown
            if (ShovelPatch.monsterHitPlayer)
            {
                killCooldownFunc();
            }
            else
            {
                killCooldown = 25f;
            }


            if (PTestPlugin.gracePeriod.Value)
            {
                introCountDown -= Time.deltaTime;
                PTestPlugin.cDown(introCountDown);
            }
            else
            {
                introCountDown = 15f;
                //GameNetworkManager.Instance.localPlayerController.TeleportPlayer(PTestPlugin.playerSpawnPositions[GameNetworkManager.Instance.localPlayerController.playerClientId].position);
            }



   

            if (PTestPlugin.gameStarted.Value == true && PTestPlugin.Gamemode.Value != null)
            {
                impControls();
            }


            // Turn Imp Name Red
            if (PTestPlugin.monster && PTestPlugin.Gamemode.Value == "normal")
            {
                string imp1 = PTestPlugin.globalPlayerImp.Value.Replace("globalImp/", "");
                turnImpNameRed(imp1);

                if (PTestPlugin.globalPlayerImp2.Value != null)
                {
                    string imp2 = PTestPlugin.globalPlayerImp2.Value.Replace("globalImp/", "");
                    turnImpNameRed(imp2);
                }

            }



            // Make Imposter Invisible
            if ((PTestPlugin.globalPlayerImp.Value != null) && EntranceTeleportPatch.SkeldMap == null && PTestPlugin.globalPlayerImp.Value.Contains("globalImp/") && invisMon == false)
            {
                theImp = PTestPlugin.globalPlayerImp.Value.Replace("globalImp/", "");
                playerInScene(theImp, false);
                invisMon = true;
            }

            if (PTestPlugin.globalPlayerImp.Value == "none" && invisMon == true)
            {
                playerInScene(theImp, true);
                invisMon = false;
            }





            // Spawn Scraps on Skeld

            if (PTestPlugin.playerClientID == 0 && EntranceTeleportPatch.SkeldMap)
            {
                System.Random randomNum = new System.Random();
                int index = randomNum.Next(0, PTestPlugin.scrapNames.ToArray().Length);
                scrapTimer -= Time.deltaTime;

                if (scrapTimer <= 0)
                {
                    PTestPlugin.Instance.logger.LogInfo(">>> Spawned Item in x Location");
                    PTestPlugin.SpawnScrapFunc($"spwscr {PTestPlugin.scrapNames[index]}", "skeldmap");
                    scrapTimer = 45f;
                }
            }

            foreach (PlayerControllerB plr in StartOfRound.Instance.allPlayerScripts)
            {
                plr.ResetPlayerBloodObjects();
            }






            // Spawn Vents

            if (PTestPlugin.playerClientID == 0 && vents == false && EntranceTeleportPatch.SkeldMap)
            {
                vents = true;
                PTestPlugin.SpawnScrapFunc("spwscr metalsheet", "vnt");
                PTestPlugin.SpawnScrapFunc("spwscr enginepart", "btn");
            }



            // Make stuff invis
            if (EntranceTeleportPatch.SkeldMap != null && invisArea == false)
            {

                Scene targetScene = SceneManager.GetSceneByName("SampleSceneRelay");

                if (targetScene != null)
                {
                    foreach (GameObject obj in targetScene.GetRootGameObjects())
                    {
                        if (obj.name == "MetalSheet(Clone)" || obj.name == "EnginePart(Clone)")
                        {
                            obj.GetComponent<Renderer>().enabled = false;
                        }
                    }
                    PTestPlugin.Instance.logger.LogInfo(">>>AAAA attempting to invis items");

                    invisArea = true;
                }
            }





            // Meeting Cooldown
            if (PTestPlugin.votetime <= 0)
            {
                PTestPlugin.inMeetingCooldown -= Time.deltaTime;
            }







            // Player called a meeting
            if (PTestPlugin.inMeeting.Value)
            {

                CheckPlayerLocation();

                PTestPlugin.Instance.logger.LogInfo($">>> ejectTime is {PTestPlugin.ejectTime}");
                PTestPlugin.Instance.logger.LogInfo($">>> votetime is {PTestPlugin.votetime}");
                if (PTestPlugin.votetime > 0 && PTestPlugin.playerClientID == 0)
                {
                    // Meeting Time Display
                    if (PTestPlugin.playerClientID == 0)
                    {
                        PTestPlugin.customServerMessage.SendAllClients("Section1votetime");
                    }


                }
                else
                {
                    if (PTestPlugin.ejectTime <= 0 && PTestPlugin.playerClientID == 0)
                    {
                        // Basic Reset
                        if (PTestPlugin.playerClientID == 0)
                        {
                            PTestPlugin.customServerMessage.SendAllClients("Section2ejectTime");
                        }

                    }
                    else
                    {

                        // Eject Player

                        if (PTestPlugin.Section6suckingPart.Value && PTestPlugin.suckingPart && notsafe == true)
                        {
                            ___hangarDoorsClosed = false;
                            ___suckingPlayersOutOfShip = true;
                            ___suckingFurnitureOutOfShip = true;
                            ___choseRandomFlyDirForPlayer = false;
                            SuckLocalPlayerOutOfShipDoor(
                                suckingPower: ___suckingPower,
                                middleOfShipNode: ___middleOfShipNode,
                                shipDoorNode: ___shipDoorNode,
                                collidersAndRoomMask: ___collidersAndRoomMask,
                                choseRandomFlyDirForPlayer: ___choseRandomFlyDirForPlayer,
                                randomFlyDir: ___randomFlyDir,
                                middleOfSpaceNode: ___middleOfSpaceNode,
                                moveAwayFromShipNode: ___moveAwayFromShipNode
                                );

                            PTestPlugin.suckingPart = false;
                        }
                        if (PTestPlugin.Section8showStarsPart.Value && PTestPlugin.showStarsPart)
                        {
                            ___suckingFurnitureOutOfShip = false;
                            ___suckingPlayersOutOfShip = false;
                            ___choseRandomFlyDirForPlayer = false;
                            ___suckingPower = 0f;
                        }



                        EjectPlayerParent();

                        EjectPlayer(
                                shipAnimatorObject: ___shipAnimatorObject,
                                shipRoomLights: ___shipRoomLights,
                                shipDoorsAnimator: ___shipDoorsAnimator,
                                airPressureSFX: ___airPressureSFX,
                                starSphereObject: ___starSphereObject,
                                unlockablesList: ___unlockablesList,
                                shipDoorAudioSource: ___shipDoorAudioSource,
                                alarmSFX: ___alarmSFX,
                                suckedIntoSpaceSFX: ___suckedIntoSpaceSFX
                            );
                    }
                }
            }
        }

        private static void EjectPlayerParent()
        {
            if (PTestPlugin.playerClientID != 0) return;

            // PTestPlugin.Instance.logger.LogInfo(">>> ABOVE for-each loop");
            foreach (string colorName in PTestPlugin.allColors.Keys)            // Determine who to eject

            {
                // PTestPlugin.Instance.logger.LogInfo(">>> IN for-each loop");
                int maxValue = PTestPlugin.allColors.Values.Max();

                tempMaxValue = maxValue;
                //PTestPlugin.Instance.logger.LogInfo($">>> tempMax has a value of: {tempMaxValue}");
                if (PTestPlugin.skipVotes < tempMaxValue && PTestPlugin.skipVotes != tempMaxValue)
                {
                    //PTestPlugin.Instance.logger.LogInfo(">>> IN IF-STATE AFTER tempMax");
                    if (PTestPlugin.allColors[colorName] == maxValue)
                    {

                        PTestPlugin.customServerMessage.SendAllClients($"notsafe/{colorName}");
                        break;
                    }
                }
            }


            // Eject player out of ship
            if (PTestPlugin.votetime <= 1f && PTestPlugin.skipVotes < tempMaxValue && PTestPlugin.skipVotes != tempMaxValue)
            {

                PTestPlugin.ejectTime -= Time.deltaTime;

                //yield return new WaitForSeconds(5f);
                if (PTestPlugin.alarmPart && PTestPlugin.ejectTime <= 30f && PTestPlugin.playerClientID == 0)
                {
                    if (PTestPlugin.playerClientID == 0) PTestPlugin.customServerMessage.SendAllClients("Section4alarmPart");
                }


                // yield return new WaitForSeconds(9.37f);
                if (PTestPlugin.orbitPart && PTestPlugin.ejectTime <= 25f && PTestPlugin.playerClientID == 0)
                {
                    if (PTestPlugin.playerClientID == 0) PTestPlugin.customServerMessage.SendAllClients("Section5orbitPart");
                }

                //yield return new WaitForSeconds(0.25f);
                if (PTestPlugin.suckingPart && PTestPlugin.ejectTime <= 25.63f && PTestPlugin.playerClientID == 0)
                {
                    if (PTestPlugin.playerClientID == 0) PTestPlugin.customServerMessage.SendAllClients("Section6suckingPart");
                }

                // yield return new WaitForSeconds(6f);
                if (PTestPlugin.ejectedScreenPart && PTestPlugin.ejectTime <= 19f && PTestPlugin.playerClientID == 0)
                {
                    if (PTestPlugin.playerClientID == 0) PTestPlugin.customServerMessage.SendAllClients("Section7ejectedScreenPart");
                }

                //yield return new WaitForSeconds(2f);
                if ((PTestPlugin.showStarsPart && PTestPlugin.ejectTime <= 17.38f && PTestPlugin.playerClientID == 0))
                {
                    if (PTestPlugin.playerClientID == 0) PTestPlugin.customServerMessage.SendAllClients("Section8showStarsPart");

                    PTestPlugin.ejectTime = 5f;
                }
            }


            else
            {
                PTestPlugin.ejectTime = 0;
            }
        }




        private static void EjectPlayer(
            NetworkObject shipAnimatorObject,
            ShipLights shipRoomLights,
            Animator shipDoorsAnimator,
            AudioClip airPressureSFX,
            GameObject starSphereObject,
            UnlockablesList unlockablesList,
            AudioSource shipDoorAudioSource,
            AudioClip alarmSFX,
            AudioClip suckedIntoSpaceSFX
            )
        {

            // Eject player out of ship


            //yield return new WaitForSeconds(5f);
            if (PTestPlugin.alarmPart && PTestPlugin.Section4alarmPart.Value)
            {
                PTestPlugin.Instance.logger.LogInfo(">>> Alarm Part");
                PTestPlugin.alarmPart = false;
                shipAnimatorObject.gameObject.GetComponent<Animator>().SetBool("AlarmRinging", value: true);
                shipRoomLights.SetShipLightsOnLocalClientOnly(setLightsOn: true);
                shipDoorAudioSource.PlayOneShot(alarmSFX);
            }


            // yield return new WaitForSeconds(9.37f);
            if (PTestPlugin.Section5orbitPart.Value && PTestPlugin.orbitPart)
            {
                PTestPlugin.Instance.logger.LogInfo(">>> Orbit Part");
                PTestPlugin.orbitPart = false;
                shipDoorsAnimator.SetBool("OpenInOrbit", value: true);
                shipDoorAudioSource.PlayOneShot(airPressureSFX);
                starSphereObject.SetActive(value: true);
                starSphereObject.transform.position = GameNetworkManager.Instance.localPlayerController.transform.position;

            }

            //yield return new WaitForSeconds(0.25f);
            if (PTestPlugin.Section6suckingPart.Value && PTestPlugin.suckingPart2)
            {
                PTestPlugin.Instance.logger.LogInfo(">>> Sucking Part2");

                PTestPlugin.suckingPart2 = false;
                if (notsafe == true)
                {
                    // Suck func
                    PlaceableShipObject[] array = UnityEngine.Object.FindObjectsOfType<PlaceableShipObject>();
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i].parentObject == null)
                        {
                            Debug.Log("Error! No parentObject for placeable object: " + unlockablesList.unlockables[array[i].unlockableID].unlockableName);
                        }
                        array[i].parentObject.StartSuckingOutOfShip();
                        if (unlockablesList.unlockables[array[i].unlockableID].spawnPrefab)
                        {
                            Collider[] componentsInChildren = array[i].parentObject.GetComponentsInChildren<Collider>();
                            for (int j = 0; j < componentsInChildren.Length; j++)
                            {
                                componentsInChildren[j].enabled = false;
                            }
                        }
                    }
                    GameNetworkManager.Instance.localPlayerController.inSpecialInteractAnimation = true;
                    GameNetworkManager.Instance.localPlayerController.DropAllHeldItems();
                    HUDManager.Instance.UIAudio.PlayOneShot(suckedIntoSpaceSFX);
                }
            }

            // yield return new WaitForSeconds(6f);
            if (PTestPlugin.Section7ejectedScreenPart.Value && PTestPlugin.ejectedScreenPart)
            {
                PTestPlugin.ejectedScreenPart = false;
                //SoundManager.Instance.SetDiageticMixerSnapshot(3, 2f);
                HUDManager.Instance.ShowPlayersFiredScreen(show: true);
            }

            //yield return new WaitForSeconds(2f);
            if (PTestPlugin.Section8showStarsPart.Value && PTestPlugin.showStarsPart)
            {
                PTestPlugin.showStarsPart = false;
                starSphereObject.SetActive(value: false);
                shipDoorAudioSource.Stop();

                Debug.Log("Calling reset ship!");

                ResetShipFurn();

                if (notsafe == true)
                {
                    GameNetworkManager.Instance.localPlayerController.KillPlayer(new Vector3(0f, 0f, 0f), false);
                }
            }

        }



        private static void CheckPlayerLocation()
        {
            // Outside of ship when meeting is called
            if (GameNetworkManager.Instance.localPlayerController.transform.position.x <= -7f && notsafe == false)
            {
                GameNetworkManager.Instance.localPlayerController.TeleportPlayer(PTestPlugin.playerSpawnPositions[GameNetworkManager.Instance.localPlayerController.playerClientId].position);
            }
        }




        public static void SuckLocalPlayerOutOfShipDoor(
        float suckingPower,
        Transform middleOfShipNode,
        Transform shipDoorNode,
        int collidersAndRoomMask,
        bool choseRandomFlyDirForPlayer,
        Vector3 randomFlyDir,
        Transform middleOfSpaceNode,
        Transform moveAwayFromShipNode
        )
        {

            suckingPower += Time.deltaTime * 2f;
            GameNetworkManager.Instance.localPlayerController.fallValue = 0f;
            GameNetworkManager.Instance.localPlayerController.fallValueUncapped = 0f;
            if (Vector3.Distance(GameNetworkManager.Instance.localPlayerController.transform.position, middleOfShipNode.position) < 25f)
            {
                if (Physics.Linecast(GameNetworkManager.Instance.localPlayerController.transform.position, shipDoorNode.position, collidersAndRoomMask))
                {
                    GameNetworkManager.Instance.localPlayerController.externalForces = Vector3.Normalize(middleOfShipNode.position - GameNetworkManager.Instance.localPlayerController.transform.position) * 350f;
                }
                else
                {
                    GameNetworkManager.Instance.localPlayerController.externalForces = Vector3.Normalize(middleOfSpaceNode.position - GameNetworkManager.Instance.localPlayerController.transform.position) * (350f / Vector3.Distance(moveAwayFromShipNode.position, GameNetworkManager.Instance.localPlayerController.transform.position)) * (suckingPower / 2.25f);
                }
                return;
            }

            if (!choseRandomFlyDirForPlayer)
            {
                //choseRandomFlyDirForPlayer = true;
                randomFlyDir = new Vector3(-1f, 0f, UnityEngine.Random.Range(-0.7f, 0.7f));
            }
            GameNetworkManager.Instance.localPlayerController.externalForces = Vector3.Scale(Vector3.one, randomFlyDir) * 70f;
        }




        private static void ResetShipFurn()
        {

            for (int i = 0; i < StartOfRound.Instance.unlockablesList.unlockables.Count; i++)
            {
                if (StartOfRound.Instance.unlockablesList.unlockables[i].alreadyUnlocked || !StartOfRound.Instance.unlockablesList.unlockables[i].spawnPrefab)
                {
                    continue;
                }
                if (!StartOfRound.Instance.SpawnedShipUnlockables.TryGetValue(i, out var value))
                {
                    StartOfRound.Instance.SpawnedShipUnlockables.Remove(i);
                    continue;
                }
                if (value == null)
                {
                    StartOfRound.Instance.SpawnedShipUnlockables.Remove(i);
                    continue;
                }
                StartOfRound.Instance.SpawnedShipUnlockables.Remove(i);
            }

            StartOfRound.Instance.closetLeftDoor.SetBoolOnClientOnly(setTo: false);
            StartOfRound.Instance.closetRightDoor.SetBoolOnClientOnly(setTo: false);

            ShipTeleporter.hasBeenSpawnedThisSession = false;
            ShipTeleporter.hasBeenSpawnedThisSessionInverse = false;

            PlaceableShipObject[] array = Object.FindObjectsOfType<PlaceableShipObject>();
            for (int j = 0; j < array.Length; j++)
            {
                if (StartOfRound.Instance.unlockablesList.unlockables[array[j].unlockableID].alreadyUnlocked && !StartOfRound.Instance.unlockablesList.unlockables[array[j].unlockableID].spawnPrefab)
                {
                    array[j].parentObject.disableObject = false;
                    ShipBuildModeManager.Instance.ResetShipObjectToDefaultPosition(array[j]);
                }
            }

            GameNetworkManager.Instance.ResetUnlockablesListValues(true);

        }
    }


}
