using System.Collections;
using System;
using UnityEngine;

public class PlayerTileMover : MonoBehaviour
{
    [SerializeField] private BoardTile currentTile;
    [SerializeField] private MonthManager monthManager;
    [SerializeField] private CardStackManager cardStackManager;
    [SerializeField] private DrawnCardPresenter drawnCardPresenter;
    [SerializeField] private PlayerEmotionController emotionController;
    [SerializeField] private Transform playerVisual;
    [SerializeField] private float moveDuration = 0.35f;
    [SerializeField] private float jumpHeight = 0.35f;

    private bool isMoving;
    private int lastDiceResult;
    private int completedJumps;

    public event Action<BoardTile> TileReached;

    private void Start()
    {
        if (emotionController == null)
        {
            emotionController = GetComponent<PlayerEmotionController>();
        }

        if (emotionController == null)
        {
            emotionController = gameObject.AddComponent<PlayerEmotionController>();
        }

        if (playerVisual == null)
        {
            Transform foundVisual = transform.Find("Player Visual");
            playerVisual = foundVisual != null ? foundVisual : transform;
        }

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
        completedJumps++;
        TileReached?.Invoke(currentTile);

        if (completedJumps % 4 == 0)
        {
            RotatePlayerVisual();
        }

        if (monthManager != null)
        {
            monthManager.SetCurrentMonth(currentTile.MonthName);
        }

        if (emotionController != null)
        {
            yield return emotionController.PlayEmotionForDiceResult(lastDiceResult);
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

    private void RotatePlayerVisual()
    {
        if (playerVisual == null)
        {
            return;
        }

        playerVisual.localRotation *= Quaternion.Euler(0f, 90f, 0f);
    }
}
