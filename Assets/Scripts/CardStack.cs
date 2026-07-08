using UnityEngine;

public class CardStack : MonoBehaviour
{
    [SerializeField] private Transform[] cards;
    [SerializeField] private Vector3 drawOffset = new Vector3(0f, 0.4f, -1.2f);

    private Transform[] originalParents;
    private Vector3[] originalLocalPositions;
    private Quaternion[] originalLocalRotations;
    private Vector3[] originalLocalScales;

    private int nextCardIndex;

    private void Awake()
    {
        originalParents = new Transform[cards.Length];
        originalLocalPositions = new Vector3[cards.Length];
        originalLocalRotations = new Quaternion[cards.Length];
        originalLocalScales = new Vector3[cards.Length];

        for (int i = 0; i < cards.Length; i++)
        {
            originalParents[i] = cards[i].parent;
            originalLocalPositions[i] = cards[i].localPosition;
            originalLocalRotations[i] = cards[i].localRotation;
            originalLocalScales[i] = cards[i].localScale;
        }
    }

    public Transform DrawNextCard()
    {
        if (cards == null || cards.Length == 0)
        {
            Debug.LogWarning(name + " has no cards.");
            return null;
        }

        if (nextCardIndex >= cards.Length)
        {
            Debug.LogWarning(name + " is empty.");
            return null;
        }

        Transform card = cards[nextCardIndex];

        card.localPosition += drawOffset;

        Debug.Log("Draw card: " + card.name + " from " + name);

        nextCardIndex++;

        return card;
    }

    public void ResetStack()
    {
        nextCardIndex = 0;

        if (cards == null)
        {
            return;
        }

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null)
            {
                continue;
            }

            cards[i].SetParent(originalParents[i], false);
            cards[i].localPosition = originalLocalPositions[i];
            cards[i].localRotation = originalLocalRotations[i];
            cards[i].localScale = originalLocalScales[i];
            cards[i].gameObject.SetActive(true);
        }
    }
}