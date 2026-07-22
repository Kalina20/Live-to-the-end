using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SummerTileVisualSetup
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string VisualName = "Summer Tile Visual";
    private const string ModelFolder = "Assets/Models/Tiles/Summer/";

    [MenuItem("Game/Setup/Apply Summer Tiles")]
    public static void ApplySummerTiles()
    {
        EditorSceneManager.OpenScene(ScenePath);
        CleanupSummerTiles();

        ApplyVisual("June_Tile_1", ModelFolder + "Summer_Tile_1.fbx");
        ApplyVisual("June_Tile_2", ModelFolder + "Summer_Tile_2.fbx");
        ApplyVisual("June_Tile_3", ModelFolder + "Summer_Tile_3.fbx");
        ApplyVisual("July_Tile_1", ModelFolder + "Summer_Tile_1.fbx");
        ApplyVisual("July_Tile_2", ModelFolder + "Summer_Tile_2.fbx");
        ApplyVisual("July_Tile_3", ModelFolder + "Summer_Tile_3.fbx");
        ApplyVisual("August_Tile_1", ModelFolder + "Summer_Tile_1.fbx");
        ApplyVisual("August_Tile_2", ModelFolder + "Summer_Tile_2.fbx");
        ApplyVisual("August_Tile_3", ModelFolder + "Summer_Tile_3.fbx");

        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Game/Setup/Cleanup Summer Tiles")]
    public static void CleanupSummerTiles()
    {
        EditorSceneManager.OpenScene(ScenePath);

        CleanupTile("June_Tile_1");
        CleanupTile("June_Tile_2");
        CleanupTile("June_Tile_3");
        CleanupTile("July_Tile_1");
        CleanupTile("July_Tile_2");
        CleanupTile("July_Tile_3");
        CleanupTile("August_Tile_1");
        CleanupTile("August_Tile_2");
        CleanupTile("August_Tile_3");

        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }

    private static void ApplyVisual(string tileName, string modelPath)
    {
        GameObject tile = FindTile(tileName);
        GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);

        if (tile == null || modelPrefab == null)
        {
            Debug.LogError($"Could not apply summer visual. Tile: {tileName}, model: {modelPath}");
            return;
        }

        Transform oldVisual = tile.transform.Find(VisualName);
        if (oldVisual != null)
        {
            Object.DestroyImmediate(oldVisual.gameObject);
        }

        MeshRenderer tileRenderer = tile.GetComponent<MeshRenderer>();
        if (tileRenderer != null)
        {
            tileRenderer.enabled = false;
        }

        GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab, tile.transform);
        visual.name = VisualName;
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        visual.transform.localScale = Vector3.one;

        FitVisualToTile(tile, visual, tileRenderer);

        EditorUtility.SetDirty(tile);
        EditorSceneManager.MarkSceneDirty(tile.scene);
    }

    private static void CleanupTile(string tileName)
    {
        GameObject tile = FindTile(tileName);
        if (tile == null)
        {
            return;
        }

        Transform oldVisual = tile.transform.Find(VisualName);
        if (oldVisual != null)
        {
            Object.DestroyImmediate(oldVisual.gameObject);
        }

        MeshRenderer tileRenderer = tile.GetComponent<MeshRenderer>();
        if (tileRenderer != null)
        {
            tileRenderer.enabled = true;
            EditorUtility.SetDirty(tileRenderer);
        }

        EditorUtility.SetDirty(tile);
        EditorSceneManager.MarkSceneDirty(tile.scene);
    }

    private static GameObject FindTile(string tileName)
    {
        GameObject exactMatch = GameObject.Find(tileName);
        if (exactMatch != null)
        {
            return exactMatch;
        }

        BoardTile[] tiles = Object.FindObjectsByType<BoardTile>(FindObjectsSortMode.None);
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] != null && tiles[i].name.Trim() == tileName)
            {
                return tiles[i].gameObject;
            }
        }

        return null;
    }

    private static void FitVisualToTile(GameObject tile, GameObject visual, MeshRenderer tileRenderer)
    {
        if (tileRenderer == null)
        {
            Debug.LogError($"Tile has no renderer for size reference: {tile.name}");
            return;
        }

        Bounds tileBounds = tileRenderer.bounds;
        Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError($"Summer visual has no renderers: {visual.name}");
            return;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        Vector3 size = bounds.size;
        if (size.x <= 0f || size.y <= 0f || size.z <= 0f)
        {
            Debug.LogError($"Summer visual has invalid bounds: {visual.name}");
            return;
        }

        float uniformScale = Mathf.Min(
            tileBounds.size.x / size.x,
            tileBounds.size.z / size.z
        );

        visual.transform.localScale *= uniformScale;

        renderers = visual.GetComponentsInChildren<Renderer>();
        bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        Vector3 targetCenter = tileBounds.center;
        Vector3 offset = targetCenter - bounds.center;
        visual.transform.position += offset;
    }
}
