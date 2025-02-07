using UnityEngine;

public class PlayerMovement2D : PlayerMovementBase
{
    private Rigidbody2D rb;
    private GameObject grabbedObject; // 记录抓取的对象
    private Vector2 initialGrabPosition;

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
    public override Throwable Grab(Vector2 input)
    {
        Debug.LogAssertion(input);
        RaycastHit2D hit = Physics2D.Raycast(input, Vector2.zero);
        Throwable throwable = hit.collider.GetComponent<Throwable>();
        Debug.LogAssertion(hit.collider);


        if (hit.collider != null && throwable != null)
        {
            initialGrabPosition = input;
            grabbedObject = hit.collider.gameObject;
            hit.collider.transform.SetParent(transform);
            hit.collider.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            return throwable;
        }


        return new Throwable();
    }

    public override void Throw()
    {
        if (grabbedObject != null)
        {
            // 解除父子关系
            
            Rigidbody2D grb = grabbedObject.GetComponent<Rigidbody2D>();

            if (grb != null)
            {
                grb.bodyType = RigidbodyType2D.Dynamic;

                // 投掷方向和力度计算
                Vector2 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 throwDirection = (currentMousePosition - initialGrabPosition).normalized;
                float throwForce = Vector2.Distance(currentMousePosition, initialGrabPosition) * 10f;

                grb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
                Debug.Log($"Thrown with force: {throwDirection * throwForce}");
            }

            // 清空抓取对象
            grabbedObject = null;
        }
    }
}