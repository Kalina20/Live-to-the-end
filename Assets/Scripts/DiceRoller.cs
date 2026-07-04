using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour
{
    [SerializeField] private Button rollButton;
    [SerializeField] private PlayerTileMover playerMover;
    [SerializeField] private float rollDuration = 1.2f;
    [SerializeField] private float jumpHeight = 1.2f;

    private Vector3 startPosition;
    private bool isRolling;

    private void Awake()
    {
        startPosition = transform.position;

        if (rollButton != null)
        {
            rollButton.onClick.AddListener(Roll);
        }
    }

    public void Roll()
    {
        if (isRolling)
        {
            return;
        }

        StartCoroutine(RollRoutine());
    }

    private IEnumerator RollRoutine()
    {
        isRolling = true;

        if (rollButton != null)
        {
            rollButton.interactable = false;
        }

        int[] diceValues = { 1, 1, 2, 2, 3, 3 };
        int result = diceValues[Random.Range(0, diceValues.Length)];

        float timer = 0f;

        while (timer < rollDuration)
        {
            timer += Time.deltaTime;

            float progress = timer / rollDuration;
            float jump = Mathf.Sin(progress * Mathf.PI) * jumpHeight;

            transform.position = startPosition + Vector3.up * jump;
            transform.Rotate(420f * Time.deltaTime, 620f * Time.deltaTime, 360f * Time.deltaTime, Space.World);

            yield return null;
        }

        transform.position = startPosition;
        transform.rotation = GetRotationForResult(result);

        Debug.Log("Dice result: " + result);
        if (playerMover != null)
        {
            playerMover.MoveByDiceResult(result);
        }

        if (rollButton != null)
        {
            rollButton.interactable = true;
        }

        isRolling = false;
    }

    private Quaternion GetRotationForResult(int result)
    {
        switch (result)
        {
            case 1:
                return Random.value < 0.5f
                    ? Quaternion.Euler(0f, 0f, 0f)
                    : Quaternion.Euler(180f, 0f, 0f);

            case 2:
                return Random.value < 0.5f
                    ? Quaternion.Euler(0f, 0f, 90f)
                    : Quaternion.Euler(0f, 0f, -90f);

            case 3:
                return Random.value < 0.5f
                    ? Quaternion.Euler(90f, 0f, 0f)
                    : Quaternion.Euler(-90f, 0f, 0f);

            default:
                return Quaternion.identity;
        }
    }
}
