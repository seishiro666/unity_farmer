using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnSettingController : MonoBehaviour
{
    public GameObject settingOn;
    public GameObject settingOff;

    public bool isOn = true;

    public void SwapSetting()
    {
        if (isOn) {
            settingOn.SetActive(true);
            settingOff.SetActive(false);
            isOn = false;
        }
        else
        {
            settingOn.SetActive(false);
            settingOff.SetActive(true);
            isOn = true;
        }
    }
}
