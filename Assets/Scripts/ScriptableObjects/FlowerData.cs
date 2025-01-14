using UnityEngine;

[CreateAssetMenu(fileName = "NewFlower", menuName = "Inventory/Flower")]
public class FlowerData : ScriptableObject
{
    public string flowerName;
    public Sprite icon;
    public GameObject model;
    public float seedGrowth;
    public float growth;
}
