using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public float rotationSpeed = 1.0f;
    public float jumpSpeed = 1.0f;
    public float airMultiplier = 0.1f;
    public float maxMovementSpeed = 1.0f;
    public float maxAirSpeed = 1.0f;
    public float maxFallingSpeed = 1.0f;
    public float movementDamping = 1.0f;
    public float movementDampingInAir = 1.0f;

    CharacterController characterController;

    [SerializeField]
    Animator characterAnimator;

    [SerializeField]
    Vector3 velocity;
    [SerializeField]
    Vector3 airVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        velocity = new Vector3();
        airVelocity = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovementWowLike();

        characterAnimator.SetFloat("Vertical", Input.GetAxis("Vertical"));
        characterAnimator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
    }

    void UpdateMovementWowLike()
    {
        float input_vertical = Input.GetAxis("Vertical");
        float input_horizontal = Input.GetAxis("Horizontal");
        Vector3 move = transform.forward;
        move *= input_vertical;
        move += transform.right * input_horizontal;
        move = move.normalized * movementSpeed;

        if (characterController.isGrounded)
        {
            velocity = move;
            characterAnimator.SetBool("Grounded", true);
            characterAnimator.SetBool("Moving", velocity.sqrMagnitude != 0);


            if (Input.GetButtonDown("Jump"))
            {
                characterAnimator.SetTrigger("JumpTrigger");
                characterAnimator.SetBool("Grounded", false);
                airVelocity.y = jumpSpeed;
            }
        }
        else
        {
            characterAnimator.SetBool("Grounded", false);

            Vector3 dumping = velocity.normalized * -1 * movementDampingInAir * Time.deltaTime;
            velocity += dumping;

            move *= airMultiplier;
            characterController.Move(move * Time.deltaTime);

            airVelocity.y += Physics.gravity.y * Time.deltaTime;

        }

        characterController.Move(velocity * Time.deltaTime);
        characterController.Move(airVelocity * Time.deltaTime);



        //Handle the rotation by mouse
        if (!Input.GetButton("LockMouse"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            float rotationAngle = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.Rotate(transform.up, rotationAngle);
        }
        else
        {
            if (Cursor.lockState != CursorLockMode.None)
                Cursor.lockState = CursorLockMode.None;
        }
    }

    void UpdateMovementPrecise()
    {
        float input_vertical = Input.GetAxis("Vertical");
        float input_horizontal = Input.GetAxis("Horizontal");
        Vector3 move = transform.forward;
        move *= input_vertical * movementSpeed;
        move += transform.right * input_horizontal * movementSpeed;

        if (characterController.isGrounded)
        {
            characterController.Move(move * Time.deltaTime);

            if (Input.GetButtonDown("Jump"))
            {
                airVelocity.y = jumpSpeed;
            }
        }
        else
        {
            move *= airMultiplier;
            characterController.Move(move * Time.deltaTime);

            airVelocity.y += Physics.gravity.y * Time.deltaTime;
            
        }


        characterController.Move(airVelocity * Time.deltaTime);



        //Handle the rotation by mouse
        if (!Input.GetButton("LockMouse"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            float rotationAngle = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.Rotate(transform.up, rotationAngle);
        }
        else
        {
            if (Cursor.lockState != CursorLockMode.None)
                Cursor.lockState = CursorLockMode.None;
        }
    }

    void UpdateMovementFinal()
    {
        float input_vertical = Input.GetAxis("Vertical");
        float input_horizontal = Input.GetAxis("Horizontal");
        Vector3 move = transform.forward;
        move *= input_vertical;
        move += transform.right * input_horizontal;
        move = move.normalized * movementSpeed;

        if (characterController.isGrounded)
        {
            characterAnimator.SetBool("Grounded", true);

            if (move.sqrMagnitude != 0)//Is input
            {
                velocity += move;
            }
            else if (velocity.sqrMagnitude > maxMovementSpeed * 0.2f)
            {
                Vector3 dumping = velocity.normalized * -1 * movementDamping;
                velocity += dumping;
            }
            else
            {
                velocity = Vector3.zero;
            }

            characterAnimator.SetBool("Moving", velocity.sqrMagnitude != 0);

            if (Input.GetButtonDown("Jump"))
            {
                characterAnimator.SetTrigger("JumpTrigger");
                characterAnimator.SetBool("Grounded", false);
                airVelocity.y = jumpSpeed;
            }
        }
        else
        {
            characterAnimator.SetBool("Grounded", false);

            Vector3 dumping = velocity.normalized * -1 * movementDampingInAir;
            velocity += dumping;

            move *= airMultiplier;
            characterController.Move(move * Time.deltaTime);

            airVelocity.y += Physics.gravity.y * Time.deltaTime;

        }


        //Handle the rotation by mouse
        if (!Input.GetButton("LockMouse"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            float rotationAngle = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.Rotate(transform.up, rotationAngle);
        }
        else
        {
            if (Cursor.lockState != CursorLockMode.None)
                Cursor.lockState = CursorLockMode.None;
        }

        velocity = velocity.normalized * Mathf.Min(velocity.magnitude, maxMovementSpeed);
        //velocity = transform.forward * velocity.magnitude;

        characterController.Move(velocity * Time.deltaTime);
        characterController.Move(airVelocity * Time.deltaTime);



        
    }
}
