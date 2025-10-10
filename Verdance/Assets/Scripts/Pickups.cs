
using Unity.Cinemachine;
using UnityEngine;

public class Pickups : MonoBehaviour, IPickup
{
    public enum PickupType { HealthPotion}
    public PickupType type = PickupType.HealthPotion;

    [SerializeField] int healAmount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            OnPickup(other.gameObject);
            Destroy(other.gameObject);
        }
    }

    public void OnPickup(GameObject collector)
    {
        //Logic for health pickup goes here
        //maybe a switch case with type so that
        //other pickup items can be added with their logic

    }

    
}
