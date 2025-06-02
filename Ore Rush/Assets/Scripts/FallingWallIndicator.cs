using UnityEngine;

public class FallingWallIndicator : MonoBehaviour
{
    public Material InstanceMaterial;
    private void Awake()
    {
        InstanceMaterial = GetComponent<Renderer>().material;
    }
}
