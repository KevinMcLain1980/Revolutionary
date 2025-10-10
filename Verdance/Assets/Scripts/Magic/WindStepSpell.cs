using UnityEngine;

[CreateAssetMenu(fileName = "WindStep", menuName = "Magic/Spell/Wind Step")]
public class WindStepSpell : MagicSpell
{
    public override void Cast(Vector3 position, Vector3 direction)
    {
        PlayerController2D player = Object.FindFirstObjectByType<PlayerController2D>();
        if (player != null)
        {
            WindStep windStep = player.GetComponent<WindStep>();
            if (windStep != null)
            {
                windStep.Activate();
            }
        }
    }
}
