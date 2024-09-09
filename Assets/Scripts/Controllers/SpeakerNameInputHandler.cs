using UnityEngine;
using TMPro;

public class SpeakerNameInputHandler : MonoBehaviour
{
    public Speaker speaker;
    public TMP_InputField nameInputField;
    public TextMeshProUGUI speakerNameTextMeshPro; // Reference to the TextMeshPro object where speaker name will be displayed

    public void SetSpeakerCustomName()
    {
        string newName = nameInputField.text;
        speaker.SetCustomName(newName);

        // Update TextMeshPro text with the new speaker name
        if (speakerNameTextMeshPro != null)
        {
            speakerNameTextMeshPro.text = newName;
        }
    }
}