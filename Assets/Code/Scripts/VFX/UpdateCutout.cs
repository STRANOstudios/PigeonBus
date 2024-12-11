using UnityEngine;

public class UpdateCutout : MonoBehaviour
{
    public Material cutoutMaterial;
    public Transform cutoutCenter;
    public float cutoutRadius = 1f;

    void Update()
    {
        if (cutoutMaterial != null && cutoutCenter != null)
        {
            cutoutMaterial.SetVector("_CutoutCenter", cutoutCenter.position);
            cutoutMaterial.SetFloat("_CutoutRadius", cutoutRadius);
        }
    }
}
