using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int knowledge = 20;
    [SerializeField] private int friendship = 20;
    [SerializeField] private int money = 15000;

    public int Knowledge => knowledge;
    public int Friendship => friendship;
    public int Money => money;

    public event Action StatsChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsurePlayerStatsInScene()
    {
        PlayerStats playerStats = FindAnyObjectByType<PlayerStats>();

        if (playerStats == null)
        {
            GameObject playerStatsObject = new GameObject("Player Stats");
            playerStats = playerStatsObject.AddComponent<PlayerStats>();
        }

        PlayerStatsUI.EnsureDefaultHud(playerStats);
    }

    private void Start()
    {
        StatsChanged?.Invoke();
    }

    public void InitializeStats(int startKnowledge, int startFriendship, int startMoney)
    {
        knowledge = startKnowledge;
        friendship = startFriendship;
        money = startMoney;
        StatsChanged?.Invoke();
    }

    public bool CanApplyAnswer(AnswerData answer)
    {
        if (answer == null)
        {
            return false;
        }

        return knowledge + answer.knowledgeChange >= 0 &&
               friendship + answer.friendshipChange >= 0 &&
               money + answer.moneyChange >= 0;
    }

    public void ApplyAnswer(AnswerData answer)
    {
        if (!CanApplyAnswer(answer))
        {
            return;
        }

        knowledge += answer.knowledgeChange;
        friendship += answer.friendshipChange;
        money += answer.moneyChange;

        StatsChanged?.Invoke();
    }

    public int GetStatValue(StatType statType)
    {
        switch (statType)
        {
            case StatType.Knowledge: return knowledge;
            case StatType.Friendship: return friendship;
            case StatType.Money: return money;
            default: return 0;
        }
    }
}