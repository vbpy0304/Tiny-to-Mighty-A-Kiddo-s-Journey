using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ButtonUI : MonoBehaviour
{
    [Header("Refs")]
    public PathFollower pathFollower;
    public Button actionButton;
    public TMP_Text buttonText;
    public TMP_Text cooldownText;
    public TMP_Text counterText;   // UI for counter
    public Slider cooldownSlider; // Slider reference (1 - 120)

    [Header("Cooldown Settings")]
    public float cooldownDuration = 60f;

    [Header("Messages")]
    public string[] readyMessages;
    public string[] waitMessages;

    // PlayerPrefs keys
    const string FirstClickKey = "FirstClickUsed";
    const string CooldownEndTicksKey = "CooldownEndTicks";
    const string ClickCounterKey = "ClickCounter";
    const string CooldownDurationKey = "CooldownDuration";

    private float cooldownTimer = 0f;
    private bool firstClickUsed = false;
    private string currentWaitMessage = "";
    private int clickCounter = 0;

    void Start()
    {
        // Load saved data
        firstClickUsed = PlayerPrefs.GetInt(FirstClickKey, 0) == 1;
        clickCounter = PlayerPrefs.GetInt(ClickCounterKey, 0);
        cooldownDuration = PlayerPrefs.GetFloat(CooldownDurationKey, cooldownDuration);

        // Setup counter UI
        UpdateCounterUI();

        // Wire up button
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnButtonClick);

        // Wire up slider
        if (cooldownSlider != null)
        {
            cooldownSlider.minValue = 1f;
            cooldownSlider.maxValue = 120f;
            cooldownSlider.value = cooldownDuration;
            cooldownSlider.onValueChanged.AddListener(OnSliderChanged);
        }

        // Load saved cooldown
        LoadCooldown();
        if (cooldownTimer <= 0f)
            ResetButtonState();
        else
            ApplyCooldownUI();
    }

    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                cooldownTimer = 0f;
                PlayerPrefs.DeleteKey(CooldownEndTicksKey);
                PlayerPrefs.Save();
                ResetButtonState();
            }
            else
            {
                cooldownText.text = $"{currentWaitMessage} {Mathf.CeilToInt(cooldownTimer)}s";
                buttonText.text = "";
                actionButton.interactable = false;
            }
        }
    }

    private void OnSliderChanged(float value)
    {
        cooldownDuration = value;
        PlayerPrefs.SetFloat(CooldownDurationKey, cooldownDuration);
        PlayerPrefs.Save();
    }

    private void OnButtonClick()
    {
        if (pathFollower != null) pathFollower.MoveOneStep();

        // Count click
        clickCounter++;
        PlayerPrefs.SetInt(ClickCounterKey, clickCounter);
        PlayerPrefs.Save();
        UpdateCounterUI();

        if (firstClickUsed)
        {
            cooldownTimer = cooldownDuration;
            PickNewWaitMessage();

            DateTime end = DateTime.UtcNow.AddSeconds(cooldownDuration);
            PlayerPrefs.SetString(CooldownEndTicksKey, end.Ticks.ToString());
            PlayerPrefs.Save();

            ApplyCooldownUI();
        }
        else
        {
            firstClickUsed = true;
            PlayerPrefs.SetInt(FirstClickKey, 1);
            PlayerPrefs.Save();

            ResetButtonState();
        }
    }

    private void LoadCooldown()
    {
        if (PlayerPrefs.HasKey(CooldownEndTicksKey))
        {
            string ticksStr = PlayerPrefs.GetString(CooldownEndTicksKey, "");
            if (long.TryParse(ticksStr, out long ticks))
            {
                DateTime cooldownEndUtc = new DateTime(ticks, DateTimeKind.Utc);
                TimeSpan remaining = cooldownEndUtc - DateTime.UtcNow;
                if (remaining.TotalSeconds > 0.5)
                {
                    cooldownTimer = (float)remaining.TotalSeconds;
                    PickNewWaitMessage();
                }
                else
                {
                    PlayerPrefs.DeleteKey(CooldownEndTicksKey);
                    PlayerPrefs.Save();
                }
            }
        }
    }

    private void ResetButtonState()
    {
        cooldownTimer = 0f;
        actionButton.interactable = true;

        if (readyMessages != null && readyMessages.Length > 0)
            buttonText.text = readyMessages[UnityEngine.Random.Range(0, readyMessages.Length)];
        else
            buttonText.text = "Chore Counter:";

        cooldownText.text = "";
    }

    private void ApplyCooldownUI()
    {
        actionButton.interactable = false;
        buttonText.text = "";
        cooldownText.text = $"{currentWaitMessage} {Mathf.CeilToInt(cooldownTimer)}s";
    }

    private void PickNewWaitMessage()
    {
        if (waitMessages != null && waitMessages.Length > 0)
            currentWaitMessage = waitMessages[UnityEngine.Random.Range(0, waitMessages.Length)];
        else
            currentWaitMessage = "WAIT";
    }

    private void UpdateCounterUI()
    {
        if (counterText != null)
            counterText.text = $"Chore Counter: {clickCounter}";
    }

    public void ClearSavedData()
    {
        PlayerPrefs.DeleteKey(CooldownEndTicksKey);
        PlayerPrefs.DeleteKey(FirstClickKey);
        PlayerPrefs.DeleteKey(ClickCounterKey);
        PlayerPrefs.DeleteKey(CooldownDurationKey);
        PlayerPrefs.Save();

        clickCounter = 0;
        firstClickUsed = false;
        UpdateCounterUI();
    }
}
