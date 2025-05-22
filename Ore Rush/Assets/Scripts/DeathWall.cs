using UnityEngine;

public class DeathWall : FallingWall
{
    internal override void ImpactOnGround()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        base._isFalling = false;

        Vector3 wallPos = transform.position;
        float hitRadius = 1.1f;

        foreach (PlayerHandler player in GameManager.Instance.Players)
        {
            Vector3 playerPos = player.transform.position;
            if (Vector3.Distance(new Vector3(playerPos.x, 0, playerPos.z), wallPos) <= hitRadius)
            {
                player.Stun();
                player.transform.position = new Vector3(0,1,0);
                return;
            }
        }
        GameManager.Instance.GridGenerate.wallPositions.Add(wallPos);
    }
}
