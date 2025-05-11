using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Animator characterAnimator;
    public GameObject axePrefab;
    public GameObject bowPrefab;
    public Transform rightHandTransform;
    public Transform leftHandTransform;

    private ThirdPersonController playerController;

    public GameObject arrowEffectPrefab;
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public AudioClip arrowShootSound;
    public AudioClip axeSwingSound;
    public AudioClip weaponChangeSound;

    public GameObject axeSlashEffectPrefab;

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

        isAttacking = false; 

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
        // Xoá vũ khí hiện tại nếu có
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }

        // Cập nhật vũ khí mới
        switch (weaponType)
        {
            case "axe":
                currentWeapon = Instantiate(axePrefab, rightHandTransform);
                currentWeapon.transform.localPosition = new Vector3(-0.1f, 0f, 0.04f);
                currentWeapon.transform.localRotation = Quaternion.Euler(0f, 250f, -58.22f);
                WeaponManager.CurrentWeapon = WeaponManager.WeaponType.Axe;
                break;

            case "archery":
                currentWeapon = Instantiate(bowPrefab, leftHandTransform);
                currentWeapon.transform.localPosition = new Vector3(0f, 0.045f, 0f);
                currentWeapon.transform.localRotation = Quaternion.Euler(50f, 170f, -105f);
                WeaponManager.CurrentWeapon = WeaponManager.WeaponType.Bow;
                break;

            case "noWeapon":
            default:
            
                currentWeapon = null;
                WeaponManager.CurrentWeapon = WeaponManager.WeaponType.NoWeapon;
                break;
        }

        currentWeaponType = weaponType;
        isAttacking = false;

      
        if (weaponChangeSound != null)
        {
            AudioSource.PlayClipAtPoint(weaponChangeSound, transform.position);
        }
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

          
            StartCoroutine(PlayAxeSwingSoundWithDelay(0.4f));

            StartCoroutine(AxeDamageRaycast(0.5f));
        }
        else if (currentWeaponType == "archery")
        {
            characterAnimator.SetTrigger("ShootBow");
            StartCoroutine(ShootArrowWithDelay(1f));
        }
    }
    IEnumerator PlayAxeSwingSoundWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

     
        if (axeSwingSound != null)
        {
            AudioSource.PlayClipAtPoint(axeSwingSound, transform.position);
        }
    }

    IEnumerator AxeDamageRaycast(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (axeSlashEffectPrefab != null)
        {
            Vector3 slashSpawnPos = playerController.transform.position + playerController.transform.forward * 1.5f + Vector3.up * 0.7f; 
            Quaternion slashSpawnRot = Quaternion.LookRotation(playerController.transform.forward);

            GameObject slashEffect = Instantiate(axeSlashEffectPrefab, slashSpawnPos, slashSpawnRot);
            Destroy(slashEffect, 1.5f);
        }

        Collider[] hits = Physics.OverlapSphere(playerController.transform.position + playerController.transform.forward * 1.5f + Vector3.up * 1f, 1f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                    enemyHealth.TakeDamage(40f, "axe");
            }
            else if (hit.CompareTag("Boss"))
            {
                BossHealth bossHealth = hit.GetComponent<BossHealth>();
                if (bossHealth != null)
                    bossHealth.TakeDamage(40f, "axe");
            }
            else if (hit.CompareTag("Animal"))
            {
                AnimalHealth animalHealth = hit.GetComponent<AnimalHealth>();
                if (animalHealth != null)
                    animalHealth.TakeDamage(40f, "axe");
            }
            else
            {
                TreeResource tree = hit.GetComponent<TreeResource>();
                if (tree != null)
                {
                    tree.ChopTree();
                    continue;
                }

                StoneResource stone = hit.GetComponent<StoneResource>();
                if (stone != null)
                {
                    stone.MineStone();
                    continue;
                }
            }

        }
    }


    IEnumerator ShootArrowWithDelay(float delay)
    {
        GameObject arrowEffect = null;
        if (arrowEffectPrefab != null && arrowSpawnPoint != null)
        {
            Vector3 effectOffset = arrowSpawnPoint.forward * 0.3f + Vector3.up * 0.6f;
            Vector3 spawnPosition = arrowSpawnPoint.position + effectOffset;

            arrowEffect = Instantiate(arrowEffectPrefab, spawnPosition, arrowSpawnPoint.rotation);
        }

        yield return new WaitForSeconds(delay);

        if (arrowPrefab != null && arrowSpawnPoint != null)
        {
            Transform cam = Camera.main.transform;
            Vector3 shootDirection = cam.forward;

            RaycastHit hit;
            if (Physics.Raycast(cam.position, cam.forward, out hit, 100f))
            {
                shootDirection = (hit.point - arrowSpawnPoint.position).normalized;
            }

            Quaternion arrowRotation = Quaternion.LookRotation(shootDirection) * Quaternion.Euler(90, 0, 0);
            GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowRotation);

           
            if (arrowShootSound != null)
            {
                AudioSource.PlayClipAtPoint(arrowShootSound, arrowSpawnPoint.position);
            }

            if (arrowEffect != null)
            {
                arrowEffect.transform.SetParent(arrow.transform);
                arrowEffect.transform.localPosition = Vector3.zero;
            }

            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rb.linearVelocity = shootDirection * 50f;
            }

            Collider arrowCollider = arrow.GetComponent<Collider>();
            if (arrowCollider != null)
            {
                arrowCollider.isTrigger = true;
            }

            Destroy(arrow, 5f);
        }
    }

    public void EndAttackAnimation()
    {
        isAttacking = false;
        if (playerController != null)
            playerController.isAttacking = false; 
    }
}
