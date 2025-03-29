using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public float velocity = 5f;
    public float sprintAdittion = 3.5f;
    public float jumpForce = 5f;
    public float jumpTime = 0.6f;
    public float gravity = 9.8f;

    private float jumpElapsedTime = 0;
    private bool isJumping = false;
    private bool isSprinting = false;
    private bool isCrouching = false;

    private float inputHorizontal;
    private float inputVertical;
    private bool inputJump;
    private bool inputSprint;
    private bool inputCrouch; // Thêm biến kiểm tra phím ngồi
    public bool isAttacking = false;



    private Animator animator;
    private CharacterController cc;
    private WeaponController weaponController; // Thêm biến để điều khiển vũ khí

    private GameObject currentWeapon; // Vũ khí hiện tại

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        weaponController = GetComponent<WeaponController>(); // Lấy WeaponController

        if (animator == null)
            Debug.LogWarning("Không có Animator component, animation sẽ không hoạt động.");
    }

    void Update()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputJump = Input.GetKeyDown(KeyCode.Space);
        inputSprint = Input.GetAxis("Fire3") == 1f;
        inputCrouch = Input.GetKey(KeyCode.LeftControl);

        if (isAttacking)
        {
            inputHorizontal = 0;
            inputVertical = 0;
            inputJump = false;
            inputSprint = false;
            inputCrouch = false;
            return;
        }


        // Cập nhật trạng thái ngồi
        isCrouching = inputCrouch;
        animator.SetBool("crouch", isCrouching);

        if (Input.GetMouseButtonDown(0) && weaponController != null && weaponController.currentWeaponType == "archery")
        {
            // Nhân vật xoay theo hướng camera khi bắn
            transform.rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
        }

        // **Thay đổi vũ khí bằng phím số**
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
    }

    private void FixedUpdate()
    {
        if (isAttacking) return;

        float velocityAdittion = isSprinting ? sprintAdittion : 0;
        velocityAdittion -= isCrouching ? velocity * 0.5f : 0; // Giảm tốc độ khi ngồi


        // **Thay đổi chiều cao của CharacterController khi ngồi**
        cc.height = isCrouching ? 1f : 2f;
        cc.center = new Vector3(0, isCrouching ? 0.5f : 1f, 0); // Dịch chuyển trọng tâm tránh lỗi va chạm

        Vector3 moveDirection = new Vector3(inputHorizontal, 0, inputVertical).normalized;
        moveDirection *= (velocity + velocityAdittion) * Time.deltaTime;
        moveDirection = Camera.main.transform.TransformDirection(moveDirection);
        moveDirection.y = isJumping ? Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime : -gravity * Time.deltaTime;

        // 🔹 Nếu nhân vật đang di chuyển, thì thay đổi góc quay
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
        // Chỉ gọi hàm đổi vũ khí trong WeaponController, không tự tạo vũ khí trong script này
        if (weaponController != null)
        {
            weaponController.SetWeapon(weaponType);
        }
    }
}
