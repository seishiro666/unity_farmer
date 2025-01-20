using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header ("Scripts")]
    [SerializeField] UserData userData;
    [SerializeField] BedSpawner bedSpawner;

    [Header ("UI")]
    [SerializeField] BtnSettingController soundBtnScript;
    [SerializeField] BtnSettingController musicBtnScript;
    [SerializeField] Slider soundSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] TMP_Text moneyText;
    [SerializeField] Slider lvlSlider;
    [SerializeField] TMP_Text userLvlText;

    public float lvlProgress;
    public int money, lvl;

    private void Awake()
    {
        LoadData();
        UpdateUserUI();
    }

    public void SaveData()
    {
        userData.musicOn = musicBtnScript.isOn;
        userData.soundOn = soundBtnScript.isOn;
        userData.musicValue = musicSlider.value;
        userData.soundValue = soundSlider.value;
        userData.money = money;
        userData.lvl = lvl;
        userData.lvlProgress = lvlProgress;
    }

    private void LoadData()
    {
        lvlProgress = userData.lvlProgress;
        money = userData.money;
        lvl = userData.lvl;

        soundBtnScript.isOn = userData.soundOn;
        musicBtnScript.isOn = userData.musicOn;
        soundBtnScript.UpdateState();
        musicBtnScript.UpdateState();
    }

    private void UpdateUserUI()
    {
        soundSlider.value = userData.soundValue;
        musicSlider.value = userData.musicValue;
        moneyText.text = userData.money.ToString();
        lvlSlider.value = userData.lvlProgress;
        userLvlText.text = userData.lvl.ToString();
    }

    public void AddExperience(float amount)
    {
        lvlProgress += amount;
        if (lvlProgress >= 1f)
        {
            lvlProgress -= 1f;
            lvl++;
            bedSpawner.SetupBedCount(lvl);
        }
        SaveData();
        UpdateUserUI();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        SaveData(); 
        UpdateUserUI();
    }
}
