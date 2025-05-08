using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float PlayerSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        Vector3 moveInput = new Vector3(inputX, 0, inputY);

        moveInput = Vector3.ClampMagnitude(moveInput, 1f);

        transform.position += moveInput * PlayerSpeed * Time.deltaTime;
    }
}
