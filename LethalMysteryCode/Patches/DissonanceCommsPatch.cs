using Dissonance;
using HarmonyLib;
using UnityEngine.InputSystem;


namespace PTest.Patches
{
    [HarmonyPatch(typeof(DissonanceComms))]
    internal class DissonanceCommsPatch
    {

        private static bool ranMuteOnce = false;
        public static bool gracePeriodMuted = false;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdatePatch(ref DissonanceComms __instance)
        {
            // Mute In grace period
            if (gracePeriodMuted)
            {
                __instance.RemoteVoiceVolume = 0f;
                IngamePlayerSettings.Instance.settings.micEnabled = false;
            }
            else if (gracePeriodMuted == false && PTestPlugin.muted.Value != true)
            {
                __instance.RemoteVoiceVolume = 1f;
                IngamePlayerSettings.Instance.settings.micEnabled = true;
            }

            // Muted Round Option (Unless meeting is called)
            if (PTestPlugin.gameStarted.Value && PTestPlugin.muted.Value)
            {
                if (PTestPlugin.inMeeting.Value == false && ranMuteOnce == false)
                {
                    PTestPlugin.Instance.logger.LogInfo(">>> disabled vc");
                    __instance.RemoteVoiceVolume = 0f;
                    IngamePlayerSettings.Instance.settings.micEnabled = false;
                    ranMuteOnce = true;
                }
                else if (PTestPlugin.inMeeting.Value && ranMuteOnce == true)
                {
                    PTestPlugin.Instance.logger.LogInfo(">>> enabled vc");
                    __instance.RemoteVoiceVolume = 1f;
                    IngamePlayerSettings.Instance.settings.micEnabled = true;
                    ranMuteOnce = false;
                }
            }
            else if (PTestPlugin.gameStarted.Value == false)
            {
                __instance.RemoteVoiceVolume = 1f;
                IngamePlayerSettings.Instance.settings.micEnabled = true;
            }


        }
        
    }
}
