using Sirenix.OdinInspector;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider)), RequireComponent(typeof(Rigidbody))]
public class TriggerEnter : MonoBehaviour
{
    [Title("References")]
    [SerializeField, Tooltip("Need for relative direction")][ShowIf("_areExcludesDisabled")] private Transform parentTransform = null;

    [Title("Settings")]
    [SerializeField, Tooltip("The LayerMask to filter which objects can trigger this event.")] private LayerMask _layer;
    [SerializeField, Tooltip("Whether to trigger the event only once.")] private bool _triggerOnce = true;
    [SerializeField, ColorPalette] Color _gizmoColor = new(0, 1, 0, 0.3f);

    [SerializeField, ToggleGroup("Enter")] private bool Enter = true;
    [SerializeField, ToggleGroup("Enter"), LabelText("Exclude Direction")] private Direction _directionEnter = new();
    [SerializeField, ToggleGroup("Enter"), Tooltip("Event triggered when the player enters the trigger zone.")] private TriggerEvent _onTriggerEnter = new();

    [SerializeField, ToggleGroup("Exit")] private bool Exit = false;
    [SerializeField, ToggleGroup("Exit"), LabelText("Exclude Direction")] private Direction _directionExit = new();
    [SerializeField, ToggleGroup("Exit"), Tooltip("Event triggered when the player exits the trigger zone.")] private TriggerEvent _onTriggerExit = new();

    [SerializeField, ToggleGroup("Stay")] private bool Stay = false;
    [SerializeField, ToggleGroup("Stay"), Tooltip("Event triggered when the player stays in the trigger zone.")] private TriggerEvent _onTriggerStay = new();

    [Title("Debug")]
    [SerializeField, Tooltip("Show debug logs in the console.")] private bool _debug = false;
    [SerializeField, Tooltip("Show the Collider in the editor.")] private bool _showColliderInEditor = true;
    [SerializeField, Tooltip("Show the coordinates in the gizmos."), ShowIf("_areExcludesDisabled")] private bool _showCoordinates = false;

    [ShowIf("_areTriggersDisabled"), InfoBox("No trigger events are enabled. Please enable at least one.", InfoMessageType.Warning)]
    [Button]
    private void Fix()
    {
        Enter = true;
    }

    [Serializable]
    public class Direction
    {
        public bool nord, sud, est, owest;
        public bool all => nord || sud || est || owest;
    }

    [Serializable] public class TriggerEvent : UnityEvent { }

    private bool _areTriggersDisabled => !(Enter || Exit || Stay);
    private bool _areExcludesDisabled => _directionEnter.all || _directionExit.all;

    private void Start()
    {
        if (TryGetComponent(out Collider collider))
        {
            collider.isTrigger = true;
        }
        if (TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.useGravity = false;
        }
        if (parentTransform == null) parentTransform = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Enter || !OnTriggerCheck(other) || !OnDirectionCheck(other.ClosestPoint(transform.position), _directionEnter, other.transform.eulerAngles.y)) return;
        if (_debug) Debug.Log($"{name} Trigger Enter: {other.name}");

        _onTriggerEnter?.Invoke();

