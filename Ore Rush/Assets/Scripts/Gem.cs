using UnityEngine;

public class Gem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        other.GetComponentInChildren<StateHandler>().PickUpGem(this.gameObject);
    }
}
