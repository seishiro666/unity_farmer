using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] UserData userData;

    [SerializeField] GameObject soundBtn;
    [SerializeField] GameObject musicBtn;
    [SerializeField] GameObject soundObj;
    [SerializeField] GameObject musicObj;
    [SerializeField] GameObject moneyObj;
    [SerializeField] GameObject lvlObj;

    BtnSettingController soundBtnScript;
    BtnSettingController musicBtnScript;
    Slider soundSlider;
    Slider musicSlider;
    TMP_Text moneyText;
    Slider lvlSlider;
    TMP_Text userLvlText;

    public bool musicOn, soundOn;
    public float musicValue, soundValue, lvlProgress;
    public int money, lvl;

    void Awake()
    {
        soundBtnScript = soundBtn.GetComponent<BtnSettingController>();
        musicBtnScript = musicBtn.GetComponent<BtnSettingController>();
        soundSlider = soundObj.transform.GetChild(1).GetComponent<Slider>();
        musicSlider = musicObj.transform.GetChild(1).GetComponent<Slider>();
        moneyText = moneyObj.transform.GetChild(2).GetComponent<TMP_Text>();
        lvlSlider = lvlObj.transform.GetChild(0).GetComponent<Slider>();
        userLvlText = lvlObj.transform.GetChild(1).transform.GetChild(0).GetComponent<TMP_Text>();

        UpdateUserUI();
    }

    public void SaveData()
    {
        userData.musicOn = musicBtnScript.isOn;
        userData.soundOn = soundBtnScript.isOn;
        userData.musicValue = musicSlider.value;
        userData.soundValue = soundSlider.value;
        userData.money = int.Parse(moneyText.text);
        userData.lvl = int.Parse(userLvlText.text);
        userData.lvlProgress = lvlSlider.value;

        UpdateUserUI();
    }

    void UpdateUserUI()
    {
        musicBtnScript.isOn = userData.musicOn;
        musicBtnScript.SwapSetting();
        soundBtnScript.isOn = userData.soundOn;
        soundBtnScript.SwapSetting();
        musicSlider.value = userData.musicValue;
        soundSlider.value = userData.soundValue;
        moneyText.text = userData.money.ToString();
        lvlSlider.value = userData.lvlProgress;
        userLvlText.text = userData.lvl.ToString();

        SetData();
    }

    void SetData()
    {
        musicOn = userData.musicOn;
        soundOn = userData.soundOn;
        musicValue = userData.musicValue;
        soundValue = userData.soundValue;
        money = userData.money;
        lvl = userData.lvl;
        lvlProgress = userData.lvlProgress;
    }
}