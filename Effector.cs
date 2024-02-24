using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Effector : MonoBehaviour
{

    public interface ObjectPointEffector
    {
        public void OnPointerEnter(RaycastHit hit, GameObject gameObject);
        public void OnPointerExit(RaycastHit hit, GameObject gameObject);
    }
}
