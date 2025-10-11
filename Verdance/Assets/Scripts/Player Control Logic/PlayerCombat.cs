using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Weapon Slots")]
    [SerializeField] private Transform weaponHitboxPoint;
    [SerializeField] private GameObject primaryWeaponHitbox;
    [SerializeField] private GameObject secondaryWeaponHitbox;

    [Header("Attack Settings")]
    [SerializeField] private float primaryAttackCooldown = 0.5f;
    [SerializeField] private float secondaryAttackCooldown = 0.7f;
    [SerializeField] private float hitboxActiveDuration = 0.2f;

    private PlayerInventory inventory;
    private Animator animator;
    private bool canAttackPrimary = true;
    private bool canAttackSecondary = true;
    private int currentWeaponSlot = 0;

    private void Start()
    {
        inventory = PlayerInventory.Instance;
        animator = GetComponent<Animator>();

        if (primaryWeaponHitbox != null) primaryWeaponHitbox.SetActive(false);
        if (secondaryWeaponHitbox != null) secondaryWeaponHitbox.SetActive(false);
    }

    public void PerformAttack()
    {
        if (currentWeaponSlot == 0)
        {
            AttackPrimary();
        }
        else if (currentWeaponSlot == 1)
        {
            AttackSecondary();
        }
    }

    public void AttackPrimary()
    {
        if (!canAttackPrimary) return;

        Item weapon = inventory?.GetPrimaryWeapon();
        if (weapon == null)
        {
            Debug.Log("No primary weapon equipped!");
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("AttackTrigger");
        }

        Weapon weaponData = weapon as Weapon;
        if (weaponData != null && weaponData.swingSound != null)
        {
            AudioSource.PlayClipAtPoint(weaponData.swingSound, transform.position, weaponData.volume);
        }

        ActivateWeaponHitbox(primaryWeaponHitbox, weapon);
        canAttackPrimary = false;
        Invoke(nameof(ResetPrimaryAttack), primaryAttackCooldown);
    }

    public void AttackSecondary()
    {
        if (!canAttackSecondary) return;

        Item weapon = inventory?.GetSecondaryWeapon();
        if (weapon == null)
        {
            Debug.Log("No secondary weapon equipped!");
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("AttackTrigger");
        }

        Weapon weaponData = weapon as Weapon;
        if (weaponData != null && weaponData.swingSound != null)
        {
            AudioSource.PlayClipAtPoint(weaponData.swingSound, transform.position, weaponData.volume);
        }

        ActivateWeaponHitbox(secondaryWeaponHitbox, weapon);
        canAttackSecondary = false;
        Invoke(nameof(ResetSecondaryAttack), secondaryAttackCooldown);
    }

    private void ActivateWeaponHitbox(GameObject hitbox, Item weapon)
    {
        if (hitbox == null) return;

        DamageDealer damageDealer = hitbox.GetComponent<DamageDealer>();
        if (damageDealer == null)
        {
            damageDealer = hitbox.AddComponent<DamageDealer>();
        }

        Weapon weaponData = weapon as Weapon;
        if (weaponData != null)
        {
            // Set damage from weapon data (we'll need to add a public setter to DamageDealer)
        }

        hitbox.SetActive(true);
        StartCoroutine(DisableHitboxAfterDelay(hitbox, hitboxActiveDuration));
    }

    private IEnumerator DisableHitboxAfterDelay(GameObject hitbox, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hitbox != null)
        {
            hitbox.SetActive(false);
        }
    }

    public void SwitchWeapon(int slotIndex)
    {
        if (slotIndex == 0 || slotIndex == 1)
        {
            currentWeaponSlot = slotIndex;
            Debug.Log($"Switched to weapon slot {slotIndex}");
        }
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection)
    {
        PlayerStats stats = PlayerStats.Instance;
        if (stats != null)
        {
            stats.TakeDamage(damage);
        }

        PlayerController2D controller = GetComponent<PlayerController2D>();
        if (controller != null)
        {
            controller.TakeDamage((int)damage, knockbackDirection);
        }
    }

    private void ResetPrimaryAttack() => canAttackPrimary = true;
    private void ResetSecondaryAttack() => canAttackSecondary = true;

    public int GetCurrentWeaponSlot() => currentWeaponSlot;
}
