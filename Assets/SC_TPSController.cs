using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class SC_TPSController : MonoBehaviour
{
    public float speed = 7.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Transform playerCameraParent;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;
    AudioSource audioSource;
    CharacterController characterController;
    Animator animator;
    Vector3 moveDirection = Vector3.zero;
    Vector2 rotation = Vector2.zero;

    [HideInInspector]
    public bool canMove = true;
    public bool MoveEnable = true;
    public bool forward_run;
    public bool backward_run;
    public bool right_run;
    public bool left_run;

    public float cooldown_timer;
    public float roll_cooldown = 1f;

    public GameObject choroon;
    public AudioClip[] heal_sounds;
    public AudioClip[] detail_sounds;
    private int combo_attack;
    private float combo_default_time;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        rotation.y = transform.eulerAngles.y;
    }

    void Update()
    {
        MoveDirection();

        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate move direction based on axes
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);


            animator.SetFloat("directionZ", curSpeedX);
            animator.SetFloat("directionX", curSpeedY);

            if (Input.GetKey(KeyCode.Space) && canMove && forward_run && cooldown_timer == 0.0f)
            {
                cooldown_timer = roll_cooldown;
                animator.SetTrigger("roll_forward");
            }
            if (Input.GetKey(KeyCode.Space) && canMove && right_run && cooldown_timer == 0.0f)
            {
                cooldown_timer = roll_cooldown;
                animator.SetTrigger("roll_right");
            }
            if (Input.GetKey(KeyCode.Space) && canMove && left_run && cooldown_timer == 0.0f)
            {
                cooldown_timer = roll_cooldown;
                animator.SetTrigger("roll_left");
            }
            if (Input.GetKey(KeyCode.Space) && canMove && backward_run && cooldown_timer == 0.0f)
            {
                cooldown_timer = roll_cooldown;
                animator.SetTrigger("roll_backward");
            }

            if (Input.GetKey(KeyCode.E) && cooldown_timer == 0.0f)
            {
                audioSource.PlayOneShot(detail_sounds[0]);
                audioSource.PlayOneShot(heal_sounds[Random.Range(0, heal_sounds.Length)]);
                cooldown_timer = 2f;
                animator.SetTrigger("drink");
                choroon.SetActive(true);
            }
            else if (
                cooldown_timer == 0.0f
            )
            {
                choroon.SetActive(false);
            }


            if (Input.GetButton("Fire1") && cooldown_timer == 0.0f)
            {
                if (combo_attack == 0)
                {
                    combo_attack = 1;
                    cooldown_timer = 1.0f;
                    combo_default_time = 1.5f;
                    animator.SetTrigger("attack_1");
                }
                else if (combo_attack == 1)
                {
                    combo_attack = 2;
                    cooldown_timer = 1.3f;
                    combo_default_time = 1.6f;
                    animator.SetTrigger("attack_2");
                }
                else if (combo_attack == 2)
                {
                    combo_attack = 0;
                    cooldown_timer = 1.5f;
                    combo_default_time = 1.8f;
                    animator.SetTrigger("attack_3");
                }
            }
        }
        animator.SetBool("isgrounded", characterController.isGrounded);
        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller

        characterController.Move(moveDirection * Time.deltaTime);


        // Player and Camera rotation
        if (canMove)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
            playerCameraParent.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            transform.eulerAngles = new Vector2(0, rotation.y);
        }

        Cooldown_Timer();
    }



    void MoveDirection()
    {
        if (Input.GetAxis("Vertical") > 0.0f)
        {
            forward_run = true;
            backward_run = false;
        }
        else
        {
            forward_run = false;
        }
        if (Input.GetAxis("Vertical") < 0.0f)
        {
            forward_run = false;
            backward_run = true;
        }
        else
        {
            backward_run = false;
        }
        if (Input.GetAxis("Horizontal") < 0.0f)
        {
            left_run = true;
            right_run = false;
        }
        else
        {
            left_run = false;
        }
        if (Input.GetAxis("Horizontal") > 0.0f)
        {
            left_run = false;
            right_run = true;
        }
        else
        {
            right_run = false;
        }
    }

    void Cooldown_Timer()
    {
        if (cooldown_timer < 0.0f)
        {
            cooldown_timer = 0.0f;
        }
        if (cooldown_timer > 0.0f)
        {
            cooldown_timer = cooldown_timer - Time.deltaTime;
        }

        if (combo_default_time < 0.0f)
        {
            combo_default_time = 0.0f;
            combo_attack = 0;
        }
        if (combo_default_time > 0.0f)
        {
            combo_default_time = combo_default_time - Time.deltaTime;
        }
    }
}