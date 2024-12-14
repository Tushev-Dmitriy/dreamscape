using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveController : MonoBehaviour
{
    public float speed = 15.0f;
    public float rotateSpeed = 2.5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2.0f;

    public float lookSpeed = 2.5f; // Скорость поворота камеры
    public float maxLookAngle = 60f;

    public Transform playerCamera;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private float cameraVerticalRotation = 0f; // Текущий угол поворота камеры по вертикали

    private Vector3 playerMainPos = new Vector3(0, 1f, 0);

    [SerializeField] private GameStateSO gameState;
    private bool cursorLocked = true;
    
    [Header("test")]
    public Camera currentCamera;
    
    private Animator animator; // Ссылка на аниматор

    void OnEnable()
    {
        controller = GetComponent<CharacterController>();
       // animator = GetComponentInChildren<Animator>(); // Получаем аниматор
        // LockCursor();
    }

    private void OnDisable()
    {
        gameState.UpdateGameState(GameState.Menu);
    }

    void Update()
    {
        if (gameState.CurrentGameState != GameState.Gameplay)
        {
            var camera = GetComponentInChildren<Camera>().enabled = false;
        }
        else
        {
            var camera = GetComponentInChildren<Camera>().enabled = true;
        }
        
        if (gameState.CurrentGameState == GameState.Gameplay)
        {
            if (cursorLocked)
            {
                LockCursor();
                HandleMovement();
                HandleLook();
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (cursorLocked)
                {
                    UnlockCursor();
                }
                else
                {
                    cursorLocked = true;
                }

                return;
            }
        }
    }

    private void HandleMovement()
    {
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
        
        float movementSpeed = new Vector3(horizontal, 0, vertical).magnitude;
      //  animator.SetFloat("Speed", movementSpeed);
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up, mouseX * rotateSpeed);

        float mouseY = Input.GetAxis("Mouse Y");
        cameraVerticalRotation -= mouseY * lookSpeed;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -maxLookAngle, maxLookAngle);

        playerCamera.localRotation = Quaternion.Euler(cameraVerticalRotation, 0f, 0f);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorLocked = true;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorLocked = false;
    }

    public void SetPlayerPos()
    {
        controller.enabled = false;
        transform.position = playerMainPos;
        velocity = Vector3.zero;
        controller.enabled = true;
    }
}