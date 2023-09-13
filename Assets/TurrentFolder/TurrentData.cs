using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TurrentData", menuName = "pholisopherstone/TurrentData", order = 0)]
public class TurrentData : ScriptableObject {
    public string turrentName;
    public string description;
    public Sprite artillery_Base;
    public Sprite barrel;
    //public Sprite bullet;//future prefab
    public string targetTagName;
    public float range;
    public float fireRate;
    public float pulse;
}

