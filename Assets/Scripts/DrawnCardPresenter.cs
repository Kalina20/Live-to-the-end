using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class DrawnCardPresenter : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Button rollButton;
    [SerializeField] private QuestionManager questionManager;
    [SerializeField] private Transform discardPile;
    [SerializeField] private CameraZoomController cameraZoomController;

    [SerializeField] private float screenHeightPercent = 0.85f;
    [SerializeField] private float cardAspect = 0.7f;
    [SerializeField] private float presentDelay = 0.45f;
    [SerializeField] private float distanceFromCamera = 3f;
    [SerializeField] private Vector3 presentedScale = new Vector3(4f, 0.08f, 2.8f);
    [SerializeField] private float cardViewFieldOfView = 35f;

    private float previousFieldOfView;
    private bool hasSavedFieldOfView;

    private Transform currentCard;
    private int currentStackNumber;
    private Transform originalParent;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private bool isPresented;
    private bool isPresenting;
    public bool IsPresented => isPresented;
    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private Vector3 GetCardScaleForCamera()
    {
        float cameraHeight = 2f * distanceFromCamera * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float cardHeight = cameraHeight * screenHeightPercent;
        float cardWidth = cardHeight * cardAspect;

        return new Vector3(cardWidth, 0.08f, cardHeight);
    }
    public void SetDrawnCard(Transform card, int stackNumber)
    {
        currentCard = card;
        currentStackNumber = stackNumber;
        isPresented = false;

        if (rollButton != null)
        {
            rollButton.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (currentCard == null || isPresented || isPresenting)
        {
            return;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryPresentClickedCard(Mouse.current.position.ReadValue());
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            TryPresentClickedCard(Touchscreen.current.primaryTouch.position.ReadValue());
        }
    }

    private void TryPresentClickedCard(Vector2 screenPosition)
    {
        Ray ray = targetCamera.ScreenPointToRay(screenPosition);

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            return;
        }

        if (hit.transform != currentCard)
        {
            return;
        }

        StartCoroutine(PresentCardRoutine());
    }

    public void ResolveCurrentCard()
    {
        if (currentCard == null)
        {
            return;
        }

        currentCard.SetParent(discardPile, false);
        currentCard.localPosition = Vector3.zero;
        currentCard.localRotation = Quaternion.identity;
        currentCard.localScale = originalScale;

        isPresenting = false;
        if (hasSavedFieldOfView)
        {
            targetCamera.fieldOfView = previousFieldOfView;
            hasSavedFieldOfView = false;
        }
        currentCard = null;
        isPresented = false;

        if (rollButton != null)
        {
            rollButton.gameObject.SetActive(true);
        }
    }

    private IEnumerator PresentCardRoutine()
    {
        if (isPresenting || isPresented || currentCard == null)
        {
            yield break;
        }

        isPresenting = true;

      //  if (cameraZoomController != null)
       // {
       //     cameraZoomController.ReturnToStart();
        //}

        yield return new WaitForSeconds(presentDelay);

        originalParent = currentCard.parent;
        originalPosition = currentCard.localPosition;
        originalRotation = currentCard.localRotation;
        originalScale = currentCard.localScale;
        previousFieldOfView = targetCamera.fieldOfView;
        hasSavedFieldOfView = true;
        targetCamera.fieldOfView = cardViewFieldOfView;

        currentCard.SetParent(targetCamera.transform, false);

        currentCard.localPosition = new Vector3(0f, 0f, distanceFromCamera);
        currentCard.localRotation = Quaternion.identity;
        currentCard.localScale = presentedScale;
            //currentCard.localScale = GetCardScaleForCamera();

        isPresented = true;
        isPresenting = false;

        if (questionManager != null)
        {
            questionManager.ShowQuestionForStack(currentStackNumber);
        }
    }

    public void CloseCard()
    {
        if (currentCard == null)
        {
            return;
        }

        currentCard.SetParent(originalParent, false);
        currentCard.localPosition = originalPosition;
        currentCard.localRotation = originalRotation;
        currentCard.localScale = originalScale;

        currentCard = null;
        isPresented = false;

        if (rollButton != null)
        {
            rollButton.gameObject.SetActive(true);
        }
    }
}