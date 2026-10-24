using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovemont : MonoBehaviour
{
    private CharacterController controller;

    public float speed = 0.05f;
    public float fastSpeed = 2f;
    public float gravity = -9.18f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;
    bool isMoving;

    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Ground check
        // 角色是否处于地面
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        // Resseting the default velocity
        // 速度调整
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Getting the inputs
        // 获取输入
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Creating the move vector
        // 移动调整
        Vector3 move = transform.right * x + transform.forward * z;

        // Actually moving the player
        // 玩家实际移动
       
        // ========== 奔跑 ==========
        // 按住 Left Shift 使用 fastSpeed，否则使用普通 speed
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : speed;
        
        // 使用 currentSpeed 代替原来的 speed
        controller.Move(move * currentSpeed * Time.deltaTime);
        // =====================================

        // check if the player can jump
        // 跳跃检测
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            // Actuall jumping
            // 跳跃
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            SoundManager.Instance.playerChannel2.Stop();
            SoundManager.Instance.playerChannel2.PlayOneShot(SoundManager.Instance.playerJump);
        }

        // Falling down 
        // 下落
        velocity.y += gravity * Time.deltaTime;

        // Exectuting the jump 
        // 执行跳跃
        controller.Move(velocity * Time.deltaTime);

        if(lastPosition != gameObject.transform.position && isGrounded == true)
        {
            isMoving = true;

            if (Input.GetKey(KeyCode.LeftShift) && SoundManager.Instance.playerChannel2.isPlaying == false)
            {
                SoundManager.Instance.playerChannel2.PlayOneShot(SoundManager.Instance.playerRuning);             
            }
            else if(SoundManager.Instance.playerChannel2.isPlaying == false)
            {
                SoundManager.Instance.playerChannel2.PlayOneShot(SoundManager.Instance.playerMoving);
            }
            
        }
        else
        {
            isMoving = false;

            SoundManager.Instance.playerChannel2.Stop();
        }

        lastPosition = gameObject.transform.position;
    }
}