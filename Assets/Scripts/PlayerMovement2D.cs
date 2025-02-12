using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement2D : PlayerMovementBase
{
    public Transform grabPos;

    private Rigidbody2D rb;
    private GameObject grabbedObject; // 记录抓取的对象
    private Vector2 input2D;

    private bool isCollecting = false;

    protected override void Awake()
    {
        base.Awake(); // 调用基类的 Awake 来初始化 InputSystem
        rb = GetComponent<Rigidbody2D>();
    }

    public override void Move(Vector2 input)
    {
        input2D = input;
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
        if (isCollecting)
        {
            return new Throwable();
        }

        Debug.LogAssertion(input);
        RaycastHit2D hit = Physics2D.Raycast(input, Vector2.zero);
        Throwable throwable = hit.collider.GetComponent<Throwable>();
        Debug.LogAssertion(hit.collider);


        if (hit.collider != null && throwable != null)
        {
            //initialGrabPosition = input;
            grabbedObject = hit.collider.gameObject;
            hit.collider.transform.SetParent(transform);
            hit.collider.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            hit.collider.GetComponent<BoxCollider2D>().enabled = false;

            StartCoroutine(MoveToGrabPos(grabbedObject.transform));
            return throwable;
        }




        return new Throwable();
    }

    public override void Throw()
    {

        if (isCollecting)
        {
            return;
        }

        if (grabbedObject != null)
        {
            // 解除父子关系
            
            Rigidbody2D grb = grabbedObject.GetComponent<Rigidbody2D>();


            if (grb != null)
            {
                grb.bodyType = RigidbodyType2D.Dynamic;

                // 投掷方向和力度计算
                Vector2 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Vector2 throwDirection = (currentMousePosition - initialGrabPosition).normalized;

                Vector2 throwDirection = transform.right.normalized;
                throwDirection.y = input2D.y;

                //float throwForce = Vector2.Distance(currentMousePosition, initialGrabPosition) * 10f;
                float throwForce = Mathf.Max(10f,input2D.x*20f);
                

                grb.GetComponent<BoxCollider2D>().enabled = true;

                grb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
                Debug.Log($"Thrown with force: {throwDirection * throwForce}");
            }

            // 清空抓取对象
            grabbedObject = null;
        }
    }

    private IEnumerator MoveToGrabPos(Transform obj)
    {
        Vector3 startPos = obj.position; // 起始位置
        Vector3 targetPos = grabPos.position; // 目标位置
        float duration = 0.2f; // 移动持续时间
        float elapsed = 0f; // 已用时间
        isCollecting = true; // 标记正在收集

        // 如果物体有 Rigidbody2D，暂时禁用物理模拟
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // 平滑移动物体
        while (elapsed < duration)
        {
            // 更新目标位置（如果 grabPos 是动态的）
            targetPos = grabPos.position;

            // 使用 Lerp 插值计算当前位置
            obj.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);

            // 更新已用时间
            elapsed += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 确保物体最终到达目标位置
        obj.position = targetPos;

        // 收集完成后的逻辑
        isCollecting = false;

        // 如果需要，可以在这里重新启用物理模拟
        // if (rb != null) { rb.bodyType = RigidbodyType2D.Dynamic; }
    }
}