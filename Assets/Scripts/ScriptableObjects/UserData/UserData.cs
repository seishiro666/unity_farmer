using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "UserData", menuName = "ScriptableObject/UserData")]
public class UserData : ScriptableObject
{
    public bool musicOn = true;
    public bool soundOn = true;
    public float musicValue = 0.5f;
    public float soundValue = 0.5f;
    public int money = 150;
    public int lvl = 1;
    public float lvlProgress = 0f;
}
