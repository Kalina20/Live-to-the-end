using System.Collections;
using UnityEngine;

public class PlayerTileMover : MonoBehaviour
{
    [SerializeField] private BoardTile currentTile;
    [SerializeField] private MonthManager monthManager;
    [SerializeField] private CardStackManager cardStackManager;
    [SerializeField] private DrawnCardPresenter drawnCardPresenter;
    [SerializeField] private float moveDuration = 0.35f;
    [SerializeField] private float jumpHeight = 0.35f;

    private bool isMoving;
    private int lastDiceResult;

    private void Start()
    {
        if (currentTile != null)
        {
            transform.position = currentTile.TokenPosition;
            currentTile.SetIconVisible(false);

            if (monthManager != null)
            {
                monthManager.SetCurrentMonth(currentTile.MonthName);
            }
        }
    }

    public void MoveByDiceResult(int diceResult)
    {
        if (isMoving || currentTile == null)
        {
            return;
        }

        BoardTile nextTile = currentTile.GetNextTile(diceResult);

        if (nextTile == null)
        {
            Debug.LogWarning("No next tile for dice result: " + diceResult);
            return;
        }
        lastDiceResult = diceResult;
        StartCoroutine(MoveToTile(nextTile));
    }

    private IEnumerator MoveToTile(BoardTile targetTile)
    {
        isMoving = true;
        BoardTile previousTile = currentTile;

        Vector3 start = transform.position;
        Vector3 end = targetTile.TokenPosition;

        float timer = 0f;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / moveDuration;

            Vector3 position = Vector3.Lerp(start, end, progress);
            position.y += Mathf.Sin(progress * Mathf.PI) * jumpHeight;

            transform.position = position;

            yield return null;
        }

        transform.position = end;
        if (previousTile != null)
        {
            previousTile.SetIconVisible(true);
        }

        currentTile = targetTile;
        currentTile.SetIconVisible(false);

        if (monthManager != null)
        {
            monthManager.SetCurrentMonth(currentTile.MonthName);
        }
        if (cardStackManager != null)
        {
            Transform drawnCard = cardStackManager.DrawCardByDiceResult(lastDiceResult);

            if (drawnCardPresenter != null)
            {
                drawnCardPresenter.SetDrawnCard(drawnCard, lastDiceResult);
            }
        }

        isMoving = false;
    }
}
