using UnityEngine;

public class MagicManager : MonoBehaviour
{
    [Header("Magic Abilities")]
    [SerializeField] private WindStep windStep;
    [SerializeField] private LightPulse lightPulse;

    public void CastSpell(string spellName)
    {
        switch (spellName)
        {
            case "WindStep":
                if (windStep != null)
                    windStep.Activate();
                else
                    Debug.LogWarning("WindStep reference missing in MagicManager.");
                break;

            case "LightPulse":
                if (lightPulse != null)
                    lightPulse.Activate();
                else
                    Debug.LogWarning("LightPulse reference missing in MagicManager.");
                break;

            default:
                Debug.LogWarning($"Unknown spell: {spellName}");
                break;
        }
    }
}
