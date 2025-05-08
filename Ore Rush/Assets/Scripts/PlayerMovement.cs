using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float PlayerSpeed;

    private Vector3 _moveInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        _moveInput = new Vector3(inputX, 0, inputY);

        _moveInput = Vector3.ClampMagnitude(_moveInput, 1f);

        transform.position += _moveInput * PlayerSpeed * Time.deltaTime;


        if (_moveInput.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveInput);
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // Only rotate on Y-axis
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation
        }
    }
}
