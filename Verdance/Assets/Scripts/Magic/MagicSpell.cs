using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Magic/Spell")]
public class MagicSpell : ScriptableObject
{
    public string spellName;
    public Sprite spellIcon;
    public float manaCost;
    public float cooldown;
    public float damage;
    public GameObject spellEffectPrefab;

    public virtual void Cast(Vector3 position, Vector3 direction)
    {
        Debug.Log($"Casting {spellName}");
    }
}
