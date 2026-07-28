using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SymbolDiceVisualSetup
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string DiceName = "Dice";
    private const string VisualName = "Symbol Dice Visual";
    private const string ModelPath = "Assets/Models/Dice/Symbol_Dice.fbx";

    [MenuItem("Game/Setup/Apply Symbol Dice")]
    public static void ApplySymbolDice()
    {
        EditorSceneManager.OpenScene(ScenePath);

        GameObject dice = GameObject.Find(DiceName);
        GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);

        if (dice == null || modelPrefab == null)
        {
            Debug.LogError($"Could not apply symbol dice. Dice: {dice}, model: {modelPrefab}");
            return;
        }

        CleanupVisual(dice);

        MeshRenderer diceRenderer = dice.GetComponent<MeshRenderer>();
        MeshFilter diceMeshFilter = dice.GetComponent<MeshFilter>();
        Bounds? oldBounds = diceRenderer != null ? diceRenderer.bounds : null;

        if (diceRenderer != null)
        {
            diceRenderer.enabled = false;
        }

        if (diceMeshFilter != null)
        {
            diceMeshFilter.hideFlags = HideFlags.NotEditable;
        }

        GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab, dice.transform);
        visual.name = VisualName;
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;
        visual.transform.localScale = Vector3.one;

        FitVisualToDice(visual, oldBounds);

        EditorUtility.SetDirty(dice);
        EditorSceneManager.MarkSceneDirty(dice.scene);
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Game/Setup/Cleanup Symbol Dice")]
    public static void CleanupSymbolDice()
    {
        EditorSceneManager.OpenScene(ScenePath);

        GameObject dice = GameObject.Find(DiceName);
        if (dice == null)
        {
            return;
        }

        CleanupVisual(dice);

        MeshRenderer diceRenderer = dice.GetComponent<MeshRenderer>();
        MeshFilter diceMeshFilter = dice.GetComponent<MeshFilter>();
        if (diceRenderer != null)
        {
            diceRenderer.enabled = true;
        }

        if (diceMeshFilter != null)
        {
            diceMeshFilter.hideFlags = HideFlags.None;
        }

        EditorUtility.SetDirty(dice);
        EditorSceneManager.MarkSceneDirty(dice.scene);
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }

    private static void CleanupVisual(GameObject dice)
    {
        Transform oldVisual = dice.transform.Find(VisualName);
        if (oldVisual != null)
        {
            Object.DestroyImmediate(oldVisual.gameObject);
        }
    }

    private static void FitVisualToDice(GameObject visual, Bounds? targetBounds)
    {
        Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError($"Symbol dice visual has no renderers: {visual.name}");
            return;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        float targetSize = targetBounds.HasValue
            ? Mathf.Max(targetBounds.Value.size.x, targetBounds.Value.size.y, targetBounds.Value.size.z)
            : 1f;

        float currentSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        if (currentSize <= 0f)
        {
            return;
        }

        visual.transform.localScale *= targetSize / currentSize;

        renderers = visual.GetComponentsInChildren<Renderer>();
        bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        Vector3 targetCenter = targetBounds.HasValue ? targetBounds.Value.center : visual.transform.parent.position;
        visual.transform.position += targetCenter - bounds.center;
    }
}
