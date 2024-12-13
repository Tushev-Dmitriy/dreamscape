using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarRotation : MonoBehaviour
{
    public float sensitivityX = 2.5f;  // Чувствительность мыши по оси X
    private float rotationY = 0f;      // Текущий угол вращения объекта по оси Y

    void Update()
    {
        if (Input.GetMouseButton(0)) // 0 - это ЛКМ
        {
            float mouseX = Input.GetAxis("Mouse X");

            rotationY += mouseX * sensitivityX;

            transform.rotation = Quaternion.Euler(0f, (transform.rotation.y + rotationY), 0f);
        }
    }
}
