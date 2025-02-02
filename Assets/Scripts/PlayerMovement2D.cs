using UnityEngine;

public class PlayerMovement2D : PlayerMovementBase
{
    private Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake(); // 调用基类的 Awake 来初始化 InputSystem
        rb = GetComponent<Rigidbody2D>();
    }

    public override void Move(Vector2 input)
    {
        rb.linearVelocity = new Vector2(input.x * speed, rb.linearVelocity.y);
    }

    public override void Jump()
    {
        if (Mathf.Abs(rb.linearVelocity.y) < 0.01f) // 确保在地面上
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}