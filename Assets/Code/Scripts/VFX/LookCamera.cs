using Sirenix.OdinInspector;
using UnityEngine;

public class LookCamera : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, Tooltip("Look at the camera instead of looking at the transform")] private bool _LookAtCamera = true;

    private Camera _Camera;

    private void Start()
    {
        _Camera = Camera.main;
    }

    private void Update()
    {
        if (_LookAtCamera)
        {
            transform.LookAt(_Camera.transform);
        }
        else
        {
            transform.rotation = _Camera.transform.rotation;
        }
    }
}
