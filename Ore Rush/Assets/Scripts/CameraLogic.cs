using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    public Transform Player;        
    public Vector3 Offset = new Vector3(0, 5, -10);  
    public Vector3 FixedRotation = new Vector3(30, 0, 0); 
    void LateUpdate()
    {
        if (Player == null) return;

        // Follow the player's position with the offset
        transform.position = Player.position + Offset;

        // Keep the camera's rotation fixed
        transform.rotation = Quaternion.Euler(FixedRotation);
    }
}
