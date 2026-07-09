using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionPopup : MonoBehaviour
{
    private const int AnswerCount = 2;

    [SerializeField] private GameObject popupRoot;
    [SerializeField] private DrawnCardPresenter drawnCardPresenter;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TMP_Text[] answerTexts;
    [SerializeField] private Color answerButtonColor = new Color(0.98f, 0.84f, 0.35f, 1f);
    [SerializeField] private Color answerButtonDisabledColor = new Color(0.55f, 0.55f, 0.55f, 0.75f);
    [SerializeField] private Color answerTextColor = new Color(0.16f, 0.12f, 0.08f, 1f);

    private GameObject[] runtimeBlockedCrosses;

    private void Awake()
    {
        if (playerStats == null)
        {
            playerStats = FindAnyObjectByType<PlayerStats>();
        }

        if (playerStats == null)
        {
            GameObject playerStatsObject = new GameObject("Player Stats");
            playerStats = playerStatsObject.AddComponent<PlayerStats>();
        }

        SetupAnswerButtonsLayout();
        SetupQuestionText();
        CreateRuntimeBlockedCrosses();
    }

    public void ShowQuestion(QuestionData question)
    {
        if (question == null)
        {
            return;
        }

        popupRoot.SetActive(true);

        questionText.text = question.questionText;

        if (answerButtons == null)
        {
            return;
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool hasAnswer = i < AnswerCount &&
                             question.answers != null &&
                             i < question.answers.Length &&
                             question.answers[i] != null &&
                             !string.IsNullOrWhiteSpace(question.answers[i].answerText);

            answerButtons[i].gameObject.SetActive(hasAnswer);
            answerButtons[i].interactable = false;

            answerButtons[i].onClick.RemoveAllListeners();
            SetBlockedCross(i, false);

            if (hasAnswer)
            {
                AnswerData answer = question.answers[i];
                bool canUseAnswer = playerStats == null || playerStats.CanApplyAnswer(answer);

                if (answerTexts != null && i < answerTexts.Length && answerTexts[i] != null)
                {
                    answerTexts[i].text = answer.answerText;
                }

                answerButtons[i].interactable = canUseAnswer;
                SetBlockedCross(i, !canUseAnswer);

                if (canUseAnswer)
                {
                    answerButtons[i].onClick.AddListener(() => OnAnswerClicked(answer));
                }
            }
        }
    }

    private void OnAnswerClicked(AnswerData answer)
    {
        if (playerStats != null)
        {
            playerStats.ApplyAnswer(answer);
        }

        Hide();

        if (drawnCardPresenter != null)
        {
            drawnCardPresenter.ResolveCurrentCard();
        }
    }

    public void Hide()
    {
        popupRoot.SetActive(false);
    }

    private void SetBlockedCross(int index, bool isBlocked)
    {
        if (runtimeBlockedCrosses == null ||
            index >= runtimeBlockedCrosses.Length ||
            runtimeBlockedCrosses[index] == null)
        {
            return;
        }

        runtimeBlockedCrosses[index].SetActive(isBlocked);
    }

    private void CreateRuntimeBlockedCrosses()
    {
        if (answerButtons == null)
        {
            return;
        }

        runtimeBlockedCrosses = new GameObject[answerButtons.Length];

        for (int i = 0; i < Mathf.Min(AnswerCount, answerButtons.Length); i++)
        {
            if (answerButtons[i] == null)
            {
                continue;
            }

            GameObject crossObject = new GameObject("Blocked Cross");
            crossObject.transform.SetParent(answerButtons[i].transform, false);

            RectTransform rectTransform = crossObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            TextMeshProUGUI crossText = crossObject.AddComponent<TextMeshProUGUI>();
            crossText.text = "X";
            crossText.fontSize = 72;
            crossText.color = Color.red;
            crossText.alignment = TextAlignmentOptions.Center;
            crossText.raycastTarget = false;

            crossObject.SetActive(false);
            runtimeBlockedCrosses[i] = crossObject;
        }
    }

    private void SetupAnswerButtonsLayout()
    {
        if (answerButtons == null)
        {
            return;
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] == null)
            {
                continue;
            }

            if (i >= AnswerCount)
            {
                answerButtons[i].gameObject.SetActive(false);
                continue;
            }

            RectTransform rectTransform = answerButtons[i].GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 0f);
                rectTransform.anchorMax = new Vector2(0.5f, 0f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.sizeDelta = new Vector2(680f, 105f);
                rectTransform.anchoredPosition = new Vector2(0f, i == 0 ? 250f : 115f);
            }

            Image image = answerButtons[i].GetComponent<Image>();

            if (image != null)
            {
                image.color = answerButtonColor;
            }

            ColorBlock colors = answerButtons[i].colors;
            colors.normalColor = answerButtonColor;
            colors.highlightedColor = new Color(1f, 0.9f, 0.48f, 1f);
            colors.pressedColor = new Color(0.9f, 0.68f, 0.18f, 1f);
            colors.selectedColor = answerButtonColor;
            colors.disabledColor = answerButtonDisabledColor;
            colors.colorMultiplier = 1f;
            answerButtons[i].colors = colors;

            if (answerTexts != null && i < answerTexts.Length && answerTexts[i] != null)
            {
                answerTexts[i].color = answerTextColor;
                answerTexts[i].fontSize = 30f;
                answerTexts[i].enableAutoSizing = true;
                answerTexts[i].fontSizeMin = 20f;
                answerTexts[i].fontSizeMax = 34f;
                answerTexts[i].alignment = TextAlignmentOptions.Center;
                answerTexts[i].margin = new Vector4(24f, 8f, 24f, 8f);
            }
        }
    }

    private void SetupQuestionText()
    {
        if (questionText == null)
        {
            return;
        }

        RectTransform rectTransform = questionText.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(0f, -245f);
            rectTransform.sizeDelta = new Vector2(740f, 360f);
        }

        questionText.fontSize = 40f;
        questionText.enableAutoSizing = true;
        questionText.fontSizeMin = 26f;
        questionText.fontSizeMax = 42f;
        questionText.alignment = TextAlignmentOptions.Center;
        questionText.margin = new Vector4(24f, 18f, 24f, 18f);
    }
}
