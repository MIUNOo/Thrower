using UnityEngine;

public abstract class PlayerMovementBase : MonoBehaviour
{
    protected @InputSystem inputSystem;
    public float speed = 5f;
    public float jumpForce = 5f;

    protected virtual void Awake()
    {
        inputSystem = new @InputSystem();
        inputSystem.Enable();
    }

    protected virtual void Update()
    {
        Vector2 moveInput = inputSystem.Player.Move.ReadValue<Vector2>();

        // 子类具体实现
        Move(moveInput);

        if (inputSystem.Player.Jump.triggered)
        {
            Jump();
        }
    }

    protected void OnDestroy()
    {
        inputSystem.Dispose();
    }

    public abstract void Move(Vector2 input);
    public abstract void Jump();
}