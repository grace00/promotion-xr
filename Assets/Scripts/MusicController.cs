using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the AudioSource component
    public Button toggleMusicButton; // Reference to the UI Button
    public Sprite musicOnSprite; // Sprite for music on state
    public Sprite musicOffSprite; // Sprite for music off state

    private bool isMusicPlaying = true;

    void Start()
    {
        // Ensure the AudioSource is playing at the start
        audioSource.Play();
        isMusicPlaying = true;

        // Add listener to the button
        toggleMusicButton.onClick.AddListener(ToggleMusic);

        // Set initial button image
        UpdateButtonImage();
    }

    void ToggleMusic()
    {
        if (isMusicPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
        }

        isMusicPlaying = !isMusicPlaying;

        // Update the button image
        UpdateButtonImage();
    }

    void UpdateButtonImage()
    {
        if (isMusicPlaying)
        {
            toggleMusicButton.GetComponent<Image>().sprite = musicOnSprite;
        }
        else
        {
            toggleMusicButton.GetComponent<Image>().sprite = musicOffSprite;
        }
    }
}
