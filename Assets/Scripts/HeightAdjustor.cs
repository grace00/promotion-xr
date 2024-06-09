using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HeightAdjuster : MonoBehaviour
{
    public Button settingsButton;
    public GameObject settingsModal;
    public TMP_InputField feetInputField;
    public TMP_InputField inchesInputField;
    public Button saveHeightButton;
    public Button closeSettingsButton;
    public Transform modelToScale;
    public GameObject vrKeypad; // Reference to the VRKeypad GameObject

    private VRKeypad vrKeypadScript;
    private NewAnimationController animationController;

    private void Start()
    {
        vrKeypadScript = vrKeypad.GetComponent<VRKeypad>();
        animationController = GetComponent<NewAnimationController>();

        settingsButton.onClick.AddListener(OpenSettings);
        saveHeightButton.onClick.AddListener(SaveHeight);
        closeSettingsButton.onClick.AddListener(CloseSettings);
        settingsModal.SetActive(false);

        // Add listeners to input fields
        feetInputField.onSelect.AddListener(delegate { OpenKeypad(feetInputField); });
        inchesInputField.onSelect.AddListener(delegate { OpenKeypad(inchesInputField); });

        // Add Event Trigger for selecting the input field
        AddSelectEventTrigger(feetInputField);
        AddSelectEventTrigger(inchesInputField);
    }

    private void OpenSettings()
    {
        settingsModal.SetActive(true);
        animationController.PauseAnimation(); // Pause the animation when settings are opened
        modelToScale.gameObject.SetActive(false); // Hide the model when settings are opened
    }

    private void CloseSettings()
    {
        settingsModal.SetActive(false);
        modelToScale.gameObject.SetActive(true); // Show the model when settings are closed
        // Reset the active input field in the VRKeypad
        vrKeypadScript.ResetInputField();
    }

    private void SaveHeight()
    {
        if (int.TryParse(feetInputField.text, out int feet) && int.TryParse(inchesInputField.text, out int inches))
        {
            float totalHeightInMeters = ConvertHeightToMeters(feet, inches);
            ScaleModel(totalHeightInMeters);
            CloseSettings();
        }
        else
        {
            Debug.LogError("Invalid input for height.");
        }
    }

    private float ConvertHeightToMeters(int feet, int inches)
    {
        // Convert feet and inches to meters
        float totalHeightInInches = (feet * 12) + inches;
        return totalHeightInInches * 0.0254f; // 1 inch = 0.0254 meters
    }

    private void ScaleModel(float heightInMeters)
    {
        // Assuming the original height of the model is 1.8 meters
        float originalHeight = 1.8f; // You may need to adjust this based on your model's original height
        float scaleFactor = heightInMeters / originalHeight;
        modelToScale.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }

    private void OpenKeypad(TMP_InputField inputField)
    {
        vrKeypadScript.SetInputField(inputField);
        vrKeypad.SetActive(true); // Show the VR keypad
        inputField.Select(); // Ensure the input field is selected
    }

    private void AddSelectEventTrigger(TMP_InputField inputField)
    {
        EventTrigger trigger = inputField.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = inputField.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((eventData) => { inputField.Select(); });
        trigger.triggers.Add(entry);
    }
}
