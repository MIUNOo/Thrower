using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement2D : PlayerMovementBase
{
    public Transform grabPos;
    public Tilemap tilemap;
    public GameObject throwablePrefab;


    private Rigidbody2D rb;
    private GameObject grabbedObject; // 记录抓取的对象
    private Vector2 input2D;

    private Animator anm;
    private SpriteRenderer sr;

    private bool isCollecting = false;

    protected override void Awake()
    {
        base.Awake(); // 调用基类的 Awake 来初始化 InputSystem
        rb = GetComponent<Rigidbody2D>();
        anm = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    public override void Move(Vector2 input)
    {
        input2D = input;
        rb.linearVelocity = new Vector2(input.x * speed, rb.linearVelocity.y);

        anm.SetBool("IsWalking", (rb.linearVelocityX != 0));
        if (input.x<0)
        {
            sr.flipX = true;
        }else if (input.x>0)
        {
            sr.flipX = false;
        }
        else
        {
            anm.SetBool("IsWalking", false);
        }

    }

    public override void Jump()
    {
        if (Mathf.Abs(rb.linearVelocity.y) < 0.01f) // 确保在地面上
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    public override ThrowableTile Grab(Vector2 input)
    {
        //if (isCollecting)
        //{
        //    return new ThrowableTile();
        //}

        ////Debug.LogAssertion(input);
        ////RaycastHit2D hit = Physics2D.Raycast(input, Vector2.zero);
        ////Throwable throwable = hit.collider.GetComponent<Throwable>();
        ////Debug.LogAssertion(hit.collider);

        //ThrowableTile throwable = tilemap.GetTile<ThrowableTile>(tilemap.WorldToCell(input));


        //if (throwable != null)
        //{
        //    //initialGrabPosition = input;
        //    tilemap.SetTile(tilemap.WorldToCell(input), null);

        //    grabbedObject = Instantiate(throwablePrefab.gameObject, throwable.gameObject.transform.position, Quaternion.identity);
        //    grabbedObject.GetComponent<Throwable>().mass = throwable.mass;
        //    grabbedObject.GetComponent<Throwable>().damage = throwable.damage;
        //    grabbedObject.GetComponent<SpriteRenderer>().sprite = throwable.sprite;
        //    grabbedObject.transform.SetParent(transform);  
        //    grabbedObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        //    grabbedObject.GetComponent<BoxCollider2D>().enabled = false;

        //    StartCoroutine(MoveToGrabPos(grabbedObject.transform));
        //    return throwable;
        //}




        //return new ThrowableTile();



        if (isCollecting)
        {
            Debug.LogWarning("Currently collecting, cannot grab another tile.");
            return null;
        }

        if (grabbedObject!=null)
        {
            return null;
        }


        RaycastHit2D hit = Physics2D.Raycast(input, Vector2.zero);
        if (hit.collider != null)
        {
            Throwable throwableObject = hit.collider.GetComponent<Throwable>();
            if (throwableObject != null)
            {
                Debug.Log("Picked up an existing Throwable object!");

                // 处理拾取到的 `Throwable`
                grabbedObject = throwableObject.gameObject;
                grabbedObject.transform.SetParent(transform);

                // 临时禁用物理效果，使其可以被拖拽
                Rigidbody2D rbt = grabbedObject.GetComponent<Rigidbody2D>();
                if (rbt != null)
                {
                    rbt.bodyType = RigidbodyType2D.Static;
                }

                BoxCollider2D tcollider = grabbedObject.GetComponent<BoxCollider2D>();
                if (tcollider != null)
                {
                    tcollider.enabled = false;
                }

                StartCoroutine(MoveToGrabPos(grabbedObject.transform));
                return null;  // 因为已经拾取到了 `Throwable`，无需再从 Tilemap 里获取
            }
        }

        // 计算 Tile 位置
        Vector3Int tilePos = tilemap.WorldToCell(input);
        ThrowableTile throwable = tilemap.GetTile<ThrowableTile>(tilePos);

        if (throwable == null)
        {
            Debug.LogWarning($"No ThrowableTile found at position {tilePos}.");
            return null;
        }

        // 移除 Tile
        tilemap.SetTile(tilePos, null);
        Debug.Log($"Tile removed at {tilePos}");

        // 计算 Tile 世界坐标
        Vector3 spawnPosition = tilemap.GetCellCenterWorld(tilePos);

        // 确保 Prefab 已经在 Inspector 里正确赋值
        if (throwablePrefab == null)
        {
            Debug.LogError("throwablePrefab is NOT assigned in the Inspector!");
            return null;
        }

        // 生成投掷物对象
        grabbedObject = Instantiate(throwablePrefab.gameObject, spawnPosition, Quaternion.identity);
        if (grabbedObject == null)
        {
            Debug.LogError("Instantiation failed! Check throwablePrefab.");
            return null;
        }

        // 获取 Throwable 组件
        Throwable throwableComponent = grabbedObject.GetComponent<Throwable>();
        if (throwableComponent == null)
        {
            Debug.LogError("Throwable component is missing on throwablePrefab!");
            return null;
        }

        // 赋值属性
        throwableComponent.mass = throwable.mass;
        throwableComponent.damage = throwable.damage;

        // 设置 Sprite
        SpriteRenderer spriteRenderer = grabbedObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = throwable.sprite;
        }
        else
        {
            Debug.LogError("SpriteRenderer component is missing on throwablePrefab!");
        }

        // 设置物理属性
        grabbedObject.transform.SetParent(transform);
        Rigidbody2D rb = grabbedObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
        else
        {
            Debug.LogError("Rigidbody2D component is missing on throwablePrefab!");
        }

        BoxCollider2D collider = grabbedObject.GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        else
        {
            Debug.LogError("BoxCollider2D component is missing on throwablePrefab!");
        }

        // 启动抓取移动协程
        StartCoroutine(MoveToGrabPos(grabbedObject.transform));

        return throwable;
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

                Vector2 throwDirection = sr.flipX ? -transform.right.normalized:transform.right.normalized;
                throwDirection.y = input2D.y;

                //float throwForce = Vector2.Distance(currentMousePosition, initialGrabPosition) * 10f;
                float throwForce = Mathf.Max(10f, Mathf.Abs(input2D.x*20f));
                

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