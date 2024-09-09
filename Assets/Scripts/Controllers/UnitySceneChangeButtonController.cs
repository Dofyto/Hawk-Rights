using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UnitySceneChangeButtonController : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    private string sceneName;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    public void Setup(string sceneName)
    {
        this.sceneName = sceneName;
        buttonText.text = "Change Scene to " + sceneName;
    }

    private void OnButtonClick()
    {
        SceneManager.LoadScene(sceneName);
    }
}
