using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    float yRotation = 0f;

    public float topClamp = -90f;
    public float bottomClamp = 90f;

    void Start()
    {
        // Locking the cursor to the middle of the screen and making it invisible 
        // 锁定光标
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Getting the mouse inputs
        // 获取输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotation aroud the x axis (Look up an down)
        // 上下看
        xRotation -= mouseY;

        // Clamp the rotation
        // 上下视角限制
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        // Rotation aroud the y axis (left an right)
        // 左右看
        yRotation += mouseX;

        // Apply rotations to our transform
        // 执行旋转
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}