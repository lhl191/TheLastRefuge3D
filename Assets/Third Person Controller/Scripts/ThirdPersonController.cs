using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public float velocity = 5f;
    public float sprintAdittion = 3.5f;
    public float jumpForce = 18f;
    public float jumpTime = 0.85f;
    public float gravity = 9.8f;

    float jumpElapsedTime = 0;
    bool isJumping = false;
    bool isSprinting = false;
    bool isCrouching = false;

    float inputHorizontal;
    float inputVertical;
    bool inputJump;
    bool inputCrouch;
    bool inputSprint;

    Animator animator;
    CharacterController cc;

    // Các Prefab và Animator cho từng vũ khí
    public GameObject axePrefab; // Prefab cho rìu
    public GameObject bowPrefab; // Prefab cho cung
    public Animator axeAnimator; // Animator cho rìu
    public Animator archeryAnimator; // Animator cho cung

    private GameObject currentWeapon; // Vũ khí hiện tại
    private Animator currentWeaponAnimator; // Animator của vũ khí hiện tại

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogWarning("Không có Animator component, animation sẽ không hoạt động.");

        // Bắt đầu với không vũ khí
        currentWeapon = null;
        currentWeaponAnimator = animator;
    }

    void Update()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputJump = Input.GetAxis("Jump") == 1f;
        inputSprint = Input.GetAxis("Fire3") == 1f;

        // Kiểm tra các phím để thay đổi vũ khí
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Ấn phím 1 để lấy vũ khí rìu
        {
            SwitchWeapon("axe");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // Ấn phím 2 để lấy vũ khí cung
        {
            SwitchWeapon("archery");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // Ấn phím 3 để không vũ khí
        {
            SwitchWeapon("noWeapon");
        }

        // Kiểm tra trạng thái ngồi
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            isCrouching = !isCrouching;
        }

        if (cc.isGrounded && animator != null)
        {
            animator.SetBool("crouch", isCrouching);

            if (isCrouching && (inputHorizontal != 0 || inputVertical != 0))
            {
                animator.SetBool("run", true);
            }
            else if (!isCrouching && (inputHorizontal != 0 || inputVertical != 0))
            {
                animator.SetBool("run", true);
            }
            else
            {
                animator.SetBool("run", false);
            }

            isSprinting = cc.velocity.magnitude > 0.9f && inputSprint;
            animator.SetBool("sprint", isSprinting);
        }

        if (animator != null)
            animator.SetBool("air", !cc.isGrounded);

        if (inputJump && cc.isGrounded)
        {
            isJumping = true;
        }

        HeadHittingDetect();
    }

    private void FixedUpdate()
    {
        float velocityAdittion = 0;
        if (isSprinting)
            velocityAdittion = sprintAdittion;
        if (isCrouching)
            velocityAdittion = -(velocity * 0.50f);

        float directionX = inputHorizontal * (velocity + velocityAdittion) * Time.deltaTime;
        float directionZ = inputVertical * (velocity + velocityAdittion) * Time.deltaTime;
        float directionY = 0;

        if (isJumping)
        {
            directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime;
            jumpElapsedTime += Time.deltaTime;
            if (jumpElapsedTime >= jumpTime)
            {
                isJumping = false;
                jumpElapsedTime = 0;
            }
        }

        directionY = directionY - gravity * Time.deltaTime;

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        forward = forward * directionZ;
        right = right * directionX;

        if (directionX != 0 || directionZ != 0)
        {
            float angle = Mathf.Atan2(forward.x + right.x, forward.z + right.z) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
        }

        Vector3 verticalDirection = Vector3.up * directionY;
        Vector3 horizontalDirection = forward + right;

        Vector3 moviment = verticalDirection + horizontalDirection;
        cc.Move(moviment);
    }

    void HeadHittingDetect()
    {
        float headHitDistance = 1.1f;
        Vector3 ccCenter = transform.TransformPoint(cc.center);
        float hitCalc = cc.height / 2f * headHitDistance;

        if (Physics.Raycast(ccCenter, Vector3.up, hitCalc))
        {
            jumpElapsedTime = 0;
            isJumping = false;
        }
    }

    // Hàm chuyển đổi vũ khí
    public Transform rightHandTransform; // Đây là Transform của tay phải trong mô hình nhân vật

    public void SwitchWeapon(string weaponType)
    {
        // Tắt vũ khí hiện tại
        if (currentWeapon != null)
        {
            Destroy(currentWeapon); // Xóa vũ khí hiện tại
        }

        switch (weaponType)
        {
            case "axe":
                currentWeapon = Instantiate(axePrefab, rightHandTransform.position, Quaternion.identity, rightHandTransform); // Gắn vũ khí rìu vào tay phải
                currentWeaponAnimator = axeAnimator; // Set animator của rìu
                break;
            case "archery":
                currentWeapon = Instantiate(bowPrefab, rightHandTransform.position, Quaternion.identity, rightHandTransform); // Gắn vũ khí cung vào tay phải
                currentWeaponAnimator = archeryAnimator; // Set animator của cung
                break;
            case "noWeapon":
                currentWeapon = null; // Không có vũ khí
                currentWeaponAnimator = animator; // Set lại animator cho không vũ khí
                break;
            default:
                currentWeapon = null;
                currentWeaponAnimator = animator;
                break;
        }

        // Điều chỉnh vị trí và góc quay của vũ khí
        if (currentWeapon != null)
        {
            if (weaponType == "axe")
            {
                // Điều chỉnh vị trí và góc quay của rìu
                currentWeapon.transform.localPosition = new Vector3(-0.1f, 0f, 0.04f); // Vị trí của rìu
                currentWeapon.transform.localRotation = Quaternion.Euler(0f, 250f, -58.22f); // Góc quay của rìu
            }
            else if (weaponType == "archery")
            {
                // Điều chỉnh vị trí và góc quay của cung
                currentWeapon.transform.localPosition = new Vector3(0f, 0.1f, 0.05f); // Vị trí của cung (có thể điều chỉnh tùy theo mô hình cung)
                currentWeapon.transform.localRotation = Quaternion.Euler(0f, 170f, -80f); // Góc quay của cung (có thể điều chỉnh tùy theo mô hình cung)
            }
        }

        // Cập nhật animator
        animator.runtimeAnimatorController = currentWeaponAnimator.runtimeAnimatorController;
    }

}
