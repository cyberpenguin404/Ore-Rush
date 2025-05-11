using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        other.GetComponentInChildren<PlayerHandler>().CollectGem();
    }
}
