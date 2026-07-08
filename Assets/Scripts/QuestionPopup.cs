using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private DrawnCardPresenter drawnCardPresenter;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TMP_Text[] answerTexts;

    public void ShowQuestion(QuestionData question)
    {
        popupRoot.SetActive(true);

        questionText.text = question.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool hasAnswer = i < question.answers.Length &&
                             question.answers[i] != null &&
                             !string.IsNullOrWhiteSpace(question.answers[i].answerText);

            answerButtons[i].gameObject.SetActive(hasAnswer);

            answerButtons[i].onClick.RemoveAllListeners();

            if (hasAnswer)
            {
                answerTexts[i].text = question.answers[i].answerText;
                answerButtons[i].onClick.AddListener(OnAnswerClicked);
            }
        }
    }

    private void OnAnswerClicked()
    {
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
}