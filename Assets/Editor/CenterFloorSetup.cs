using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CenterFloorSetup
{
    private const string VisualName = "Center Floor Visual";
    private const string ModelPath = "Assets/Models/Floor/Center_Floor.fbx";

    [MenuItem("Game/Setup/Apply Center Floor")]
    public static void ApplyCenterFloor()
    {
        CleanupCenterFloor(false);

        GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
        if (modelPrefab == null)
        {
            Debug.LogError($"Could not load center floor model: {ModelPath}");
            return;
        }

        GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab);
        visual.name = VisualName;
        visual.transform.position = GetCenterPosition();
        visual.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        visual.transform.localScale = Vector3.one;

        EditorUtility.SetDirty(visual);
        EditorSceneManager.MarkSceneDirty(visual.scene);
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Game/Setup/Cleanup Center Floor")]
    public static void CleanupCenterFloor()
    {
        CleanupCenterFloor(true);
    }

    private static void CleanupCenterFloor(bool saveScene)
    {
        GameObject oldVisual = GameObject.Find(VisualName);
        if (oldVisual != null)
        {
            Object.DestroyImmediate(oldVisual);
        }

        if (saveScene)
        {
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
        }
    }

    private static Vector3 GetCenterPosition()
    {
        GameObject dice = GameObject.Find("Dice");
        if (dice == null)
        {
            return new Vector3(6f, -0.08f, 9f);
        }

        Vector3 position = dice.transform.position;
        position.y = -0.08f;
        return position;
    }
}
