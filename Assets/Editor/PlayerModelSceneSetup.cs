using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PlayerModelSceneSetup
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string PlayerModelPath = "Assets/Models/Player/Player_Character.fbx";
    private const string VisualName = "Player Visual";

    [MenuItem("Game/Setup/Replace Player Visual")]
    public static void ReplacePlayerVisual()
    {
        EditorSceneManager.OpenScene(ScenePath);
        AssetDatabase.ImportAsset(PlayerModelPath, ImportAssetOptions.ForceUpdate);

        GameObject player = GameObject.Find("Player");
        GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerModelPath);

        if (player == null || modelPrefab == null)
        {
            Debug.LogError("Player or Player_Character.fbx was not found.");
            return;
        }

        RemoveOldVisual(player);

        Transform oldVisual = player.transform.Find(VisualName);
        if (oldVisual != null)
        {
            Object.DestroyImmediate(oldVisual.gameObject);
        }

        player.transform.localScale = Vector3.one;

        GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab, player.transform);
        visual.name = VisualName;
        visual.transform.localPosition = new Vector3(0f, -0.32f, 0f);
        visual.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        visual.transform.localScale = Vector3.one * 0.34f;
        DisableImportedCameras(visual);
        RemoveImportedLights(visual);

        CapsuleCollider capsuleCollider = player.GetComponent<CapsuleCollider>();
        if (capsuleCollider != null)
        {
            capsuleCollider.radius = 0.22f;
            capsuleCollider.height = 0.78f;
            capsuleCollider.center = new Vector3(0f, 0.08f, 0f);
            capsuleCollider.direction = 1;
        }

        if (player.GetComponent<PlayerEmotionController>() == null)
        {
            player.AddComponent<PlayerEmotionController>();
        }

        EditorUtility.SetDirty(player);
        EditorSceneManager.MarkSceneDirty(player.scene);
        EditorSceneManager.SaveScene(player.scene);
        AssetDatabase.SaveAssets();
    }

    private static void RemoveOldVisual(GameObject player)
    {
        MeshRenderer meshRenderer = player.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            Object.DestroyImmediate(meshRenderer);
        }

        MeshFilter meshFilter = player.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Object.DestroyImmediate(meshFilter);
        }
    }

    private static void DisableImportedCameras(GameObject visual)
    {
        foreach (Camera camera in visual.GetComponentsInChildren<Camera>(true))
        {
            camera.enabled = false;
            camera.tag = "Untagged";
            camera.gameObject.name = "Player Preview Camera";
        }
    }

    private static void RemoveImportedLights(GameObject visual)
    {
        foreach (Light light in visual.GetComponentsInChildren<Light>(true))
        {
            Object.DestroyImmediate(light.gameObject);
        }
    }
}
