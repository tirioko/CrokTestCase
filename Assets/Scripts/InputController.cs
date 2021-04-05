using UnityEngine;
using System;
using UnityEngine.EventSystems;
public class InputController : MonoBehaviour
{
    public static event Action<Ray> onMousePressed;
    public static event Action<Ray> onMouseDrag;
    public static event Action<Ray> onMouseDragEnd;

    void Update()
    {       
        if (Input.GetMouseButtonDown(0)&& !CheckPressUI())
        {
            onMousePressed?.Invoke(GetRay());
        }
        else if (Input.GetMouseButton(0) && !CheckPressUI())
        {
            onMouseDrag?.Invoke(GetRay());
        }
        else if (Input.GetMouseButtonUp(0) && !CheckPressUI())
        {
            onMouseDragEnd?.Invoke(GetRay());
        }
    }
    private Ray GetRay()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }
    public static bool CheckPressUI()
    {        
        return EventSystem.current.IsPointerOverGameObject();       
    }
}
