using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static InteractiveUtility;

public class InputController : MonoBehaviour
{
    public static InputController Inst { get; private set; }
    private void Awake() {
        Inst = this;
    }

    private Camera mainCamera;
    private Camera AuxCamera;

    private IMouseInteractive lastHitInstance;
    private IMouseDraggable dragableInstance;

    [Serializable]
    public enum State {
        Idle, onDrag
    }

    [ReadOnly] public State state;

    public bool onUI;
    public bool hovered;

    void Start() {
        mainCamera = Camera.main;
        AuxCamera = GameObject.Find("AuxCamera").GetComponent<Camera>();
    }

    void EnvironmentUpdate() {
        // Check Region (UI or GameScreen)
        onUI = EventSystem.current.IsPointerOverGameObject();
        if (onUI) {
            mainCamera.tag = "Untagged";
            AuxCamera.tag = "MainCamera";
        }
        else {
            mainCamera.tag = "MainCamera";
            AuxCamera.tag = "Untagged";
        }
    }

    private void Reset() {
    }

    void Update() {
        EnvironmentUpdate();

        switch (state) {
            case State.Idle:
                IdleProcess();
                break;
            case State.onDrag:
                DragProcess();
                break;
        }
    }

    void DragProcess() {
        if (dragableInstance != null && Input.GetMouseButton(0)) {
            state = State.onDrag;
            dragableInstance.OnDrag();
        }
        if (Input.GetMouseButtonUp(0)) {   
            state = State.Idle;
        }

        if (state != State.onDrag) ResetState(State.onDrag);
    }

    void ResetState(State state) {
        switch (state) {
            case State.Idle:
                if (lastHitInstance != null) {
                    lastHitInstance.OnPointerExit();
                    lastHitInstance = null;
                }
                break;
            case State.onDrag:
                if (dragableInstance != null) {
                    dragableInstance.OnEndDrag();
                    dragableInstance = null;
                }
                break;
        }
    }
    void IdleProcess() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {
            IMouseInteractive hitInstance = hit.collider.GetComponentInParent<IMouseInteractive>();
            if (hitInstance != null) {
                if (hitInstance == lastHitInstance) {
                    hovered = true;
                }
                else {
                    hovered = false;
                    ResetState(State.Idle);
                    lastHitInstance = hitInstance;
                    hitInstance.OnPointerEnter(hit);
                }
            }

            // Idle => draggable
            IMouseDraggable draggable = hit.collider.GetComponentInParent<IMouseDraggable>();
            if (draggable != null) {
                if (Input.GetMouseButtonDown(0)) {
                    if (hit.collider != null) {
                        if (hitInstance != null) {
                            ResetState(State.Idle);
                            dragableInstance = draggable;
                            dragableInstance.OnBeginDrag(hit);
                            state = State.onDrag;
                        }
                    }
                }
            }
        }else {
            hovered = false;
            ResetState(State.Idle);
        }
    }
}
