using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider uiVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Volume Text")]
    [SerializeField] private TMP_Text masterVolumeText;
    [SerializeField] private TMP_Text uiVolumeText;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private TMP_Text sfxVolumeText;

    [Header("Keybind Buttons")]
    [SerializeField] private Button moveLeftKeybindButton;
    [SerializeField] private Button moveRightKeybindButton;
    [SerializeField] private Button jumpKeybindButton;
    [SerializeField] private Button attackKeybindButton;
    [SerializeField] private Button inventory1KeybindButton;
    [SerializeField] private Button inventory2KeybindButton;
    [SerializeField] private Button inventory3KeybindButton;
    [SerializeField] private Button inventory4KeybindButton;
    [SerializeField] private Button spell1KeybindButton;
    [SerializeField] private Button spell2KeybindButton;
    [SerializeField] private Button spell3KeybindButton;

    [Header("Keybind Text")]
    [SerializeField] private TMP_Text moveLeftKeybindText;
    [SerializeField] private TMP_Text moveRightKeybindText;
    [SerializeField] private TMP_Text jumpKeybindText;
    [SerializeField] private TMP_Text attackKeybindText;
    [SerializeField] private TMP_Text inventory1KeybindText;
    [SerializeField] private TMP_Text inventory2KeybindText;
    [SerializeField] private TMP_Text inventory3KeybindText;
    [SerializeField] private TMP_Text inventory4KeybindText;
    [SerializeField] private TMP_Text spell1KeybindText;
    [SerializeField] private TMP_Text spell2KeybindText;
    [SerializeField] private TMP_Text spell3KeybindText;

    [Header("Buttons")]
    [SerializeField] private Button resetButton;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button closeButton;

    [Header("Rebinding")]
    [SerializeField] private GameObject rebindingPrompt;
    [SerializeField] private TMP_Text rebindingText;
    [SerializeField] private InputActionAsset inputActions;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private InputAction currentActionToRebind;
    private int bindingIndex;

    private void Start()
    {
        SetupSliders();
        SetupButtons();
        LoadSettings();
        LoadBindingOverrides();

        if (rebindingPrompt != null)
            rebindingPrompt.SetActive(false);
    }

    private void SetupSliders()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }

        if (uiVolumeSlider != null)
        {
            uiVolumeSlider.onValueChanged.AddListener(OnUIVolumeChanged);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
    }

    private void SetupButtons()
    {
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetClicked);

        if (applyButton != null)
            applyButton.onClick.AddListener(OnApplyClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);

        if (moveLeftKeybindButton != null)
            moveLeftKeybindButton.onClick.AddListener(() => StartRebinding("Move Left"));

        if (moveRightKeybindButton != null)
            moveRightKeybindButton.onClick.AddListener(() => StartRebinding("Move Right"));

        if (jumpKeybindButton != null)
            jumpKeybindButton.onClick.AddListener(() => StartRebinding("Jump"));

        if (attackKeybindButton != null)
            attackKeybindButton.onClick.AddListener(() => StartRebinding("Attack"));

        if (inventory1KeybindButton != null)
            inventory1KeybindButton.onClick.AddListener(() => StartRebinding("Inventory Slot 1"));

        if (inventory2KeybindButton != null)
            inventory2KeybindButton.onClick.AddListener(() => StartRebinding("Inventory Slot 2"));

        if (inventory3KeybindButton != null)
            inventory3KeybindButton.onClick.AddListener(() => StartRebinding("Inventory Slot 3"));

        if (inventory4KeybindButton != null)
            inventory4KeybindButton.onClick.AddListener(() => StartRebinding("Inventory Slot 4"));

        if (spell1KeybindButton != null)
            spell1KeybindButton.onClick.AddListener(() => StartRebinding("Spell Slot 5"));

        if (spell2KeybindButton != null)
            spell2KeybindButton.onClick.AddListener(() => StartRebinding("Spell Slot 6"));

        if (spell3KeybindButton != null)
            spell3KeybindButton.onClick.AddListener(() => StartRebinding("Spell Slot 7"));
    }

    private void LoadSettings()
    {
        if (SettingsManager.Instance == null) return;

        masterVolumeSlider?.SetValueWithoutNotify(SettingsManager.Instance.GetMasterVolume());
        uiVolumeSlider?.SetValueWithoutNotify(SettingsManager.Instance.GetUIVolume());
        musicVolumeSlider?.SetValueWithoutNotify(SettingsManager.Instance.GetMusicVolume());
        sfxVolumeSlider?.SetValueWithoutNotify(SettingsManager.Instance.GetSFXVolume());

        UpdateVolumeTexts();
        UpdateKeybindTexts();
    }

    private void UpdateKeybindTexts()
    {
        if (inputActions != null)
        {
            if (moveLeftKeybindText != null)
            {
                var moveAction = inputActions.FindAction("Move");
                moveLeftKeybindText.text = moveAction != null ? GetBindingDisplayString(moveAction, 0) : "Left Arrow";
            }

            if (moveRightKeybindText != null)
            {
                var moveAction = inputActions.FindAction("Move");
                moveRightKeybindText.text = moveAction != null ? GetBindingDisplayString(moveAction, 0) : "Right Arrow";
            }

            if (jumpKeybindText != null)
            {
                var jumpAction = inputActions.FindAction("Jump");
                jumpKeybindText.text = jumpAction != null ? GetBindingDisplayString(jumpAction, 0) : "Space";
            }

            if (attackKeybindText != null)
            {
                var attackAction = inputActions.FindAction("Attack");
                attackKeybindText.text = attackAction != null ? GetBindingDisplayString(attackAction, 0) : "Attack Key";
            }
        }

        if (inventory1KeybindText != null) inventory1KeybindText.text = "1";
        if (inventory2KeybindText != null) inventory2KeybindText.text = "2";
        if (inventory3KeybindText != null) inventory3KeybindText.text = "3";
        if (inventory4KeybindText != null) inventory4KeybindText.text = "4";
        if (spell1KeybindText != null) spell1KeybindText.text = "5";
        if (spell2KeybindText != null) spell2KeybindText.text = "6";
        if (spell3KeybindText != null) spell3KeybindText.text = "7";
    }

    private string GetBindingDisplayString(InputAction action, int bindingIndex)
    {
        if (action == null || bindingIndex >= action.bindings.Count)
            return "Not Bound";

        return action.GetBindingDisplayString(bindingIndex);
    }

    private void OnMasterVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetMasterVolume(value);
        }
        UpdateVolumeText(masterVolumeText, value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetMusicVolume(value);
        }
        UpdateVolumeText(musicVolumeText, value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetSFXVolume(value);
        }
        UpdateVolumeText(sfxVolumeText, value);
    }

    private void OnUIVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetUIVolume(value);
        }
        UpdateVolumeText(uiVolumeText, value);
    }

    private void UpdateVolumeText(TMP_Text text, float value)
    {
        if (text != null)
        {
            text.text = $"{Mathf.RoundToInt(value * 100)}%";
        }
    }

    private void UpdateVolumeTexts()
    {
        UpdateVolumeText(masterVolumeText, masterVolumeSlider?.value ?? 1f);
        UpdateVolumeText(musicVolumeText, musicVolumeSlider?.value ?? 1f);
        UpdateVolumeText(sfxVolumeText, sfxVolumeSlider?.value ?? 1f);
        UpdateVolumeText(uiVolumeText, uiVolumeSlider?.value ?? 1f);
    }

    private void StartRebinding(string displayName)
    {
        if (inputActions == null)
        {
            Debug.LogError("Input Actions asset not assigned to SettingsMenu!");
            return;
        }

        InputAction action = GetInputActionFromDisplayName(displayName);
        if (action == null)
        {
            Debug.LogError($"Could not find input action for {displayName}");
            return;
        }

        currentActionToRebind = action;
        bindingIndex = GetBindingIndex(action);

        if (rebindingPrompt != null)
            rebindingPrompt.SetActive(true);

        if (rebindingText != null)
            rebindingText.text = $"Press any key for {displayName}...\n(ESC to cancel)";

        action.Disable();

        rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete())
            .OnCancel(operation => RebindCancelled())
            .WithCancelingThrough("<Keyboard>/escape")
            .Start();
    }

    private void RebindComplete()
    {
        Debug.Log($"Rebind completed for {currentActionToRebind.name}");

        currentActionToRebind.Enable();
        rebindingOperation.Dispose();

        if (rebindingPrompt != null)
            rebindingPrompt.SetActive(false);

        SaveBindingOverride(currentActionToRebind);
        UpdateKeybindTexts();
    }

    private void RebindCancelled()
    {
        Debug.Log("Rebind cancelled");

        currentActionToRebind.Enable();
        rebindingOperation.Dispose();

        if (rebindingPrompt != null)
            rebindingPrompt.SetActive(false);
    }

    private InputAction GetInputActionFromDisplayName(string displayName)
    {
        switch (displayName)
        {
            case "Move Left":
            case "Move Right":
                return inputActions.FindAction("Move");
            case "Jump":
                return inputActions.FindAction("Jump");
            case "Attack":
                return inputActions.FindAction("Attack");
            case "Inventory Slot 1":
            case "Inventory Slot 2":
            case "Inventory Slot 3":
            case "Inventory Slot 4":
            case "Spell Slot 5":
            case "Spell Slot 6":
            case "Spell Slot 7":
                return null; // These use keyboard detection in PlayerUI
            default:
                return null;
        }
    }

    private int GetBindingIndex(InputAction action)
    {
        // For Move action, we need to find the specific part of the composite
        string currentActionName = currentActionToRebind?.name;

        if (currentActionName == "Move")
        {
            // Find the binding index for left or right within the composite
            for (int i = 0; i < action.bindings.Count; i++)
            {
                var binding = action.bindings[i];

                // Skip the composite itself
                if (binding.isComposite)
                    continue;

                // Check if this is the left or right part
                if (rebindingText.text.Contains("Move Left") && binding.name == "left")
                    return i;
                if (rebindingText.text.Contains("Move Right") && binding.name == "right")
                    return i;
            }
        }

        // For non-composite actions, return first non-composite binding
        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (!action.bindings[i].isComposite && !action.bindings[i].isPartOfComposite)
                return i;
        }

        return 0;
    }

    private void SaveBindingOverride(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString($"InputBinding_{action.name}_{i}", action.bindings[i].overridePath);
        }
        PlayerPrefs.Save();
    }

    private void LoadBindingOverrides()
    {
        if (inputActions == null) return;

        foreach (var map in inputActions.actionMaps)
        {
            foreach (var action in map.actions)
            {
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    string key = $"InputBinding_{action.name}_{i}";
                    if (PlayerPrefs.HasKey(key))
                    {
                        string overridePath = PlayerPrefs.GetString(key);
                        if (!string.IsNullOrEmpty(overridePath))
                        {
                            action.ApplyBindingOverride(i, overridePath);
                        }
                    }
                }
            }
        }
    }

    private void ResetAllBindings()
    {
        if (inputActions == null) return;

        foreach (var map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }

        foreach (var map in inputActions.actionMaps)
        {
            foreach (var action in map.actions)
            {
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    PlayerPrefs.DeleteKey($"InputBinding_{action.name}_{i}");
                }
            }
        }

        PlayerPrefs.Save();
        UpdateKeybindTexts();
    }

    private void OnResetClicked()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.ResetToDefaults();
            LoadSettings();
        }

        ResetAllBindings();
        Debug.Log("Settings reset to defaults");
    }

    private void OnApplyClicked()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SaveSettings();
        }
        Debug.Log("Settings applied and saved");
    }

    private void OnCloseClicked()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        rebindingOperation?.Dispose();
    }
}