        if (_triggerOnce) gameObject.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Exit || !OnTriggerCheck(other) || !OnDirectionCheck(other.ClosestPoint(transform.position), _directionExit, other.transform.eulerAngles.y)) return;
        if (_debug) Debug.Log($"{name} Trigger Exit: {other.name}");

        _onTriggerExit?.Invoke();

        if (_triggerOnce) gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!Stay || !OnTriggerCheck(other)) return;
        if (_debug) Debug.Log($"{name} Trigger Stay: {other.name}");

        _onTriggerStay?.Invoke();

        if (_triggerOnce) gameObject.SetActive(false);
    }

    // Check if the object's layer is in the LayerMask
    private bool OnTriggerCheck(Collider other)
    {
        return (_layer.value & (1 << other.gameObject.layer)) != 0;
    }

    /// <summary>
    /// Check if the object is on the correct direction
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="excludeDirection"></param>
    /// <returns></returns>
    private bool OnDirectionCheck(Vector3 collisionPoint, Direction excludeDirection, float colliderY)
    {
        if (!excludeDirection.all) return true;

        if (_debug) Debug.DrawLine(collisionPoint, new Vector3(collisionPoint.x, 10f, collisionPoint.z), Color.red, 5f);

        collisionPoint = parentTransform.InverseTransformPoint(collisionPoint);

        collisionPoint = NormalizeToMaxDirection(collisionPoint);

        if (_debug) Debug.Log($"Collision point: {collisionPoint}");
        if (_debug) Debug.Log($"transform forward: {transform.forward} Transform y: {transform.eulerAngles.y}");

        if (colliderY == 90 && transform.eulerAngles.y == 90) collisionPoint *= -1;
        else if (colliderY == 0 && transform.eulerAngles.y == 0 && collisionPoint == Vector3.back) collisionPoint *= -1;
        else if(colliderY == 180 && transform.eulerAngles.y == 180 && collisionPoint == Vector3.back) collisionPoint *= -1;
        else if(colliderY == 270 && transform.eulerAngles.y == 270) collisionPoint *= -1;


        if (_debug) Debug.Log($"Collision point after rotation: {collisionPoint}");

        if (collisionPoint == Vector3.back)
        {
            if (_debug) Debug.Log("Nord");
            return !excludeDirection.nord;
        }
        if (collisionPoint == Vector3.forward)
        {
            if (_debug) Debug.Log("Sud");
            return !excludeDirection.sud;
        }
        if (collisionPoint == Vector3.right)
        {
            if (_debug) Debug.Log("Est");
            return !excludeDirection.est;
        }
        if (collisionPoint == Vector3.left)
        {
            if (_debug) Debug.Log("Owest");
            return !excludeDirection.owest;
        }

        return false;
    }

    /// <summary>
    /// Returns the direction with the highest magnitude
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private Vector3 NormalizeToMaxDirection(Vector3 direction)
    {
        float maxValue = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y), Mathf.Abs(direction.z));

        if (maxValue > 0)
        {
            if (Mathf.Abs(direction.x) == maxValue)
                return new Vector3(Mathf.Sign(direction.x), 0, 0);
            if (Mathf.Abs(direction.y) == maxValue)
                return new Vector3(0, Mathf.Sign(direction.y), 0);
            if (Mathf.Abs(direction.z) == maxValue)
                return new Vector3(0, 0, Mathf.Sign(direction.z));
        }

        return direction;
    }

    #region Gizmos
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (!enabled) return;

        // Get the Collider component
        if (!TryGetComponent<Collider>(out var collider))
        {
            if (_debug) Debug.LogWarning("Collider component not found.");
            return;
        }

        if (_showColliderInEditor)
        {
            Gizmos.color = _gizmoColor;

            // Draw the Collider based on its type
            if (collider is BoxCollider boxCollider)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCollider.center, boxCollider.size);
                Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
            }
            else if (collider is SphereCollider sphereCollider)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
                Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
            }
            else if (collider is CapsuleCollider capsuleCollider)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                DrawCapsuleGizmos(capsuleCollider);
            }
            else if (collider is MeshCollider meshCollider && meshCollider.sharedMesh != null)
            {
                Gizmos.DrawMesh(meshCollider.sharedMesh, transform.position, transform.rotation, transform.lossyScale);
            }

            Gizmos.matrix = Matrix4x4.identity; // Reset matrix
        }

        if (_showCoordinates)
        {
            Vector3 center = collider.transform.position;

            Handles.color = Color.white;
            Handles.Label(center + transform.forward * 1, "N");   // Nord
            Handles.Label(center - transform.forward * 1, "S");   // Sud
            Handles.Label(center + transform.right * 1, "E"); // Est
            Handles.Label(center - transform.right * 1, "W"); // Ovest
        }
    }

    // Helper to draw a Capsule Collider
    private void DrawCapsuleGizmos(CapsuleCollider capsule)
    {
        Vector3 center = capsule.center;
        float radius = capsule.radius;
        float height = Mathf.Max(0, capsule.height / 2 - radius);

        // Draw main capsule body
        Gizmos.DrawWireSphere(center + Vector3.up * height, radius);
        Gizmos.DrawWireSphere(center - Vector3.up * height, radius);
        Gizmos.DrawLine(center + Vector3.up * height + Vector3.right * radius, center - Vector3.up * height + Vector3.right * radius);
        Gizmos.DrawLine(center + Vector3.up * height - Vector3.right * radius, center - Vector3.up * height - Vector3.right * radius);
        Gizmos.DrawLine(center + Vector3.up * height + Vector3.forward * radius, center - Vector3.up * height + Vector3.forward * radius);
        Gizmos.DrawLine(center + Vector3.up * height - Vector3.forward * radius, center - Vector3.up * height - Vector3.forward * radius);
    }

#endif
    #endregion
}
