using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimationController : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component
    public Slider scrubSlider; // Reference to the Scrubbing Slider
    public Button playPauseButton; // Reference to the Play/Pause button
    public TMP_Text buttonText; // Reference to the TextMeshPro text component
    public Button speedButtonNormal; // Reference to the Normal speed button
    public Button speedButtonSlow; // Reference to the Slow speed button
    public Button speedButtonSlowest; // Reference to the Slowest speed button
    public Color selectedColor = Color.green; // Color to indicate the selected button
    public Color defaultColor = Color.white; // Default color of the button text
    public float normalSpeed = 1.0f; // Speed for Normal
    public float slowSpeed = 0.5f; // Speed for Slow
    public float slowestSpeed = 0.25f; // Speed for Slowest
    private float animationLength; // Length of the animation
    private bool isScrubbing = false; // To track if the user is scrubbing
    private bool isPlaying = false; // To track if the animation is playing
    private float currentSpeed = 1.0f; // Store the current speed
    private int animationStateHash; // Hash of the animation state

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
        UpdateButtonText(); // Initialize button text
        SetSpeed(normalSpeed); // Set default speed to normal

        // Pause the animation at start and set it to the first frame
        animator.speed = 0;
        isPlaying = false;
        animator.Play(animationStateHash, 0, 0);
        Debug.Log("Start called, initialized with isPlaying: " + isPlaying);

        // Set the animation to not loop
        animator.runtimeAnimatorController.animationClips[0].wrapMode = WrapMode.Once;
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
            UpdateButtonText(); // Update button text to "Play"
        }
    }

    public void TogglePlayPause()
    {
        Debug.Log("TogglePlayPause called, isPlaying: " + isPlaying);
        if (isPlaying)
        {
            PauseAnimation();
        }
        else
        {
            if (scrubSlider.value >= animationLength)
            {
                SetAnimationTime(0); // Restart the animation if it's at the end
            }
            PlayAnimation();
        }
        UpdateButtonText(); // Update button text based on the new state
    }

    public void PlayAnimation()
    {
        Debug.Log("PlayAnimation called");
        isPlaying = true;
        animator.speed = currentSpeed; // Play at the current speed
        Debug.Log("Animation is now playing at speed: " + currentSpeed);
    }

    public void PauseAnimation()
    {
        Debug.Log("PauseAnimation called");
        isPlaying = false;
        animator.speed = 0; // Pause the animation
        Debug.Log("Animation is now paused");
    }

    public void OnScrubSliderChanged(float value)
    {
        Debug.Log("OnScrubSliderChanged called with value: " + value);
        isScrubbing = true;
        SetAnimationTime(value);
        isScrubbing = false;
    }

    public void SetAnimationTime(float newTime)
    {
        Debug.Log("SetAnimationTime called with newTime: " + newTime);
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
        Debug.Log("SetSpeed called with speed: " + speed);
        currentSpeed = speed; // Store the current speed
        if (isPlaying)
        {
            animator.speed = currentSpeed; // Update animator speed if playing
        }
        UpdateSpeedButtons(speed);
    }

    private void UpdateSpeedButtons(float selectedSpeed)
    {
        Debug.Log("UpdateSpeedButtons called with selectedSpeed: " + selectedSpeed);
        // Reset all buttons to default color
        speedButtonNormal.GetComponentInChildren<TMP_Text>().color = defaultColor;
        speedButtonSlow.GetComponentInChildren<TMP_Text>().color = defaultColor;
        speedButtonSlowest.GetComponentInChildren<TMP_Text>().color = defaultColor;

        // Set selected button to selected color
        if (selectedSpeed == normalSpeed)
        {
            speedButtonNormal.GetComponentInChildren<TMP_Text>().color = selectedColor;
        }
        else if (selectedSpeed == slowSpeed)
        {
            speedButtonSlow.GetComponentInChildren<TMP_Text>().color = selectedColor;
        }
        else if (selectedSpeed == slowestSpeed)
        {
            speedButtonSlowest.GetComponentInChildren<TMP_Text>().color = selectedColor;
        }
    }

    private void UpdateButtonText()
    {
        Debug.Log("UpdateButtonText called, isPlaying: " + isPlaying);
        if (isPlaying)
        {
            buttonText.text = "Pause";
        }
        else
        {
            buttonText.text = "Play";
        }
        Debug.Log("Button text updated to: " + buttonText.text);
    }
}
