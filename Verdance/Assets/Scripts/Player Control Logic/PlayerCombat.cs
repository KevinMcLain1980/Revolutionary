using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Weapon Slots")]
    [SerializeField] private Transform weaponHitboxPoint;
    [SerializeField] private GameObject primaryWeaponHitbox;

    [Header("Attack Settings")]
    [SerializeField] private float primaryAttackCooldown = 0.5f;
    [SerializeField] private float hitboxActiveDuration = 0.2f;

    [Header("Audio")]
    private AudioSource audioSource; 
    [SerializeField] private AudioClip attackSwingSound;
    

    private PlayerInventory inventory;
    private Animator animator;
    private bool canAttackPrimary = true;

    private void Awake()
    {
       // Debug.Log($"[PlayerCombat] Awake called on {gameObject.name}");

        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
           // Debug.Log("[PlayerCombat] Created new AudioSource in Awake");
        }
    }

    private void Start()
    {
        inventory = PlayerInventory.Instance;
        animator = GetComponent<Animator>();

        if (primaryWeaponHitbox != null) primaryWeaponHitbox.SetActive(false);

        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
               // Debug.Log("[PlayerCombat] Re-created AudioSource in Start after scene reload");
            }
        }

       // Debug.Log($"[PlayerCombat] Start called. AudioSource: {(audioSource != null ? "Found" : "NULL")}, Attack sound: {(attackSwingSound != null ? "Assigned" : "NULL")}");
    }

    public void PerformAttack()
    {
        AttackPrimary();
    }

    public void AttackPrimary()
    {
        if (!canAttackPrimary) return;

       
        if (inventory != null && !inventory.HasSword())
        {
            //Debug.Log("No primary weapon equipped!");
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("AttackTrigger");
        }

        
        if (attackSwingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSwingSound, SettingsManager.Instance.GetSFXVolume());
        }

        ActivateWeaponHitbox(primaryWeaponHitbox, null);
        canAttackPrimary = false;
        Invoke(nameof(ResetPrimaryAttack), primaryAttackCooldown);
    }



    private void ActivateWeaponHitbox(GameObject hitbox, Item weapon)
    {
        if (hitbox == null) return;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            float direction = spriteRenderer.flipX ? -1 : 1;
            Vector3 localPos = hitbox.transform.localPosition;
            localPos.x = Mathf.Abs(localPos.x) * direction;
            hitbox.transform.localPosition = localPos;
        }

        DamageDealer damageDealer = hitbox.GetComponent<DamageDealer>();
        if (damageDealer == null)
        {
            damageDealer = hitbox.AddComponent<DamageDealer>();
        }

        Weapon weaponData = weapon as Weapon;
        if (weaponData != null)
        {
            
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
}
