using UnityEngine;

public class CardStackManager : MonoBehaviour
{
    [SerializeField] private CardStack stackOne;
    [SerializeField] private CardStack stackTwo;
    [SerializeField] private CardStack stackThree;

    public Transform DrawCardByDiceResult(int diceResult)
    {
        CardStack selectedStack = GetStackByDiceResult(diceResult);

        if (selectedStack == null)
        {
            Debug.LogWarning("No card stack for dice result: " + diceResult);
            return null;
        }

        return selectedStack.DrawNextCard();
    }

    private CardStack GetStackByDiceResult(int diceResult)
    {
        switch (diceResult)
        {
            case 1:
                return stackOne;
            case 2:
                return stackTwo;
            case 3:
                return stackThree;
            default:
                return null;
        }
    }
    public void ResetAllStacks()
    {
        if (stackOne != null)
        {
            stackOne.ResetStack();
        }

        if (stackTwo != null)
        {
            stackTwo.ResetStack();
        }

        if (stackThree != null)
        {
            stackThree.ResetStack();
        }
    }
}
