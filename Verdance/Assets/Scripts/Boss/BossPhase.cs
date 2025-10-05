using UnityEngine;

[System.Serializable]
public class BossPhase
{
    public string phaseName;
    public int triggerHealth;
    public GameObject[] attackPatterns;

    public void ActivatePhase()
    {
        foreach (var pattern in attackPatterns)
        {
            if (pattern != null) pattern.SetActive(true);
        }
    }
}
