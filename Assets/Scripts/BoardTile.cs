using UnityEngine;

public class BoardTile : MonoBehaviour
{
    [SerializeField] private BoardTile optionOne;
    [SerializeField] private BoardTile optionTwo;
    [SerializeField] private BoardTile optionThree;

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
}