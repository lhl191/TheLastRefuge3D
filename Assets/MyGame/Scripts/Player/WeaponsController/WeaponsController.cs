using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Animator characterAnimator;
    public GameObject axePrefab;
    public GameObject bowPrefab;
    public Transform rightHandTransform;
    public Transform leftHandTransform;
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    private ThirdPersonController playerController;


    private GameObject currentWeapon;
    public string currentWeaponType = "noWeapon";
    private bool isAttacking = false;

    void Start()
    {
        playerController = FindFirstObjectByType<ThirdPersonController>(); // Lấy reference đến PlayerController

        if (characterAnimator == null)
        {
            characterAnimator = GetComponent<Animator>();
        }

        characterAnimator.ResetTrigger("AttackAxe");
        characterAnimator.ResetTrigger("ShootBow");

        isAttacking = false;  // 🔹 Đảm bảo không bị khóa ngay từ đầu

        if (currentWeaponType != "noWeapon")
        {
            SetWeapon(currentWeaponType);
        }
    }



    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking && currentWeaponType != "noWeapon")
        {
            Attack();
        }
    }

    public void SetWeapon(string weaponType)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        if (weaponType == "axe")
        {
            currentWeapon = Instantiate(axePrefab, rightHandTransform);
            currentWeapon.transform.localPosition = new Vector3(-0.1f, 0f, 0.04f);
            currentWeapon.transform.localRotation = Quaternion.Euler(0f, 250f, -58.22f);
        }
        else if (weaponType == "archery")
        {
            currentWeapon = Instantiate(bowPrefab, leftHandTransform);
            currentWeapon.transform.localPosition = new Vector3(0f, 0.045f, 0f);
            currentWeapon.transform.localRotation = Quaternion.Euler(50f, 170f, -105f);
        }
        else
        {
            currentWeapon = null;
        }

        currentWeaponType = weaponType;
        isAttacking = false;

       
    }



    void Attack()
    {
        if (characterAnimator == null || isAttacking) return;

        isAttacking = true;
        if (playerController != null)
            playerController.isAttacking = true;

        if (currentWeaponType == "axe")
        {
            characterAnimator.SetTrigger("AttackAxe");
            StartCoroutine(AxeDamageRaycast(0.5f)); 
        }
        else if (currentWeaponType == "archery")
        {
            characterAnimator.SetTrigger("ShootBow");
            StartCoroutine(ShootArrowWithDelay(1f));
        }
    }
    IEnumerator AxeDamageRaycast(float delay)
    {
        yield return new WaitForSeconds(delay);

        RaycastHit hit;
        if (Physics.Raycast(playerController.transform.position + Vector3.up * 1f, playerController.transform.forward, out hit, 2f))
        {
            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Boss") || hit.collider.CompareTag("Animal"))
            {
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(40f);
                }

                BossHealth bossHealth = hit.collider.GetComponent<BossHealth>();
                if (bossHealth != null)
                {
                    bossHealth.TakeDamage(40f);
                }

                AnimalHealth animalHealth = hit.collider.GetComponent<AnimalHealth>();
                if (animalHealth != null)
                {
                    animalHealth.TakeDamage(40f);
                }
            }
            TreeResource tree = hit.collider.GetComponent<TreeResource>();
            if (tree != null)
            {
                tree.ChopTree();  // Gọi hàm chặt cây
            }

        }
    }



    void ShootArrow()
    {
        if (arrowPrefab != null && arrowSpawnPoint != null)
        {
            Transform cam = Camera.main.transform;
            Vector3 shootDirection = cam.forward; // Mặc định bắn thẳng theo hướng camera

            RaycastHit hit;
            if (Physics.Raycast(cam.position, cam.forward, out hit, 100f))
            {
                // Nếu raycast trúng mục tiêu, ta vẫn giữ hướng gốc nhưng không đổi khi va chạm
                shootDirection = (hit.point - arrowSpawnPoint.position).normalized;
            }

            // 🔹 Tạo mũi tên với góc xoay đúng
            Quaternion arrowRotation = Quaternion.LookRotation(shootDirection) * Quaternion.Euler(90, 0, 0);
            GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowRotation);

            // 🔹 Lấy Rigidbody và thiết lập bay thẳng
            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Giúp va chạm chính xác
                rb.linearVelocity = shootDirection * 50f; // Bay thẳng, không đổi hướng
            }

            // 🔹 Kiểm tra nếu arrow có Collider, đảm bảo trigger để tính dame
            Collider arrowCollider = arrow.GetComponent<Collider>();
            if (arrowCollider != null)
            {
                arrowCollider.isTrigger = true; // Để tính sát thương nhưng không đổi hướng khi va chạm
            }
     
            Destroy(arrow, 5f);
        }
    }


    IEnumerator ShootArrowWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShootArrow();
    }

    public void EndAttackAnimation()
    {
        isAttacking = false;
        if (playerController != null)
            playerController.isAttacking = false; // 🔹 Cho phép di chuyển lại
    }


}
