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
                2 => "Chương 2: Bóng Tối Chiến Tranh",
                3 => "Chương 3: Lá Thư Cuối Cùng",
                _ => ""
            };

            string subtitle = next switch
            {
                2 => "Lén lút qua rừng đêm — tránh lính tuần tra,\ntìm chỗ trú mưa trước khi giao thư.",
                3 => "Tìm manh mối trên chiến trường cũ,\ngiao lá thư cuối cùng.",
                _ => ""
            };

            SceneTransition.Instance?.TransitionToChapter(nextScene, title, subtitle, next);
        }
        else
        {
            SceneTransition.Instance?.ShowEnding(
                "Chiến tranh có thể chia cắt con người,\nnhưng hy vọng luôn tìm được đường để đến nơi cần đến.",
                () => SceneTransition.Instance.LoadScene(SceneMainMenu));
        }
    }

    public void StartNewGame()
    {
        PlayerPrefs.DeleteKey("CurrentChapter");
        SetCurrentChapter(1);
        SceneTransition.Instance?.TransitionToChapter(
            SceneChapter1,
            "Chương 1: Con Đường Hy Vọng",
            "Nam bắt đầu hành trình đưa thư\nqua vùng chiến sự miền Trung.",
            1);
    }
}
