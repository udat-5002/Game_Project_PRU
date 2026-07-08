using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public const string SceneMainMenu = "MainMenu";
    public const string SceneChapter1 = "Chapter1";
    public const string SceneChapter2 = "Chapter2";
    public const string SceneChapter3 = "Chapter3";

    public int CurrentChapter { get; private set; } = 1;
    public bool InputLocked { get; private set; }

    [Header("Player")]
    public Transform player;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
    }

    public void FindPlayer()
    {
        if (player != null) return;

        var controller = FindFirstObjectByType<ThirdPersonController>();
        if (controller != null)
            player = controller.transform;
    }

    public void SetCurrentChapter(int chapter)
    {
        CurrentChapter = chapter;
        PlayerPrefs.SetInt("CurrentChapter", chapter);
        PlayerPrefs.Save();
    }

    public void LockInput(bool locked)
    {
        InputLocked = locked;
    }

    public void TeleportPlayer(Vector3 position)
    {
        FindPlayer();
        if (player == null) return;

        position = GroundSnap.SnapPlayer(position, player);

        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.position = position;

        if (cc != null) cc.enabled = true;

        var tpc = player.GetComponent<ThirdPersonController>();
        tpc?.ResetVerticalVelocity();

        Physics.SyncTransforms();
    }

    public void CompleteChapter(int chapter)
    {
        if (chapter < 3)
        {
            int next = chapter + 1;
            string nextScene = next switch
            {
                2 => SceneChapter2,
                3 => SceneChapter3,
                _ => SceneChapter1
            };

            string title = next switch
            {
                2 => "Chương 2: Lá thư của người lính",
                3 => "Chương 3: Lá Thư Cuối Cùng",
                _ => ""
            };

            string subtitle = next switch
            {
                2 => Chapter2Dialogue.TransitionSubtitle,
                3 => Chapter3Dialogue.TransitionSubtitle,
                _ => ""
            };

            string voiceKey = next switch
            {
                2 => Chapter2Voice.Transition,
                3 => Chapter3Voice.Transition,
                _ => null
            };

            SceneTransition.Instance?.TransitionToChapter(nextScene, title, subtitle, next, voiceKey);
        }
        else
        {
            SceneTransition.Instance?.ShowEnding(
                "Chiến tranh lấy đi tất cả, chỉ để lại một thứ duy nhất để chúng ta sống tiếp: đó là hy vọng.",
                () => SceneTransition.Instance.LoadScene(SceneMainMenu),
                Chapter3Voice.Ending);
        }
    }

    public void StartNewGame()
    {
        PlayerPrefs.DeleteKey("CurrentChapter");
        SetCurrentChapter(1);
        SceneTransition.Instance?.TransitionToChapter(
            SceneChapter1,
            "Chương 1: Con Đường Hy Vọng",
            Chapter1Dialogue.TransitionSubtitle,
            1,
            Chapter1Voice.Transition);
    }
}
