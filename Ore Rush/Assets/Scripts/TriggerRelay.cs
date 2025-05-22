using UnityEngine;

public class TriggerRelay : MonoBehaviour
{
    [SerializeField] private ScaffholdingScript _relayTarget;
    private void OnTriggerStay(Collider other)
    {
        _relayTarget.OnTriggerStayRelayed(other);
    }
    private void OnTriggerExit(Collider other)
    {
        _relayTarget.OnTriggerExitRelayed(other);
    }
}
