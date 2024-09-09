using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameScene currentScene;
    public BottomBarController bottomBar;
    public SpriteSwitcher backgroundController;
    public ChooseController chooseController;
    public AudioController audioController;
    public UnitySceneChangeManager sceneChangeManager;
    public GameObject characterContainer; // Container for characters in the scene
    public Vector3 offScreenPosition = new Vector3(-1000, -1000, -1000); // Off-screen position for characters

    public DataHolder data;

    public string menuScene;

    private State state = State.IDLE;

    private List<StoryScene> history = new List<StoryScene>();

    private enum State
    {
        IDLE, ANIMATE, CHOOSE
    }

    private float lastClickTime = 0f;
    private int clickCount = 0;
    private const float clickInterval = 0.5f; // Interval for click detection

    void Start()
    {
        // Check for saved game data first
        if (SaveManager.IsGameSaved())
        {
            LoadGame(); // Load saved game data
        }
        else
        {
            StartNewGame(); // Start a new game if no saved data exists
        }
    }

    void Update()
    {
        if (state == State.IDLE)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleClick();
            }
            if (Input.GetMouseButtonDown(1))
            {
                HandleRightClick();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SaveGameAndLoadMenu();
            }
        }
    }

    private void StartNewGame()
    {
        if (currentScene is StoryScene)
        {
            StoryScene storyScene = currentScene as StoryScene;
            history.Add(storyScene);
            bottomBar.PlayScene(storyScene);
            backgroundController.SetImage(storyScene.background);
            PlayAudio(storyScene.sentences[0]);
        }
        else if (currentScene is UnityGameScene)
        {
            UnityGameScene unityGameScene = currentScene as UnityGameScene;
            SceneManager.LoadScene(unityGameScene.sceneName);
        }
    }

    private void LoadGame()
    {
        SaveData savedData = SaveManager.LoadGame();

        if (savedData != null)
        {
            history.Clear();
            foreach (int sceneIndex in savedData.prevScenes)
            {
                history.Add((StoryScene)data.scenes[sceneIndex]);
            }

            StoryScene lastScene = history[history.Count - 1];
            currentScene = lastScene;
            bottomBar.SetSentenceIndex(savedData.sentence);

            // Directly set the scene without initializing it again
            backgroundController.SetImage(lastScene.background);
            bottomBar.PlayScene(lastScene, savedData.sentence, false);

            // Set the state to IDLE immediately to avoid any unintended actions
            state = State.IDLE;
        }
    }

    private void HandleClick()
    {
        clickCount++;
        float currentTime = Time.time;

        if (clickCount == 1)
        {
            lastClickTime = currentTime;
        }

        if (clickCount == 2 && (currentTime - lastClickTime) <= clickInterval)
        {
            bottomBar.SpeedUp();
            clickCount = 0; // Reset click count
        }
        else if (clickCount == 3 && (currentTime - lastClickTime) <= clickInterval)
        {
            bottomBar.StopTyping();
            clickCount = 0; // Reset click count
        }

        if (currentTime - lastClickTime > clickInterval)
        {
            clickCount = 0; // Reset click count if clicks are too slow
        }

        if (bottomBar.IsCompleted())
        {
            if (bottomBar.IsLastSentence())
            {
                PlayScene((currentScene as StoryScene).nextScene);
            }
            else
            {
                bottomBar.PlayNextSentence();
                PlayAudio((currentScene as StoryScene).sentences[bottomBar.GetSentenceIndex()]);
            }
        }
    }

    private void HandleRightClick()
    {
        if (bottomBar.IsFirstSentence())
        {
            if (history.Count > 1)
            {
                bottomBar.StopTyping();
                bottomBar.HideSprites();
                history.RemoveAt(history.Count - 1);
                StoryScene scene = history[history.Count - 1];
                history.RemoveAt(history.Count - 1);
                PlayScene(scene, scene.sentences.Count - 2, false);
            }
        }
        else
        {
            bottomBar.GoBack();
        }
    }

    private void SaveGameAndLoadMenu()
    {
        List<int> historyIndices = new List<int>();
        history.ForEach(scene =>
        {
            historyIndices.Add(this.data.scenes.IndexOf(scene));
        });
        SaveData data = new SaveData
        {
            sentence = bottomBar.GetSentenceIndex(),
            prevScenes = historyIndices
        };
        SaveManager.SaveGame(data);
        SceneManager.LoadScene(menuScene);
    }

    public void PlayScene(GameScene scene, int sentenceIndex = -1, bool isAnimated = true)
    {
        CleanupScene(isAnimated);
        StartCoroutine(SwitchScene(scene, sentenceIndex, isAnimated));
    }

    private void CleanupScene(bool isAnimated)
    {
        foreach (Transform child in characterContainer.transform)
        {
            Animator animator = child.GetComponent<Animator>();
            if (animator != null && isAnimated)
            {
                animator.SetTrigger("FadeOut"); // Trigger fade-out animation
                StartCoroutine(MoveOffScreenAfterAnimation(child.gameObject, animator));
            }
            else
            {
                MoveOffScreen(child.gameObject); // Move off-screen without animation
            }
        }
    }

    private IEnumerator MoveOffScreenAfterAnimation(GameObject obj, Animator animator)
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        MoveOffScreen(obj);
    }

    private void MoveOffScreen(GameObject obj)
    {
        obj.transform.position = offScreenPosition; // Move object off-screen
    }

    private IEnumerator SwitchScene(GameScene scene, int sentenceIndex = -1, bool isAnimated = true)
    {
        state = State.ANIMATE;
        currentScene = scene;
        if (isAnimated)
        {
            bottomBar.Hide();
            yield return new WaitForSeconds(1f);
        }
        if (scene is StoryScene)
        {
            StoryScene storyScene = scene as StoryScene;
            history.Add(storyScene);
            PlayAudio(storyScene.sentences[sentenceIndex + 1]);
            if (isAnimated)
            {
                backgroundController.SwitchImage(storyScene.background);
                yield return new WaitForSeconds(1f);
                bottomBar.ClearText();
                bottomBar.Show();
                yield return new WaitForSeconds(1f);
            }
            else
            {
                backgroundController.SetImage(storyScene.background);
                bottomBar.ClearText();
            }
            bottomBar.PlayScene(storyScene, sentenceIndex, isAnimated);
            state = State.IDLE;
        }
        else if (scene is ChooseScene)
        {
            state = State.CHOOSE;
            chooseController.SetupChoose(scene as ChooseScene);
        }
        else if (scene is UnityGameScene)
        {
            UnityGameScene unityGameScene = scene as UnityGameScene;
            SceneManager.LoadScene(unityGameScene.sceneName);
        }
    }

    private void PlayAudio(StoryScene.Sentence sentence)
    {
        audioController.PlayAudio(sentence.music, sentence.sound);
    }
}
