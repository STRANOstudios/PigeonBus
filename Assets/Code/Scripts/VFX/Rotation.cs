using UnityEngine;

namespace VFX.Animation
{    public class Rotation : MonoBehaviour
    {
        public enum RotationAxis
        {
            X,
            Y,
            Z
        }

        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private RotationAxis rotationAxis = RotationAxis.Z;

        Vector3 axis = Vector3.zero;

        private void Start()
        {
            // Get axis to rotate
            switch (rotationAxis)
            {
                case RotationAxis.X:
                    axis = Vector3.right;
                    break;
                case RotationAxis.Y:
                    axis = Vector3.up;
                    break;
                case RotationAxis.Z:
                    axis = Vector3.forward;
                    break;
            }

            transform.Rotate(axis * Random.Range(0f, 360f));
        }

        void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime * axis);
        }
    }
}
