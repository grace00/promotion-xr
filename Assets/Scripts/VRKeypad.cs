using UnityEngine;
using TMPro;

public class VRKeypad : MonoBehaviour
{
    private TMP_InputField activeInputField;

    // Set the active input field
    public void SetInputField(TMP_InputField inputField)
    {
        activeInputField = inputField;
        Debug.Log("Active Input Field set to: " + inputField.name);
    }

    // Insert a number into the active input field
    public void InsertNumber(string number)
    {
        if (activeInputField != null)
        {
            activeInputField.text += number;
        }
    }

    // Remove the last character from the active input field
    public void Backspace()
    {
        if (activeInputField != null && activeInputField.text.Length > 0)
        {
            activeInputField.text = activeInputField.text.Substring(0, activeInputField.text.Length - 1);
        }
    }

    // Handle the enter key logic if needed
    public void Enter()
    {
        // Hide the VR keypad
        gameObject.SetActive(false);
    }

    // Reset the active input field
    public void ResetInputField()
    {
        activeInputField = null;
    }
}
