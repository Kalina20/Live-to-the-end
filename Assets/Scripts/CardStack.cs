using UnityEngine;

public class CardStack : MonoBehaviour
{
    [SerializeField] private Transform[] cards;
    [SerializeField] private Vector3 drawOffset = new Vector3(0f, 0.4f, -1.2f);
    [SerializeField] private Transform drawTarget;
    [SerializeField] private bool drawTowardTarget = true;
    [SerializeField] private float drawTowardTargetDistance = 0.45f;

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

        if (drawTarget == null)
        {
            GameObject dice = GameObject.Find("Dice");
            if (dice != null)
            {
                drawTarget = dice.transform;
            }
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

        MoveCardToDrawPosition(card);
        nextCardIndex++;

        return card;
    }

    private void MoveCardToDrawPosition(Transform card)
    {
        if (card == null)
        {
            return;
        }

        if (!drawTowardTarget || drawTarget == null)
        {
            card.localPosition += drawOffset;
            return;
        }

        Vector3 directionToTarget = drawTarget.position - card.position;
        directionToTarget.y = 0f;

        if (directionToTarget.sqrMagnitude < 0.001f)
        {
            directionToTarget = transform.forward;
            directionToTarget.y = 0f;
        }

        float drawDistance = drawTowardTargetDistance;
        Vector3 drawMovement = directionToTarget.normalized * drawDistance;
        drawMovement.y = drawOffset.y;

        card.position += drawMovement;
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
