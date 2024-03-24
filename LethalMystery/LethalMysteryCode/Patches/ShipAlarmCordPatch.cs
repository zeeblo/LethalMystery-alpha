using HarmonyLib;

namespace PTest.Patches
{
    [HarmonyPatch(typeof(ShipAlarmCord))]
    internal class ShipAlarmCordPatch
    {
        public static bool calledMeetingOnce = false;
        public static bool reportedBody = false;

        [HarmonyPatch(typeof(ShipAlarmCord), nameof(ShipAlarmCord.StopHorn))]
        [HarmonyPostfix]
        static void CallAMeeting()
        {
            if ( (!PTestPlugin.gameStarted.Value || PTestPlugin.inMeeting.Value || PTestPlugin.inMeetingCooldown > 0f) ) return;
            if (calledMeetingOnce && reportedBody == false) return;
            // true, false (returns) | true, true, (calls) | false, false (calls) | false, true (calls)

            PTestPlugin.customClientMessage.SendServer("callMeeting");
            calledMeetingOnce = true;
            reportedBody = false;
        }


    }
}