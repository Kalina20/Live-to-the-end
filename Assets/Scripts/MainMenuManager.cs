using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private PlayerStats playerStats;
    
    [Header("In-Game UI")]
    [SerializeField] private GameObject rollButton;

    [Header("Sliders")]
    [SerializeField] private Slider knowledgeSlider;
    [SerializeField] private Slider friendshipSlider;
    [SerializeField] private Slider moneySlider;

    [Header("Texts")]
    [SerializeField] private TMP_Text knowledgeText;
    [SerializeField] private TMP_Text friendshipText;
    [SerializeField] private TMP_Text moneyText;

    [Header("Buttons")]
    [SerializeField] private Button startButton;

    private PlayerStatsUI inGameHud;

    private void Start()
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

        if (playerStats == null)
            playerStats = FindAnyObjectByType<PlayerStats>();

        SetupSlider(knowledgeSlider, 0, 100, 20);
        SetupSlider(friendshipSlider, 0, 100, 20);
        SetupSlider(moneySlider, 0, 50000, 15000);

        knowledgeSlider.onValueChanged.AddListener(v => knowledgeText.text = $"Знания: {v}");
        friendshipSlider.onValueChanged.AddListener(v => friendshipText.text = $"Дружба: {v}");
        moneySlider.onValueChanged.AddListener(v => moneyText.text = $"Деньги: {v}");

        knowledgeText.text = $"Знания: {knowledgeSlider.value}";
        friendshipText.text = $"Дружба: {friendshipSlider.value}";
        moneyText.text = $"Деньги: {moneySlider.value}";

        startButton.onClick.AddListener(StartGame);
    }

    private void SetupSlider(Slider slider, float min, float max, float startValue)
    {
        if (slider != null)
        {
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = startValue;
            slider.wholeNumbers = true; 
        }
    }

    private void StartGame()
    {
        if (playerStats != null)
        {
            playerStats.InitializeStats(
                (int)knowledgeSlider.value, 
                (int)friendshipSlider.value, 
                (int)moneySlider.value
            );
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
}