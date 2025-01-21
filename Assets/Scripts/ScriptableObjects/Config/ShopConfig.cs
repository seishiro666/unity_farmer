using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewConfig", menuName = "ScriptableObject/Config")]
public class ShopConfig : ScriptableObject
{
    public List<FlowerData> shopConfigData;
}
