using System.Xml;
using UnityEngine;

public class FallingWall : MonoBehaviour
{
    internal bool _isFalling = true;
    private const float _gravity = 9.81f * 1.5f;
    [SerializeField] private float _visibilityHeight = 5;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private GameObject _fallInidicator;
    internal GameObject _currentFallInidicator;
    private FallingWallIndicator _currentFallIndicatorScript;
    private void Start()
    {
        _meshRenderer.enabled = false; 
        gameObject.layer = LayerMask.NameToLayer("FallingWall");
    }
    virtual public void Update()
    {
        if (_isFalling)
        {
            if (_currentFallInidicator == null)
            {
                Debug.Log("instantiated indicator");
                _currentFallInidicator = Instantiate(_fallInidicator, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
                _currentFallIndicatorScript = _currentFallInidicator.GetComponent<FallingWallIndicator>();
            }
            else
            {
                float normalisedDistanceToGround = (1 / ((transform.position.y / 10) + 1));
                //_currentFallInidicator.transform.localScale = Vector3.one * normalisedDistanceToGround;

                Color wallIndicatorColor = _currentFallIndicatorScript.InstanceMaterial.color;
                _currentFallIndicatorScript.InstanceMaterial.color = new Color(wallIndicatorColor.r, wallIndicatorColor.g, wallIndicatorColor.b, normalisedDistanceToGround);
            }

            transform.position -= new Vector3(0, _gravity * Time.deltaTime, 0);
            if (transform.position.y < _visibilityHeight)
            {
                _meshRenderer.enabled = true;
            }
            if (transform.position.y < 0)
            {
                ImpactOnGround();
            }
        }
    }

    internal virtual void ImpactOnGround()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        _isFalling = false;

        Destroy(_currentFallInidicator);
        _currentFallInidicator = null;

        Vector3 wallPos = transform.position;
        float hitRadius = 1.1f;

        Collider[] colliders = Physics.OverlapSphere(wallPos, 0.2f);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Unbreakable"))
            {
                Debug.Log("hit unbreakable wall");
                Destroy(gameObject);
                return;
            }
        }

        if (Vector3.Distance(GameManager.Instance.GridGenerate.collectionZone.position, wallPos) <= 3)
        {
            Debug.Log("hit collection zone");
            Destroy(gameObject);
            return;
        }
        foreach (PlayerHandler player in GameManager.Instance.Players)
        {
            Vector3 playerPos = player.transform.position;
            if (Vector3.Distance(new Vector3(playerPos.x, 0, playerPos.z), wallPos) <= hitRadius)
            {
                player.Stun();
                Debug.Log("hit player");
                Destroy(gameObject);
                return;
            }
        }
        foreach (GameObject wall in GameManager.Instance.GridGenerate.wallObjects)
        {
            if (Vector3.Distance(wall.transform.position, wallPos) <= 0.5f)
            {
                Debug.Log("hit wall");
                Destroy(gameObject);
                return;
            }
        }
        foreach (GameObject gem in GameManager.Instance.SpawnManager.GemObjects)
        {
            if (Vector3.Distance(gem.transform.position, wallPos) <= 0.9f)
            {
                Debug.Log("hit gem");
                GetComponent<WallScript>().gemInsideMe = gem;
                GetComponent<WallScript>().UpdateWallMesh();
                return;
            }
        }

        gameObject.layer = LayerMask.NameToLayer("Pickaxeable");
        GameManager.Instance.GridGenerate.wallPositions.Add(wallPos);
        GameManager.Instance.GridGenerate.wallObjects.Add(gameObject);
        GameManager.Instance.SpawnManager.EmptyWalls.Add(gameObject);
        GameManager.Instance.SpawnManager.EmptyTiles.Remove(wallPos);

    }
}
