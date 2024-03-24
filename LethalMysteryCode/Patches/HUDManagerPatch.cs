using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PTest.Patches
{




    [HarmonyPatch(typeof(HUDManager))]
    internal class HUDManagerPatch
    {
        public static bool hudCrewWin = false;
        public static bool hudImpWin = false;


        [HarmonyPatch(typeof(HUDManager), "DisplayNewScrapFound")]
        [HarmonyPrefix]
        static bool TasksPatch()
        {

            if (PTestPlugin.inMeeting.Value == true) return false;

            PTestPlugin.userfinishedTasks += 10;
            return true;
        }
       



        [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.DisplayTip))]
        [HarmonyPostfix]
        static void DisplayRolePatch(ref TextMeshProUGUI ___tipsPanelHeader)
        {
            if (___tipsPanelHeader.text.Contains("Sherif"))
            {
                ___tipsPanelHeader.color = Color.cyan;
            }
            else
            {
                ___tipsPanelHeader.color = new Color(0f, 0f, 0f, 1);
            }
        }



        [HarmonyPatch(typeof(HUDManager), "Update")]
        [HarmonyPostfix]
        static void PopupPatch(ref TextMeshProUGUI[] ___scanElementText)
        {
            if ((___scanElementText != null) && EntranceTeleportPatch.SkeldMap)
            {
                if (___scanElementText[0].text.Contains("metal"))
                {
                    ___scanElementText[0].text = "Vent";
                    ___scanElementText[1].text = "";
                }

                if (___scanElementText[0].text.Contains("engine"))
                {
                    ___scanElementText[0].text = "Exit Button";
                    ___scanElementText[1].text = "";
                }
            }
        }



        [HarmonyPatch(typeof(HUDManager), "Start")]
        [HarmonyPostfix]
        static void ButtonSFXPatch(ref AudioClip ___shipAlarmHornSFX)
        {
            //___shipAlarmHornSFX = PTestPlugin.SoundFX[1];
        }




        /*
        [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.DisplayDaysLeft))]
        [HarmonyPrefix]
        static bool GracePeriodPatch(ref TextMeshProUGUI ___profitQuotaDaysLeftText, ref TextMeshProUGUI ___profitQuotaDaysLeftText2, ref Animator ___reachedProfitQuotaAnimator, ref AudioSource ___UIAudio, ref AudioClip ___profitQuotaDaysLeftCalmSFX)
        {
            if (PTestPlugin.gracePeriod.Value)
            {
                ___profitQuotaDaysLeftText.text = "Grace Period";
                ___profitQuotaDaysLeftText2.text = "Teleporting soon...";
                ___reachedProfitQuotaAnimator.SetTrigger("displayDaysLeftCalm");
                ___UIAudio.PlayOneShot(___profitQuotaDaysLeftCalmSFX);
                return false;
            }
            else
            {
                return true;
            }
        }
        */

        [HarmonyPatch(typeof(HUDManager), "Update")]
        [HarmonyPostfix]
        private static void GameOverScreenPatch(ref Animator ___playersFiredAnimator)
        {
            if (___playersFiredAnimator == null) return;
            
            if (PTestPlugin.gameStarted.Value && PTestPlugin.gameEnded == false && PTestPlugin.inMeeting.Value)
            {
                // Find the "MaskImage" child object
                ___playersFiredAnimator.gameObject.transform.Find("MaskImage").Find("HeaderText").GetComponent<TextMeshProUGUI>().text = "Ejected";
                ___playersFiredAnimator.gameObject.transform.Find("MaskImage").Find("HeaderText (1)").GetComponent<TextMeshProUGUI>().text = "";
            }
            else if ( (PTestPlugin.crewWin.Value || hudCrewWin) && PlayerControllerBPatch.localhudCrewWin == false)
            {
                //PTestPlugin.crewWin.Value = false;
                hudCrewWin = false;
                HUDManager.Instance.ShowPlayersFiredScreen(show: true);
                ___playersFiredAnimator.gameObject.transform.Find("MaskImage").Find("HeaderText").GetComponent<TextMeshProUGUI>().text = "Employees won!";
                ___playersFiredAnimator.gameObject.transform.Find("MaskImage").Find("HeaderText (1)").GetComponent<TextMeshProUGUI>().text = "";
                //HUDManager.Instance.UIAudio.PlayOneShot(PTestPlugin.SoundFX[3], 1f);
            }
            else if ( (PTestPlugin.monsterWin.Value || hudImpWin) && PlayerControllerBPatch.localhudImpWin == false)
            {
                //PTestPlugin.monsterWin.Value = false;
                HUDManager.Instance.ShowPlayersFiredScreen(show: true);
                hudImpWin = false;
                ___playersFiredAnimator.gameObject.transform.Find("MaskImage").Find("HeaderText").GetComponent<TextMeshProUGUI>().text = $"Monsters won!";
                ___playersFiredAnimator.gameObject.transform.Find("MaskImage").Find("HeaderText (1)").GetComponent<TextMeshProUGUI>().text = "";
                //HUDManager.Instance.UIAudio.PlayOneShot(PTestPlugin.SoundFX[4], 1f);
            }

        }



    }
}
