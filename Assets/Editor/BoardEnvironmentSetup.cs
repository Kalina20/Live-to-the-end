using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BoardEnvironmentSetup
{
    private const string VisualName = "Board Table Environment Visual";
    private const string ModelPath = "Assets/Models/Environment/Board_Table_Environment.fbx";

    private static readonly Vector3 EnvironmentPosition = new Vector3(6f, -0.28f, 9f);
    private static readonly Quaternion EnvironmentRotation = Quaternion.Euler(90f, 0f, 0f);
    private static readonly Color CameraBackground = new Color(0.72f, 0.86f, 0.96f, 1f);

    [MenuItem("Game/Setup/Apply Board Environment")]
    public static void ApplyBoardEnvironment()
    {
        GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
        if (modelPrefab == null)
        {
            Debug.LogError($"Could not apply board environment. Model: {modelPrefab}");
            return;
        }

        CleanupEnvironment();

        GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab);
        visual.name = VisualName;
        visual.transform.position = EnvironmentPosition;
        visual.transform.rotation = EnvironmentRotation;
        visual.transform.localScale = Vector3.one;

        MoveToBackOfHierarchy(visual);
        ApplyCameraBackground();

        EditorUtility.SetDirty(visual);
        EditorSceneManager.MarkSceneDirty(visual.scene);
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Game/Setup/Cleanup Board Environment")]
    public static void CleanupBoardEnvironment()
    {
        CleanupEnvironment();
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }

    private static void CleanupEnvironment()
    {
        GameObject oldVisual = GameObject.Find(VisualName);
        if (oldVisual != null)
        {
            Object.DestroyImmediate(oldVisual);
        }
    }

    private static void ApplyCameraBackground()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            return;
        }

        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = CameraBackground;
        EditorUtility.SetDirty(camera);
    }

    private static void MoveToBackOfHierarchy(GameObject visual)
    {
        visual.transform.SetAsFirstSibling();
    }
}
