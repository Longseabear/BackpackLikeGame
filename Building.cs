using System;
using System.Collections;
using System.Collections.Generic;
using static InteractiveUtility;
using UnityEngine;
using UnityEngine.UIElements;
using System.Drawing;

using Color = UnityEngine.Color;
using System.Data;
using UnityEditor.TextCore.Text;

public class Building : MonoBehaviour, IMouseInteractive, IMouseDraggable
{
    Transform CubesHolder;
    Transform EventsHolder;

    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();
    [ReadOnly, SerializeField] private Vector3 originalPosition;

    private float targetHeight;
    private float draggableMarginPercent = 0.5f;
    private float draggableUISpaceMarginPercent = 2.5f;

    private bool dragMode;

    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private List<Vector2> originalCubePositions;

    public float rotationSpeed = 5.0f;
    public float rotationPercentage = 0.0f;

//    public bool previewMode;

    private Quaternion originRotation;
    bool rotationFlag = false;
    

    public void OnBeginDrag(RaycastHit hit) {
        EventsHolder.gameObject.SetActive(true);

        dragMode = true;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalCubePositions = GetAllBuildingPosition();
        if (MapGenerator.Inst.boardModel.VerifyAllCellsMatchBuildings(this, originalCubePositions.ToArray())) {
            MapGenerator.Inst.boardModel.SetBuilding(null, originalCubePositions.ToArray());
        }

        targetHeight = transform.position.y;

        // Save original color
        foreach (Renderer child in GetComponentsInChildren<Renderer>()) {
            originalColors[child] = child.material.color;
        }
    }

    public void OnDrag() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * targetHeight);

        float rayDistance;
        if (groundPlane.Raycast(ray, out rayDistance)) {
            Vector3 point = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, point, Color.red);

            GridBoard.Cell? targetCell = MapGenerator.Inst.boardModel.GetCellUsingGlobalPoint(new Vector2(point.x, point.z));
            if(targetCell != null && targetCell.Value.active) {
                Vector3 position = targetCell.Value.tile.position;
                transform.position = new Vector3(position.x, targetHeight, position.z);
            }
            else {
                if(!InputController.Inst.onUI) transform.position = new Vector3(point.x, targetHeight + targetHeight * draggableMarginPercent, point.z);
                else transform.position = new Vector3(point.x, targetHeight + targetHeight * draggableUISpaceMarginPercent, point.z);
            }
        }

        ShowRestrictedAreas();

        if (Input.GetMouseButton(1) && !rotationFlag) {
            targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z);
            rotationPercentage = 0.0f;
            rotationFlag = true;
            originRotation = transform.rotation;
        }
    }
    public void ShowRestrictedAreas() {
        foreach (Renderer child in GetComponentsInChildren<Renderer>()) {
            Vector3 position = child.transform.position;
            GridBoard.Cell? targetCell = MapGenerator.Inst.boardModel.GetCellUsingGlobalPoint(new Vector2(position.x, position.z));
            if (targetCell == null || !targetCell.Value.active) {
                Color targetColor = Color.red;
                targetColor.a = 0.5f;
                child.material.color = targetColor;
            }
            else {
                if (originalColors.TryGetValue(child, out var outColor)) {
                    child.material.color = outColor;
                }
            }
        }

    }

    public List<Vector2> GetAllBuildingPosition() {
        Transform[] childTransforms = CubesHolder.GetComponentsInChildren<Transform>();
        List<Vector2> allCoordinate = new List<Vector2>();
        for (int i = 1; i < childTransforms.Length; i++) {
            allCoordinate.Add(new Vector2(childTransforms[i].position.x, childTransforms[i].position.z));
        }
        return allCoordinate;
    }

    public void OnEndDrag() {
        dragMode = false;

        List<Vector2> allCoordinate = GetAllBuildingPosition();
        if (MapGenerator.Inst.boardModel.IsPlaceable(allCoordinate.ToArray())) {
            transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
            MapGenerator.Inst.boardModel.SetBuilding(this, allCoordinate.ToArray());
        }
        else {
            if(!InputController.Inst.onUI) transform.position = new Vector3(originalPosition.x, targetHeight, originalPosition.z);
            else transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);

            if (MapGenerator.Inst.boardModel.IsPlaceable(originalCubePositions.ToArray())) {
                MapGenerator.Inst.boardModel.SetBuilding(this, originalCubePositions.ToArray());
            }

            transform.rotation = originalRotation;
            targetRotation = originalRotation;
        }

        //Reset
        foreach (Renderer child in GetComponentsInChildren<Renderer>()) {
            if(originalColors.TryGetValue(child, out var outColor)) {
                child.material.color = outColor;
            }
        }

        EventsHolder.gameObject.SetActive(false);
    }

    public void OnPointerEnter(RaycastHit hit) {
        foreach (Renderer child in GetComponentsInChildren<Renderer>()) {
            originalColors[child] = child.material.color;
            Color targetColor = Color.red;
            targetColor.a = 0.5f;
            child.material.color = targetColor;
        }
    }

    public void OnPointerExit() {
        foreach (Renderer child in GetComponentsInChildren<Renderer>()) {
            if (originalColors.TryGetValue(child, out var outColor)) {
                child.material.color = outColor;
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        targetRotation = transform.rotation;
        CubesHolder = transform.Find("Cubes");
        if(CubesHolder == null) {
            CubesHolder = new GameObject("Cubes").transform;
            CubesHolder.parent = transform;
        }
        EventsHolder = transform.Find("Events");
        if(EventsHolder == null) {
            EventsHolder = new GameObject("Events").transform;
            EventsHolder.parent = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rotationFlag) {
            if (rotationPercentage > 1.0f) rotationFlag = false;

            transform.rotation = Quaternion.Lerp(originRotation, targetRotation, rotationPercentage);
            rotationPercentage += Time.deltaTime * rotationSpeed;
        }
    }
}

