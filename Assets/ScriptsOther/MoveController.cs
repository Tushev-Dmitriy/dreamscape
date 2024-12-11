using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public float speed = 15.0f;
    public float rotateSpeed = 2.5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2.0f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Vector3 playerMainPos = new Vector3(0, 1f, 0);

    [SerializeField] private GameStateSO gameState;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (gameState.CurrentGameState == GameState.Gameplay) {
            isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;

            camForward.y = 0;
            camRight.y = 0;

            Vector3 movement = camForward.normalized * vertical + camRight.normalized * horizontal;

            controller.Move(movement * speed * Time.deltaTime);

            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, mouseX * rotateSpeed);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speed *= 2;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                speed /= 2;
            }
        }
    }

    public void SetPlayerPos()
    {
        controller.enabled = false;
        transform.position = playerMainPos;
        velocity = Vector3.zero;
        controller.enabled = true;
    }
}
