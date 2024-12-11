using Sirenix.OdinInspector;
using UnityEngine;

public class PerpetualMovement : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, MinValue(0f)] private float speed;

    private void Start() { }

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.forward);
    }
}
