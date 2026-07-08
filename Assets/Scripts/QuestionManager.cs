using System.Collections.Generic;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    [SerializeField] private QuestionSet[] allQuestionSets;
    [SerializeField] private QuestionPopup questionPopup;

    private string currentMonthName = "September";

    private readonly HashSet<string> usedQuestionKeys = new HashSet<string>();

    public void ShowQuestionForStack(int stackNumber)
    {
        QuestionSet questionSet = GetQuestionSet(stackNumber);

        if (questionSet == null)
        {
            Debug.LogWarning("No question set for stack: " + stackNumber);
            return;
        }

        QuestionData question = GetRandomUnusedQuestion(questionSet);

        if (question == null)
        {
            Debug.LogWarning("No unused questions left for: " + questionSet.name);
            return;
        }

        questionPopup.ShowQuestion(question);
    }

    private QuestionSet GetQuestionSet(int stackNumber)
    {
        for (int i = 0; i < allQuestionSets.Length; i++)
        {
            QuestionSet questionSet = allQuestionSets[i];

            if (questionSet == null)
            {
                continue;
            }

            if (questionSet.monthName == currentMonthName &&
                questionSet.stackNumber == stackNumber)
            {
                return questionSet;
            }
        }

        return null;
    }

    public void SetCurrentMonth(string monthName)
    {
        currentMonthName = monthName;
    }

    private QuestionData GetRandomUnusedQuestion(QuestionSet questionSet)
    {
        List<int> availableIndexes = new List<int>();

        for (int i = 0; i < questionSet.questions.Length; i++)
        {
            string key = questionSet.name + "_" + i;

            bool hasQuestionText = questionSet.questions[i] != null &&
                                   !string.IsNullOrWhiteSpace(questionSet.questions[i].questionText);

            if (!usedQuestionKeys.Contains(key) && hasQuestionText)
            {
                availableIndexes.Add(i);
            }
        }

        if (availableIndexes.Count == 0)
        {
            return null;
        }

        int randomListIndex = Random.Range(0, availableIndexes.Count);
        int questionIndex = availableIndexes[randomListIndex];

        usedQuestionKeys.Add(questionSet.name + "_" + questionIndex);

        return questionSet.questions[questionIndex];
    }
}