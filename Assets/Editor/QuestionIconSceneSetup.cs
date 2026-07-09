using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class QuestionIconSceneSetup
{
    private const string ModelPath = "Assets/Models/Icons/Question_Icon.fbx";
    private const string PrefabFolder = "Assets/Prefabs";
    private const string PrefabPath = "Assets/Prefabs/QuestionIcon.prefab";
    private const string MaterialPath = "Assets/Materials/QuestionIconYellow.mat";
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string IconObjectName = "Question Tile Icon";

    [MenuItem("Tools/Setup Question Icons")]
    public static void Setup()
    {
        if (!AssetDatabase.IsValidFolder(PrefabFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        CreatePrefab();
        PlaceIconsInScene();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void CreatePrefab()
    {
        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);

        if (model == null)
        {
            throw new FileNotFoundException("Question icon model not found.", ModelPath);
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(model);
        instance.name = "QuestionIcon";
        Material yellowMaterial = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);

        if (instance.GetComponent<RotatingIcon>() == null)
        {
            instance.AddComponent<RotatingIcon>();
        }

        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (yellowMaterial != null)
            {
                renderers[i].sharedMaterial = yellowMaterial;
            }

            renderers[i].shadowCastingMode = ShadowCastingMode.Off;
            renderers[i].receiveShadows = false;
        }

        PrefabUtility.SaveAsPrefabAsset(instance, PrefabPath);
        Object.DestroyImmediate(instance);
    }

    private static void PlaceIconsInScene()
    {
        EditorSceneManager.OpenScene(ScenePath);

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);

        if (prefab == null)
        {
            throw new FileNotFoundException("Question icon prefab not found.", PrefabPath);
        }

        BoardTile[] tiles = Object.FindObjectsByType<BoardTile>(FindObjectsSortMode.None);

        for (int i = 0; i < tiles.Length; i++)
        {
            BoardTile tile = tiles[i];

            if (tile == null || !tile.name.Trim().EndsWith("_Tile_2"))
            {
                continue;
            }

            Transform oldIcon = tile.transform.Find(IconObjectName);

            if (oldIcon != null)
            {
                Object.DestroyImmediate(oldIcon.gameObject);
            }

            GameObject icon = (GameObject)PrefabUtility.InstantiatePrefab(prefab, tile.transform);
            icon.name = IconObjectName;
            icon.transform.localPosition = new Vector3(0f, 2f, 0f);
            icon.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            icon.transform.localScale = new Vector3(50f, 300f, 20f);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }
}
