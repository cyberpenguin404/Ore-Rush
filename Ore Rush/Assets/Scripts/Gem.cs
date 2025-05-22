using UnityEngine;

public class Gem : MonoBehaviour
{
    float value;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }
        other.GetComponentInChildren<PlayerHandler>().PickUpGem(this.gameObject);
    }
}
