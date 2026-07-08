using UnityEngine;

public class MonthManager : MonoBehaviour
{
    [SerializeField] private CardStackManager cardStackManager;
    [SerializeField] private QuestionManager questionManager;

    private string currentMonthName;

    public void SetCurrentMonth(string monthName)
    {
        if (string.IsNullOrWhiteSpace(monthName))
        {
            return;
        }

        if (currentMonthName == monthName)
        {
            return;
        }

        currentMonthName = monthName;

        if (cardStackManager != null)
        {
            cardStackManager.ResetAllStacks();
        }

        if (questionManager != null)
        {
            questionManager.SetCurrentMonth(monthName);
        }

        Debug.Log("Current month: " + currentMonthName);
    }
}