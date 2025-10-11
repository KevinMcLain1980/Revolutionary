using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class Weapon : Item
{
    [Header("Weapon Stats")]
    public float damage = 10f;
    public float attackSpeed = 1f;
    public float range = 1f;
    public bool isPrimary = true;

    [Header("Visual")]
    public GameObject hitboxPrefab;
    public AnimationClip attackAnimation;

    [Header("Audio")]
    public AudioClip swingSound;
    public AudioClip hitSound;
    [Range(0f, 1f)] public float volume = 0.8f;

    public override void Use()
    {
        Debug.Log($"Attacking with {itemName} - Damage: {damage}");
    }

    public override bool IsUsable()
    {
        return true;
    }
}
