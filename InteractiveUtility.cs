using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public static class InteractiveUtility
{
    public interface IMouseInteractive
    {
        public void OnPointerEnter(RaycastHit hit);
        public void OnPointerExit();
    }

    public interface IMouseDraggable
    {
        public void OnBeginDrag(RaycastHit hit);
        public void OnDrag();
        public void OnEndDrag();
    }
}
