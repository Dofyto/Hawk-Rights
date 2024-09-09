using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitySceneChangeManager : MonoBehaviour
{
    public GameObject unitySceneChangeButtonPrefab; // Assign this in the inspector

    public void CreateSceneChangeButton(string sceneName, Vector3 position)
    {
        if (unitySceneChangeButtonPrefab == null)
        {
            Debug.LogError("unitySceneChangeButtonPrefab is not assigned.");
            return;
        }

        GameObject buttonInstance = Instantiate(unitySceneChangeButtonPrefab, transform);
        buttonInstance.transform.localPosition = position;

        UnitySceneChangeButtonController buttonController = buttonInstance.GetComponent<UnitySceneChangeButtonController>();
        if (buttonController != null)
        {
            buttonController.Setup(sceneName);
        }
        else
        {
            Debug.LogError("UnitySceneChangeButtonController is not attached to the prefab.");
        }
    }
}
