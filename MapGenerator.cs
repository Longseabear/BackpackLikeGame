using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Inst { get; private set; }
    void Awake() => Inst = this;

    public Transform tilePrefab;

    public BoardProperties initialBoardProperties;
    public GridBoard boardModel;

    void Start()
    {
        GenerateMap();
        Inst = this;
    }

    public void GenerateMap() {
        Inst = this;
        boardModel.Load(initialBoardProperties);
        boardModel.holder.parent = transform;
    }
    void Update()
    {
        
    }
}
