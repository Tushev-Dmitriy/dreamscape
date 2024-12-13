using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float warkSpeed = 4f;
    public float maxVelocityChange = 10f;
    public float sprintSpeed = 14f;

    [Space]
    public float jumpungHeight = 5f;

    [Space]
    public float airControl = 0.5f;

    private Vector3 _input;
    private Rigidbody _rb;

    private bool _sprinting;
    private bool _jumping;

    private bool _grounded = false;
    

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _input.Normalize();

        _sprinting = Input.GetButton("Sprint");
        _jumping = Input.GetButton("Jump");
    }

    private void FixedUpdate()
    {
        if (_grounded)
        {
            if (_jumping)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, jumpungHeight, _rb.velocity.z);
            }
            else if (_input.magnitude > 0.5f)
            {
                _rb.AddForce(CalculateMovement(_sprinting ? sprintSpeed : warkSpeed), ForceMode.VelocityChange);
            }
            else
            {
                var velocity = _rb.velocity;
                velocity = new Vector3(velocity.x * 0.2f * Time.deltaTime, velocity.y, velocity.z * 0.2f * Time.deltaTime);
                _rb.velocity = velocity;
            }
        }
        else
        {
            if (_input.magnitude > 0.5f)
            {
                _rb.AddForce(CalculateMovement(_sprinting ? sprintSpeed * airControl : warkSpeed * airControl), ForceMode.VelocityChange);
            }
            else
            {
                var velocity = _rb.velocity;
                velocity = new Vector3(velocity.x * 0.2f * Time.deltaTime, velocity.y, velocity.z * 0.2f * Time.deltaTime);
                _rb.velocity = velocity;
            }
        }


        _grounded = false;

    }

    private void OnTriggerStay(Collider other)
    {
        _grounded = true;
    }

    private Vector3 CalculateMovement(float speed)
    {
        Vector3 targetVelocity = new Vector3(_input.x, 0, _input.y);
        targetVelocity = transform.TransformDirection(targetVelocity);

        targetVelocity *= speed;

        Vector3 velocity = _rb.velocity;

        if (_input.magnitude > 0.5f)
        {
            Vector3 velocityChange = targetVelocity - velocity;

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);

            velocityChange.y = 0;

            return (velocityChange);
        }
        else
        {
            return new Vector3();
        }  
    }
}
