using UnityEngine;

public class FallingWall : MonoBehaviour
{
    private bool _isFalling = true;
    private const float _gravity = 9.81f;
    void Update()
    {
        if (_isFalling)
        {
            transform.position -= new Vector3(0, _gravity * Time.deltaTime, 0);
            if (transform.position.y < 0)
            {
                ImpactOnGround();
            }
        }
    }

    private void ImpactOnGround()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        _isFalling = false;

        Vector3 wallPos = transform.position;
        float hitRadius = 1.1f; // Increased range for easier hits

        foreach (PlayerHandler player in GameManager.Instance.Players)
        {
            Vector3 playerPos = player.transform.position;
            if (Vector3.Distance(new Vector3(playerPos.x, 0, playerPos.z), wallPos) <= hitRadius)
            {
                player.Stun();
                Destroy(gameObject);
                return;
            }
        }
    }

}
