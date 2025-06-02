using UnityEngine;

public class BillboardEffect : MonoBehaviour
{
    private Camera _cam;
    private void Start()
    {
        _cam = Camera.main;
    }
    private void LateUpdate()
    {
        if (_cam != null)
        {
            transform.LookAt(transform.position + _cam.transform.rotation * Vector3.forward,
                         _cam.transform.rotation * Vector3.up);
        }
    }
}
