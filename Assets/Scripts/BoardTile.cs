using UnityEngine;

public class BoardTile : MonoBehaviour
{
    private const string QuestionIconName = "Question Tile Icon";
    private const string PlusIconName = "Plus Tile Icon";
    private const string MinusIconName = "Minus Tile Icon";

    [SerializeField] private string monthName;
    [SerializeField] private BoardTile optionOne;
    [SerializeField] private BoardTile optionTwo;
    [SerializeField] private BoardTile optionThree;

    public string MonthName => monthName;
    public Vector3 TokenPosition => transform.position + Vector3.up * 0.35f;

    public BoardTile GetNextTile(int diceResult)
    {
        switch (diceResult)
        {
            case 1:
                return optionOne;
            case 2:
                return optionTwo;
            case 3:
                return optionThree;
            default:
                return null;
        }
    }

    public void SetIconVisible(bool isVisible)
    {
        SetChildActive(QuestionIconName, isVisible);
        SetChildActive(PlusIconName, isVisible);
        SetChildActive(MinusIconName, isVisible);
    }

    private void SetChildActive(string childName, bool isActive)
    {
        Transform child = transform.Find(childName);

        if (child != null)
        {
            child.gameObject.SetActive(isActive);
        }
    }
}
