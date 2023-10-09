using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpController : MonoBehaviour
{
    private CharacterController characterController;
    
    [SerializeField]
    float speed = 20f;
    
    [SerializeField]
    float gravity = -19.62f;
    
    Vector3 velocity;
    
    [SerializeField]
    Transform groundCheck;
    
    [SerializeField]
    float groundDistance = 0.4f;
    
    [SerializeField]
    LayerMask groundMask;

    public bool isGrounded;

    public bool freeze;

    public bool activeGrapple;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && activeGrapple == false)
        {
            velocity = new Vector3(0, 0, 0);
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        Vector3 move = transform.right * x + transform.forward * z;

        characterController.Move(move * Time.deltaTime * speed);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        if (freeze || activeGrapple)
        {
            speed = 0;
        }
        else if (freeze == false && activeGrapple == false && isGrounded == true)
        {
            speed = 20;
        }
        else if (!isGrounded)
        {
            speed = 7;
        }
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;
        
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        velocity = velocityToSet * 1.3f;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
}
