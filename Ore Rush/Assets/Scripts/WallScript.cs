using UnityEngine;

public class WallScript : MonoBehaviour
{
    [Header("Gem Reference")]
    public GameObject gemInsideMe;

    [Header("Wall Meshes")]
    public GameObject defaultWallVisual;
    public GameObject GemWallVisual;

    [SerializeField]
    private MeshFilter _meshFilter;
    public void UpdateWallMesh()
    {
        if (gemInsideMe != null)
        {
            defaultWallVisual.SetActive(false);
            GemWallVisual.SetActive(true);
        }
        else
        {
            defaultWallVisual.SetActive(true);
            GemWallVisual.SetActive(false);
        }
    }
}
