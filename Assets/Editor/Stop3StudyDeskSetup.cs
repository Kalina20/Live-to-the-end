using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Stop3StudyDeskSetup
{
    private const string StopName = "Stop_3";
    private const string VisualName = "Stop 3 Study Desk Visual";
    private const string ModelPath = "Assets/Models/Stops/Stop_3_Study_Desk.fbx";

    [MenuItem("Game/Setup/Apply Stop 3 Study Desk")]
    public static void ApplyStop3StudyDesk()
    {
        GameObject stop = GameObject.Find(StopName);
        GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);

        if (stop == null || modelPrefab == null)
        {
            Debug.LogError($"Could not apply Stop 3 study desk visual. Stop: {stop}, model: {modelPrefab}");
            return;
        }

        CleanupVisual(stop);

        MeshRenderer stopRenderer = stop.GetComponent<MeshRenderer>();
        Bounds? stopBounds = stopRenderer != null ? stopRenderer.bounds : null;

        if (stopRenderer != null)
        {
            stopRenderer.enabled = false;
            EditorUtility.SetDirty(stopRenderer);
        }

        GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab, stop.transform);
        visual.name = VisualName;
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        visual.transform.localScale = Vector3.one;

        FitVisualToStop(visual, stopBounds);

        EditorUtility.SetDirty(stop);
        EditorSceneManager.MarkSceneDirty(stop.scene);
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Game/Setup/Cleanup Stop 3 Study Desk")]
    public static void CleanupStop3StudyDesk()
    {
        GameObject stop = GameObject.Find(StopName);
        if (stop == null)
        {
            return;
        }

        CleanupVisual(stop);

        MeshRenderer stopRenderer = stop.GetComponent<MeshRenderer>();
        if (stopRenderer != null)
        {
            stopRenderer.enabled = true;
            EditorUtility.SetDirty(stopRenderer);
        }

        EditorUtility.SetDirty(stop);
        EditorSceneManager.MarkSceneDirty(stop.scene);
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }

    private static void CleanupVisual(GameObject stop)
    {
        Transform oldVisual = stop.transform.Find(VisualName);
        if (oldVisual != null)
        {
            Object.DestroyImmediate(oldVisual.gameObject);
        }
    }

    private static void FitVisualToStop(GameObject visual, Bounds? targetBounds)
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
