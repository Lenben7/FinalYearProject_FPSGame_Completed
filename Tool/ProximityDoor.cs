using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("动画组件")]
    private Animator animator;

    [Header("检测设置")]
    [Tooltip("检测玩家的范围半径")]
    public float detectionRadius = 6f;
    [Tooltip("玩家身上的标签，默认为'Player'")]
    public string playerTag = "Player";

    [Header("状态")]
    private int playersNearby = 0;      // 区域内的玩家数量

    private void Start()
    {
        // 获取动画组件
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("DoorController: 没有找到Animator组件！");
        }

        // 设置触发器
        SetupTrigger();
    }

    /// <summary>
    /// 自动添加或配置球形触发器
    /// </summary>
    private void SetupTrigger()
    {
            // 添加一个球形碰撞器并设为触发器
            SphereCollider sphere = gameObject.AddComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = detectionRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查进入的对象是否为玩家
        if (other.CompareTag(playerTag))
        {
            playersNearby++;
            UpdateDoorState(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 检查离开的对象是否为玩家
        if (other.CompareTag(playerTag))
        {
            playersNearby--;
            if (playersNearby <= 0)
            {
                playersNearby = 0;
                UpdateDoorState(false);
            }
        }
    }

    /// <summary>
    /// 更新门的动画状态
    /// </summary>
    /// <param name="shouldOpen">是否应该打开</param>
    private void UpdateDoorState(bool shouldOpen)
    {
        if (animator != null)
        {
            animator.SetBool("character_nearby", shouldOpen);
        }
    }

    // 在编辑器中绘制检测范围（方便调试）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}