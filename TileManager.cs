using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static InteractiveUtility;
using static UnityEngine.GraphicsBuffer;

public enum TileType
{
    Inactive, Grass
}

[Serializable]
public struct TileProperties
{
    public Coordinate position; // normalized coordinate
    public TileType type;
}

[Serializable]
public class TilePrefab
{
    public TileType tileType;
    public GameObject tilePrefab;
}

[CreateAssetMenu(fileName = "TileData", menuName = "TileEditor/TileData", order = 1)]
public class TileData : ScriptableObject
{
    public TilePrefab[] prefabs;
}


public abstract class TileMouseEffector
{
    protected Tile target;

    public abstract void OnPointerEnter(RaycastHit hit);
    public abstract void OnPointerExit();

    public TileMouseEffector(Tile _target) {
        target = _target;
    }
}

/// <summary>
/// Simple effector for tile
/// </summary>
[Serializable]
public class DefaultTileEffector : TileMouseEffector
{
    public Color hitColor;
    private Color originalColor;

    public DefaultTileEffector(Tile _target, Color? _hitColor = null) : base(_target) {
        hitColor = _hitColor ?? Color.red;
    }

    public override void OnPointerEnter(RaycastHit hit) 
    {
        var renderer = target.GetComponent<Renderer>();
        originalColor = renderer.material.color;
        renderer.material.color = hitColor;
    }

    public override void OnPointerExit() {
        if(target != null) target.GetComponent<Renderer>().material.color = originalColor;
    }
}

public class TileManager : MonoBehaviour
{
    public static TileManager Inst { get; private set; }

    public TileMouseEffector GenerateTileEffector(Tile targetObject) {
        switch (targetObject.property.type) {
            default:
                return new DefaultTileEffector(targetObject);
        }
    }

    void Awake() {
        Inst = this;
    }

    [SerializeField]
    public TileData tileDatas;
    
    public Transform GetTilePrefabs(TileType type) {
        for (int i = 0; i < tileDatas.prefabs.Length; i++) if (tileDatas.prefabs[i].tileType == type) return tileDatas.prefabs[i].tilePrefab.transform;
        return null;
    }
}
