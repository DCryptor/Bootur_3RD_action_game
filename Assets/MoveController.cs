using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public bool isGrounded;
    public bool isMove = true;
    public float speed;
    public LayerMask groundLayer;
    private CapsuleCollider capsuleCollider;
    public Rigidbody rb;
    public Animator animator;
    public Vector3 moveDirection;
    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, capsuleCollider.bounds.min.y, transform.position.z), 0.2f, groundLayer);
        animator.SetBool("isgrounded", isGrounded);

        if (Input.GetButton("Jump") && isGrounded && isMove)
        {
            animator.SetTrigger("jump");
            isMove = false;
        }

        if (Input.GetKey(KeyCode.LeftShift) && isMove && isGrounded)
        {
            if (Input.GetAxis("Vertical") > 0f)
            {
                animator.SetTrigger("roll_forward");
            }
            else if (Input.GetAxis("Vertical") < 0f)
            {
                animator.SetTrigger("roll_backward");
            }
            else if (Input.GetAxis("Horizontal") < 0f)
            {
                animator.SetTrigger("roll_left");
            }
            else if (Input.GetAxis("Horizontal") > 0f)
            {
                animator.SetTrigger("roll_right");
            }
        }

        moveDirection.z = Input.GetAxis("Vertical");
        moveDirection.x = Input.GetAxis("Horizontal");

        if (isMove)
        {
            animator.SetFloat("directionZ", moveDirection.z);
            animator.SetFloat("directionX", moveDirection.x);
            moveDirection.Normalize();
            transform.Translate(moveDirection * speed * Time.deltaTime);

        }
    }

    public void Jump()
    {
        rb.AddForce(Vector3.up * 12f, ForceMode.Impulse);
    }
    public void Landing()
    {
        isMove = true;
    }
}
