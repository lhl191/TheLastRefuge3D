using System.Collections;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public float velocity = 5f;
    public float sprintAdittion = 3.5f;
    public float jumpForce = 5f;
    public float jumpTime = 1f;
    public float gravity = 9.8f;

    private float jumpElapsedTime = 0;
    private bool isJumping = false;
    private bool isSprinting = false;
    private bool isCrouching = false;

    private float inputHorizontal;
    private float inputVertical;
    private bool inputJump;
    private bool inputSprint;
    private bool inputCrouch; 
    public bool isAttacking = false;
    private bool isDead = false;
    private bool isPickingUp = false;

    [Header("Footstep Sounds")]
    public AudioClip walkSound;
    public AudioClip runSound;
    private AudioSource audioSource;


    private Animator animator;
    private CharacterController cc;
    private WeaponController weaponController; 

    private GameObject currentWeapon; 
    private InteractableObjects nearObject = null;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        weaponController = GetComponent<WeaponController>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = true; 


    }

    void Update()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputJump = Input.GetKeyDown(KeyCode.Space);
        inputSprint = Input.GetAxis("Fire3") == 1f;
        inputCrouch = Input.GetKey(KeyCode.LeftControl);

        if (isDead) return;

        if (Input.GetKeyDown(KeyCode.E) && nearObject != null)
        {
            nearObject.Interact();
        }
        if (Input.GetKeyDown(KeyCode.E) && nearObject != null && !isPickingUp)
        {
            StartCoroutine(PickupCoroutine());
        }
        if (isAttacking)
        {
            inputHorizontal = 0;
            inputVertical = 0;
            inputJump = false;
            inputSprint = false;
            inputCrouch = false;
            return;
        }


      
        isCrouching = inputCrouch;
        animator.SetBool("crouch", isCrouching);

        if (Input.GetMouseButtonDown(0) && weaponController != null && weaponController.currentWeaponType == "archery")
        {
        
            transform.rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
        }

       
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon("axe");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon("archery");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon("noWeapon");
        }

        if (cc.isGrounded)
        {
            animator.SetBool("run", inputHorizontal != 0 || inputVertical != 0);
            isSprinting = cc.velocity.magnitude > 0.9f && inputSprint;
            animator.SetBool("sprint", isSprinting);
        }

        animator.SetBool("jump", !cc.isGrounded);
        if (inputJump && cc.isGrounded)
        {
            isJumping = true;
        }

        HeadHittingDetect();
        HandleFootsteps();
    }

    private void FixedUpdate()
    {
        if (isAttacking) return;

        float velocityAdittion = isSprinting ? sprintAdittion : 0;
        velocityAdittion -= isCrouching ? velocity * 0.5f : 0; // Giảm tốc độ khi ngồi


        // **Thay đổi chiều cao của CharacterController khi ngồi**
        cc.height = isCrouching ? 1f : 2f;
        cc.center = new Vector3(0, isCrouching ? 0.5f : 1f, 0);

        Vector3 moveDirection = new Vector3(inputHorizontal, 0, inputVertical).normalized;
        moveDirection *= (velocity + velocityAdittion) * Time.deltaTime;
        moveDirection = Camera.main.transform.TransformDirection(moveDirection);
        moveDirection.y = isJumping ? Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime : -gravity * Time.deltaTime;

       
        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        cc.Move(moveDirection);

        if (isJumping)
        {
            jumpElapsedTime += Time.deltaTime;
            if (jumpElapsedTime >= jumpTime)
            {
                isJumping = false;
                jumpElapsedTime = 0;
            }
        }
    }
    void HandleFootsteps()
    {
        if (!cc.isGrounded || isAttacking || isDead)
        {
            if (audioSource.isPlaying) audioSource.Stop();
            return;
        }

        bool isMoving = Mathf.Abs(inputHorizontal) > 0.1f || Mathf.Abs(inputVertical) > 0.1f;

        if (isMoving)
        {
            AudioClip clipToPlay = isSprinting ? runSound : walkSound;

            
            audioSource.pitch = isSprinting ? 1.1f : 0.9f;
          

            if (audioSource.clip != clipToPlay)
            {
                audioSource.clip = clipToPlay;
                audioSource.Play();
            }
            else if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying) audioSource.Stop();
        }
    }



    void HeadHittingDetect()
    {
        if (Physics.Raycast(transform.TransformPoint(cc.center), Vector3.up, cc.height / 2f * 1.1f))
        {
            jumpElapsedTime = 0;
            isJumping = false;
        }
    }

    public void SwitchWeapon(string weaponType)
    {
       
        if (weaponController != null)
        {
            weaponController.SetWeapon(weaponType);
        }
    }
    public void KillPlayer()
    {
        StartCoroutine(DieCoroutine());
    }
    IEnumerator DieCoroutine()
    {
        isDead = true;
        SwitchWeapon("noWeapon");

        animator.SetBool("Die", true);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        this.enabled = false;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable")) 
        {
            nearObject = other.GetComponent<InteractableObjects>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            nearObject = null;
        }
    }
    IEnumerator PickupCoroutine()
    {
        isPickingUp = true;
        animator.SetTrigger("pickUp");

        yield return new WaitForSeconds(0.5f); 
        if (nearObject != null)
        {
            if (nearObject.interactUI != null)
            {
                nearObject.interactUI.SetActive(false); 
            }

            nearObject.Interact();
            nearObject = null; 
        }

        isPickingUp = false;
    }

}
