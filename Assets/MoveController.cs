using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public float speed = 15.0f;
    public float rotateSpeed = 5.0f;
    private Vector3 playerRoomPos = new Vector3(0, 1f, 100);
    private Vector3 playerMainPos = new Vector3(0, 1f, 0);

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up, mouseX);

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        Vector3 movement = camForward * vertical + camRight * horizontal;

        controller.Move(movement * speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, 1f, transform.position.z);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed *= 2;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed /= 2;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.position = playerRoomPos;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            gameObject.transform.position = playerMainPos;
        }
    }

}
