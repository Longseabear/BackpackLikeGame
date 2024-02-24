using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using static InteractiveUtility;
using static UnityEngine.GraphicsBuffer;


public class Tile : MonoBehaviour, IMouseInteractive
{
    public TileProperties property;
    public TileMouseEffector effector;

    public string buildingName;

    private void Start() {
        effector = TileManager.Inst.GenerateTileEffector(this);
    }

    void ColorChanger(Coordinate coord, Building building) {
        if(MapGenerator.Inst.boardModel.GlobalToBoardCoordinate(new Vector2(transform.position.x, transform.position.z)) == coord) {
            if (building == null) {
                buildingName = "Null";
            }
            else {
                buildingName = building.name;
            }
        }
    }
    void Update() {
//        MapGenerator.Inst.boardModel.OnBuildingUpdate += ColorChanger;
    }

    public void OnPointerEnter(RaycastHit hit) {
        effector.OnPointerEnter(hit);
    }

    public void OnPointerExit() {
        effector.OnPointerExit();
    }
}
