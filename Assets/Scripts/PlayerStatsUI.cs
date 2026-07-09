using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    private const float PanelWidth = 360f;
    private const float PanelHeight = 165f;

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider knowledgeSlider;
    [SerializeField] private Slider friendshipSlider;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text knowledgeText;
    [SerializeField] private TMP_Text friendshipText;

    public static PlayerStatsUI EnsureDefaultHud(PlayerStats stats)
    {
        RemoveDuplicateStatsPanels();

        PlayerStatsUI[] existingHuds = FindObjectsByType<PlayerStatsUI>(FindObjectsSortMode.None);

        if (existingHuds.Length > 0)
        {
            PlayerStatsUI mainHud = existingHuds[0];
            mainHud.SetPlayerStats(stats);

            for (int i = 1; i < existingHuds.Length; i++)
            {
                Destroy(existingHuds[i].gameObject);
            }

            return mainHud;
        }

        Canvas canvas = FindAnyObjectByType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        GameObject panel = new GameObject("Player Stats Panel");
        panel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 1f);
        panelRect.anchorMax = new Vector2(0f, 1f);
        panelRect.pivot = new Vector2(0f, 1f);
        panelRect.anchoredPosition = new Vector2(24f, -24f);
        panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.55f);

        PlayerStatsUI hud = panel.AddComponent<PlayerStatsUI>();
        hud.moneyText = CreateText(panel.transform, "Money Text", new Vector2(18f, -16f), new Vector2(320f, 34f), "\u0414\u0435\u043d\u044c\u0433\u0438: 0", 30);

        CreateText(panel.transform, "Knowledge Label", new Vector2(18f, -66f), new Vector2(110f, 28f), "\u0417\u043d\u0430\u043d\u0438\u044f", 23);
        hud.knowledgeText = CreateText(panel.transform, "Knowledge Value", new Vector2(310f, -66f), new Vector2(34f, 28f), "0", 23);
        hud.knowledgeSlider = CreateSlider(panel.transform, "Knowledge Slider", new Vector2(130f, -68f), new Vector2(165f, 24f), new Color(0.12f, 0.42f, 1f, 1f));

        CreateText(panel.transform, "Friendship Label", new Vector2(18f, -112f), new Vector2(110f, 28f), "\u0414\u0440\u0443\u0436\u0431\u0430", 23);
        hud.friendshipText = CreateText(panel.transform, "Friendship Value", new Vector2(310f, -112f), new Vector2(34f, 28f), "0", 23);
        hud.friendshipSlider = CreateSlider(panel.transform, "Friendship Slider", new Vector2(130f, -114f), new Vector2(165f, 24f), new Color(0.95f, 0.12f, 0.12f, 1f));

        hud.SetPlayerStats(stats);
        return hud;
    }

    private static void RemoveDuplicateStatsPanels()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        bool hasMainPanel = false;

        for (int i = 0; i < allObjects.Length; i++)
        {
            GameObject currentObject = allObjects[i];

            if (currentObject == null || currentObject.name != "Player Stats Panel")
            {
                continue;
            }

            if (!hasMainPanel)
            {
                hasMainPanel = true;
                continue;
            }

            Destroy(currentObject);
        }
    }

    public void SetPlayerStats(PlayerStats stats)
    {
        if (playerStats != null)
        {
            playerStats.StatsChanged -= UpdateUI;
        }

        playerStats = stats;

        if (isActiveAndEnabled && playerStats != null)
        {
            playerStats.StatsChanged += UpdateUI;
        }

        UpdateUI();
    }

    private void OnEnable()
    {
        if (playerStats != null)
        {
            playerStats.StatsChanged += UpdateUI;
        }

        UpdateUI();
    }

    private void OnDisable()
    {
        if (playerStats != null)
        {
            playerStats.StatsChanged -= UpdateUI;
        }
    }

    private void UpdateUI()
    {
        if (playerStats == null)
        {
            return;
        }

        if (knowledgeSlider != null)
        {
            knowledgeSlider.value = playerStats.Knowledge;
        }

        if (friendshipSlider != null)
        {
            friendshipSlider.value = playerStats.Friendship;
        }

        if (moneyText != null)
        {
            moneyText.text = "\u0414\u0435\u043d\u044c\u0433\u0438: " + playerStats.Money;
        }

        if (knowledgeText != null)
        {
            knowledgeText.text = playerStats.Knowledge.ToString();
        }

        if (friendshipText != null)
        {
            friendshipText.text = playerStats.Friendship.ToString();
        }
    }

    private static TMP_Text CreateText(Transform parent, string name, Vector2 position, Vector2 size, string text, int fontSize)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;

        TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Left;

        return textComponent;
    }

    private static Slider CreateSlider(Transform parent, string name, Vector2 position, Vector2 size, Color fillColor)
    {
        GameObject sliderObject = new GameObject(name);
        sliderObject.transform.SetParent(parent, false);

        RectTransform sliderRect = sliderObject.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0f, 1f);
        sliderRect.anchorMax = new Vector2(0f, 1f);
        sliderRect.pivot = new Vector2(0f, 1f);
        sliderRect.anchoredPosition = position;
        sliderRect.sizeDelta = size;

        Slider slider = sliderObject.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = 0f;
        slider.interactable = false;

        GameObject backgroundObject = new GameObject("Background");
        backgroundObject.transform.SetParent(sliderObject.transform, false);
        RectTransform backgroundRect = backgroundObject.AddComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;
        Image backgroundImage = backgroundObject.AddComponent<Image>();
        backgroundImage.color = new Color(1f, 1f, 1f, 0.2f);

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

        slider.fillRect = fillRect;
        slider.targetGraphic = backgroundImage;

        return slider;
    }
}
