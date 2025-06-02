using UnityEngine;

public class DeathWall : FallingWall
{
    private const float HitRadius = 1.1f;

    internal override void ImpactOnGround()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        base._isFalling = false;
        Destroy(_currentFallInidicator);

        Vector3 wallPos = transform.position;

        foreach (PlayerHandler player in GameManager.Instance.Players)
        {
            Vector3 playerPos = player.transform.position;
            if (Vector3.Distance(new Vector3(wallPos.x, 0, wallPos.z), playerPos) <= HitRadius)
            {
                player.Stun();
                player.transform.position = new Vector3(0,1,0);
                return;
            }
        }
        foreach (GameObject wall in GameManager.Instance.GridGenerate.wallObjects)
        {
            Vector3 hitWallPos = wall.transform.position;
            if (Vector3.Distance(new Vector3(wallPos.x, 0, wallPos.z), hitWallPos) <= 0.2f && !wall.CompareTag("DeathWall"))
            {
                GameManager.Instance.GridGenerate.wallPositions.Remove(hitWallPos);
                GameManager.Instance.SpawnManager.EmptyWalls.Remove(wall);
                GameManager.Instance.GridGenerate.wallObjects.Remove(wall);
                Destroy(wall);
                return;
            }
        }
        GameManager.Instance.SpawnManager.EmptyTiles.Remove(wallPos);
        GameManager.Instance.GridGenerate.wallObjects.Add(gameObject);
        GameManager.Instance.GridGenerate.wallPositions.Add(wallPos);
    }
    public override void Update()
    {
        base.Update();
        foreach (PlayerHandler player in GameManager.Instance.Players)
        {
            Vector3 playerPos = player.transform.position;
            if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), playerPos) <= HitRadius)
            {
                player.Stun();
                player.transform.position = new Vector3(0, 1, 0);
                return;
            }
        }
    }
}
