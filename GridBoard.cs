using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// Meta data about map information
/// It contains only map state information and follows normalized map coordinates.
/// </summary>  
[Serializable]
public struct BoardProperties
{
    public List<BuildingProperties> buildings;
    public List<TileProperties> tiles;
}

public class GridBoardView : MonoBehaviour
{
    private void OnDrawGizmos() {
        Vector3 pos = Camera.current.transform.position;
        Debug.Log("OnDrawGizmos" + pos);
        //Gizmos.color = UnityEngine.Color.blue;

        //if (boardModel.maxWidth <= 0 || boardModel.maxHeight <= 0)
        //    return;

        //for (float y = pos.y - 540f; y < pos.y + 540f; y += boardModel.maxHeight) {
        //    Gizmos.DrawLine(new Vector3(10000000.0f, Mathf.Floor(y / boardModel.maxHeight) * boardModel.maxHeight, 0f),
        //    new Vector3(-10000000, Mathf.Floor(y / boardModel.maxHeight) * boardModel.maxHeight, 0f));
        //}

        //for (float x = pos.x - 540f; x < pos.x + 540f; x += boardModel.maxWidth) {
        //    Gizmos.DrawLine(new Vector3(Mathf.Floor(x / boardModel.maxWidth) * boardModel.maxWidth, 10000000, 0f),
        //    new Vector3(Mathf.Floor(x / boardModel.maxWidth) * boardModel.maxWidth, -10000000, 0f));
        //}
    }
}

[Serializable]
public class GridBoard
{
    // Cell structure
    [Serializable]
    public struct Cell
    {
        public bool active;
        public Transform tile;
        public Building building;

    }

    // TileSize
    public float tileSize;
    public int maxHeight, maxWidth;

    private TileManager tileManager;
    public Transform holder;
    public Vector3 boardPosition;

    // Event
    public event System.Action<Coordinate, Building> OnBuildingUpdate;


    [Serializable]
    public struct TileViewOption {
        [Range(0, 1)]
        public float tilePercentage;
    }

    public TileViewOption tileViewOption;

    public Coordinate mapCentre
    {
        get {
            return new Coordinate(maxWidth / 2, maxHeight / 2);
        }
    }

    private Cell[,] cells;

    public Cell? GetCellUsingGlobalPoint(Vector2 globalPosition) {
        var coordinate = GlobalToBoardCoordinate(globalPosition);
        if (coordinate.x < 0 || coordinate.x >= maxWidth || coordinate.y < 0 || coordinate.y >= maxHeight) return null;
        return cells[coordinate.x, coordinate.y];
    }

    public bool IsPlaceable(Coordinate coordinate) {
        if (coordinate.x < 0 || coordinate.x >= maxWidth || coordinate.y < 0 || coordinate.y >= maxHeight) return false;
        var cell = cells[coordinate.x, coordinate.y];
        if (cell.active == false || cell.tile == null || cell.building != null) return false;
        return true;
    }
    public bool IsPlaceable(Vector2 globalPosition) {
        return IsPlaceable(GlobalToBoardCoordinate(globalPosition));
    }

    public bool VerifyAllCellsMatchBuildings(Building target, Vector2[] globalPositions) {
        foreach (Vector2 position in globalPositions) {
            Cell? cell = GetCellUsingGlobalPoint(position);
            if (!cell.HasValue || cell.Value.building != target) return false;
        }
        return true;
    }

    public bool IsPlaceable(Vector2[] globalPositions) {
        foreach (Vector2 position in globalPositions) {
            if (!IsPlaceable(GlobalToBoardCoordinate(position))) return false;
        }
        return true;
    }

    public void SetBuilding(Building building, Vector2[] globalPositions) {
        foreach (Vector2 position in globalPositions) {
            var coord = GlobalToBoardCoordinate(position);
            cells[coord.x, coord.y].building = building;
            OnBuildingUpdate?.Invoke(coord, building);
        }
    }

    public Vector2 NormalizedToGlobal(Coordinate coordinate) {
        return new Vector2(coordinate.x * tileSize, coordinate.y * tileSize);
    }
    public Vector2 BoardCoordinateToGlobal(Coordinate coordinate) {
        return new Vector2((coordinate.x - mapCentre.x) * tileSize, (coordinate.y - mapCentre.y) * tileSize);
    }
    public Coordinate GlobalToBoardCoordinate(Vector2 globalPosition) {
        return new Coordinate((int)Mathf.Floor(globalPosition.x / tileSize + 0.5f + mapCentre.x), (int)Mathf.Floor(globalPosition.y / tileSize + 0.5f + mapCentre.y));
    }
    public Coordinate NormalizedToBoardCoordinate(Vector2 globalPosition) {
        return new Coordinate((int)Mathf.Floor(globalPosition.x + 0.5f + mapCentre.x), (int)Mathf.Floor(globalPosition.y + 0.5f + mapCentre.y));
    }

    public void Reset() {
        if (holder == null) return;
        GameObject.DestroyImmediate(holder.gameObject);
    }
    ~GridBoard() {
        Reset();
    }
    public GridBoard(float tileSize = 1.0f, int maxWidth = 100, int maxHeight = 100) {
        this.tileSize = tileSize;
        this.maxWidth = maxWidth;
        this.maxHeight = maxHeight;
    }

    public void Load(BoardProperties boardProperties) {
        Reset();
        holder = new GameObject("GridBoard").transform;
        holder.position = boardPosition;
        tileManager = GameObject.FindObjectOfType<TileManager>();

        cells = new Cell[maxWidth, maxHeight];


        foreach (TileProperties tile in boardProperties.tiles) {
            Coordinate coord = NormalizedToBoardCoordinate(new Vector2(tile.position.x, tile.position.y));

            var position = NormalizedToGlobal(tile.position);
            Transform newTile = GameObject.Instantiate(tileManager.GetTilePrefabs(tile.type), new Vector3(position.x, 0, position.y), Quaternion.Euler(Vector3.right * 90)) as Transform;
            newTile.parent = holder;
            newTile.localScale = new Vector3(1.0f, 1.0f, 1.0f) * tileViewOption.tilePercentage * tileSize;
            newTile.AddComponent<Tile>().property = tile;

            // Todo: Error handling required (out of bound)
            cells[coord.x, coord.y].active = true;
            cells[coord.x, coord.y].tile = newTile;
        }
    }

    public BoardProperties ToProperties() {
        return new BoardProperties();
    }

}
