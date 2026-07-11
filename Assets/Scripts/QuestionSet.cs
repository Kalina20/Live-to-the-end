using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Question Set")]
public class QuestionSet : ScriptableObject
{
    public string monthName;
    public int stackNumber; // 1 = Клетка "+", 2 = Клетка "-", 3 = Клетка "?"

    public QuestionData[] questions = new QuestionData[4];
}

public enum StatType { None, Knowledge, Friendship, Money }

[Serializable]
public class QuestionData
{
    [TextArea(2, 5)]
    public string questionText;

    [Header("Answers (Options or Success/Fail outcomes)")]
    public AnswerData[] answers = new AnswerData[2];

    [Header("Check Event Settings (Only for cell '?')")]
    public bool isCheckEvent; // Ставим галочку, если это событие-проверка
    public StatType statToCheck = StatType.None; // Какую характеристику проверяем
    public int checkThreshold; // Порог (например, 20)
}

[Serializable]
public class AnswerData
{
    public string answerText;
    public int knowledgeChange;
    public int friendshipChange;
    public int moneyChange;
}