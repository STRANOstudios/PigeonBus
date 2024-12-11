using Sirenix.OdinInspector;
using UnityEngine;

public class RaycastDebuger : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, Tooltip("Layer mask that the raycast will hit")] private LayerMask _layerMask;
    [SerializeField, ColorPalette, Tooltip("Color of the debug ray")] private Color _rayColor;
    [Title("Gizmos")]
    [SerializeField, Tooltip("Show gizmos in editor, a perpetual ray will be drawn")] private bool _showGizmos = false;

    //we don't want to show this in the inspector, if _showGizmos is false
    [SerializeField, ColorPalette, ShowIf("_showGizmos"), Tooltip("Color of the gizmos")] private Color _gizmosColor;

    private float _distance = 100f;

    private void Start()
    {
        //get the far clip plane of the camera for the maximum distance of the ray
        _distance = Camera.main.farClipPlane;
    }

    private void Update()
    {
        //if the left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            //create a ray, from camera position to mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //draw the ray line in the scene
            Debug.DrawRay(ray.origin, ray.direction, _rayColor, _distance);

            if (Physics.Raycast(ray, out RaycastHit hit, _distance, _layerMask))
            {
                //print the name of the object that was hit
                Debug.Log(hit.transform.name);
            }
        }
    }

    private void OnDrawGizmos()
    {
        //if the _showGizmos is false, don't draw the gizmos
        if (!_showGizmos || !enabled) return;

        Gizmos.color = _gizmosColor;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //draw the ray line in the scene
        Gizmos.DrawRay(ray.origin, ray.direction * _distance);
    }
}
