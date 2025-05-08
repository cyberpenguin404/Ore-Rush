using UnityEngine;

public class Pickaxe : MonoBehaviour
{
    public float mineRange = 3f; // how far the pickaxe can reach
    public GameObject FrontOfPlayer; // assign your player camera in inspector

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Mine();
        }
    }

    void Mine()
    {
        Ray ray = new Ray(FrontOfPlayer.transform.position, FrontOfPlayer.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, mineRange))
        {
            if (hit.collider.CompareTag("Mineable"))
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }
}
