using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BookmarkManager : MonoBehaviour
{
    public Button backswingButton;
    public Button downswingButton;
    public Button followThroughButton;
    public Button finishButton;
    public Color selectedBackgroundColor = new Color32(124, 64, 249, 255); // 7C40F9
    public Color defaultTextColor = new Color32(247, 243, 255, 255); // F7F3FF
    public Color defaultBackgroundColor = new Color32(54, 18, 131, 255); // 361283
    public Sprite defaultIcon;
    public Sprite selectedIcon;

    private NewAnimationController animationController;

    void Start()
    {
        animationController = FindObjectOfType<NewAnimationController>();

        // Add listeners to the buttons
        backswingButton.onClick.AddListener(() => SelectBookmark(backswingButton, 0.0f));
        downswingButton.onClick.AddListener(() => SelectBookmark(downswingButton, 0.46f));
        followThroughButton.onClick.AddListener(() => SelectBookmark(followThroughButton, 0.625f));
        finishButton.onClick.AddListener(() => SelectBookmark(finishButton, 0.99f));

        // Initialize button colors and icons
        ResetAllBookmarks();
    }

    void SelectBookmark(Button button, float normalizedTime)
    {
        ResetAllBookmarks(); // Reset all bookmarks to default state

        // Set the selected button's background color and icon
        button.GetComponentInChildren<TMP_Text>().color = defaultTextColor;
        button.GetComponent<Image>().sprite = selectedIcon; // Update icon image
        button.GetComponent<Image>().color = selectedBackgroundColor; // Update background color
        button.GetComponent<Image>().enabled = true; // Ensure the image is visible

        // Jump to the corresponding animation time
        animationController.JumpToPosition(normalizedTime * animationController.animationLength);
    }

    void ResetAllBookmarks()
    {
        // Reset colors and icons for all buttons
        ResetBookmark(backswingButton);
        ResetBookmark(downswingButton);
        ResetBookmark(followThroughButton);
        ResetBookmark(finishButton);
    }

    void ResetBookmark(Button button)
    {
        button.GetComponentInChildren<TMP_Text>().color = defaultTextColor;
        button.GetComponent<Image>().sprite = defaultIcon;
        button.GetComponent<Image>().color = defaultBackgroundColor; // Set background color
        button.GetComponent<Image>().enabled = false; // Ensure the image is hidden
    }
}
