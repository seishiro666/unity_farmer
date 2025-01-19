using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFlower", menuName = "ScriptableObject/Flower")]
public class FlowerData : ScriptableObject
{
    public string flowerName;
    public Sprite icon;
    public Sprite seedIcon;
    public GameObject model;
    public float seedGrowth;
    public float growth;
    public List<FlowerData> potentialSeeds;
    public int price;
    public float expReward;
}
