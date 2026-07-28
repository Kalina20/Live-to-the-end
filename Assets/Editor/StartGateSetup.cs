using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class StartGateSetup
{
    private const string StartName = "Start";
    private const string VisualName = "Start Gate Visual";
    private const string ModelPath = "Assets/Models/Stops/Start_Gate.fbx";

    [MenuItem("Game/Setup/Apply Start Gate")]
    public static void ApplyStartGate()
    {
        GameObject start = GameObject.Find(StartName);
        GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);

        if (start == null || modelPrefab == null)
        {
            Debug.LogError($"Could not apply Start gate visual. Start: {start}, model: {modelPrefab}");
            return;
        }

        CleanupVisual(start);

        MeshRenderer startRenderer = start.GetComponent<MeshRenderer>();
        Bounds? startBounds = startRenderer != null ? startRenderer.bounds : null;

        if (startRenderer != null)
        {
            startRenderer.enabled = false;
            EditorUtility.SetDirty(startRenderer);
        }

        GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab, start.transform);
        visual.name = VisualName;
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        visual.transform.localScale = Vector3.one;

        FitVisualToStart(visual, startBounds);

        EditorUtility.SetDirty(start);
        EditorSceneManager.MarkSceneDirty(start.scene);
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Game/Setup/Cleanup Start Gate")]
    public static void CleanupStartGate()
    {
        GameObject start = GameObject.Find(StartName);
        if (start == null)
        {
            return;
        }

        CleanupVisual(start);

        MeshRenderer startRenderer = start.GetComponent<MeshRenderer>();
        if (startRenderer != null)
        {
            startRenderer.enabled = true;
            EditorUtility.SetDirty(startRenderer);
        }

        EditorUtility.SetDirty(start);
        EditorSceneManager.MarkSceneDirty(start.scene);
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }

    private static void CleanupVisual(GameObject start)
    {
        Transform oldVisual = start.transform.Find(VisualName);
        if (oldVisual != null)
        {
            Object.DestroyImmediate(oldVisual.gameObject);
        }
    }

    private static void FitVisualToStart(GameObject visual, Bounds? targetBounds)
    {
        Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0 || !targetBounds.HasValue)
        {
            return;
        }

        Bounds bounds = GetBounds(renderers);
        Vector3 visualSize = bounds.size;
        Vector3 targetSize = targetBounds.Value.size;

        if (visualSize.x <= 0f || visualSize.z <= 0f)
        {
            return;
        }

        float uniformScale = Mathf.Min(targetSize.x / visualSize.x, targetSize.z / visualSize.z);
        visual.transform.localScale *= uniformScale;

        renderers = visual.GetComponentsInChildren<Renderer>();
        bounds = GetBounds(renderers);

        Vector3 targetCenter = targetBounds.Value.center;
        targetCenter.y = targetBounds.Value.max.y;

        Vector3 bottomCenter = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
        visual.transform.position += targetCenter - bottomCenter;
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
