using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

public class ShamblerAnimatorAutoLinker : EditorWindow
{
    [MenuItem("Tools/Auto-Link Shambler Animator")]
    public static void LinkAnimator()
    {
        string animPath = "Assets/Animations/Shambler/";
        string controllerPath = animPath + "ShamblerAnimatorController.controller";

        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        if (controller == null)
        {
            controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            Debug.Log("Created new ShamblerAnimatorController.");
        }

        AnimatorStateMachine sm = controller.layers[0].stateMachine;

        // Add parameters
        string[] triggers = { "ChargeTrigger", "IsHurt", "IsSpawning" };
        string[] bools = { "IsDead", "IsTwitching", "IsStaggering" };
        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);

        foreach (string t in triggers)
            controller.AddParameter(t, AnimatorControllerParameterType.Trigger);
        foreach (string b in bools)
            controller.AddParameter(b, AnimatorControllerParameterType.Bool);

        // Add states and transitions
        string[] states = { "Idle", "Walk", "Charge", "Hurt", "Death", "Spawn", "Twitch", "Stagger" };
        AnimatorState idleState = null;

        foreach (string stateName in states)
        {
            string clipPath = $"{animPath}Shambler_{stateName}.anim";
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
            if (clip == null)
            {
                Debug.LogWarning($"Missing clip: {clipPath}");
                continue;
            }

            AnimatorState state = sm.AddState(stateName);
            state.motion = clip;

            if (stateName == "Idle")
                idleState = state;

            if (stateName == "Walk")
            {
                AnimatorStateTransition toWalk = idleState.AddTransition(state);
                toWalk.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
                toWalk.hasExitTime = false;

                AnimatorStateTransition toIdle = state.AddTransition(idleState);
                toIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
                toIdle.hasExitTime = false;
            }
            else if (stateName == "Charge")
            {
                AnimatorStateTransition toCharge = sm.AddAnyStateTransition(state);
                toCharge.AddCondition(AnimatorConditionMode.If, 0, "ChargeTrigger");
                toCharge.hasExitTime = false;
            }
            else if (stateName == "Hurt")
            {
                AnimatorStateTransition toHurt = sm.AddAnyStateTransition(state);
                toHurt.AddCondition(AnimatorConditionMode.If, 0, "IsHurt");
                toHurt.hasExitTime = false;
            }
            else if (stateName == "Death")
            {
                AnimatorStateTransition toDeath = sm.AddAnyStateTransition(state);
                toDeath.AddCondition(AnimatorConditionMode.If, 0, "IsDead");
                toDeath.hasExitTime = false;
            }
            else if (stateName == "Spawn")
            {
                AnimatorStateTransition toSpawn = idleState.AddTransition(state);
                toSpawn.AddCondition(AnimatorConditionMode.If, 0, "IsSpawning");
                toSpawn.hasExitTime = false;
            }
            else if (stateName == "Twitch")
            {
                AnimatorStateTransition toTwitch = idleState.AddTransition(state);
                toTwitch.AddCondition(AnimatorConditionMode.If, 0, "IsTwitching");
                toTwitch.hasExitTime = false;
            }
            else if (stateName == "Stagger")
            {
                AnimatorStateTransition toStagger = idleState.AddTransition(state);
                toStagger.AddCondition(AnimatorConditionMode.If, 0, "IsStaggering");
                toStagger.hasExitTime = false;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("ShamblerAnimatorController transitions linked.");
    }
}
