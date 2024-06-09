using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class NewAnimationController : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component
    public Slider scrubSlider; // Reference to the Scrubbing Slider
    public Button playPauseButton; // Reference to the Play/Pause button
    public Image playPauseImage; // Reference to the Image component for the play/pause button
    public Sprite playSprite; // Sprite for the play state
    public Sprite pauseSprite; // Sprite for the pause state
    public Button speedButtonNormal; // Reference to the Normal speed button
    public Button speedButtonSlow; // Reference to the Slow speed button
    public Button speedButtonSlowest; // Reference to the Slowest speed button
    public float normalSpeed = 1.0f; // Speed for Normal
    public float slowSpeed = 0.25f; // Speed for Slow
    public float slowestSpeed = 0.1f; // Speed for Slowest
    public float animationLength; // Length of the animation
    private bool isScrubbing = false; // To track if the user is scrubbing
    private bool isPlaying = false; // To track if the animation is playing
    private bool hasPlayedOnce = false; // To track if the animation has played once
    private float currentSpeed; // Store the current speed
    private int animationStateHash; // Hash of the animation state

    // Reference to the "Good Job!" text
    public GameObject goodJobText;

    // Reference to the Countdown Text
    public TMP_Text countdownText;

    // References to the bookmark indicators
    public GameObject backswingStartIndicator;
    public GameObject downswingStartIndicator;
    public GameObject followThroughIndicator;
    public GameObject finishIndicator;

    private Coroutine countdownCoroutine;

    void Start()
    {
        // Get the full path hash of the first animation clip
        animationStateHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        animationLength = animator.runtimeAnimatorController.animationClips[0].length;
        scrubSlider.maxValue = animationLength;
        scrubSlider.onValueChanged.AddListener(OnScrubSliderChanged);
        playPauseButton.onClick.AddListener(TogglePlayPause); // Add listener to play/pause button
        speedButtonNormal.onClick.AddListener(() => SetSpeed(normalSpeed));
        speedButtonSlow.onClick.AddListener(() => SetSpeed(slowSpeed));
        speedButtonSlowest.onClick.AddListener(() => SetSpeed(slowestSpeed));
        UpdateButtonSprite(); // Initialize button sprite

        // Set the default speed to slowest
        SetSpeed(slowestSpeed); // Set the initial speed to slowest
        UpdateSpeedButtons(slowestSpeed); // Highlight the slowest speed button

        // Pause the animation at start and set it to the first frame
        animator.speed = 0;
        isPlaying = false;
        animator.Play(animationStateHash, 0, 0);

        // Set the animation to not loop
        animator.runtimeAnimatorController.animationClips[0].wrapMode = WrapMode.Once;

        // Position and add listeners to bookmark indicators
        PositionIndicator(backswingStartIndicator, 0f);
        PositionIndicator(downswingStartIndicator, 0.46f);
        PositionIndicator(followThroughIndicator, 0.625f);
        PositionIndicator(finishIndicator, 0.99f);

        // Add EventTrigger components for click handling
        AddEventTrigger(backswingStartIndicator, JumpToBackswingStart);
        AddEventTrigger(downswingStartIndicator, JumpToDownswingStart);
        AddEventTrigger(followThroughIndicator, JumpToFollowThrough);
        AddEventTrigger(finishIndicator, JumpToFinish);

        // Ensure the "Good Job!" text and countdown text are initially hidden
        goodJobText.SetActive(false);
        countdownText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlaying && !isScrubbing) // Update slider if animation is playing and not scrubbing
        {
            float currentAnimationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime * animationLength;
            scrubSlider.value = Mathf.Clamp(currentAnimationTime, 0, animationLength); // Clamp to ensure it stays within bounds
        }

        if (isPlaying && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            PauseAnimation();
            scrubSlider.value = animationLength; // Ensure the scrubber is set to the end
            UpdateButtonSprite(); // Update button sprite to "Play"
            if (!hasPlayedOnce)
            {
                hasPlayedOnce = true;
                ShowGoodJobText();
            }
        }
    }

    public void TogglePlayPause()
    {
        if (isPlaying)
        {
            PauseAnimation();
            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;
                countdownText.gameObject.SetActive(false);
            }
        }
        else
        {
            if (countdownCoroutine == null)
            {
                if (scrubSlider.value >= animationLength)
                {
                    SetAnimationTime(0); // Restart the animation if it's at the end
                }
                countdownCoroutine = StartCoroutine(StartAnimationWithCountdown());
            }
        }
        UpdateButtonSprite(); // Update button sprite based on the new state
    }

    private IEnumerator StartAnimationWithCountdown()
    {
        isPlaying = true;
        animator.speed = 0; // Pause the animation during the countdown
        UpdateButtonSprite(); // Show the pause button during countdown
        countdownText.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(0);
            Debug.Log("waiting 3 secs, is playing?: " + isPlaying);

        }
        countdownText.gameObject.SetActive(false);
        Debug.Log("countdown done");
        PlayAnimation();
        countdownCoroutine = null;
    }


    public void PlayAnimation()
    {
         Debug.Log("play animate");
        animator.speed = currentSpeed; // Play at the current speed
        UpdateButtonSprite();
    }

    public void PauseAnimation()
    {
        isPlaying = false;
        animator.speed = 0; // Pause the animation
        UpdateButtonSprite();
    }

    public void OnScrubSliderChanged(float value)
    {
        if (!isScrubbing)
        {
            isScrubbing = true;
            SetAnimationTime(value);
            isScrubbing = false;
        }
    }

    public void SetAnimationTime(float newTime)
    {
        animator.Play(animationStateHash, 0, newTime / animationLength);
        animator.Update(0); // Ensure the animator updates immediately
        if (isPlaying) // If playing, continue playing after scrubbing
        {
            animator.speed = currentSpeed; // Play at the current speed
        }
        else // Otherwise, stay paused after scrubbing
        {
            animator.speed = 0;
        }
    }

    public void SetSpeed(float speed)
    {
        currentSpeed = speed; // Store the current speed
        if (isPlaying)
        {
            animator.speed = currentSpeed; // Update animator speed if playing
        }
        UpdateSpeedButtons(speed);
    }

    private void UpdateSpeedButtons(float selectedSpeed)
    {
        // Reset all buttons to their default colors
        ResetButtonUI(speedButtonNormal);
        ResetButtonUI(speedButtonSlow);
        ResetButtonUI(speedButtonSlowest);

        // Set selected button to selected background and font colors
        if (selectedSpeed == normalSpeed)
        {
            SetButtonUI(speedButtonNormal);
        }
        else if (selectedSpeed == slowSpeed)
        {
            SetButtonUI(speedButtonSlow);
        }
        else if (selectedSpeed == slowestSpeed)
        {
            SetButtonUI(speedButtonSlowest);
        }
    }

    private void SetButtonUI(Button button)
    {
        button.GetComponent<Image>().color = new Color32(212, 192, 255, 255); // #D4C0FF
        button.GetComponentInChildren<TMP_Text>().color = new Color32(54, 18, 131, 255);
    }

    private void ResetButtonUI(Button button)
    {
        button.GetComponent<Image>().color = new Color32(95, 61, 232, 255); // #5F3DE8
        button.GetComponentInChildren<TMP_Text>().color = Color.white;
    }

    private void UpdateButtonSprite()
    {
        if (isPlaying)
        {
            playPauseImage.sprite = pauseSprite;
        }
        else
        {
            playPauseImage.sprite = playSprite;
        }
    }

    private void PositionIndicator(GameObject indicator, float normalizedTime)
    {
        float scrubWidth = scrubSlider.GetComponent<RectTransform>().rect.width;
        float xPos = scrubWidth * normalizedTime - scrubWidth / 2 + 1;
        RectTransform rectTransform = indicator.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(xPos, rectTransform.anchoredPosition.y);
    }

    private void AddEventTrigger(GameObject obj, UnityEngine.Events.UnityAction action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = obj.AddComponent<EventTrigger>();
        }

        var entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((eventData) => { action(); });
        trigger.triggers.Add(entry);
    }

    public void JumpToPosition(float newTime)
    {
        isScrubbing = true;
        SetAnimationTime(newTime);
        scrubSlider.value = newTime; // Update the scrubber value
        isScrubbing = false;
        
    }

    // Public methods for the UnityEvent system
    public void JumpToBackswingStart() { JumpToPosition(0.0f * animationLength); }
    public void JumpToDownswingStart() { JumpToPosition(0.46f * animationLength); }
    public void JumpToFollowThrough() { JumpToPosition(0.625f * animationLength); }
    public void JumpToFinish() { JumpToPosition(0.99f * animationLength); }

    private void ShowGoodJobText()
    {
        goodJobText.SetActive(true);
        StartCoroutine(HideGoodJobTextAfterDelay(3.0f));
    }

    private IEnumerator HideGoodJobTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        goodJobText.SetActive(false);
    }
}
