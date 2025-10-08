using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

public class ShamblerAnimatorBuilder : EditorWindow
{
    [MenuItem("Tools/Create Shambler Animator")]
    public static void CreateAnimator()
    {
        string animPath = "Assets/Animations/Shambler/";
        string controllerPath = animPath + "ShamblerAnimatorController.controller";

        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        // Add parameters
        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("ChargeTrigger", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("IsDead", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsHurt", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("IsSpawning", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("IsTwitching", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsStaggering", AnimatorControllerParameterType.Bool);

        string[] states = { "Idle", "Walk", "Charge", "Hurt", "Death", "Spawn", "Twitch", "Stagger" };

        AnimatorState idleState = null;

        foreach (string stateName in states)
        {
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{animPath}Shambler_{stateName}.anim");
            if (clip == null)
            {
                Debug.LogWarning($"Missing animation clip: {stateName}");
                continue;
            }

            AnimatorState state = controller.layers[0].stateMachine.AddState(stateName);
            state.motion = clip;

            if (stateName == "Idle")
                idleState = state;

            // Add transitions
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
                AnimatorStateTransition toCharge = controller.layers[0].stateMachine.AddAnyStateTransition(state);
                toCharge.AddCondition(AnimatorConditionMode.If, 0, "ChargeTrigger");
                toCharge.hasExitTime = false;
            }
            else if (stateName == "Hurt")
            {
                AnimatorStateTransition toHurt = controller.layers[0].stateMachine.AddAnyStateTransition(state);
                toHurt.AddCondition(AnimatorConditionMode.If, 0, "IsHurt");
                toHurt.hasExitTime = false;
            }
            else if (stateName == "Death")
            {
                AnimatorStateTransition toDeath = controller.layers[0].stateMachine.AddAnyStateTransition(state);
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

        Debug.Log("Shambler Animator Controller created.");
    }
}
