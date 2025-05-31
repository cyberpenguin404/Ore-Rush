using UnityEngine;

public class ClickToSpawn : MonoBehaviour
{
    [SerializeField]
    private LayerMask _clickableLayers;
    [SerializeField]
    private GameObject _objectToSpawn;
    [SerializeField]
    private Camera _camera;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mousePositionRay = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Debug.Log("Clicked");

            if (Physics.Raycast(mousePositionRay, out hit, 1000, _clickableLayers))
            {
                Debug.Log("hit");
                Instantiate(_objectToSpawn, hit.point, Quaternion.identity);
            }
        }
    }
}
