using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class QuestionCardCoverSetup
{
    private const string StackName = "Card Stack 2";
    private static readonly string[] AllStackNames = { "Card Stack 1", "Card Stack 2", "Card Stack 3" };
    private const string VisualName = "Question Card Cover Visual";
    private const string ModelPath = "Assets/Models/Cards/Question_Card_Cover.fbx";

    [MenuItem("Game/Setup/Apply Question Card Covers")]
    public static void ApplyQuestionCardCovers()
    {
        CleanupAllStacks(false);

        GameObject stack = GameObject.Find(StackName);
        GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);

        if (stack == null || modelPrefab == null)
        {
            Debug.LogError($"Could not apply question card covers. Stack: {stack}, model: {modelPrefab}");
            return;
        }

        foreach (Transform card in stack.transform)
        {
            ApplyCover(card, modelPrefab);
        }

        EditorUtility.SetDirty(stack);
        EditorSceneManager.MarkSceneDirty(stack.scene);
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Game/Setup/Cleanup Question Card Covers")]
    public static void CleanupQuestionCardCovers()
    {
        CleanupAllStacks(true);
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }

    private static void CleanupAllStacks(bool showBaseCards)
    {
        foreach (string stackName in AllStackNames)
        {
            GameObject stack = GameObject.Find(stackName);
            if (stack == null)
            {
                continue;
            }

            CleanupStack(stack, showBaseCards);
        }
    }

    private static void CleanupStack(GameObject stack, bool showBaseCards)
    {
        foreach (Transform card in stack.transform)
        {
            CleanupCover(card);

            MeshRenderer renderer = card.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.enabled = showBaseCards;
                EditorUtility.SetDirty(renderer);
            }
        }

        EditorUtility.SetDirty(stack);
        EditorSceneManager.MarkSceneDirty(stack.scene);
    }

    private static void ApplyCover(Transform card, GameObject modelPrefab)
    {
        CleanupCover(card);
        EnsureVisualGuard(card);

        MeshRenderer cardRenderer = card.GetComponent<MeshRenderer>();
        Bounds? cardBounds = cardRenderer != null ? cardRenderer.bounds : null;

        if (cardRenderer != null)
        {
            cardRenderer.enabled = false;
            EditorUtility.SetDirty(cardRenderer);
        }

        GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab, card);
        visual.name = VisualName;
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        visual.transform.localScale = Vector3.one;

        FitVisualToCard(visual, cardBounds);

        EditorUtility.SetDirty(card);
    }

    private static void EnsureVisualGuard(Transform card)
    {
        CardCoverVisualGuard guard = card.GetComponent<CardCoverVisualGuard>();
        if (guard == null)
        {
            guard = card.gameObject.AddComponent<CardCoverVisualGuard>();
        }

        EditorUtility.SetDirty(guard);
    }

    private static void CleanupCover(Transform card)
    {
        Transform[] children = card.GetComponentsInChildren<Transform>(true);
        for (int i = children.Length - 1; i >= 0; i--)
        {
            Transform child = children[i];
            if (child != null && child != card && child.name == VisualName)
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }
    }

    private static void FitVisualToCard(GameObject visual, Bounds? targetBounds)
    {
        Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError($"Question card cover has no renderers: {visual.name}");
            return;
        }

        Bounds bounds = GetBounds(renderers);

        if (!targetBounds.HasValue)
        {
            return;
        }

        Vector3 targetSize = targetBounds.Value.size;
        Vector3 visualSize = bounds.size;

        if (visualSize.x <= 0f || visualSize.y <= 0f || visualSize.z <= 0f)
        {
            return;
        }

        visual.transform.localScale = new Vector3(
            visual.transform.localScale.x * targetSize.x / visualSize.x,
            visual.transform.localScale.y * targetSize.y / visualSize.y,
            visual.transform.localScale.z * targetSize.z / visualSize.z
        );

        renderers = visual.GetComponentsInChildren<Renderer>();
        bounds = GetBounds(renderers);

        visual.transform.position += targetBounds.Value.center - bounds.center;
        visual.transform.position += Vector3.up * (targetSize.y * 0.55f);
    }

    private static Bounds GetBounds(Renderer[] renderers)
    {
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }
}
