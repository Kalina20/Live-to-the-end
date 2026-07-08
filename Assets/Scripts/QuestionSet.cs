using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Question Set")]
public class QuestionSet : ScriptableObject
{
    public string monthName;
    public int stackNumber;

    public QuestionData[] questions = new QuestionData[4];
}

[Serializable]
public class QuestionData
{
    [TextArea(2, 5)]
    public string questionText;

    public AnswerData[] answers = new AnswerData[3];
}

[Serializable]
public class AnswerData
{
    public string answerText;
}