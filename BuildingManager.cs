using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct BuildingProperties
{
    public string buildingName;
    public bool batched;
    public Coordinate coord;    
}

[Serializable]
public class BuildingPrefab
{
    public BuildingProperties property;
    public Transform buildingPrefab;
}

[CreateAssetMenu(fileName = "BuildingData", menuName = "BuildingEditor/BuildingData", order = 1)]
public class BuildingData : ScriptableObject
{
    public BuildingPrefab[] prefabs;
}

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Inst { get; private set; }
    void Awake() => Inst = this;

    private Dictionary<string, BuildingPrefab> nameToBuildingPrefab;

    void Start() {
        nameToBuildingPrefab = new Dictionary<string, BuildingPrefab>();
        foreach (BuildingPrefab bp in buildingDatas.prefabs) {
            nameToBuildingPrefab[bp.property.buildingName] = bp;
        }
    }

    [SerializeField]
    public BuildingData buildingDatas;
}
