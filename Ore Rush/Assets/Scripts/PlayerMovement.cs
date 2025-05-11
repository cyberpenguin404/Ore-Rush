using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float PlayerSpeed;
    [SerializeField]
    private CharacterController _charController;

    private Vector2 _moveInput;

    public void OnMovement(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }
    void Update()
    {
        _moveInput = Vector2.ClampMagnitude(_moveInput, 1f);

        Vector3 move = new Vector3(_moveInput.x, 0, _moveInput.y);

        _charController.Move(move * PlayerSpeed * Time.deltaTime);


        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // Only rotate on Y-axis
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation
        }
    }
}
