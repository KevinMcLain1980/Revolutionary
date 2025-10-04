using UnityEngine;

public class MagicManager : MonoBehaviour
{
    public LightPulse lightPulse;
    public WindStep windStep;

    public void CastSpell(string spellName)
    {
        switch (spellName)
        {
            case "LightPulse":
                lightPulse?.Activate();
                break;
            case "WindStep":
                windStep?.Activate();
                break;
            default:
                Debug.LogWarning($"Unknown spell: {spellName}");
                break;
        }
    }
}
