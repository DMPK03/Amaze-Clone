using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
           if(Vector2.SqrMagnitude(touch.deltaPosition) > 5)
           {
               if(Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y)) Debug.Log($"X: {touch.deltaPosition.x}");
               else Debug.Log($"Y: {touch.deltaPosition.y}");

           }
        }
    }
}
