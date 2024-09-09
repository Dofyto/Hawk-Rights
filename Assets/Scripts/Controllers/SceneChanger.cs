using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneChanger : MonoBehaviour
{
    public TMP_InputField inputField;
    public string sceneToLoad;

    private void Start()
    {
        // Subscribe to the OnEndEdit event of the TMP_InputField
        inputField.onEndEdit.AddListener(delegate { OnEndEdit(inputField); });
    }

    // This method will be called when the user ends editing the TMP_InputField
    private void OnEndEdit(TMP_InputField inputField)
    {
        // Check if the input field's text meets your condition (e.g., pressing Enter)
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // If the condition is met, change the scene
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
