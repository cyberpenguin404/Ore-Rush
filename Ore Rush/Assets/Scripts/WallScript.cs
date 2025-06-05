using UnityEngine;

public class WallScript : MonoBehaviour
{
    [Header("Gem Reference")]
    public GameObject gemInsideMe;

    [Header("Wall Meshes")]
    public GameObject defaultWallVisual;
    public GameObject GemWallVisual;

    [SerializeField] public Material _instanceMaterial;
    [SerializeField] private Renderer _gemRenderer;



    public void UpdateWallMesh()
    {
        if (gemInsideMe != null)
        {
            defaultWallVisual.SetActive(false);
            GemWallVisual.SetActive(true);

            _instanceMaterial = _gemRenderer.material;


            _instanceMaterial.color = gemInsideMe.GetComponent<Gem>().Color;
        }
        else
        {
            defaultWallVisual.SetActive(true);
            GemWallVisual.SetActive(false);
        }
    }
}
