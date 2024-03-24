/*
 * To the unfortunate soul that is reading this,
 * This is essentially a nightmare for programmers.
 * You are basically diving into the 9th circle by trying to make
 * sense of this project file.
 * Variables that do nothing, if-statements that could just be a dictionary, the list goes on.
 * To boil it all down, yes the code is poop. It's not meant to be looked at our edited again
 * so I didn't put any effort into readability or efficiency.
 * 
 * 
 * - syntax_z
 */


using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using PTest.Patches;
using LethalAPI.LibTerminal;
using LethalAPI.LibTerminal.Attributes;
using LethalAPI.LibTerminal.Models.Enums;
using LethalAPI.LibTerminal.Models;
using LethalNetworkAPI;
using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using _3rdPerson.Helper;
using BepInEx.Configuration;
using _3rdPerson;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;




/*
 * -- 2/4/2024
 - Added new thing to DropPatch
 - Infinite ammo now only displays when the round starts
 - Spawned ship
 - Updated playerList
 6:32 PM
 - added duplicate ship test

* -- 2/6/2024
* ~ Items despawn when dropped
* ! Players are now assigned colors based on their clientID (type colors)
*/


/* TODO:
 * - Nutcracker must die at the start of the round and then no more
 * - "Monsters Win/Employee win" Only shows up for host but not client when x dies to fall damage or dies in general?
 * - Currently if a player dies to fall damage or some other unknown way, they arent removed from the list
 * make it so that if AllowPlayerDeath is called, that player is removed
 * 
 * 
 * ! Remove extra quota
 * ! Add Grace Period text when players get teleported
 * ! Teleport players back to ship when grace period ends
 * 
 * ! Fix player position not being on top
 * ! Clients not being able to call a meeting
 * ! Fix door not closing for clients
 * ! Clear blood of clothes or have checkpoints where imps can clean blood off in shower
 * - Add Sherif
 * 
 * 
 * TODO:
 * ! Add Lighting to skeld map
 * ! Add Vents
 * ! Add places for items to randomly spawn
 * ! Add Higher box collider for the walls so players cant jump on top
 * ~ Add way to exit skeld map (if the player jumps off the map then just tp them back to spawn)
 * ! Emergency button should be the exit
 * ~ Make vents gui appear red
 * ~ Add Custom Ship prefab
 * 
 * 
 *  Scrap Positions: 
 *  Admin: 10.77f, -147.75f, 103.59f | 30.05f, -147.75f, 86.63f
 *  Comm: 16.8f, -147.75f, 51.3f | 8.02f, -147.75f, 51.3f
 *  Storage: -4.67f, -147.75f, 51.3f | -25.6f, -147.75f, 51.3f | -25.6f, -147.75f, 83.3f
 *  Elec: -54.43f, -147.75f, 90.8f
 *  Medic: -54.43f, -147.75f, 114.9f
 *  Cams: -80.9f, -147.75f, 114.9f
 *  Reactor: -124f, -147.75f, 114.9f
 *  Random Obj: -102.4f, -147.75f, 62.1f |  -102.4f, -147.75f, 156.91f
 *  Weapons: 50.7f, -147.75f, 156.91f
 *  Leaf room: 27.4f, -147.75f, 120.5f
 *  Nav: 89.6f, -147.75f, 113.61f
 *  Shields: 48f, -147.75f, 67.6f
 *  
 *  
 *  Vent Locations:
 *  Cafe: 18.07f, -147.75f, 138.62f
 *  Weapons: 44.36f, -147.75f, 160.82f
 *  Nav: 86.06f, -147.75f, 124.16f | 86.06f, -147.75f, 103.75f
 *  Nav Hallway: 48.3f, -147.75f, 107f
 *  Shield: 48.3f, -147.75f, 58.9f
 *  Admin: 9.72f, -147.75f, 83.64f
 *  Elec: -58.12f, -147.75f, 96.18f
 *  Lower Reactor: -89.5f, -147.75f, 62.27f
 *  Cams: -73.87f, -147.75f, 102.99f
 *  Reactor: -119.34f, -147.75f, 100.27f | -126.72f, -147.75f, 118.25f
 *  Top Reactor: -89.32f, -147.75f, 156.87f
 *  Medbay: -63.64f, -147.75f, 115.43f
 *  
 *  
 *  Button Position: -8.86f, -146.29f, 146.6f 
 *  
 *  
 *  TODO:
 *  ! Rebuild the moon (delete random cube and move fire door further away)
 *  ! Mesh for actual floors
 *  ~ Add ladders under vents
 *  ~ Only imps can go in vents
 *  ! meeting SFX
 *  ! Vent SFX
 *  ! Fix delay on vent sfx
 *  ! Be able to report body
 *  ! Despawn Body
 *  ~ Respawn All items
 *  ! Test if voting someone out still works
 *  - Add 2 imposters
 *  ! Add suits mod
 *  - Add more players mod
 *  ! Disable shovels/yieldsigns from spawning
 *  - Force mute everyone for a few rounds
 *  
 *  2/21 TODO:
 *  ~ Move camera with user inputs
 *  
 *  
 *  
 *  Monsters to Add:
 *  Outside:
 *  ~ Circuit Bee
 *  ~ Dog
 *  
 *  Inside:
 *  ! Flowerman (Bracken)
 *  ! Hygrodere (Slime)
 *  ! Thumper
 *  ! Spider
 *  
 *  
 *  
 *  2/27 3:35 AM
 *  ! Work on testing voting someone out (client side seems to not work)
 *  ! should get 100 credits when match starts as monster gamemode
 *  - recently added suits code to controllerb (might be causing the 3:30 AM error?)
 *  ! Add a "canVote" variable. If true, individual user can vote. (should be true by default and set to false if they voted in a meeting, then set back to true when meeting is over)
 *  
 *  
 *  3/1 8:26 PM
 *  ! Freeze player positions when the game starts
 *  
 *  3/3 12:31 AM
 *  ! Test other custom moons (do they work w/ the lethal expansion mod?)
 *  ! kill cooldown (when player hit something)
 *  ! Make sure tasks still work
 *  ! Fix UI being stuck when ejecting & winning
 *  ! Test Bracken not going to furthest node (bracken)
 *  ! Test spider outside
 *  ! Test Weapons in other slot mod
 *  - Test More players
 *  ! Remove weights for yieldsign
 *  ! Return false when ppl bring YieldSign on the ship
 *  - Test lower task quota (130 to maybe 70 or 80)
 *  ! gamemode for muting everyone. Eg. "start normal mute"
 *  ~ Bracken rotation should move w/ global imp
 *  ! Test if voting works ONLINE
 *  ! HUD Popup when killcool down resets
 *  ! Lower Eject time
 *  ! infinite time
 *  ! IMPORTANT: Fix voting. When I tried voting as client2 it bugged out (For replication, try voting the nonhost)
 *  ! Maybe disable everyone from seeing people at the start instead of tping them above
 *  ! Players cant walk outside of ship when in a meeting (unlesss notsafe == true)
 *  - Make imps take 0 dmg to shovels
 *  - Add 1,2,3,4 mod to switch to item
 *  - Dead people can talk
 *  ! Make imposter name red (check playerName screenshot)
 *  - Var to specify amount of monsters in the startgame terminal
 *  - Get rid of mines and turrents
 *  - Disable autosave
 *  ! Reset all variables without shutting down the game
 *  - Maybe make the kill cooldown random for 2+ imps
 *  - Maybe make cosmetics invis too
 *  ! Edit task amt from terminal
 *  ! Edit meeting time from terminal
 */


// Reminder: cplayerlist cmd prints out what playerlist looks like




