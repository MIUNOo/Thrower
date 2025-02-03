using UnityEngine;

public class PlayerMovement3D : PlayerMovementBase
{
    private CharacterController characterController;
    private Vector3 velocity; // 用于管理重力
    private bool isGrounded;
    private float gravity = -9.81f;

    protected override void Awake()
    {
        base.Awake();
        characterController = GetComponent<CharacterController>();
    }

    public override void Move(Vector2 input)
    {
        // 计算移动方向（相对于玩家的前方和右方）
        Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;
        
        // 使用 CharacterController.Move 来移动
        characterController.Move(moveDirection * speed * Time.deltaTime);
        

        // 应用重力
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    public override void Jump()
    {
        // 检测是否在地面上
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 轻微向下确保贴地
        }

        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity); // 计算跳跃初速度
        }
    }

    public override Throwable Grab(Vector2 input)
    {
        Debug.LogAssertion(input);
        return new Throwable();
    }

    public override void Throw()
    {
        
    }
}
