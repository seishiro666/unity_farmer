using UnityEngine;

public class BtnSettingController : MonoBehaviour
{
    [SerializeField] GameObject settingOn;
    [SerializeField] GameObject settingOff;
    [SerializeField] PlayerController controller;

    public bool isOn = true;

    private void Start()
    {
        UpdateState();
    }

    public void SwapSettings()
    {
        isOn = !isOn;
        controller.SaveData();
        UpdateState();
    }

    public void UpdateState()
    {
        settingOn.SetActive(isOn);
        settingOff.SetActive(!isOn);
    }
}
