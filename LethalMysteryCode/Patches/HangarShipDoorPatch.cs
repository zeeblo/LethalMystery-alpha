using HarmonyLib;

namespace PTest.Patches
{
    internal class HangarShipDoorPatch
    {
        [HarmonyPatch(typeof(HangarShipDoor), nameof(HangarShipDoor.Update))]
        [HarmonyPrefix]
        private static bool UpdatePatch()
        {
            if (PTestPlugin.inMeeting.Value)
            {
                if (PTestPlugin.playerClientID == 0)
                {
                    PTestPlugin.doorServerMessage.SendAllClients(true);
                }


                //___doorPower = PTestPlugin.votetime * 1.8f;

                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
