using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndManager : MonoBehaviour
{
    private const int RequiredLapsToWin = 2;
    private const string StartTileName = "Start";

    private PlayerStats playerStats;
    private PlayerTileMover playerTileMover;
    private Canvas endCanvas;
    private TMP_Text titleText;
    private TMP_Text reasonText;
    private int completedLaps;
    private bool isGameEnded;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureGameEndManager()
    {
        if (FindAnyObjectByType<GameEndManager>() != null)
        {
            return;
        }

        GameObject managerObject = new GameObject("Game End Manager");
        managerObject.AddComponent<GameEndManager>();
    }

    private void Awake()
    {
        playerStats = FindAnyObjectByType<PlayerStats>();
        playerTileMover = FindAnyObjectByType<PlayerTileMover>();

        if (playerStats != null)
        {
            playerStats.AnswerApplied += OnAnswerApplied;
        }

        if (playerTileMover != null)
        {
            playerTileMover.TileReached += OnTileReached;
        }

        CreateEndScreen();
    }

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.AnswerApplied -= OnAnswerApplied;
        }

        if (playerTileMover != null)
        {
            playerTileMover.TileReached -= OnTileReached;
        }
    }

    private void OnAnswerApplied(AnswerData answer)
    {
        if (isGameEnded || answer == null || playerStats == null)
        {
            return;
        }

        if (answer.knowledgeChange < 0 && playerStats.Knowledge <= 0)
        {
            ShowLose("Знания закончились");
            return;
        }

        if (answer.friendshipChange < 0 && playerStats.Friendship <= 0)
        {
            ShowLose("Дружба закончилась");
            return;
        }

        if (answer.moneyChange < 0 && playerStats.Money <= 0)
        {
            ShowLose("Деньги закончились");
        }
    }

    private void OnTileReached(BoardTile tile)
    {
        if (isGameEnded || tile == null || tile.name != StartTileName)
        {
            return;
        }

        completedLaps++;

        if (completedLaps >= RequiredLapsToWin)
        {
            ShowWin();
        }
    }

    private void ShowLose(string reason)
    {
        ShowEndScreen("ТЫ ПРОИГРАЛ", reason);
    }

    private void ShowWin()
    {
        ShowEndScreen("ТЫ ВЫЖИЛ ДО КОНЦА ГОДА", "Ты прошёл 2 круга");
    }

    private void ShowEndScreen(string title, string reason)
    {
        isGameEnded = true;
        Time.timeScale = 0f;

        titleText.text = title;
        reasonText.text = reason;
        endCanvas.gameObject.SetActive(true);
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void CreateEndScreen()
    {
        GameObject canvasObject = new GameObject("Game End Canvas");
        canvasObject.transform.SetParent(transform, false);

        endCanvas = canvasObject.AddComponent<Canvas>();
        endCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        endCanvas.sortingOrder = 200;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject background = new GameObject("End Screen Background");
        background.transform.SetParent(canvasObject.transform, false);

        RectTransform backgroundRect = background.AddComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = new Color(0.04f, 0.06f, 0.08f, 0.78f);

        GameObject card = new GameObject("End Screen Card");
        card.transform.SetParent(background.transform, false);

        RectTransform cardRect = card.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = Vector2.zero;
        cardRect.sizeDelta = new Vector2(820f, 760f);

        Image cardImage = card.AddComponent<Image>();
        cardImage.color = new Color(0.96f, 0.82f, 0.56f, 0.98f);

        titleText = CreateText(card.transform, "End Title", new Vector2(0f, 180f), new Vector2(700f, 170f), 58, TextAlignmentOptions.Center);
        reasonText = CreateText(card.transform, "End Reason", new Vector2(0f, 20f), new Vector2(660f, 150f), 36, TextAlignmentOptions.Center);

        Button restartButton = CreateButton(card.transform);
        restartButton.onClick.AddListener(RestartGame);

        canvasObject.SetActive(false);
    }

    private static TMP_Text CreateText(Transform parent, string name, Vector2 position, Vector2 size, int fontSize, TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.fontSize = fontSize;
        text.color = new Color(0.18f, 0.16f, 0.13f, 1f);
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;

        return text;
    }

    private static Button CreateButton(Transform parent)
    {
        GameObject buttonObject = new GameObject("Restart Button");
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(0f, -235f);
        rectTransform.sizeDelta = new Vector2(520f, 110f);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.16f, 0.68f, 0.42f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        ColorBlock colors = button.colors;
        colors.normalColor = image.color;
        colors.highlightedColor = Color.Lerp(image.color, Color.white, 0.16f);
        colors.pressedColor = Color.Lerp(image.color, Color.black, 0.18f);
        button.colors = colors;

        TMP_Text label = CreateText(buttonObject.transform, "Label", Vector2.zero, rectTransform.sizeDelta, 38, TextAlignmentOptions.Center);
        label.text = "НАЧАТЬ ЗАНОВО";
        label.color = Color.white;

        return button;
    }
}
