using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveController : MonoBehaviourPun
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
        animator = GetComponentInChildren<Animator>(); // Получаем аниматор

        if (photonView.IsMine)
        {
            EnableLocalPlayerCamera();
        }
        else
        {
            DisableOtherPlayerCamera();
        }
    }

    private void EnableLocalPlayerCamera()
    {
        Debug.LogError("EnableOtherPlayerCamera");

        if (currentCamera != null)
        {
            currentCamera.gameObject.SetActive(true);
            currentCamera.enabled = true;
        }

        var audioListener = GetComponentInChildren<AudioListener>();
        if (audioListener != null)
        {
            audioListener.enabled = true;
        }
    }

    private void DisableOtherPlayerCamera()
    {
        Debug.LogError("DisableOtherPlayerCamera");
        if (currentCamera != null)
        {
            currentCamera.gameObject.SetActive(false);
            currentCamera.enabled = false;
        }

        var audioListener = GetComponentInChildren<AudioListener>();
        if (audioListener != null)
        {
            audioListener.enabled = false;
        }
    }

    private void OnDisable()
    {
        if (photonView.IsMine) gameState.UpdateGameState(GameState.Menu);
    }

    void Update()
    {
        if (!photonView.IsMine) return;

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

        Vector3 camForward = currentCamera.transform.forward;
        Vector3 camRight = currentCamera.transform.right;

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

        float forward = movement.magnitude > 0 ? Vector3.Dot(movement.normalized, camForward.normalized) : 0f;

        // Обновляем параметр Speed для Blend Tree
        if (animator != null)
        {
            animator.SetFloat("Speed", forward); // 0.1f - плавность перехода
        }
        else
        {
            Debug.LogError("Animator is null!");
        }
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