using UnityEngine;

public class CardCoverVisualGuard : MonoBehaviour
{
    private const string MinusCoverName = "Minus Card Cover Visual";
    private const string QuestionCoverName = "Question Card Cover Visual";
    private const string PlusCoverName = "Plus Card Cover Visual";

    private MeshRenderer cardRenderer;

    private void Awake()
    {
        cardRenderer = GetComponent<MeshRenderer>();
        HideBaseCardIfCoverExists();
    }

    private void OnEnable()
    {
        HideBaseCardIfCoverExists();
    }

    private void LateUpdate()
    {
        HideBaseCardIfCoverExists();
    }

    public void HideBaseCardIfCoverExists()
    {
        if (cardRenderer == null)
        {
            cardRenderer = GetComponent<MeshRenderer>();
        }

        if (cardRenderer != null && HasCoverVisual())
        {
            cardRenderer.enabled = false;
        }
    }

    private bool HasCoverVisual()
    {
        return transform.Find(MinusCoverName) != null ||
               transform.Find(QuestionCoverName) != null ||
               transform.Find(PlusCoverName) != null;
    }
}
