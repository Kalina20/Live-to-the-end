using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private const int StartTotalPoints = 20;
    private const int StartMoney = 2000;

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject rollButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Camera menuCamera;
    [SerializeField] private CameraZoomController cameraZoomController;
    [SerializeField] private Vector3 menuCameraPosition = new Vector3(3f, 7.5f, 0.8f);
    [SerializeField] private Vector3 menuCameraLookAt = new Vector3(3f, 0f, 6f);

    private PlayerStatsUI inGameHud;
    private GameObject rulesPanel;
    private GameObject setupPanel;
    private Slider knowledgeSlider;
    private Slider friendshipSlider;
    private TMP_Text knowledgeValueText;
    private TMP_Text friendshipValueText;
    private bool isUpdatingSetupSliders;
    private Vector3 gameplayCameraPosition;
    private Quaternion gameplayCameraRotation;
    private bool cameraZoomWasEnabled;
    private bool isPrepared;

    private void Awake()
    {
        Time.timeScale = 0f;

        if (rollButton != null)
        {
            rollButton.SetActive(false);
        }

        PrepareMenuCamera();
        PrepareCanvas();
        PrepareMenu();
        isPrepared = true;
    }

    private void Start()
    {
        Time.timeScale = 0f;

        if (!isPrepared)
        {
            PrepareMenuCamera();
            PrepareCanvas();
            PrepareMenu();
        }

        HideGameUi();

        if (playerStats == null)
        {
            playerStats = FindAnyObjectByType<PlayerStats>();
        }

        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(ShowSetup);
        }
    }

    private void PrepareMenuCamera()
    {
        if (menuCamera == null)
        {
            menuCamera = Camera.main;
        }

        if (menuCamera == null)
        {
            return;
        }

        gameplayCameraPosition = menuCamera.transform.position;
        gameplayCameraRotation = menuCamera.transform.rotation;

        if (cameraZoomController == null)
        {
            cameraZoomController = menuCamera.GetComponent<CameraZoomController>();
        }

        if (cameraZoomController != null)
        {
            cameraZoomWasEnabled = cameraZoomController.enabled;
            cameraZoomController.enabled = false;
        }

        menuCamera.transform.position = menuCameraPosition;
        menuCamera.transform.rotation = Quaternion.LookRotation(menuCameraLookAt - menuCameraPosition, Vector3.up);
    }

    private void PrepareCanvas()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = gameObject.AddComponent<CanvasScaler>();
        }

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;

        if (GetComponent<GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }

        transform.localScale = Vector3.one;
    }

    private void PrepareMenu()
    {
        if (mainMenuPanel == null)
        {
            mainMenuPanel = new GameObject("Main Menu Panel");
            mainMenuPanel.transform.SetParent(transform, false);
        }

        mainMenuPanel.SetActive(true);

        RectTransform panelRect = mainMenuPanel.GetComponent<RectTransform>();
        if (panelRect == null)
        {
            panelRect = mainMenuPanel.AddComponent<RectTransform>();
        }

        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panelRect.localScale = Vector3.one;

        Image panelImage = mainMenuPanel.GetComponent<Image>();
        if (panelImage == null)
        {
            panelImage = mainMenuPanel.AddComponent<Image>();
        }

        panelImage.color = new Color(0.04f, 0.06f, 0.08f, 0.42f);
        panelImage.raycastTarget = true;

        ClearPanelChildren();
        BuildMenuContent(mainMenuPanel.transform);
        BuildSetupPanel(mainMenuPanel.transform);
        BuildRulesPanel(mainMenuPanel.transform);
    }

    private void ClearPanelChildren()
    {
        for (int i = mainMenuPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(mainMenuPanel.transform.GetChild(i).gameObject);
        }
    }

    private void BuildMenuContent(Transform parent)
    {
        GameObject content = new GameObject("Main Menu Content");
        content.transform.SetParent(parent, false);

        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.pivot = new Vector2(0.5f, 0.5f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        Image contentImage = content.AddComponent<Image>();
        contentImage.color = new Color(0.04f, 0.06f, 0.08f, 0.2f);

        CreateText(content.transform, "Game Title", new Vector2(0f, 260f), new Vector2(820f, 140f), "LIVE TO THE END", 76, Color.white, TextAlignmentOptions.Center);

        startButton = CreateButton(content.transform, "Start Button", new Vector2(0f, -120f), new Vector2(520f, 120f), "ИГРАТЬ", new Color(0.16f, 0.68f, 0.42f, 1f));
        startButton.onClick.AddListener(ShowSetup);

        Button rulesButton = CreateButton(content.transform, "Rules Button", new Vector2(0f, -265f), new Vector2(520f, 105f), "ПРАВИЛА", new Color(0.14f, 0.42f, 0.82f, 1f));
        rulesButton.onClick.AddListener(ShowRules);
    }

    private void BuildSetupPanel(Transform parent)
    {
        setupPanel = new GameObject("Start Setup Panel");
        setupPanel.transform.SetParent(parent, false);

        RectTransform panelRect = setupPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = setupPanel.AddComponent<Image>();
        panelImage.color = new Color(0.04f, 0.06f, 0.08f, 0.72f);

        GameObject card = new GameObject("Start Setup Card");
        card.transform.SetParent(setupPanel.transform, false);

        RectTransform cardRect = card.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = Vector2.zero;
        cardRect.sizeDelta = new Vector2(840f, 980f);

        Image cardImage = card.AddComponent<Image>();
        cardImage.color = new Color(0.96f, 0.82f, 0.56f, 0.98f);

        CreateText(card.transform, "Setup Title", new Vector2(0f, 355f), new Vector2(700f, 90f), "СТАРТ ИГРЫ", 54, new Color(0.18f, 0.16f, 0.13f, 1f), TextAlignmentOptions.Center);
        CreateText(card.transform, "Setup Hint", new Vector2(0f, 260f), new Vector2(680f, 80f), "Распредели 20 очков", 34, new Color(0.24f, 0.2f, 0.17f, 1f), TextAlignmentOptions.Center);

        CreateText(card.transform, "Knowledge Label", new Vector2(-235f, 140f), new Vector2(260f, 55f), "Знания", 32, new Color(0.18f, 0.16f, 0.13f, 1f), TextAlignmentOptions.Left);
        knowledgeValueText = CreateText(card.transform, "Knowledge Value", new Vector2(295f, 140f), new Vector2(100f, 55f), "10", 34, new Color(0.18f, 0.16f, 0.13f, 1f), TextAlignmentOptions.Center);
        knowledgeSlider = CreateSlider(card.transform, "Knowledge Start Slider", new Vector2(0f, 70f), new Vector2(620f, 46f), new Color(0.12f, 0.42f, 1f, 1f));

        CreateText(card.transform, "Friendship Label", new Vector2(-235f, -35f), new Vector2(260f, 55f), "Дружба", 32, new Color(0.18f, 0.16f, 0.13f, 1f), TextAlignmentOptions.Left);
        friendshipValueText = CreateText(card.transform, "Friendship Value", new Vector2(295f, -35f), new Vector2(100f, 55f), "10", 34, new Color(0.18f, 0.16f, 0.13f, 1f), TextAlignmentOptions.Center);
        friendshipSlider = CreateSlider(card.transform, "Friendship Start Slider", new Vector2(0f, -105f), new Vector2(620f, 46f), new Color(0.95f, 0.12f, 0.12f, 1f));

        CreateText(card.transform, "Money Start Text", new Vector2(0f, -225f), new Vector2(640f, 65f), "Деньги: 2000", 36, new Color(0.18f, 0.16f, 0.13f, 1f), TextAlignmentOptions.Center);

        knowledgeSlider.onValueChanged.AddListener(OnKnowledgeStartChanged);
        friendshipSlider.onValueChanged.AddListener(OnFriendshipStartChanged);
        SetStartValues(10, 10);

        Button beginButton = CreateButton(card.transform, "Begin Game Button", new Vector2(0f, -345f), new Vector2(520f, 105f), "НАЧАТЬ", new Color(0.16f, 0.68f, 0.42f, 1f));
        beginButton.onClick.AddListener(StartGame);

        Button backButton = CreateButton(card.transform, "Back To Menu Button", new Vector2(0f, -465f), new Vector2(420f, 80f), "НАЗАД", new Color(0.78f, 0.2f, 0.18f, 1f));
        backButton.onClick.AddListener(HideSetup);

        setupPanel.SetActive(false);
    }

    private void BuildRulesPanel(Transform parent)
    {
        rulesPanel = new GameObject("Rules Panel");
        rulesPanel.transform.SetParent(parent, false);

        RectTransform panelRect = rulesPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = rulesPanel.AddComponent<Image>();
        panelImage.color = new Color(0.04f, 0.06f, 0.08f, 0.72f);

        GameObject rulesCard = new GameObject("Rules Card");
        rulesCard.transform.SetParent(rulesPanel.transform, false);

        RectTransform cardRect = rulesCard.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = Vector2.zero;
        cardRect.sizeDelta = new Vector2(840f, 1020f);

        Image cardImage = rulesCard.AddComponent<Image>();
        cardImage.color = new Color(0.98f, 0.88f, 0.66f, 0.98f);

        CreateText(rulesCard.transform, "Rules Title", new Vector2(0f, 390f), new Vector2(690f, 90f), "ПРАВИЛА", 54, new Color(0.18f, 0.16f, 0.13f, 1f), TextAlignmentOptions.Center);

        string rulesText =
            "1. Нажми \"Играть\", чтобы начать партию.\n\n" +
            "2. Нажимай Roll и бросай кубик.\n\n" +
            "3. Фишка переходит на клетку по выпавшему символу.\n\n" +
            "4. После хода вытягивается карта из нужной стопки.\n\n" +
            "5. Выбери один из двух ответов. Ответ меняет знания, дружбу или деньги.\n\n" +
            "6. Если ресурса не хватает, такой ответ выбрать нельзя.";

        CreateText(rulesCard.transform, "Rules Text", new Vector2(0f, 35f), new Vector2(690f, 590f), rulesText, 30, new Color(0.24f, 0.2f, 0.17f, 1f), TextAlignmentOptions.Left);

        Button closeButton = CreateButton(rulesCard.transform, "Close Rules Button", new Vector2(0f, -405f), new Vector2(420f, 95f), "НАЗАД", new Color(0.78f, 0.2f, 0.18f, 1f));
        closeButton.onClick.AddListener(HideRules);

        rulesPanel.SetActive(false);
    }

    private void HideGameUi()
    {
        inGameHud = FindAnyObjectByType<PlayerStatsUI>();
        if (inGameHud != null)
        {
            inGameHud.gameObject.SetActive(false);
        }

        if (rollButton != null)
        {
            rollButton.SetActive(false);
        }
    }

    private void StartGame()
    {
        Time.timeScale = 1f;
        RestoreGameplayCamera();

        if (playerStats == null)
        {
            playerStats = FindAnyObjectByType<PlayerStats>();
        }

        if (playerStats != null)
        {
            playerStats.InitializeStats(
                Mathf.RoundToInt(knowledgeSlider.value),
                Mathf.RoundToInt(friendshipSlider.value),
                StartMoney
            );
        }

        if (inGameHud == null)
        {
            inGameHud = FindAnyObjectByType<PlayerStatsUI>(FindObjectsInactive.Include);
        }

        if (inGameHud != null)
        {
            inGameHud.gameObject.SetActive(true);
        }

        if (rollButton != null)
        {
            rollButton.SetActive(true);
        }

        mainMenuPanel.SetActive(false);
    }

    private void ShowSetup()
    {
        if (setupPanel != null)
        {
            setupPanel.SetActive(true);
        }
    }

    private void HideSetup()
    {
        if (setupPanel != null)
        {
            setupPanel.SetActive(false);
        }
    }

    private void RestoreGameplayCamera()
    {
        if (menuCamera != null)
        {
            menuCamera.transform.position = gameplayCameraPosition;
            menuCamera.transform.rotation = gameplayCameraRotation;
        }

        if (cameraZoomController != null)
        {
            cameraZoomController.enabled = cameraZoomWasEnabled;
        }
    }

    private void ShowRules()
    {
        if (rulesPanel != null)
        {
            rulesPanel.SetActive(true);
        }
    }

    private void HideRules()
    {
        if (rulesPanel != null)
        {
            rulesPanel.SetActive(false);
        }
    }

    private void OnKnowledgeStartChanged(float value)
    {
        if (isUpdatingSetupSliders)
        {
            return;
        }

        SetStartValues(Mathf.RoundToInt(value), StartTotalPoints - Mathf.RoundToInt(value));
    }

    private void OnFriendshipStartChanged(float value)
    {
        if (isUpdatingSetupSliders)
        {
            return;
        }

        SetStartValues(StartTotalPoints - Mathf.RoundToInt(value), Mathf.RoundToInt(value));
    }

    private void SetStartValues(int knowledge, int friendship)
    {
        isUpdatingSetupSliders = true;

        knowledge = Mathf.Clamp(knowledge, 0, StartTotalPoints);
        friendship = Mathf.Clamp(friendship, 0, StartTotalPoints);

        knowledgeSlider.value = knowledge;
        friendshipSlider.value = friendship;

        if (knowledgeValueText != null)
        {
            knowledgeValueText.text = knowledge.ToString();
        }

        if (friendshipValueText != null)
        {
            friendshipValueText.text = friendship.ToString();
        }

        isUpdatingSetupSliders = false;
    }

    private static TMP_Text CreateText(Transform parent, string name, Vector2 position, Vector2 size, string text, int fontSize, Color color, TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;

        TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = color;
        textComponent.alignment = alignment;
        textComponent.textWrappingMode = TextWrappingModes.Normal;

        return textComponent;
    }

    private static Button CreateButton(Transform parent, string name, Vector2 position, Vector2 size, string text, Color color)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = size;

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = color;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;

        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.Lerp(color, Color.white, 0.16f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.18f);
        button.colors = colors;

        CreateText(buttonObject.transform, "Label", Vector2.zero, size, text, 42, Color.white, TextAlignmentOptions.Center);
        return button;
    }

    private static Slider CreateSlider(Transform parent, string name, Vector2 position, Vector2 size, Color fillColor)
    {
        GameObject sliderObject = new GameObject(name);
        sliderObject.transform.SetParent(parent, false);

        RectTransform sliderRect = sliderObject.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.5f, 0.5f);
        sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
        sliderRect.pivot = new Vector2(0.5f, 0.5f);
        sliderRect.anchoredPosition = position;
        sliderRect.sizeDelta = size;

        Slider slider = sliderObject.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = StartTotalPoints;
        slider.wholeNumbers = true;

        GameObject backgroundObject = new GameObject("Background");
        backgroundObject.transform.SetParent(sliderObject.transform, false);
        RectTransform backgroundRect = backgroundObject.AddComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;
        Image backgroundImage = backgroundObject.AddComponent<Image>();
        backgroundImage.color = new Color(1f, 1f, 1f, 0.25f);

        GameObject fillAreaObject = new GameObject("Fill Area");
        fillAreaObject.transform.SetParent(sliderObject.transform, false);
        RectTransform fillAreaRect = fillAreaObject.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;

        GameObject fillObject = new GameObject("Fill");
        fillObject.transform.SetParent(fillAreaObject.transform, false);
        RectTransform fillRect = fillObject.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image fillImage = fillObject.AddComponent<Image>();
        fillImage.color = fillColor;

        GameObject handleObject = new GameObject("Handle");
        handleObject.transform.SetParent(sliderObject.transform, false);
        RectTransform handleRect = handleObject.AddComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(52f, 52f);
        Image handleImage = handleObject.AddComponent<Image>();
        handleImage.color = Color.white;

        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;

        return slider;
    }
}
