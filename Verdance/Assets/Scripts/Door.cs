using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private object nextLevel;



    public void OpenChest(PlayerInventory inventory)
    {
        if (inventory != null && inventory.HasKey())
        {
            if (!LevelManager.Instance.CheckEnemiesAndBossDefeated())
            {
               // Debug.Log("You must defeat all enemies first");
                return;
            }

            LevelManager.Instance.OnKeyUsed();

            if (LevelManager.Instance.CheckLevelCompletion())
            {
                LevelManager.Instance.ForceCompleteLevel();
            }
        }
        else
        {
            //Debug.Log("Find the key!");
        }
    }
}
