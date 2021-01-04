using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// Drag Camera feature, copied from internet
/// </summary>
public class DragCamera : MonoBehaviour
{
    public float dragSpeed = 2;
    private Vector3 dragOrigin;
 
 
    void LateUpdate()
    {
        // Right click camera drag
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return;
 
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);
 
        transform.Translate(move, Space.World);  
    }
 
 
}