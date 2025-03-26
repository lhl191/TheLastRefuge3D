using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Animator characterAnimator;
    public GameObject axePrefab;
    public GameObject bowPrefab;
    public Transform rightHandTransform;
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;

    private GameObject currentWeapon;
    private string currentWeaponType = "noWeapon";
    private bool isAttacking = false; // 🔹 Kiểm soát trạng thái tấn công

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking) // 🔹 Chỉ cho phép tấn công khi chưa có animation đang chạy
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
            currentWeapon = Instantiate(bowPrefab, rightHandTransform);
            currentWeapon.transform.localPosition = new Vector3(0f, 0.1f, 0.05f);
            currentWeapon.transform.localRotation = Quaternion.Euler(0f, 170f, -80f);
        }
        else
        {
            currentWeapon = null;
        }

        currentWeaponType = weaponType;
    }

    void Attack()
    {
        if (characterAnimator == null) return;

        isAttacking = true; // 🔹 Ngăn chặn spam attack

        if (currentWeaponType == "axe")
        {
            characterAnimator.SetTrigger("AttackAxe");
        }
        else if (currentWeaponType == "archery")
        {
            characterAnimator.SetTrigger("ShootBow");
            ShootArrow();
        }
    }

    void ShootArrow()
    {
        if (arrowPrefab != null && arrowSpawnPoint != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = arrowSpawnPoint.forward * 25f;
            }
        }
    }

    // 🔹 Gọi từ Animation Event khi animation kết thúc
    public void EndAttackAnimation()
    {
        isAttacking = false; 
    }
}