namespace PTest
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("LethalNetworkAPI")]
    public class PTestPlugin : BaseUnityPlugin
    {
        private const string modGUID = "pts.PTest";
        private const string modName = "PTest";
        private const string modVersion = "1.0.0";
        private readonly Harmony harmony = new Harmony(modGUID);
        public static PTestPlugin Instance;
        internal ManualLogSource logger;
        //internal static List<AudioClip> SoundFX;
        internal static AssetBundle Bundle;

        private TerminalModRegistry TCommands;


        public static GameObject ShipRails;
        public static GameObject environmentParent;
        public static GameObject OutOfBoundsTerrain;

        public static RoundManager currentRound;
        // Run Once
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> calledAMeeting = new LethalNetworkVariable<bool>(identifier: "synz.calledAMeeting");

        // Roles (Gonna replace these w/ gameobjects and parent them)
        public static bool crew = false;
        public static bool sherif = false;
        public static bool monster = false;

        public static int skipVotes = 0;
        public static Dictionary<string, int> allColors = new Dictionary<string, int>();


        public static List<ulong> playerList = new List<ulong>();
        public static List<ulong> defaultPlayerList = new List<ulong>();

        [PublicNetworkVariable]
        public static LethalNetworkVariable<int> numberOfImposters = new LethalNetworkVariable<int>(identifier: "synz.numberOfImposters");

        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> crewWin = new LethalNetworkVariable<bool>(identifier: "synz.CrewWin");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> monsterWin = new LethalNetworkVariable<bool>(identifier: "synz.monsterWin");

        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> gameStarted = new LethalNetworkVariable<bool>(identifier: "synz.gameStarted");

        public static bool gameEnded;
        // [PublicNetworkVariable]
        //s public static LethalNetworkVariable<int> userfinishedTasks = new LethalNetworkVariable<int>(identifier: "synz.userfinishedTasks");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<string> killMonster = new LethalNetworkVariable<string>(identifier: "synz.killMonster");

        public static int userfinishedTasks;
        public static float roundOverTime;
        // Meeting Related
        public static float defaultVotetime = 60f;
        public static float defaultCooldown = 25f;
        public static float votetime;
        public static float ejectTime;
        //public static bool inMeeting;
        public static float inMeetingCooldown;
        //public static int votes;
        public static bool alarmPart;
        public static bool orbitPart;
        public static bool suckingPart;
        public static bool suckingPart2;
        public static bool ejectedScreenPart;
        public static bool showStarsPart;
        public static bool fixShipPart;
        public static GameObject _cam;
        public static List<string> allMonsters = new List<string>();
        public static bool canVote = true;
        //public static Dictionary<string, string> allMonsters = new Dictionary<string, string>();
        //public static GrabbableObject globalVentComponent;
        //public static GrabbableObject globalBTNComponent;

        /*
        [PublicNetworkVariable]
        public static LethalNetworkVariable<GrabbableObject> globalVentComponent = new LethalNetworkVariable<GrabbableObject>(identifier: "synz.globalVentComponent");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<GrabbableObject> globalBTNComponent = new LethalNetworkVariable<GrabbableObject>(identifier: "synz.globalBTNComponent");
        */



        public static Transform[] playerSpawnPositions;

        public static float ycoordMap = 140f;
        public static float ycoordPlayer = 52f;
        public static bool impSpawn = false;
        public static bool gpStart = true;
        public static bool gpOver = false;
        public static bool roundBegin = false;
        public static List<Vector3> scrapLocations = new List<Vector3>();
        public static List<string> scrapNames = new List<string>();
        //public static List<Vector3> ventPositions = new List<Vector3>();
        public static Dictionary<Vector3, Vector3> ventPositions = new Dictionary<Vector3, Vector3>();
        public static Dictionary<ulong, int> PlayerIDToSuitID = new Dictionary<ulong, int>();
        public static int numImps = 1;


        // Networking

        public static ulong playerClientID;
        public static ulong playerIsHost = 0;
        public static string playerName;


        [PublicNetworkVariable]
        public static LethalNetworkVariable<Dictionary<string, string>> Roles = new LethalNetworkVariable<Dictionary<string, string>>(identifier: "synz.Roles");

        [PublicNetworkVariable]
        public static LethalNetworkVariable<ulong> Sherif = new LethalNetworkVariable<ulong>(identifier: "synz.Sherif");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<ulong> Monster = new LethalNetworkVariable<ulong>(identifier: "synz.Monster");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<ulong> Monster2 = new LethalNetworkVariable<ulong>(identifier: "synz.Monster2");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<ulong> Crew = new LethalNetworkVariable<ulong>(identifier: "synz.Crew");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<string> Gamemode = new LethalNetworkVariable<string>(identifier: "synz.Gamemode");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<GameObject> GrabItem = new LethalNetworkVariable<GameObject>(identifier: "synz.GrabItem");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> inMeeting = new LethalNetworkVariable<bool>(identifier: "synz.inMeeting");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<Transform[]> tp = new LethalNetworkVariable<Transform[]>(identifier: "synz.tp");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<int> votes = new LethalNetworkVariable<int>(identifier: "synz.votes");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<string> votingPlayer = new LethalNetworkVariable<string>(identifier: "synz.votingPlayer");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<string[]> playerColors = new LethalNetworkVariable<string[]>(identifier: "synz.playerColors");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<Vector3> teleportPosition = new LethalNetworkVariable<Vector3>(identifier: "synz.teleportPosition");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<ulong> tpColorID = new LethalNetworkVariable<ulong>(identifier: "synz.tpColorID");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> gracePeriod = new LethalNetworkVariable<bool>(identifier: "synz.gracePeriod");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> lightsOn = new LethalNetworkVariable<bool>(identifier: "synz.lightsOn");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<string> globalPlayerImp = new LethalNetworkVariable<string>(identifier: "synz.globalPlayerImp");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<string> globalPlayerImp2 = new LethalNetworkVariable<string>(identifier: "synz.globalPlayerImp2");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<string> chosenMonster = new LethalNetworkVariable<string>(identifier: "synz.chosenMonster");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> GiveEveryoneTheirSuits = new LethalNetworkVariable<bool>(identifier: "synz.GiveEveryoneTheirSuits");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> ShipInside = new LethalNetworkVariable<bool>(identifier: "synz.ShipInside");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> Section0inMeeting = new LethalNetworkVariable<bool>(identifier: "synz.Section0inMeeting");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> Section1votetime = new LethalNetworkVariable<bool>(identifier: "synz.Section1votetime");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> Section2ejectTime = new LethalNetworkVariable<bool>(identifier: "synz.Section2ejectTime");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> Section3ejectSeries = new LethalNetworkVariable<bool>(identifier: "synz.Section3ejectSeries");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> Section4alarmPart = new LethalNetworkVariable<bool>(identifier: "synz.Section4alarmPart");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> Section5orbitPart = new LethalNetworkVariable<bool>(identifier: "synz.Section5orbitPart");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> Section6suckingPart = new LethalNetworkVariable<bool>(identifier: "synz.Section6suckingPart");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> Section7ejectedScreenPart = new LethalNetworkVariable<bool>(identifier: "synz.Section7ejectedScreenPart");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> Section8showStarsPart = new LethalNetworkVariable<bool>(identifier: "synz.Section8showStarsPart");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> Section9fixShipPart = new LethalNetworkVariable<bool>(identifier: "synz.Section9fixShipPart");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> muted = new LethalNetworkVariable<bool>(identifier: "synz.muted");
        [PublicNetworkVariable]
        public static LethalNetworkVariable<bool> canEnter = new LethalNetworkVariable<bool>(identifier: "synz.canEnter");



        public static List<string> listOfMonsters = new List<string>();
        public static List<string> listOfCrew = new List<string>();
        //public InputAction movementAction;


        public static readonly LethalServerMessage<string> customServerMessage = new LethalServerMessage<string>(identifier: "role", ReceiveByServer);
        public static readonly LethalClientMessage<string> customClientMessage = new LethalClientMessage<string>(identifier: "role", ReceiveFromServer, ReceiveFromClient);

        public static readonly LethalServerMessage<string> VoteServerMessage = new LethalServerMessage<string>(identifier: "votePlayer", VoteReceiveByServer);
        public static readonly LethalClientMessage<string> VoteClientMessage = new LethalClientMessage<string>(identifier: "votePlayer", VoteReceiveFromServer, VoteReceiveFromClient);


        // other stuff

        //public static ConfigEntry<string> sKeybindEntry = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            logger = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            logger.LogInfo("This is a test mod");

            PatchAllStuff();
            //CustomNetworkEvents();
            ScrapLocationTasks();
            ScrapNames();
            VentPositions();
            AvailableMonstersList();
            PlayerToSuits();

            /*
            SoundFX = new List<AudioClip>();
            string FolderLoc = Instance.Info.Location;
            FolderLoc = FolderLoc.TrimEnd("PTest.dll".ToCharArray());
            Bundle = AssetBundle.LoadFromFile(FolderLoc + "Modules\\amongussfx");
            if (Bundle != null)
            {
                Instance.logger.LogInfo(">>>Successfully loaded bundle");
                SoundFX = Bundle.LoadAllAssets<AudioClip>().ToList();
                // PTestPlugin.SoundFX[0]; Body reported
                // PTestPlugin.SoundFX[1]; Button Called
                // PTestPlugin.SoundFX[2]; Vent
                // PTestPlugin.SoundFX[3]; Victory crew
                // PTestPlugin.SoundFX[4]; Victory monster

                foreach (AudioClip clip in SoundFX)
                {
                    Instance.logger.LogInfo($">>>Clip: {clip}");
                }
            }
            else
            {
                Instance.logger.LogInfo(">>>Failed to load bundle");
            }
            */

            PTestPlugin.Instance.logger.LogInfo(">>>Apple Pie");
            TCommands = TerminalRegistry.CreateTerminalRegistry();
            TCommands.RegisterFrom(this);


            //sKeybindEntry = ((BaseUnityPlugin)this).Config.Bind<string>("Monster Settings", "Keybind", "8", "The keybind used to transform");
        }



        private void PatchAllStuff()
        {
            harmony.PatchAll(typeof(PTestPlugin));
            harmony.PatchAll(typeof(DissonanceCommsPatch));
            harmony.PatchAll(typeof(EnemyAIPatch));
            harmony.PatchAll(typeof(EntranceTeleportPatch));
            //harmony.PatchAll(typeof(FlowermanAI));
            harmony.PatchAll(typeof(GrabbableObjectPatch));
            harmony.PatchAll(typeof(HangarShipDoorPatch));
            harmony.PatchAll(typeof(HUDManagerPatch));
            harmony.PatchAll(typeof(NutcrackerEnemyAIPatch));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            harmony.PatchAll(typeof(RoundManagerPatch));
            harmony.PatchAll(typeof(ShipAlarmCordPatch));
            harmony.PatchAll(typeof(ShovelPatch));
            //harmony.PatchAll(typeof(SoundManagerPatch));
            harmony.PatchAll(typeof(StartMatchLeverPatch));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(ThirdPersonCameraPatch));
            harmony.PatchAll(typeof(TimeOfDayPatch));
            harmony.PatchAll(typeof(UnlockableSuitPatch));

            Instance.logger.LogInfo(">>> Applied all patches");
        }


        private void ScrapLocationTasks()
        {
            // Random Obj
            scrapLocations.Add(new Vector3(-102.4f, -147.75f, 62.1f));
            scrapLocations.Add(new Vector3(-102.4f, -147.75f, 156.91f));
            // Admin
            scrapLocations.Add(new Vector3(10.77f, -147.75f, 103.59f));
            scrapLocations.Add(new Vector3(30.05f, -147.75f, 86.63f));
            // Comm
            scrapLocations.Add(new Vector3(16.8f, -147.75f, 51.3f));
            scrapLocations.Add(new Vector3(8.02f, -147.75f, 51.3f));
            // Storage
            scrapLocations.Add(new Vector3(-4.67f, -147.75f, 51.3f));
            scrapLocations.Add(new Vector3(-25.6f, -147.75f, 51.3f));
            scrapLocations.Add(new Vector3(-25.6f, -147.75f, 83.3f));
            // Elec
            scrapLocations.Add(new Vector3(-54.43f, -147.75f, 90.8f));
            // Medic
            scrapLocations.Add(new Vector3(-54.43f, -147.75f, 114.9f));
            // Cams
            scrapLocations.Add(new Vector3(-80.9f, -147.75f, 114.9f));
            // Reactor
            scrapLocations.Add(new Vector3(-124f, -147.75f, 114.9f));
            // Weapons
            scrapLocations.Add(new Vector3(50.7f, -147.75f, 156.91f));
            // Leaf room
            scrapLocations.Add(new Vector3(27.4f, -147.75f, 120.5f));
            // Nav
            scrapLocations.Add(new Vector3(89.6f, -147.75f, 113.61f));
            // Shields
            scrapLocations.Add(new Vector3(48f, -147.75f, 67.6f));

            Instance.logger.LogInfo(">>> Added all scrap locations");
        }

        private void ScrapNames()
        {
            scrapNames.Add("picklejar");
            scrapNames.Add("fancyring");
            //scrapNames.Add("metalsheet");
            scrapNames.Add("cog");
            scrapNames.Add("dustpan");
            scrapNames.Add("bigbolt");
            scrapNames.Add("dustpan");
            scrapNames.Add("flask");
            scrapNames.Add("eggbeater");
            //scrapNames.Add("diyflashbang");
            //scrapNames.Add("enginepart");

            Instance.logger.LogInfo(">>> Added Scrap Names");
        }


        private void AvailableMonstersList()
        {
            // Inside
            allMonsters.Add("flowerman");
            allMonsters.Add("blob");
            allMonsters.Add("crawler"); // thumper
            allMonsters.Add("spider");

            Instance.logger.LogInfo(">>> Added monster names");
        }


        private void PlayerToSuits()
        {
            PlayerIDToSuitID.Add(0, 35);
            PlayerIDToSuitID.Add(1, 26);
            PlayerIDToSuitID.Add(2, 28);
            PlayerIDToSuitID.Add(3, 33);
            PlayerIDToSuitID.Add(4, 32);
            PlayerIDToSuitID.Add(5, 37);
            PlayerIDToSuitID.Add(6, 25);
            PlayerIDToSuitID.Add(7, 36);
            PlayerIDToSuitID.Add(8, 34);
            PlayerIDToSuitID.Add(9, 29);

            Instance.logger.LogInfo(">>> Added player suits");
        }

        /*
        private void AvailableMonstersDict()
        {
            // Outside
            //allMonsters.Add("redlocustbees", "outside");
            //allMonsters.Add("mouthdog", "outside");

            // Inside
            allMonsters.Add("flowerman", "inside");
            allMonsters.Add("blob", "inside");
            allMonsters.Add("crawlers", "inside"); // thumper
            allMonsters.Add("spider", "both");
        }
        */

        private void CustomInputs()
        {

        }

        private void VentPositions()
        {
            // Cafe
            ventPositions.Add(new Vector3(18.07f, -147.75f, 138.62f), new Vector3(44.36f, -147.75f, 160.82f));
            // Weapons
            ventPositions.Add(new Vector3(44.36f, -147.75f, 160.82f), new Vector3(18.07f, -147.75f, 138.62f));
            // Nav 
            ventPositions.Add(new Vector3(86.06f, -147.75f, 124.16f), new Vector3(44.36f, -147.75f, 160.82f));
            ventPositions.Add(new Vector3(86.06f, -147.75f, 103.75f), new Vector3(44.36f, -147.75f, 160.82f));
            // Nav Hallway
            ventPositions.Add(new Vector3(48.3f, -147.75f, 107f), new Vector3(86.06f, -147.75f, 124.16f));
            // Shield
            ventPositions.Add(new Vector3(48.3f, -147.75f, 58.9f), new Vector3(48.3f, -147.75f, 107f));
            // Admin
            ventPositions.Add(new Vector3(9.72f, -147.75f, 83.64f), new Vector3(48.3f, -147.75f, 107f));
            // Elec
            ventPositions.Add(new Vector3(-58.12f, -147.75f, 96.18f), new Vector3(-63.64f, -147.75f, 115.43f));
            // Lower Reactor
            ventPositions.Add(new Vector3(-89.5f, -147.75f, 62.27f), new Vector3(-119.34f, -147.75f, 100.27f));
            // Cams
            ventPositions.Add(new Vector3(-73.87f, -147.75f, 102.99f), new Vector3(-58.12f, -147.75f, 96.18f));
            // Reactor
            ventPositions.Add(new Vector3(-119.34f, -147.75f, 100.27f), new Vector3(-89.32f, -147.75f, 156.87f));
            ventPositions.Add(new Vector3(-126.72f, -147.75f, 118.25f), new Vector3(-89.32f, -147.75f, 156.87f));
            // Top Reactor
            ventPositions.Add(new Vector3(-89.32f, -147.75f, 156.87f), new Vector3(18.07f, -147.75f, 138.62f));
            // Medbay
            ventPositions.Add(new Vector3(-63.64f, -147.75f, 115.43f), new Vector3(-73.87f, -147.75f, 102.99f));

            Instance.logger.LogInfo(">>> Added vent positions");
        }


        public static void CustomNetworkEvents()
        {
            PTestPlugin.gameStarted.OnValueChanged += PTestPlugin.atStartGame;
            PTestPlugin.Sherif.OnValueChanged += PTestPlugin.atSherifRole;
            PTestPlugin.Monster.OnValueChanged += PTestPlugin.atMonsterRole;
            PTestPlugin.Monster2.OnValueChanged += PTestPlugin.atMonsterRole2;
            PTestPlugin.GrabItem.OnValueChanged += PTestPlugin.atGrabItem;
            PTestPlugin.inMeeting.OnValueChanged += PTestPlugin.atInMeeting;
            PTestPlugin.votingPlayer.OnValueChanged += PTestPlugin.atVotingPlayer;
            PTestPlugin.teleportPosition.OnValueChanged += PTestPlugin.atTeleportPosition;
            PTestPlugin.gracePeriod.OnValueChanged += PTestPlugin.atGracePeriod;
            PTestPlugin.lightsOn.OnValueChanged += PTestPlugin.atLightsOn;
            PTestPlugin.GiveEveryoneTheirSuits.OnValueChanged += PTestPlugin.atGiveEveryoneTheirSuits;
            PTestPlugin.ShipInside.OnValueChanged += PTestPlugin.atShipInside;
            //PTestPlugin.Roles.OnValueChanged += PTestPlugin.atSherifRole;
        }



        [TerminalCommand("Start"), AllowedCaller(AllowedCaller.Host)]
        [CommandInfo("Assigns roles to users")]
        public string StartCommand(string gamemode, string mutePlayers)
        {
            if (!gamemode.Contains("normal") && !gamemode.Contains("monster")) return $"{gamemode} is not a valid gamemode. Try: normal, monster";
            if (!mutePlayers.Contains("unmuted") && !mutePlayers.Contains("muted")) return "Invalid. Choose: muted or unmuted";


            if (mutePlayers == "muted")
            {
                muted.Value = true;
            }
            else
            {
                muted.Value = false;
            }

            Instance.logger.LogInfo(">>>In StartCommand<<<");
            Gamemode.Value = gamemode;
            gameStarted.Value = true;
            gracePeriod.Value = true;
            //ShipInside.Value = true;
            GiveEveryoneTheirSuits.Value = true;

            Instance.logger.LogInfo($">>> graceperiod: {gracePeriod.Value}");
            return $"Game Started, Roles assigned.";
        }


        private static void ResetVariables()
        {
            Monster.Value = 404;
            Monster2.Value = 404;
            PlayerControllerBPatch.canSpawnWeapon = false;
            StartMatchLeverPatch.continueStatement = true;
            gameStarted.Value = false;
            gameEnded = true;
            roundBegin = false;
            RoundManagerPatch.roundLoaded = false;
            TimeOfDayPatch.winVar = false;
            RoundManagerPatch.tempWin = "nah";

            Sherif.Value = 404;
            playerList = defaultPlayerList;
            allColors.Clear();
            listOfCrew.Clear();
            listOfMonsters.Clear();
            monsterWin.Value = false;
            crewWin.Value = false;
            userfinishedTasks = 0;
            ShipAlarmCordPatch.calledMeetingOnce = false;
            PlayerControllerBPatch.localhudImpWin = false;
            PlayerControllerBPatch.localhudCrewWin = false;
            canEnter.Value = false;
            EntranceTeleportPatch.SpawnVents = false;
            EntranceTeleportPatch.SpawnLights = false;
            gracePeriod.Value = false;
            gpOver = false;
            // forgot 10 colors

            globalPlayerImp.Value = "none";
            globalPlayerImp2.Value = "none";
            //GiveEveryoneTheirSuits.Value = false;
        }

        [TerminalCommand("Stop"), AllowedCaller(AllowedCaller.Host)]
        [CommandInfo("Stops the mystery game")]
        public string StopCommand()
        {
            ResetVariables();
            customServerMessage.SendAllClients("egame");
            return $"(wip) Game Ended";
        }




        [TerminalCommand("Stop")]
        [CommandInfo("end game (emergency only)")]
        public string StopCommand(string pass)
        {
            if (pass != "404") return "invalid secret key";

            customClientMessage.SendServer("egame");

            return $"Game Ended";
        }


        [TerminalCommand("Colors"), CommandInfo("Shows everyone and their assigned colors")]
        public string ClrCommand()
        {
            ulong[] clientList = StartOfRound.Instance.ClientPlayerList.Keys.ToArray();
            string res = "";
            foreach (ulong num in clientList)
            {
                res += $"!!{num}\t || {num.GetPlayerController().name}\n";
            }
            return "skip\n" + res.Replace("!!0", "Red").Replace("!!1", "Blue").Replace("!!2", "Green").Replace("!!3", "Pink").Replace("!!4", "Orange").Replace("!!5", "Yellow").Replace("!!6", "Black").Replace("!!7", "White").Replace("!!8", "Purple").Replace("!!9", "Cyan");
        }




        [TerminalCommand("edit_imps"), CommandInfo("Change num of imps (Only 1 or 2)"), AllowedCaller(AllowedCaller.Host)]
        public string ImpsCommand(int num)
        {
            numImps = num;
            return $"Changed Imp num";
        }


        [TerminalCommand("edit_tasks"), CommandInfo("Change num of imps (Only 1 or 2)"), AllowedCaller(AllowedCaller.Host)]
        public string taskamtCommand(int num)
        {
            TimeOfDayPatch.taskNum = num;
            return $"Changed task num";
        }


        [TerminalCommand("edit_timevote"), CommandInfo("Change default votetime"), AllowedCaller(AllowedCaller.Host)]
        public string votetimeCommand(int num)
        {
            defaultVotetime = num;
            return $"Changed default votetime num";
        }


        [TerminalCommand("Gamemode"), CommandInfo("Assign a gamemode: normal, monster"), AllowedCaller(AllowedCaller.Host)]
        public string GamemodeCommand(string mode)
        {
            Gamemode.Value = mode;
            return $"Gamemode Set To: {mode}";
        }

        [TerminalCommand("CheckGm"), CommandInfo("Check the current gamemode")]
        public string CheckGamemodeCommand()
        {
            return $"Gamemode Set To: {Gamemode.Value}";
        }


        [TerminalCommand("roles"), CommandInfo("Check the bool values")]
        public string rolesCommand()
        {
            return $"crew: {crew} || sherif: {sherif} || monster: {monster}";
        }


        [TerminalCommand("cplayerlist"), CommandInfo("Check playerlist")]
        public string playerlistCommand()
        {
            string thing = "";
            foreach (ulong id in playerList)
            {
                thing += $"{id}";
            }
            return thing;
        }


        [TerminalCommand("Assign"), AllowedCaller(AllowedCaller.Host)]
        [CommandInfo("Assign usernames/players a color")]
        public string AssignCommand(string name, string color)
        {
            return $"{name} has been assigned the color {color}.";
        }


        [TerminalCommand("tpp"), AllowedCaller(AllowedCaller.Host)]
        [CommandInfo("teleport a player")]
        public string tppCommand(string name)
        {
            //Vector3 playerpos = new Vector3(75f, 10, 20f);
            //string foodName = name.ToLower().Replace("meat", "0").Replace("rice", "1");
            //ulong foodID = ulong.Parse(foodName);
            customServerMessage.SendAllClients($"tpp/{name}");
            //foodID.GetPlayerController().TeleportPlayer(playerpos);
            return $"{name} has been teleported.";
        }

        /*
        [TerminalCommand("tpp"), AllowedCaller(AllowedCaller.Host)]
        [CommandInfo("teleport a player")]
        public string tppCommand(string name)
        {
            Vector3 playerpos = new Vector3(75f, 10, 20f);
            string colorName = name.ToLower().Replace("red", "0").Replace("blue", "1").Replace("green", "2").Replace("pink", "3").Replace("orange", "4").Replace("yellow", "5").Replace("black", "6").Replace("white", "7").Replace("purple", "8").Replace("cyan", "9");
            ulong colorID = ulong.Parse(colorName);
            LethalNetworkExtensions.GetPlayerController(colorID).TeleportPlayer(playerpos);
            return $"{name} has been teleported.";
        }
        */



        [TerminalCommand("Vote"), CommandInfo("Vote someone out by typing their color")]
        public string VoteCommand(string color)
        {
            if (!inMeeting.Value) return "!! You can only vote when in a meeting !!";
            if (canVote == false) return "!! You already voted !!";
            if (!allColors.ContainsKey(color.ToLower()) && !color.ToLower().Contains("skip")) return "!! Not a valid color !!";

            for (int i = 0; i < StartOfRound.Instance.allPlayerScripts.Length; i++)
            {
                string tempColor = color.ToLower().Replace("red", "0").Replace("blue", "1").Replace("green", "2").Replace("pink", "3").Replace("orange", "4").Replace("yellow", "5").Replace("black", "6").Replace("white", "7").Replace("purple", "8").Replace("cyan", "9");
                if (tempColor == $"{i}" && StartOfRound.Instance.allPlayerScripts[i].isPlayerDead) return "!! This player is dead!!";
            }

            if (color.ToLower().Contains("skip"))
            {
                customClientMessage.SendServer("vote/skip");
                //votingPlayer.Value = "skip";
                canVote = false;
                return "You voted to skip";
            }

            //string pid = color.ToLower().Replace("red", "0").Replace("blue", "1").Replace("green", "2").Replace("pink", "3").Replace("orange", "4").Replace("yellow", "5").Replace("black", "6").Replace("white", "7").Replace("purple", "8").Replace("cyan", "9");
            //ulong votedPlayer = ulong.Parse(pid);

            customClientMessage.SendServer($"vote/{color}");
            //votingPlayer.Value = color;
            //votes.Value = 1;
            canVote = false;
            return $"You voted for {color}";
        }


        [TerminalCommand("Votes"), CommandInfo("check num of votes")]
        public string VotesCommand()
        {
            string clrs = "";
            foreach (string c in allColors.Keys)
            {
                clrs += $"color: {c} || votes: {allColors[c]}\n";
            }
            return clrs += $"\nskip: {skipVotes}";
        }


        [TerminalCommand("Vent"), CommandInfo("init vents"), AllowedCaller(AllowedCaller.Host)]
        public string VentCommand()
        {
            PTestPlugin.SpawnScrapFunc("spwscr metalsheet", "vnt");
            return "Spawned Vents";

        }

        [TerminalCommand("light"), CommandInfo("add lighting test"), AllowedCaller(AllowedCaller.Host)]
        public string lightCommand()
        {
            GameObject lght = GameObject.Find("ShipElectricLights").gameObject; ;
            Quaternion rot = Quaternion.Euler(270, 0, 0);
            // Weapons Hallway
            Instantiate(lght, new Vector3(23.26f, -143.33f, 142f), rot);
            Instantiate(lght, new Vector3(23.26f, -143.33f, 150f), rot);
            // Medbay Hallway
            Instantiate(lght, new Vector3(-40.4f, -143.33f, 142f), rot);
            Instantiate(lght, new Vector3(-40.4f, -143.33f, 150f), rot);
            // Admin Hallway
            Instantiate(lght, new Vector3(-12.27f, -143.33f, 112f), rot);
            Instantiate(lght, new Vector3(-5.47f, -143.33f, 150f), rot);

            return "Added lights";

        }




        public static void atVotingPlayer(string colorName)
        {
            Instance.logger.LogInfo($">>> TOP OF VOTING PLAYER");

            if (playerClientID == 0)
            {
                customServerMessage.SendAllClients($"vote/{colorName}");
            }
        }




        private static void VoteReceiveFromServer(string data)
        {
            Instance.logger.LogInfo(">>> Message from client recieved!");
        }
        private static void VoteReceiveFromClient(string data, ulong id)
        {
            Instance.logger.LogInfo(">>> Message from client recieved!");
        }
        private static void VoteReceiveByServer(string data, ulong id)
        {
            Instance.logger.LogInfo(">>> Message by server recieved!");
        }




        [TerminalCommand("Tasks"), CommandInfo("Check how many tasks you have completed")]
        public string TasksCommand()
        {
            if (!gameStarted.Value) return "!! The game has not started yet !!";

            return $"You have {userfinishedTasks}/{TimeOfDay.Instance.profitQuota} completed.";
        }



        [TerminalCommand("cd"), CommandInfo("Check the current cooldown for the next meeting")]
        public string CooldownCommand()
        {
            if (!gameStarted.Value) return "!! The game has not started yet !!";

            return $"Cooldown: {inMeetingCooldown}";
        }



        [TerminalCommand("clientlist"), CommandInfo("(Test) Check client list")]
        public string ClientListCommand()
        {
            return $"Client IDS: {playerList.ToArray()}";
        }


        [TerminalCommand("nut"), CommandInfo("(Test) Spawn nutcracker"), AllowedCaller(AllowedCaller.Host)]
        public string NutCommand(string pclientID)
        {
            PTestPlugin.SpawnEnemyNut(pclientID);
            return "Spawned nut";
        }


        [TerminalCommand("shove"), CommandInfo("spawn weapon"), AllowedCaller(AllowedCaller.Host)]
        public string shoveCommand()
        {
            PTestPlugin.SpawnScrapFunc("spwscr yieldsign", $"{playerClientID}");
            return "spawn shovel";
        }

        [TerminalCommand("spweapon"), CommandInfo("spawn weapon if you're imp")]
        public string spweaponCommand()
        {
            if (monster)
            {
                customClientMessage.SendServer($"spweapon/{PTestPlugin.playerClientID}");
                return "spawned weapon";
            }
            return "You aren't a monster.";
        }



        [TerminalCommand("suits"), CommandInfo("Give everyone their suits"), AllowedCaller(AllowedCaller.Host)]
        public string SuitsCommand()
        {
            GiveEveryoneTheirSuits.Value = false;
            Instance.logger.LogInfo($">>> globalSuitVal: {GiveEveryoneTheirSuits.Value}");
            return "Suit up!";
        }


        [TerminalCommand("scoins"), CommandInfo("Give coins"), AllowedCaller(AllowedCaller.Host)]
        public string coinsCommand(int num)
        {
            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
            if (terminal != null)
            {
                terminal.groupCredits = num;
            }

            return $"Added {num} credits";
        }



        [TerminalCommand("rmvbg"), CommandInfo("Remove background"), AllowedCaller(AllowedCaller.Host)]
        public string bgCommand(string mode)
        {
            if (mode == "on")
            {
                RemoveEnvironment(false);
                return $"removed background";
            }
            else
            {
                RemoveEnvironment(true);
                return $"enabled background";
            }
        }






        public static void startGame()
        {
            Instance.logger.LogInfo(">>> TOP OF Normal StartGame");


            // Assign Roles (Append a gameobject role to users instead)
            //int rnd = Random.Range(0, 4);

            bool amHost = playerClientID.GetPlayerController().IsHost;

            if (!amHost) return;

            // Spawn Locations



            // Imposter

            ulong rndPlayer1 = randomPlayerSelect();
            if (rndPlayer1 != 0)
            {
                Instance.logger.LogInfo($">>> NormalM1 Player ID: {rndPlayer1}");
                Monster.Value = rndPlayer1;
            }
            else
            {
                Instance.logger.LogInfo(">>> PlaceholderM Host ID");
                Monster.Value = 999;
            }

            globalPlayerImp.Value = $"{rndPlayer1}";

            // later
            if (numImps == 2)
            {
                ulong rndPlayer2 = randomPlayerSelect();
                if (rndPlayer2 != 0)
                {
                    Instance.logger.LogInfo($">>> NormalM2 Player ID: {rndPlayer2}");
                    Monster2.Value = rndPlayer2;
                }
                else
                {
                    Instance.logger.LogInfo(">>> PlaceholderM Host ID");
                    Monster2.Value = 999;
                }

                globalPlayerImp2.Value = $"{rndPlayer2}";
            }

            Instance.logger.LogInfo(">>> BELOW MONSTER SendClient");

            /*
            ulong rndPlayer3 = randomPlayerSelect();
            if (rndPlayer3 != 0)
            {
                Instance.logger.LogInfo(">>> Normal Player ID");
                Sherif.Value = rndPlayer3;
            }
            else
            {
                Instance.logger.LogInfo(">>> Placeholder Host ID");
                Sherif.Value = 999;
            }
            */
            Instance.logger.LogInfo(">>> BELOW SHERIF SendClient");


            if (playerList.ToArray().Length > 0)
            {
                Instance.logger.LogInfo(">>> REACHED PlayerList IF-statement");
                foreach (ulong ID in playerList)
                {
                    string colorName = $"{ID}".Replace("0", "red").Replace("1", "blue").Replace("2", "green").Replace("3", "pink").Replace("4", "orange").Replace("5", "yellow").Replace("6", "black").Replace("7", "white").Replace("8", "purple").Replace("9", "cyan");
                    if (!listOfCrew.Contains(colorName))
                    {
                        listOfCrew.Add(colorName);
                    }

                    Instance.logger.LogInfo($"CREW ID: {ID}");
                    customServerMessage.SendClient("crew", ID);
                }
                // spawnLocation("crew");
            }

            roundBegin = true;

        }




        public static void startGameMonster()
        {
            Instance.logger.LogInfo(">>> TOP OF Normal StartGame");


            // Assign Roles (Append a gameobject role to users instead)
            //int rnd = Random.Range(0, 4);

            bool amHost = playerClientID.GetPlayerController().IsHost;

            if (!amHost) return;

            // Spawn Locations

            // Imposter

            ulong rndPlayer1 = randomPlayerSelect();
            if (rndPlayer1 != 0)
            {
                Instance.logger.LogInfo($">>> NormalM1 Player ID: {rndPlayer1}");
                Monster.Value = rndPlayer1;
            }
            else
            {
                Instance.logger.LogInfo(">>> PlaceholderM Host ID");
                Monster.Value = 999;
            }
            //globalPlayerImp.Value = $"{rndPlayer1}";
            Instance.logger.LogInfo(">>> BELOW SHERIF SendClient");


            if (playerList.ToArray().Length > 0)
            {
                Instance.logger.LogInfo(">>> REACHED PlayerList IF-statement");
                foreach (ulong ID in playerList)
                {
                    string colorName = $"{ID}".Replace("0", "red").Replace("1", "blue").Replace("2", "green").Replace("3", "pink").Replace("4", "orange").Replace("5", "yellow").Replace("6", "black").Replace("7", "white").Replace("8", "purple").Replace("9", "cyan");
                    if (!listOfCrew.Contains(colorName))
                    {
                        listOfCrew.Add(colorName);
                    }

                    Instance.logger.LogInfo($"CREW ID: {ID}");
                    customServerMessage.SendClient("crew", ID);
                }
                //spawnLocation("crew");
            }

            roundBegin = true;
        }








        public static void DefaultValues()
        {
            // Goal
            gameEnded = false;
            userfinishedTasks = 0;
            roundOverTime = 15f;

            // Roles
            crew = false;
            sherif = false;
            monster = false;
            numberOfImposters.Value = 0;
            crewWin.Value = false;
            monsterWin.Value = false;

            /*
            if (playerClientID == 0)
            {
                DisableMonsters();
            }
            */

            // Vote
            ejectTime = 30f;
            inMeetingCooldown = 0f;
            votes.Value = 0;
            alarmPart = true;
            orbitPart = true;
            suckingPart = true;
            ejectedScreenPart = true;
            showStarsPart = true;
            fixShipPart = true;

            PTestPlugin.Instance.logger.LogInfo(">>> actually in defaultValues method");
            //HUDManager.Instance.ShowPlayersFiredScreen(show: false);
        }


        public static void RoleWin()
        {

            if (monster)
            {
                PlayerControllerBPatch.turnImpNameRed(globalPlayerImp.Value, reset: true);
                PlayerControllerBPatch.turnImpNameRed(globalPlayerImp2.Value, reset: true);
            }

            Instance.logger.LogInfo(">>> INSIDE Rolewin");
            gameEnded = false;
            userfinishedTasks = 0;
            //RoundManagerPatch.subEventsOnce= false;
            EntranceTeleportPatch.SkeldMap = null;
            if (roundOverTime <= 8f && gameEnded == false)
            {
                userfinishedTasks = 0;
                HUDManager.Instance.ShowPlayersFiredScreen(show: false);
                gameEnded = true;
                TimeOfDayPatch.winVar = false;
                RoundManagerPatch.tempWin = "nah";
                Instance.logger.LogInfo(">>> INSIDE Rolewin IF-STATE");
                gameStarted.Value = false;
                /*
                HUDManager.Instance.ShowPlayersFiredScreen(show: false);
                gameEnded = true;
                TimeOfDayPatch.winVar = false;
                RoundManagerPatch.tempWin = "nah";
                Instance.logger.LogInfo(">>> INSIDE Rolewin IF-STATE");
                gameStarted.Value = false;
                */
            }
        }



        public static void EnableMonsters()
        {
            PTestPlugin.playerClientID.GetPlayerController().gameObject.tag = "Untagged";
            PTestPlugin.playerClientID.GetPlayerController().gameObject.layer = LayerMask.NameToLayer("Enemies");
            PTestPlugin.customClientMessage.SendServer($"spawnImp/{chosenMonster.Value}/{PTestPlugin.playerClientID}");
            PTestPlugin.customClientMessage.SendServer($"globalImp/{PTestPlugin.playerClientID}");
            PlayerControllerBPatch.turnedImp = true;
            ThirdPersonCamera.Toggle();
        }

        public static void DisableMonsters()
        {
            PlayerControllerBPatch.turnedImp = false;
            PTestPlugin.customClientMessage.SendServer($"killMonster/{chosenMonster.Value}");
            ThirdPersonCamera.Toggle();
        }







        public static void RemoveEnvironment(bool enabled = true)
        {
            // Environment 
            PTestPlugin.ShipRails = GameObject.Find("HangarShip").gameObject.transform.Find("ShipRails")?.gameObject;
            //PTestPlugin.environmentParent = GameObject.FindWithTag("OutsideLevelNavMesh").gameObject;
            PTestPlugin.OutOfBoundsTerrain = GameObject.Find("OutOfBoundsTerrain")?.gameObject;

            //Scene targetScene = SceneManager.GetSceneByName("Level1Experimentation");
            //Scene targetSkeld = SceneManager.GetSceneByName("InitSceneLaunchOptions");

            Scene currentScene = SceneManager.GetActiveScene();


            if (enabled)
            {
                ShipRails?.SetActive(false);
                OutOfBoundsTerrain?.SetActive(false);


                // Search root objects 
                foreach (GameObject obj in currentScene.GetRootGameObjects())
                {
                    if (obj.name == "Environment")
                    {
                        obj?.SetActive(false);
                        break;
                    }
                    else if (obj.name == "Skeld(Clone)")
                    {
                        obj?.SetActive(false);
                        break;
                    }
                }



            }
            else
            {
                ShipRails?.SetActive(true);
                //environmentParent.SetActive(true);
                OutOfBoundsTerrain?.SetActive(true);


                // Search root objects 
                foreach (GameObject obj in currentScene.GetRootGameObjects())
                {
                    if (obj.name == "Environment")
                    {
                        obj?.SetActive(true);
                        break;
                    }
                    else if (obj.name == "Skeld(Clone)")
                    {

                        obj?.SetActive(true);
                        break;
                    }
                }


            }

        }



        // Modified Method Copied from ChatCommand Mod https://github.com/Toemmsen96/ChatCommands/blob/master/ChatCommands/Commands/Commands.cs#L339
        public static void SpawnScrapFunc(string text, string name)
        {
            string[] segments = (text.Substring(1)).Split(' ');
            if (segments.Length < 2)
            {
                Instance.logger.LogWarning("Missing Arguments For Spawn\n'/spawnscrap <name> (amount=<amount>) (position={random, @me, @<playername>})");
            }
            string toSpawn = segments[1].ToLower(); // item name
                                                    //int amount = 1;
            Vector3 position = Vector3.zero;
            //int value = 1000;
            var args = segments.Skip(2);

            Instance.logger.LogInfo(">>> ABOVE CALC <<<");
            if (name != "vnt" || name != "btn")
            {
                position = PTestPlugin.CalculateSpawnPosition(name);
            }

            Instance.logger.LogInfo(">>> BELOW CALC <<<");

            int len = PTestPlugin.currentRound.currentLevel.spawnableScrap.Count();

            Instance.logger.LogInfo(">>> @ LEN THING <<<");
            bool spawnable = false;
            for (int i = 0; i < len; i++)
            {
                Item scrap = PTestPlugin.currentRound.currentLevel.spawnableScrap[i].spawnableItem;
                if (scrap.spawnPrefab.name.ToLower().Contains(toSpawn))
                {
                    if (name == "vnt")
                    {
                        Instance.logger.LogInfo(">>> Spawning vents <<<");
                        foreach (Vector3 vpos in ventPositions.Keys)
                        {
                            Instance.logger.LogInfo($">>> VPOS: {vpos}");
                            GameObject objToSpawn = scrap.spawnPrefab;
                            GameObject gameObject = UnityEngine.Object.Instantiate(objToSpawn, vpos, Quaternion.identity, PTestPlugin.currentRound.spawnedScrapContainer);
                            GrabbableObject component = gameObject.GetComponent<GrabbableObject>();

                            // Disable its renderer
                            //Renderer renderer = component.GetComponent<Renderer>();
                            //renderer.enabled = false;

                            component.startFallingPosition = vpos;
                            component.targetFloorPosition = component.GetItemFloorPosition(vpos);
                            //component.SetScrapValue(0); // Set Scrap Value
                            component.NetworkObject.Spawn();
                            spawnable = true;
                        }
                    }
                    else if (name == "btn")
                    {
                        Instance.logger.LogInfo(">>> Spawning Exit Button <<<");
                        Vector3 vpos = new Vector3(-8.86f, -146.29f, 146.6f);
                        GameObject objToSpawn = scrap.spawnPrefab;
                        GameObject gameObject = UnityEngine.Object.Instantiate(objToSpawn, vpos, Quaternion.identity, PTestPlugin.currentRound.spawnedScrapContainer);
                        GrabbableObject component = gameObject.GetComponent<GrabbableObject>();

                        // Disable its renderer
                        //Renderer renderer = component.GetComponent<Renderer>();
                        //renderer.enabled = false;

                        component.startFallingPosition = vpos;
                        component.targetFloorPosition = component.GetItemFloorPosition(vpos);
                        component.SetScrapValue(0); // Set Scrap Value
                        component.NetworkObject.Spawn();
                        spawnable = true;

                    }
                    else
                    {
                        GameObject objToSpawn = scrap.spawnPrefab;
                        GameObject gameObject = UnityEngine.Object.Instantiate(objToSpawn, position, Quaternion.identity, PTestPlugin.currentRound.spawnedScrapContainer);
                        GrabbableObject component = gameObject.GetComponent<GrabbableObject>();
                        /*
                        if ( name != "red")
                        {
                            position = new Vector3(position.x, position.y -6f, position.z);
                            Instance.logger.LogInfo(">>> Lowered position of Item <<<");
                        }
                        */
                        component.startFallingPosition = position;
                        component.targetFloorPosition = component.GetItemFloorPosition(position);
                        component.SetScrapValue(10); // Set Scrap Value
                        component.NetworkObject.Spawn();
                        spawnable = true;
                    }
                    break;
                }
            }
            if (!spawnable)
            {
                Instance.logger.LogWarning("Could not spawn " + toSpawn);
            }
        }


        // Modified Method Copied from ChatCommand Mod https://github.com/Toemmsen96/ChatCommands/blob/d32ffe59203f681ea7c645efa9730fef57c9f053/ChatCommands/Commands/Commands.cs#L86
        public static void SpawnEnemyNut(string name)
        {
            int amount = 1;
            Vector3 position = Vector3.zero;
            position = CalculateSpawnPosition(name);


            string enemyName = "";
            foreach (SpawnableEnemyWithRarity enemy in PTestPlugin.currentRound.currentLevel.Enemies)
            {
                if (enemy.enemyType.enemyName.ToLower().Contains("nut"))
                {
                    try
                    {
                        enemyName = enemy.enemyType.enemyName;
                        PTestPlugin.SpawnEnemy(enemy, amount, location: position);
                    }
                    catch
                    {
                        Instance.logger.LogInfo((object)"Could not spawn enemy");
                    }
                    Instance.logger.LogInfo($"Spawned: {enemyName}");
                    break;
                }
            }
        }



        public static void SpawnImposter(string name)
        {

            string[] pos = name.Split('/');
            string eny = pos[1];
            name = pos[2];


            int amount = 1;
            Vector3 position = Vector3.zero;
            position = CalculateSpawnPosition(name.ToLower());

            Instance.logger.LogInfo($">>> Trying to spawn: {eny}");
            string enemyName = "";
            foreach (SpawnableEnemyWithRarity enemy in PTestPlugin.currentRound.currentLevel.Enemies)
            {
                if (enemy.enemyType.enemyName.ToLower().Contains(eny))
                {
                    Instance.logger.LogInfo($">>> Inside eny if-s: ");
                    try
                    {
                        enemyName = enemy.enemyType.enemyName;
                        PTestPlugin.SpawnEnemy(enemy, amount, location: position);
                    }
                    catch
                    {
                        Instance.logger.LogInfo((object)"Could not spawn enemy");
                    }
                    Instance.logger.LogInfo($"Spawned: {enemyName} || Position: {position}");
                    break;
                }
            }
        }



        static void SpawnEnemy(SpawnableEnemyWithRarity enemy, int amount, Vector3 location)
        {
            try
            {
                for (int i = 0; i < amount; i++)
                {
                    currentRound.SpawnEnemyOnServer(location, 0.0f, currentRound.currentLevel.Enemies.IndexOf(enemy));
                }
                return;
            }
            catch
            {
                Instance.logger.LogInfo("Failed to spawn enemies, check your command.");
                return;
            }
        }




        // Modified Method Copied from ChatCommand Mod https://github.com/Toemmsen96/ChatCommands/blob/master/ChatCommands/Commands/Commands.cs#L855
        private static Vector3 CalculateSpawnPosition(string username)
        {
            Vector3 position = Vector3.zero;
            if (username == "skeldmap")
            {
                Instance.logger.LogInfo(">>> REACHED SKELD THING<<<");

                System.Random randomNum = new System.Random();
                int index = randomNum.Next(0, scrapLocations.ToArray().Length);
                position = scrapLocations[index];


            }
            else
            {
                //username = username.Replace("player (1)", "player #1").Replace("player (2)", "player #2").Replace("player (3)", "player #3").Replace("player (4)", "player #4").Replace("player (5)", "player #5").Replace("player (6)", "player #6").Replace("player (7)", "player #7").Replace("player (8)", "player #8").Replace("player (9)", "player #9");
                Instance.logger.LogInfo(">>> REACHED @PLAYER THING<<<");
                PlayerControllerB[] allPlayerScripts = StartOfRound.Instance.allPlayerScripts;
                foreach (PlayerControllerB testedPlayer in allPlayerScripts)
                {
                    Instance.logger.LogInfo($"Checking Playername: {testedPlayer.playerUsername.ToLower()} || {username}");
                    if ($"{testedPlayer.playerClientId}" == username)
                    {
                        Instance.logger.LogInfo($"Found player {testedPlayer.playerUsername}");
                        position = testedPlayer.transform.position;

                        break;
                    }
                }
            }

            return position;
        }


        public static ulong randomPlayerSelect()
        {
            Instance.logger.LogInfo(">>> in SELECTPLAYER IF");

            //ulong[] keys = StartOfRound.Instance.ClientPlayerList.Keys.ToArray();
            ulong[] keys = playerList.ToArray();
            Instance.logger.LogInfo(">>> Below SELECTPLAYER array");

            System.Random randomNum = new System.Random();
            Instance.logger.LogInfo(">>> After SELECTPLAYER Random");

            int index = randomNum.Next(0, keys.Length);
            //int index = 0;
            Instance.logger.LogInfo(">>> After SELECTPLAYER NEXT");

            ulong randomPlayerID = keys[index];
            Instance.logger.LogInfo(">>> After SELECTPLAYER RND ID");
            playerList.Remove(randomPlayerID);

            return randomPlayerID;
        }

        public static string randomMonsterSelect()
        {
            string[] keys = allMonsters.ToArray();

            System.Random randomNum = new System.Random();

            int index = randomNum.Next(0, keys.Length);


            string randomMonsterName = keys[index];


            return randomMonsterName;
            //return "flowerman";
        }



        public static void RoleRelated(string data)
        {
            Instance.logger.LogInfo(">>> IN rolerelated");
            string[] rawData = data.Split('/');

            Instance.logger.LogInfo(">>> before parse");
            ulong plID = ulong.Parse(rawData[1]);

            Instance.logger.LogInfo(">>> Afterparse");
            string name = plID.GetPlayerController().name.ToLower();

            if (rawData[0] == "sherif")
            {
                customClientMessage.SendServer("sherif");
            }
            else if (data == "monster")
            {

            }
            else if (data == "crew")
            {

            }
        }




        // Networking


        public static void DespawnSpecificPrefabs()
        {
            GameObject[] allPrefabs = FindObjectsOfType<GameObject>();
            foreach (GameObject prefabInstance in allPrefabs)
            {
                if (prefabInstance.name.ToLower().Contains("testroom")) // TestRoom(Clone) is the actual name in the inspector
                {
                    Destroy(prefabInstance);
                }
            }
        }

        public static void spawnLocation(string role)
        {
            //teleportPosition.Value = role;
            Instance.logger.LogInfo($">>> TOP Loop ycoordMap: {ycoordMap}");
            Instance.logger.LogInfo($">>> TOP Loop ycoordPlayer: {ycoordPlayer}");

            // playerpos = new Vector3(75f, 206f, 20f);
            // playerpos2 = new Vector3(75f, 406f, 20f);
            //float ycoordMap = 140f;
            //float ycoordPlayer = 196f;
            if (role == "crew")
            {
                foreach (string crewname in listOfCrew)
                {

                    customServerMessage.SendAllClients($"tpprd/{ycoordPlayer}/{crewname}/crew");

                    Instance.logger.LogInfo($">>> In SpawnLocation method, crewname is: {crewname}");
                    /*
                    GameObject ground = StartOfRound.Instance.testRoomPrefab;
                    GameObject gameObject = UnityEngine.Object.Instantiate(ground, new Vector3(0f, ycoordMap, 0f), Quaternion.identity, currentRound.mapPropsContainer.transform);
                    gameObject.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
                    ycoordMap += 200f;
                    */
                    ycoordPlayer += 52f;

                    Instance.logger.LogInfo($">>> Loop ycoordMap: {ycoordMap}");
                    Instance.logger.LogInfo($">>> Loop ycoordPlayer: {ycoordPlayer}");
                }
                Instance.logger.LogInfo($">>> AFTER Loop ycoordMap: {ycoordMap}");
                Instance.logger.LogInfo($">>> AFTER Loop ycoordPlayer: {ycoordPlayer}");
            }



            // Spawn Imps
            else if (role == "imp")
            {
                /*
                if (impSpawn == false)
                {
                    impSpawn = true;
                    GameObject impground = StartOfRound.Instance.testRoomPrefab;
                    GameObject impgameObject = UnityEngine.Object.Instantiate(impground, new Vector3(635f, 140, 635f), Quaternion.identity, currentRound.mapPropsContainer.transform);
                    impgameObject.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
                }
                */


                foreach (string impname in listOfMonsters)
                {
                    //Vector3 monsterPos = new Vector3(75f, ycoordPlayer + 200, 20f);
                    //string monsterName = name.ToLower().Replace("red", "0").Replace("blue", "1").Replace("green", "2").Replace("pink", "3").Replace("orange", "4").Replace("yellow", "5").Replace("black", "6").Replace("white", "7").Replace("purple", "8").Replace("cyan", "9");
                    //ulong monsterID = ulong.Parse(monsterName);

                    Instance.logger.LogInfo($">>> In SpawnLocation method, impname is: {impname}");
                    //tpColorID.Value = monsterID;
                    //teleportPosition.Value = monsterPos;
                    //monsterID.GetPlayerController().TeleportPlayer(monsterPos);


                    //customServerMessage.SendAllClients($"tpprd/{ycoordPlayer}/{impname}/imp");
                    GameNetworkManager.Instance.localPlayerController.TeleportPlayer(PTestPlugin.playerSpawnPositions[GameNetworkManager.Instance.localPlayerController.playerClientId].position);
                }
            }
        }


        public static void atTeleportPosition(Vector3 pos)
        {
            PTestPlugin.Instance.logger.LogInfo(">>> in TP METHOD");
            PTestPlugin.Instance.logger.LogInfo($">>> tpcolorID: {tpColorID.Value}");
            tpColorID.Value.GetPlayerController().TeleportPlayer(pos);

            PTestPlugin.Instance.logger.LogInfo(">>> BELOW TP");
        }









        public static void atStartGame(bool value)
        {
            if (value != false)
            {
                PTestPlugin.allColors.Add("red", 0);
                PTestPlugin.allColors.Add("blue", 0);
                PTestPlugin.allColors.Add("green", 0);
                PTestPlugin.allColors.Add("pink", 0);
                PTestPlugin.allColors.Add("orange", 0);
                PTestPlugin.allColors.Add("yellow", 0);
                PTestPlugin.allColors.Add("black", 0);
                PTestPlugin.allColors.Add("white", 0);
                PTestPlugin.allColors.Add("purple", 0);


            }
            else
            {
                allColors.Clear();
                HUDManager.Instance.ShowPlayersFiredScreen(show: true);
                monster = false;
                crew = false;


            }

            if (value && Gamemode.Value == "normal")
            {
                PTestPlugin.Instance.logger.LogInfo(">>> top in atGameStart normal gm");
                if (playerClientID == 0)
                {
                    customServerMessage.SendAllClients("defaultSetup");
                }

                // Starter Credits
                Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                if (terminal != null)
                {
                    terminal.groupCredits = 100;
                }

                GameNetworkManager.Instance.localPlayerController.TeleportPlayer(PTestPlugin.playerSpawnPositions[GameNetworkManager.Instance.localPlayerController.playerClientId].position);
                startGame();
            }
            else if (value && Gamemode.Value == "monster")
            {
                if (playerClientID == 0)
                {
                    customServerMessage.SendAllClients("defaultSetup");
                }

                // Starter Credits
                Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                if (terminal != null)
                {
                    terminal.groupCredits = 100;
                }

                GameNetworkManager.Instance.localPlayerController.TeleportPlayer(PTestPlugin.playerSpawnPositions[GameNetworkManager.Instance.localPlayerController.playerClientId].position);
                startGameMonster();
            }
        }


        private static void ReceiveFromServer(string data)
        {
            if (data == "defaultSetup")
            {
                Instance.logger.LogInfo(">>> SENT DEFAULT VALUES TO ALL CLIENTS: ");
                DefaultValues();
            }
            else if (data.Contains("tpprd/"))
            {
                // Intro spawn tp
                string[] splitParts = data.Split('/');
                string name = splitParts[2];
                string role = splitParts[3];
                float ypos = float.Parse(splitParts[1]);
                Vector3 playerpos;

                if (role == "imp")
                {
                    playerpos = new Vector3(0f, 4f, 0f);
                }
                else
                {
                    playerpos = new Vector3(0f, ypos, 0f);
                }

                string colorName = name.ToLower().Replace("red", "0").Replace("blue", "1").Replace("green", "2").Replace("pink", "3").Replace("orange", "4").Replace("yellow", "5").Replace("black", "6").Replace("white", "7").Replace("purple", "8").Replace("cyan", "9");
                ulong colorID = ulong.Parse(colorName);
                colorID.GetPlayerController().TeleportPlayer(playerpos);
            }
            else if (data == "crew")
            {
                /*
                string colorID = $"{data.Replace("crew/", "")}";
                string colorName = $"{colorID}".Replace("0", "red").Replace("1", "blue").Replace("2", "green").Replace("3", "pink").Replace("4", "orange").Replace("5", "yellow").Replace("6", "black").Replace("7", "white").Replace("8", "purple").Replace("9", "cyan");
                if (!listOfCrew.Contains(colorName))
                {
                    listOfCrew.Add(colorName);
                }
                */
                crew = true;
                Instance.logger.LogInfo(">>> Role is: " + data);
                HUDManager.Instance.DisplayTip("You Are An Employee!", "Do your tasks, find the monster(s) and vote them out.", false);
            }
            else if (data.Contains("disableRender/"))
            {

            }
            else if (data == "taskWin")
            {
                HUDManager.Instance.ShowPlayersFiredScreen(show: true);
                PTestPlugin.gameStarted.Value = false;
                PTestPlugin.crewWin.Value = true;
                HUDManagerPatch.hudCrewWin = true;
                PTestPlugin.roundOverTime -= Time.deltaTime;
                PTestPlugin.RoleWin();
            }
            else if (data == "monsterWin")
            {
                HUDManager.Instance.ShowPlayersFiredScreen(show: true);
                PTestPlugin.gameStarted.Value = false;
                monsterWin.Value = true;
                Instance.logger.LogInfo($">>> MonsterWin is: {monsterWin}");
                HUDManagerPatch.hudImpWin = true;
                PTestPlugin.roundOverTime -= Time.deltaTime;
                PTestPlugin.RoleWin();
            }
            else if (data == "crewWin")
            {
                HUDManager.Instance.ShowPlayersFiredScreen(show: true);
                PTestPlugin.gameStarted.Value = false;
                PTestPlugin.crewWin.Value = true;
                HUDManagerPatch.hudCrewWin = true;
                PTestPlugin.roundOverTime -= Time.deltaTime;
                PTestPlugin.RoleWin();
            }
            else if (data.Contains("vote/"))
            {
                string votedPlayer = data.Split('/')[1];

                if (votedPlayer.ToLower().Contains("skip"))
                {
                    skipVotes += 1;
                    return;
                }

                foreach (string c in allColors.Keys)
                {
                    if (c == votedPlayer)
                    {
                        allColors[votedPlayer] += 1; // later
                        break;
                    }
                }
            }
            else if (data == "Section1votetime")
            {
                Section0inMeeting.Value = true;
                Section1votetime.Value = true;
                Section2ejectTime.Value = false; // intended


                // Show time left for meeting
                HUDManager.Instance.tipsPanelBody.text = PTestPlugin.votetime.ToString();
                PTestPlugin.votetime -= Time.deltaTime;

                // Show popup near 20 seconds mark
                if (PTestPlugin.votetime <= 20f && PTestPlugin.votetime >= 18f)
                {
                    HUDManager.Instance.DisplayTip("Meeting Time", $"{PTestPlugin.votetime}s remaining", true);
                }

            }
            else if (data == "Section2ejectTime")
            {
                Section2ejectTime.Value = true;

                if (PTestPlugin.skipVotes >= PlayerControllerBPatch.tempMaxValue)
                {
                    HUDManager.Instance.DisplayTip("Skipping Eject.", "", false);
                }
                // - - - - Reset - - - -  (other reset in atInMeeting)//
                PlayerControllerBPatch.notsafe = false;
                PlayerControllerBPatch.ranSection1votetime = false;
                PlayerControllerBPatch.tempMaxValue = 0;
                PTestPlugin.canVote = true;
                PTestPlugin.inMeeting.Value = false;


                RoundManager.Instance.playersManager.shipDoorsAnimator.SetBool("Closed", value: false);
                UnityEngine.Object.FindObjectOfType<HangarShipDoor>().SetDoorButtonsEnabled(doorButtonsEnabled: true);

            }
            else if (data == "Section3ejectSeries")
            {
                Section3ejectSeries.Value = true;
            }
            else if (data == "Section4alarmPart")
            {
                PTestPlugin.Instance.logger.LogInfo(">>> Alarm Part (server)");
                Section4alarmPart.Value = true;
            }
            else if (data == "Section5orbitPart")
            {
                PTestPlugin.Instance.logger.LogInfo(">>> Orbit Part (server)");
                Section5orbitPart.Value = true;
            }
            else if (data == "Section6suckingPart")
            {
                PTestPlugin.Instance.logger.LogInfo(">>> suckingPart2 (server)");
                Section6suckingPart.Value = true;
            }
            else if (data == "Section7ejectedScreenPart")
            {
                Section7ejectedScreenPart.Value = true;
            }
            else if (data == "Section8showStarsPart")
            {
                Section8showStarsPart.Value = true;
            }
            else if (data.Contains("notsafe/"))
            {

                string rawColor = data.Split('/')[1];
                string votedColor = $"{PTestPlugin.playerClientID}".Replace("0", "red").Replace("1", "blue").Replace("2", "green").Replace("3", "pink").Replace("4", "orange").Replace("5", "yellow").Replace("6", "black").Replace("7", "white").Replace("8", "purple").Replace("9", "cyan");

                if (rawColor.ToLower() == votedColor.ToLower())
                {
                    PlayerControllerBPatch.notsafe = true;
                }


            }
            else if (data == "canEnter")
            {
                canEnter.Value = true;
            }
            else if (data == "roundedBool")
            {
                RoundManagerPatch.roundLoaded = true;
            }
            else if (data == "tpall")
            {
                GameNetworkManager.Instance.localPlayerController.TeleportPlayer(PTestPlugin.playerSpawnPositions[GameNetworkManager.Instance.localPlayerController.playerClientId].position);
            }
            else if (data == "resetSucking")
            {
                PlayerControllerBPatch.resetSucking = true;
            }
            else if (data.Contains("spweapon/"))
            {
                string impID = data.Split('/')[1];
                if (PTestPlugin.playerClientID == 0)
                {
                    PTestPlugin.SpawnScrapFunc("spwscr yieldsign", $"{impID}");
                }
            }

            if (data == "endgame")
            {
                //HUDManager.Instance.ShowPlayersFiredScreen(show: false);
                //PTestPlugin.DefaultValues();
            }

        }
        private static void ReceiveFromClient(string data, ulong id)
        {


            Instance.logger.LogInfo(">>> Message from client recieved!" + data + "CLIENT ID: " + id);
            StartOfRound.Instance.ResetShip();


        }
        private static void ReceiveByServer(string data, ulong id)
        {
            if (data.Contains("killed/crew/"))
            {
                string colorName = data.Replace("killed/crew/", "");
                PTestPlugin.listOfCrew.Remove(colorName);
                PTestPlugin.Instance.logger.LogInfo($">>> colorName K: {colorName}");
            }
            else if (data.Contains("killed/monster/"))
            {
                string colorName = data.Replace("killed/monster/", "");
                PTestPlugin.listOfMonsters.Remove(colorName);
                PTestPlugin.Instance.logger.LogInfo($">>> colorName Killed: {colorName}");
            }
            else if (data == "callMeeting")
            {
                PTestPlugin.inMeeting.Value = true;
            }
            else if (data == "cleanCrew")
            {
                foreach (PlayerControllerB plr in StartOfRound.Instance.allPlayerScripts)
                {
                    plr.ResetPlayerBloodObjects();
                }
            }
            else if (data == "bodyReport")
            {
                PTestPlugin.Instance.logger.LogInfo($">>> Reported Body");
                //HUDManager.Instance.UIAudio.PlayOneShot(PTestPlugin.SoundFX[0], 4f);
                inMeeting.Value = true;
                if (playerClientID == 0)
                {
                    PlayerControllerBPatch.DespawnBodies();
                }
            }
            else if (data.Contains("spawnImp/"))
            {
                if (playerClientID == 0)
                {
                    //PTestPlugin.Instance.logger.LogInfo($">>> Name is: {data}");
                    PTestPlugin.SpawnImposter(data);
                    //PlayerControllerBPatch.DespawnBodies();
                }
            }
            else if (data.Contains("killMonster/"))
            {
                PTestPlugin.killMonster.Value = $"true/{data.Replace("killMonster/", "")}";
                PTestPlugin.globalPlayerImp.Value = "none";
            }
            else if (data.Contains("globalImp/"))
            {
                PTestPlugin.globalPlayerImp.Value = data;
            }
            else if (data.Contains("globalImp2/"))
            {
                PTestPlugin.globalPlayerImp2.Value = data;
            }
            else if (data.Contains("switchSuit/"))
            {
                string[] keys = data.Split('/');
                string currentPlrId = keys[1];
                int currentSuitID = int.Parse(keys[2]);
                if (currentPlrId == $"{GameNetworkManager.Instance.localPlayerController.playerClientId}")
                {
                    UnlockableSuit.SwitchSuitForPlayer(GameNetworkManager.Instance.localPlayerController, currentSuitID);
                }
            }
            else if (data.Contains("vote/"))
            {
                string votedPlayer = data.Split('/')[1];

                if (votedPlayer.ToLower().Contains("skip"))
                {
                    skipVotes += 1;
                    return;
                }
                // later
                foreach (string c in allColors.Keys)
                {
                    if (c == votedPlayer)
                    {
                        allColors[votedPlayer] += 1;
                        break;
                    }
                }
            }
            else if (data == "egame")
            {
                //Roles.Value.Clear();
                ResetVariables();
            }
            else if (data.Contains("spweapon/"))
            {
                if (PTestPlugin.playerClientID == 0)
                {
                    customServerMessage.SendAllClients(data);
                }
            }

        }



        public static void atSherifRole(ulong data)
        {
            Instance.logger.LogInfo($">>> TOP OF EVENT");
            if (data == 404) return;

            // Host is Sherif
            if (data == 999 && playerClientID == 0)
            {
                hostSherifRole();
                return;
            }


            // Client is sherif
            //string name = data.GetPlayerController().name.ToLower();
            Instance.logger.LogInfo($">>> CLIENT ID IS: {data}");

            if (playerClientID == data)
            {
                Instance.logger.LogInfo(">>> CLIENT is sherif!");
                HUDManager.Instance.DisplayTip("You Are The Manager!", "Use your weapon to kill the monster. If you kill an Employee you'll both die.", false);

            }
            if (playerClientID == 0)
            {
                if (Gamemode.Value == "normal")
                {
                    string name = data.GetPlayerController().name.ToLower();
                    PTestPlugin.SpawnEnemyNut(name);
                }
                else
                {

                }
            }

        }

        public static void hostSherifRole()
        {
            string name = playerClientID.GetPlayerController().name.ToLower();
            Instance.logger.LogInfo($">>> HOST CLIENT ID IS: {playerClientID}");

            Instance.logger.LogInfo(">>> Host is sherif!");
            HUDManager.Instance.DisplayTip("You Are The Manager!", "Use your weapon to kill the monster. If you kill an Employee you'll both die.", false);
            if (Gamemode.Value == "normal")
            {
                PTestPlugin.SpawnEnemyNut(name);
            }
            else
            {

            }

        }








        public static void atMonsterRole(ulong data)
        {
            Instance.logger.LogInfo($">>> TOP OF MONSTER EVENT");
            if (data == 404) return;
            Instance.logger.LogInfo($">>> BELOW MONSTER EVENT");
            string colorName = $"{data}".Replace("999", "red").Replace("1", "blue").Replace("2", "green").Replace("3", "pink").Replace("4", "orange").Replace("5", "yellow").Replace("6", "black").Replace("7", "white").Replace("8", "purple").Replace("9", "cyan");


            if (!listOfMonsters.Contains(colorName))
            {
                listOfMonsters.Add(colorName);
                if (playerClientID == 0)
                {
                    //spawnLocation("imp");
                }
            }


            // Host is Monster
            if (data == 999 && playerClientID == 0)
            {
                monster = true;
                hostMonsterRole();
                return;
            }

            // Client is Monster
            //string name = data.GetPlayerController().name.ToLower();
            Instance.logger.LogInfo($">>> DATA ID IS: {data}");
            Instance.logger.LogInfo($">>> UNITY ID THING IS: {GameNetworkManager.Instance.localPlayerController.playerClientId}");
            Instance.logger.LogInfo($">>> Playerclient ID IS: {playerClientID}");

            if (GameNetworkManager.Instance.localPlayerController.playerClientId == data)
            {
                monster = true;
                Instance.logger.LogInfo(">>> CLIENT is MONSTER!");
                HUDManager.Instance.DisplayTip("You Are A Monster!", "Kill all the employees", true);
            }
            if (playerClientID == 0)
            {
                if (Gamemode.Value == "normal")
                {
                    ulong nameID = data.GetPlayerController().playerClientId;
                    PTestPlugin.SpawnScrapFunc("spwscr yieldsign", $"{nameID}");
                }
                else
                {
                    //string name = data.GetPlayerController().name.ToLower();
                    chosenMonster.Value = randomMonsterSelect();
                }
            }

        }

        public static void hostMonsterRole()
        {
            Instance.logger.LogInfo($">>> HOST CLIENT ID IS: {playerClientID}");

            Instance.logger.LogInfo(">>> Host is MONSTER!");
            HUDManager.Instance.DisplayTip("You Are A Monster!", "Kill all the employees", true);

            if (Gamemode.Value == "normal")
            {
                PTestPlugin.SpawnScrapFunc("spwscr yieldsign", $"{playerClientID}");
            }
            else
            {
                //string name = data.GetPlayerController().name.ToLower();
                chosenMonster.Value = randomMonsterSelect();
            }

        }







        public static void atMonsterRole2(ulong data)
        {
            Instance.logger.LogInfo($">>> TOP OF MONSTER EVENT(2)");
            if (data == 404) return;
            string colorName = $"{data}".Replace("999", "red").Replace("1", "blue").Replace("2", "green").Replace("3", "pink").Replace("4", "orange").Replace("5", "yellow").Replace("6", "black").Replace("7", "white").Replace("8", "purple").Replace("9", "cyan");

            if (!listOfMonsters.Contains(colorName))
            {
                listOfMonsters.Add(colorName);
                if (playerClientID == 0)
                {
                    //spawnLocation("imp");
                }
            }


            // Host is Monster
            if (data == 999 && playerClientID == 0)
            {
                monster = true;
                hostMonsterRole();
                return;
            }

            // Client is Monster
            //string name = data.GetPlayerController().name.ToLower();
            Instance.logger.LogInfo($">>> DATA ID IS: {data}");
            Instance.logger.LogInfo($">>> UNITY ID THING IS: {GameNetworkManager.Instance.localPlayerController.playerClientId}");
            Instance.logger.LogInfo($">>> Playerclient ID IS: {playerClientID}");

            if (GameNetworkManager.Instance.localPlayerController.playerClientId == data)
            {
                monster = true;
                Instance.logger.LogInfo(">>> CLIENT is MONSTER!");
                HUDManager.Instance.DisplayTip("You Are A Monster!", "Kill all the employees", true);
            }
            if (playerClientID == 0)
            {
                if (Gamemode.Value == "normal")
                {
                    ulong nameID = data.GetPlayerController().playerClientId;
                    PTestPlugin.SpawnScrapFunc("spwscr yieldsign", $"{nameID}");
                }
                else
                {
                    //string name = data.GetPlayerController().name.ToLower();
                    chosenMonster.Value = randomMonsterSelect();
                }
            }

        }







        public static void atCrewRole(ulong data)
        {
            Instance.logger.LogInfo($">>> TOP OF CREW EVENT");
            if (data == 404) return;
            if (data != 999) return;

            Instance.logger.LogInfo($">>> CLIENT ID IS: {data}");

            if (playerList.Contains(playerClientID))
            {
                Instance.logger.LogInfo(">>> CLIENT is CREW!");
                HUDManager.Instance.DisplayTip("You Are An Employee!", "Find the Manager and kill all the Employees.", true);
            }

        }


        public static void atLightsOn(bool data)
        {
            GameObject lght = GameObject.Find("ShipElectricLights").gameObject;
            Quaternion rot = Quaternion.Euler(270, 0, 0);
            // Weapons Hallway
            Instantiate(lght, new Vector3(23.26f, -143.33f, 142f), rot);
            Instantiate(lght, new Vector3(23.26f, -143.33f, 150f), rot);
            // Medbay Hallway
            Instantiate(lght, new Vector3(-40.4f, -143.33f, 142f), rot);
            Instantiate(lght, new Vector3(-40.4f, -143.33f, 150f), rot);
            // Admin Hallway
            Instantiate(lght, new Vector3(-12.27f, -143.33f, 112f), rot);
            Instantiate(lght, new Vector3(-5.47f, -143.33f, 150f), rot);
        }




        public static void atShipInside(bool data)
        {
            GameObject inShi = GameObject.Find("ShipInside").gameObject;
            Quaternion rot = Quaternion.Euler(270, 0, 0);

            Instantiate(inShi, new Vector3(1.2f, 50f, 1.2f), rot); // player pos: 0f, 52y, 0f
            Instantiate(inShi, new Vector3(1.2f, 100f, 1.2f), rot); // player pos: 0f, 102y, 0f
            Instantiate(inShi, new Vector3(1.2f, 150f, 1.2f), rot);
            Instantiate(inShi, new Vector3(1.2f, 200f, 1.2f), rot);
            Instantiate(inShi, new Vector3(1.2f, 250f, 1.2f), rot);
            Instantiate(inShi, new Vector3(1.2f, 300f, 1.2f), rot);
            Instantiate(inShi, new Vector3(1.2f, 350f, 1.2f), rot);
            Instantiate(inShi, new Vector3(1.2f, 400f, 1.2f), rot);
        }







        private static readonly LethalServerMessage<GameObject> dropServerMessage = new LethalServerMessage<GameObject>(identifier: "dropitem", DropReceiveByServer);
        private static readonly LethalClientMessage<GameObject> dropClientMessage = new LethalClientMessage<GameObject>(identifier: "dropitem", DropReceiveFromServer, DropReceiveFromClient);


        private static void DropReceiveFromServer(GameObject item)
        {
            PTestPlugin.Instance.logger.LogInfo(">>> Item is: " + item.name);
            //item.transform.gameObject.SetActive(false);

            //Destroy(item);
            if (playerClientID == 0)
            {
                Destroy(item);
            }
        }
        private static void DropReceiveFromClient(GameObject item, ulong id)
        {
            PTestPlugin.Instance.logger.LogInfo(">>> Message from client recieved! " + item + "CLIENT ID: " + id);
        }
        private static void DropReceiveByServer(GameObject data, ulong id)
        {
            PTestPlugin.Instance.logger.LogInfo($">>> Message by server recieved!  {data}");
        }


        public static void atGrabItem(GameObject item)
        {
            Instance.logger.LogInfo(">>> DROPPED ITEM!");
            if (item == null) return;
            if (playerClientID != 0) return;

            //dropServerMessage.SendAllClients(item);
        }




        public static void atInMeeting(bool data)
        {
            if (data)
            {
                // Add timer to test
                PTestPlugin.votetime = PTestPlugin.defaultVotetime;
                PTestPlugin.inMeetingCooldown = PTestPlugin.defaultCooldown;
                ShipAlarmCordPatch.reportedBody = false;

                HUDManager.Instance.DisplayTip("Meeting Time", $"{PTestPlugin.votetime}s remaining", false);

                //RoundManager.Instance.playersManager.shipDoorsAnimator.SetBool("Closed", value: true);
                //UnityEngine.Object.FindObjectOfType<HangarShipDoor>().SetDoorButtonsEnabled(doorButtonsEnabled: false);

                // Disable the Map
                PTestPlugin.RemoveEnvironment();
                //PTestPlugin.DisableMonsters();
                GameNetworkManager.Instance.localPlayerController.TeleportPlayer(playerSpawnPositions[GameNetworkManager.Instance.localPlayerController.playerClientId].position);

            }
            else
            {
                // --- Reset ---- //

                PTestPlugin.votetime = 0;
                PTestPlugin.votes.Value = 0;
                PTestPlugin.ejectTime = 30f;
                skipVotes = 0;
                alarmPart = true;
                orbitPart = true;
                suckingPart = true;
                ejectedScreenPart = true;
                showStarsPart = true;
                fixShipPart = true;
                allColors["red"] = 0;
                allColors["blue"] = 0;
                allColors["green"] = 0;
                allColors["pink"] = 0;
                allColors["orange"] = 0;
                allColors["yellow"] = 0;
                allColors["black"] = 0;
                allColors["white"] = 0;
                allColors["purple"] = 0;
                allColors["cyan"] = 0;
                PTestPlugin.Section0inMeeting.Value = false;
                PTestPlugin.Section1votetime.Value = false;
                //PTestPlugin.Section2ejectTime.Value = false;
                PTestPlugin.Section3ejectSeries.Value = false;
                PTestPlugin.Section4alarmPart.Value = false;
                PTestPlugin.Section5orbitPart.Value = false;
                PTestPlugin.Section6suckingPart.Value = false;
                PTestPlugin.Section7ejectedScreenPart.Value = false;
                PTestPlugin.Section8showStarsPart.Value = false;


                Instance.logger.LogInfo($">>> IN RESET atInMeeting || Value for inMeeting: {inMeeting.Value} ");
                // Enable Monsters again
                //PTestPlugin.EnableMonsters();

                //RoundManager.Instance.playersManager.shipDoorsAnimator.SetBool("Closed", value: false);
                //UnityEngine.Object.FindObjectOfType<HangarShipDoor>().SetDoorButtonsEnabled(doorButtonsEnabled: true);

                // Enable the Map
                PTestPlugin.RemoveEnvironment(false);

                // ---- Reset Stuff ---- //
                PTestPlugin.fixShipPart = false;
                RoundManager.Instance.playersManager.shipAnimatorObject.gameObject.GetComponent<Animator>().SetBool("AlarmRinging", value: false);
                GameNetworkManager.Instance.localPlayerController.TeleportPlayer(playerSpawnPositions[GameNetworkManager.Instance.localPlayerController.playerClientId].position);
                GameNetworkManager.Instance.localPlayerController.inSpecialInteractAnimation = false;
                RoundManager.Instance.playersManager.shipDoorsAnimator.SetBool("OpenInOrbit", value: false);
                RoundManager.Instance.playersManager.currentPlanetPrefab.transform.position = RoundManager.Instance.playersManager.planetContainer.transform.position;
                //suckingPlayersOutOfShip = false;
                //choseRandomFlyDirForPlayer = false;
                //suckingPower = 0f;
                RoundManager.Instance.playersManager.shipRoomLights.SetShipLightsOnLocalClientOnly(setLightsOn: true);
                HUDManager.Instance.ShowPlayersFiredScreen(show: false);


                if (playerClientID == 0)
                {
                    Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                    if (terminal != null)
                    {
                        terminal.groupCredits = 100;
                    }
                }
            }

        }



        public static void cDown(float num)
        {
            foreach (PlayerControllerB p in StartOfRound.Instance.allPlayerScripts)
            {
                if (monster == false)
                {
                    PlayerControllerBPatch.playerInScene($"{p.playerClientId}", false);
                    if (p.playerClientId == 0)
                    {
                        // GameNetworkManager.Instance.localPlayerController.TeleportPlayer()
                        p.transform.position = new Vector3(21.4f, 0.3f, -14.8f);
                        p.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }

                    if (p.playerClientId == 1)
                    {
                        p.transform.position = new Vector3(41.4f, 0.3f, -14.8f);
                        p.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }

                    if (p.playerClientId == 2)
                    {
                        p.transform.position = new Vector3(-1.4f, 0.3f, 21.8f);
                        p.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }

                    if (p.playerClientId == 3)
                    {
                        p.transform.position = new Vector3(-81.4f, 0.3f, 34.8f);
                        p.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }

                    if (p.playerClientId == 4)
                    {
                        p.transform.position = new Vector3(-1.4f, 0.3f, 54.8f);
                        p.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }

                    if (p.playerClientId == 5)
                    {
                        p.transform.position = new Vector3(-21.4f, 0.3f, -14.8f);
                        p.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }

                    if (p.playerClientId == 6)
                    {
                        p.transform.position = new Vector3(-41.4f, 0.3f, -14.8f);
                        p.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }

                    if (p.playerClientId == 7)
                    {
                        p.transform.position = new Vector3(-20.4f, 0.3f, -34.8f);
                        p.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }

                    if (p.playerClientId == 8)
                    {
                        p.transform.position = new Vector3(-1.4f, 0.3f, -54.8f);
                        p.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }

                    if (p.playerClientId == 9)
                    {
                        p.transform.position = new Vector3(-1.4f, 0.3f, -14.8f);
                        p.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }

                }
            }

            DissonanceCommsPatch.gracePeriodMuted = true;

            if (num <= 0f && gpOver == false)
            {
                gpOver = true;
                gracePeriod.Value = false;
                Instance.logger.LogInfo($">>> woooo reee in grace if");
                DissonanceCommsPatch.gracePeriodMuted = false;
            }
        }


        public static void atGracePeriod(bool data)
        {
            Instance.logger.LogInfo(">>> grace evevnt");
            if (data)
            {
                if (gpStart)
                {
                    //HUDManager.Instance.Display
                    //HUDManager.DisplayGlobalNotification("New creature data sent to terminal!");
                    gpStart = false;
                    Instance.logger.LogInfo(">>> gpStaer");
                }
            }
            else if (data == false)
            {
                Instance.logger.LogInfo(">>> data is now false");
                DissonanceCommsPatch.gracePeriodMuted = false;
                GameNetworkManager.Instance.localPlayerController.TeleportPlayer(playerSpawnPositions[GameNetworkManager.Instance.localPlayerController.playerClientId].position);
                //DespawnSpecificPrefabs();


                foreach (PlayerControllerB p in StartOfRound.Instance.allPlayerScripts)
                {
                    PlayerControllerBPatch.playerInScene($"{p.playerClientId}", true);
                }


                if (chosenMonster.Value != null && monster && Gamemode.Value != "normal")
                {
                    HUDManager.Instance.DisplayTip($"You Are A {chosenMonster.Value.Replace("flowerman", "Bracken").Replace("blob", "Slime").Replace("crawler", "Thumper").Replace("spider", "Spider")}", "Press 8 to transform when you're ready", false);
                }
            }

        }




        public static readonly LethalServerMessage<bool> doorServerMessage = new LethalServerMessage<bool>(identifier: "door", doorReceiveByServer);
        public static readonly LethalClientMessage<bool> doorClientMessage = new LethalClientMessage<bool>(identifier: "door", doorReceiveFromServer, doorReceiveFromClient);


        private static void doorReceiveFromServer(bool data)
        {
            // Update Taskbar (Quota)
            TimeOfDay.Instance.quotaFulfilled = PTestPlugin.userfinishedTasks;
            //PTestPlugin.DisableMonsters();


            // Door closed
            if (data && ejectTime == 30f)
            {
                RoundManager.Instance.playersManager.shipDoorsAnimator.SetBool("Closed", value: true);
                UnityEngine.Object.FindObjectOfType<HangarShipDoor>().SetDoorButtonsEnabled(doorButtonsEnabled: false);
            }
            if (data == false)
            {
                //PTestPlugin.RemoveEnvironment(false);
                RoundManager.Instance.playersManager.shipDoorsAnimator.SetBool("Closed", value: false);
                UnityEngine.Object.FindObjectOfType<HangarShipDoor>().SetDoorButtonsEnabled(doorButtonsEnabled: true);
            }

            if (inMeeting.Value)
            {
                // Ejecting player
                if (PTestPlugin.ejectTime > 25.70f)
                {
                    RoundManager.Instance.playersManager.shipDoorsAnimator.SetBool("Closed", value: true);
                    UnityEngine.Object.FindObjectOfType<HangarShipDoor>().SetDoorButtonsEnabled(doorButtonsEnabled: false);
                    //___doorPower = PTestPlugin.votetime * 1.8f;
                    return;
                }

                if (PTestPlugin.ejectTime <= 25.63f && PTestPlugin.suckingPart)
                {
                    RoundManager.Instance.playersManager.shipDoorsAnimator.SetBool("Closed", value: false);
                    //___doorPower = 0f;
                    return;
                }
            }


        }
        private static void doorReceiveFromClient(bool item, ulong id)
        {
            PTestPlugin.Instance.logger.LogInfo(">>> Message from client recieved! " + item + "CLIENT ID: " + id);
        }
        private static void doorReceiveByServer(bool data, ulong id)
        {
            PTestPlugin.Instance.logger.LogInfo($">>> Message by server recieved!  {data}");
        }





        public static void atGiveEveryoneTheirSuits(bool data)
        {
            Instance.logger.LogInfo(">>> In Suits Global");


            foreach (ulong pd in PlayerIDToSuitID.Keys)
            {

                Material material = StartOfRound.Instance.unlockablesList.unlockables[PlayerIDToSuitID[pd]].suitMaterial;
                PlayerControllerB playerControllerB = StartOfRound.Instance.allPlayerScripts[pd];

                playerControllerB.thisPlayerModel.material = material;
                playerControllerB.thisPlayerModelLOD1.material = material;
                playerControllerB.thisPlayerModelLOD2.material = material;
                playerControllerB.thisPlayerModelArms.material = material;
                playerControllerB.currentSuitID = PlayerIDToSuitID[pd];

            }

        }
    }
}