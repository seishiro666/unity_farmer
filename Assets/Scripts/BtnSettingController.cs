using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnSettingController : MonoBehaviour
{
    [SerializeField] GameObject settingOn;
    [SerializeField] GameObject settingOff;
    [SerializeField] PlayerController controller;
    public bool isOn = true;

    public void SwapSetting()
    {
        if (isOn) {
            settingOn.SetActive(false);
            settingOff.SetActive(true);
            isOn = false;
        }
        else
        {
            settingOn.SetActive(true);
            settingOff.SetActive(false);
            isOn = true;
        }

        controller.SaveData();
    }
}
