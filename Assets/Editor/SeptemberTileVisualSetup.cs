using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SeptemberTileVisualSetup
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string VisualName = "Autumn Leaf Visual";
    private const string OldSeptemberVisualName = "September Leaf Visual";
    private const string VisualRootName = "September Leaf Tile Visuals";
    private const string ModelFolder = "Assets/Models/Tiles/September/";

    [MenuItem("Game/Setup/Apply Autumn Leaf Tiles")]
    public static void ApplyAutumnLeafTiles()
    {
        EditorSceneManager.OpenScene(ScenePath);
        CleanupAutumnLeafTiles();

        ApplyVisual("September_Tile_1", ModelFolder + "September_Tile_1_Leaves.fbx");
        ApplyVisual("September_Tile_2", ModelFolder + "September_Tile_2_Leaves.fbx");
        ApplyVisual("September_Tile_3", ModelFolder + "September_Tile_3_Leaves.fbx");
        ApplyVisual("October_Tile_1", ModelFolder + "September_Tile_1_Leaves.fbx");
        ApplyVisual("October_Tile_2", ModelFolder + "September_Tile_2_Leaves.fbx");
        ApplyVisual("October_Tile_3", ModelFolder + "September_Tile_3_Leaves.fbx");
        ApplyVisual("November_Tile_1", ModelFolder + "September_Tile_1_Leaves.fbx");
        ApplyVisual("November_Tile_2", ModelFolder + "September_Tile_2_Leaves.fbx");
        ApplyVisual("November_Tile_3", ModelFolder + "September_Tile_3_Leaves.fbx");

        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Game/Setup/Apply September Leaf Tiles")]
    public static void ApplySeptemberLeafTiles()
    {
        ApplyAutumnLeafTiles();
    }

    [MenuItem("Game/Setup/Cleanup Autumn Leaf Tiles")]
    public static void CleanupAutumnLeafTiles()
    {
        EditorSceneManager.OpenScene(ScenePath);

        GameObject visualRoot = GameObject.Find(VisualRootName);
        if (visualRoot != null)
        {
            Object.DestroyImmediate(visualRoot);
        }

        CleanupTile("September_Tile_1");
        CleanupTile("September_Tile_2");
        CleanupTile("September_Tile_3");
        CleanupTile("October_Tile_1");
        CleanupTile("October_Tile_2");
        CleanupTile("October_Tile_3");
        CleanupTile("November_Tile_1");
        CleanupTile("November_Tile_2");
        CleanupTile("November_Tile_3");

        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Game/Setup/Cleanup September Leaf Tiles")]
    public static void CleanupSeptemberLeafTiles()
    {
        CleanupAutumnLeafTiles();
    }

    private static void ApplyVisual(string tileName, string modelPath)
    {
        GameObject tile = FindTile(tileName);
        GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);

        if (tile == null || modelPrefab == null)
        {
            Debug.LogError($"Could not apply September visual. Tile: {tileName}, model: {modelPath}");
            return;
        }

        Transform oldVisual = tile.transform.Find(VisualName);
        if (oldVisual != null)
        {
            Object.DestroyImmediate(oldVisual.gameObject);
        }

        Transform oldSeptemberVisual = tile.transform.Find(OldSeptemberVisualName);
        if (oldSeptemberVisual != null)
        {
            Object.DestroyImmediate(oldSeptemberVisual.gameObject);
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

        Transform oldSeptemberVisual = tile.transform.Find(OldSeptemberVisualName);
        if (oldSeptemberVisual != null)
        {
            Object.DestroyImmediate(oldSeptemberVisual.gameObject);
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
            Debug.LogError($"September visual has no renderers: {visual.name}");
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
            Debug.LogError($"September visual has invalid bounds: {visual.name}");
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
