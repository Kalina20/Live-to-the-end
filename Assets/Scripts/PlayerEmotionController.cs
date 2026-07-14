using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEmotionController : MonoBehaviour
{
    public enum PlayerEmotion
    {
        Cry,
        Think,
        Celebrate
    }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float emotionDuration = 2f;
    [SerializeField] private float animationSpeed = 8f;
    [SerializeField] private Vector3 bubbleLocalPosition = new Vector3(0f, 0.68f, 0.08f);

    private readonly Dictionary<Transform, PoseData> startPoses = new Dictionary<Transform, PoseData>();
    private Coroutine emotionRoutine;
    private readonly List<Transform> leftArmParts = new List<Transform>();
    private readonly List<Transform> rightArmParts = new List<Transform>();
    private readonly List<Transform> headParts = new List<Transform>();
    private readonly List<Transform> bodyParts = new List<Transform>();
    private Transform emotionBubble;
    private Material emotionIconMaterial;
    private Texture2D angryIcon;
    private Texture2D surpriseIcon;
    private Texture2D happyIcon;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (visualRoot == null)
        {
            Transform foundVisual = transform.Find("Player Visual");
            visualRoot = foundVisual != null ? foundVisual : transform;
        }

        FindBodyParts();
        SetupPlayerCamera();
        CreateEmotionBubble();
        RememberStartPoses();
    }

    public IEnumerator PlayEmotionForDiceResult(int diceResult)
    {
        switch (diceResult)
        {
            case 1:
                yield return PlayEmotion(PlayerEmotion.Cry);
                break;

            case 2:
                yield return PlayEmotion(PlayerEmotion.Think);
                break;

            case 3:
                yield return PlayEmotion(PlayerEmotion.Celebrate);
                break;
        }
    }

    public IEnumerator PlayEmotion(PlayerEmotion emotion)
    {
        if (emotionRoutine != null)
        {
            StopCoroutine(emotionRoutine);
            ResetPose();
        }

        emotionRoutine = StartCoroutine(EmotionRoutine(emotion));
        yield return emotionRoutine;
    }

    private IEnumerator EmotionRoutine(PlayerEmotion emotion)
    {
        SetPlayerCameraActive(true);
        SetEmotionBubbleActive(true, emotion);

        float timer = 0f;

        while (timer < emotionDuration)
        {
            timer += Time.deltaTime;
            float pulse = Mathf.Sin(timer * 8f) * 0.5f + 0.5f;
            ApplyEmotionPose(emotion, pulse);
            UpdateEmotionBubble();

            yield return null;
        }

        float resetTimer = 0f;

        while (resetTimer < 0.25f)
        {
            resetTimer += Time.deltaTime;
            SmoothResetPose();

            yield return null;
        }

        ResetPose();
        SetEmotionBubbleActive(false, emotion);
        SetPlayerCameraActive(false);
        emotionRoutine = null;
    }

    private void ApplyEmotionPose(PlayerEmotion emotion, float pulse)
    {
        switch (emotion)
        {
            case PlayerEmotion.Cry:
                SetLocalRotation(leftArmParts, Quaternion.Euler(18f, -8f, 58f));
                SetLocalRotation(rightArmParts, Quaternion.Euler(18f, 8f, -58f));
                SetLocalPosition(leftArmParts, new Vector3(-0.18f, -0.18f + pulse * 0.04f, 0.22f));
                SetLocalPosition(rightArmParts, new Vector3(0.18f, -0.18f + pulse * 0.04f, 0.22f));
                SetLocalRotation(headParts, Quaternion.Euler(10f, 0f, Mathf.Lerp(-8f, 8f, pulse)));
                SetLocalPosition(bodyParts, new Vector3(Mathf.Lerp(-0.04f, 0.04f, pulse), -0.02f, 0f));
                break;

            case PlayerEmotion.Think:
                SetLocalRotation(leftArmParts, Quaternion.Euler(20f, -8f, -28f));
                SetLocalRotation(rightArmParts, Quaternion.Euler(20f, 8f, 28f));
                SetLocalPosition(leftArmParts, new Vector3(-0.16f, 0.22f + pulse * 0.03f, 0.24f));
                SetLocalPosition(rightArmParts, new Vector3(0.16f, 0.22f + pulse * 0.03f, 0.24f));
                SetLocalRotation(headParts, Quaternion.Euler(-10f, 0f, Mathf.Lerp(-3f, 3f, pulse)));
                SetLocalPosition(bodyParts, new Vector3(0f, pulse * 0.03f, 0f));
                break;

            case PlayerEmotion.Celebrate:
                SetLocalRotation(leftArmParts, Quaternion.Euler(34f, -12f, -52f));
                SetLocalRotation(rightArmParts, Quaternion.Euler(34f, 12f, 52f));
                SetLocalPosition(leftArmParts, new Vector3(-0.2f, 0.34f + pulse * 0.05f, 0.26f));
                SetLocalPosition(rightArmParts, new Vector3(0.2f, 0.34f + pulse * 0.05f, 0.26f));
                SetLocalRotation(headParts, Quaternion.Euler(-6f, 0f, 0f));
                SetLocalPosition(bodyParts, new Vector3(0f, pulse * 0.08f, 0f));
                break;
        }
    }

    private void FindBodyParts()
    {
        leftArmParts.Clear();
        rightArmParts.Clear();
        headParts.Clear();
        bodyParts.Clear();

        FindParts("Left Arm", leftArmParts);
        FindParts("Right Arm", rightArmParts);
        FindParts("Head", headParts);
        FindParts("Body", bodyParts);
    }

    private void FindParts(string namePart, List<Transform> results)
    {
        foreach (Transform child in visualRoot.GetComponentsInChildren<Transform>(true))
        {
            if (child.name.Contains(namePart))
            {
                results.Add(child);
            }
        }
    }

    private void SetupPlayerCamera()
    {
        if (playerCamera == null)
        {
            foreach (Camera camera in GetComponentsInChildren<Camera>(true))
            {
                if (camera != mainCamera)
                {
                    playerCamera = camera;
                    break;
                }
            }
        }

        if (playerCamera == null)
        {
            GameObject cameraObject = new GameObject("Player Emotion Camera");
            cameraObject.transform.SetParent(transform);
            cameraObject.transform.localPosition = new Vector3(0f, 1.25f, -2.45f);
            cameraObject.transform.localRotation = Quaternion.Euler(10f, 0f, 0f);
            playerCamera = cameraObject.AddComponent<Camera>();
            playerCamera.fieldOfView = 48f;
        }

        playerCamera.tag = "Untagged";
        playerCamera.enabled = false;
        playerCamera.depth = 10f;
    }

    private void CreateEmotionBubble()
    {
        if (emotionBubble != null)
        {
            return;
        }

        GameObject bubbleObject = new GameObject("Emotion Bubble");
        bubbleObject.transform.SetParent(transform);
        bubbleObject.transform.localPosition = bubbleLocalPosition;
        emotionBubble = bubbleObject.transform;

        Material bubbleMaterial = new Material(FindShader("Universal Render Pipeline/Lit"));
        bubbleMaterial.color = new Color(1f, 1f, 1f, 0.95f);

        CreateBubblePart("Bubble Left", new Vector3(-0.14f, 0f, 0f), new Vector3(0.26f, 0.2f, 0.05f), bubbleMaterial);
        CreateBubblePart("Bubble Center", new Vector3(0.02f, 0.04f, 0f), new Vector3(0.34f, 0.25f, 0.05f), bubbleMaterial);
        CreateBubblePart("Bubble Right", new Vector3(0.21f, 0f, 0f), new Vector3(0.25f, 0.19f, 0.05f), bubbleMaterial);
        CreateBubblePart("Bubble Tail", new Vector3(-0.16f, -0.17f, 0f), new Vector3(0.09f, 0.07f, 0.04f), bubbleMaterial);

        CreateEmotionIcons();
        CreateEmotionIconQuad();

        emotionBubble.gameObject.SetActive(false);
    }

    private void CreateBubblePart(string partName, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        part.name = partName;
        part.transform.SetParent(emotionBubble);
        part.transform.localPosition = localPosition;
        part.transform.localRotation = Quaternion.identity;
        part.transform.localScale = localScale;

        Collider collider = part.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        Renderer renderer = part.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }
    }

    private void CreateEmotionIcons()
    {
        angryIcon = CreateAngryIcon();
        surpriseIcon = CreateSurpriseIcon();
        happyIcon = CreateHappyIcon();

        emotionIconMaterial = new Material(FindShader("Unlit/Transparent"));
        emotionIconMaterial.mainTexture = happyIcon;
        emotionIconMaterial.color = Color.white;
    }

    private void CreateEmotionIconQuad()
    {
        GameObject iconObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
        iconObject.name = "Emotion Icon";
        iconObject.transform.SetParent(emotionBubble);
        iconObject.transform.localPosition = new Vector3(0.02f, 0.03f, -0.08f);
        iconObject.transform.localRotation = Quaternion.identity;
        iconObject.transform.localScale = new Vector3(0.17f, 0.17f, 1f);

        Collider collider = iconObject.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        Renderer renderer = iconObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = emotionIconMaterial;
        }
    }

    private Shader FindShader(string shaderName)
    {
        Shader shader = Shader.Find(shaderName);

        if (shader != null)
        {
            return shader;
        }

        return Shader.Find("Standard");
    }

    private void SetEmotionBubbleActive(bool isActive, PlayerEmotion emotion)
    {
        if (emotionBubble == null)
        {
            return;
        }

        if (emotionIconMaterial != null)
        {
            emotionIconMaterial.mainTexture = GetEmotionIcon(emotion);
        }

        emotionBubble.gameObject.SetActive(isActive);
        UpdateEmotionBubble();
    }

    private Texture2D GetEmotionIcon(PlayerEmotion emotion)
    {
        switch (emotion)
        {
            case PlayerEmotion.Cry:
                return angryIcon;

            case PlayerEmotion.Think:
                return surpriseIcon;

            case PlayerEmotion.Celebrate:
                return happyIcon;

            default:
                return happyIcon;
        }
    }

    private Texture2D CreateAngryIcon()
    {
        Texture2D texture = CreateBlankIcon();
        DrawFilledCircle(texture, 64, 64, 45, new Color(1f, 0.33f, 0.18f, 1f));
        DrawCircle(texture, 64, 64, 45, 4, new Color(0.6f, 0.05f, 0.02f, 1f));
        DrawFilledCircle(texture, 47, 63, 6, Color.black);
        DrawFilledCircle(texture, 81, 63, 6, Color.black);
        DrawLine(texture, 35, 83, 56, 73, 5, Color.black);
        DrawLine(texture, 72, 73, 93, 83, 5, Color.black);
        DrawLine(texture, 47, 42, 82, 42, 5, Color.black);
        DrawLine(texture, 40, 101, 27, 113, 4, new Color(1f, 0.75f, 0.12f, 1f));
        DrawLine(texture, 88, 101, 101, 113, 4, new Color(1f, 0.75f, 0.12f, 1f));
        texture.Apply();
        return texture;
    }

    private Texture2D CreateSurpriseIcon()
    {
        Texture2D texture = CreateBlankIcon();
        DrawFilledCircle(texture, 64, 64, 45, new Color(1f, 0.82f, 0.18f, 1f));
        DrawCircle(texture, 64, 64, 45, 4, new Color(0.82f, 0.48f, 0.02f, 1f));
        DrawFilledCircle(texture, 48, 70, 7, Color.black);
        DrawFilledCircle(texture, 80, 70, 7, Color.black);
        DrawCircle(texture, 64, 47, 13, 6, Color.black);
        DrawLine(texture, 41, 91, 54, 96, 4, Color.black);
        DrawLine(texture, 74, 96, 87, 91, 4, Color.black);
        texture.Apply();
        return texture;
    }

    private Texture2D CreateHappyIcon()
    {
        Texture2D texture = CreateBlankIcon();
        DrawFilledCircle(texture, 64, 64, 45, new Color(0.34f, 0.9f, 0.3f, 1f));
        DrawCircle(texture, 64, 64, 45, 4, new Color(0.05f, 0.52f, 0.12f, 1f));
        DrawFilledCircle(texture, 48, 70, 6, Color.black);
        DrawFilledCircle(texture, 80, 70, 6, Color.black);

        for (int angle = 205; angle <= 335; angle += 2)
        {
            float radians = angle * Mathf.Deg2Rad;
            int x = Mathf.RoundToInt(64 + Mathf.Cos(radians) * 25f);
            int y = Mathf.RoundToInt(65 + Mathf.Sin(radians) * 25f);
            DrawFilledCircle(texture, x, y, 3, Color.black);
        }

        DrawFilledCircle(texture, 41, 86, 4, new Color(1f, 0.95f, 0.25f, 1f));
        DrawFilledCircle(texture, 87, 86, 4, new Color(1f, 0.95f, 0.25f, 1f));
        texture.Apply();
        return texture;
    }

    private Texture2D CreateBlankIcon()
    {
        Texture2D texture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;

        Color clear = new Color(0f, 0f, 0f, 0f);
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, clear);
            }
        }

        return texture;
    }

    private void DrawFilledCircle(Texture2D texture, int centerX, int centerY, int radius, Color color)
    {
        int radiusSquared = radius * radius;

        for (int y = centerY - radius; y <= centerY + radius; y++)
        {
            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                int dx = x - centerX;
                int dy = y - centerY;

                if (dx * dx + dy * dy <= radiusSquared)
                {
                    SetPixelSafe(texture, x, y, color);
                }
            }
        }
    }

    private void DrawCircle(Texture2D texture, int centerX, int centerY, int radius, int thickness, Color color)
    {
        int outer = radius * radius;
        int innerRadius = radius - thickness;
        int inner = innerRadius * innerRadius;

        for (int y = centerY - radius; y <= centerY + radius; y++)
        {
            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                int dx = x - centerX;
                int dy = y - centerY;
                int distance = dx * dx + dy * dy;

                if (distance <= outer && distance >= inner)
                {
                    SetPixelSafe(texture, x, y, color);
                }
            }
        }
    }

    private void DrawLine(Texture2D texture, int startX, int startY, int endX, int endY, int thickness, Color color)
    {
        int steps = Mathf.Max(Mathf.Abs(endX - startX), Mathf.Abs(endY - startY));

        for (int i = 0; i <= steps; i++)
        {
            float t = steps == 0 ? 0f : (float)i / steps;
            int x = Mathf.RoundToInt(Mathf.Lerp(startX, endX, t));
            int y = Mathf.RoundToInt(Mathf.Lerp(startY, endY, t));
            DrawFilledCircle(texture, x, y, thickness, color);
        }
    }

    private void SetPixelSafe(Texture2D texture, int x, int y, Color color)
    {
        if (x < 0 || y < 0 || x >= texture.width || y >= texture.height)
        {
            return;
        }

        texture.SetPixel(x, y, color);
    }

    private void UpdateEmotionBubble()
    {
        if (emotionBubble == null || playerCamera == null)
        {
            return;
        }

        emotionBubble.localPosition = bubbleLocalPosition;
        emotionBubble.rotation = Quaternion.LookRotation(emotionBubble.position - playerCamera.transform.position);
    }

    private void RememberStartPoses()
    {
        startPoses.Clear();
        RememberPoses(leftArmParts);
        RememberPoses(rightArmParts);
        RememberPoses(headParts);
        RememberPoses(bodyParts);
    }

    private void RememberPoses(List<Transform> targets)
    {
        foreach (Transform target in targets)
        {
            RememberPose(target);
        }
    }

    private void RememberPose(Transform target)
    {
        if (target != null && !startPoses.ContainsKey(target))
        {
            startPoses.Add(target, new PoseData(target.localPosition, target.localRotation));
        }
    }

    private void SmoothResetPose()
    {
        foreach (KeyValuePair<Transform, PoseData> pose in startPoses)
        {
            if (pose.Key == null)
            {
                continue;
            }

            pose.Key.localPosition = Vector3.Lerp(
                pose.Key.localPosition,
                pose.Value.LocalPosition,
                Time.deltaTime * animationSpeed
            );
            pose.Key.localRotation = Quaternion.Slerp(
                pose.Key.localRotation,
                pose.Value.LocalRotation,
                Time.deltaTime * animationSpeed
            );
        }
    }

    private void ResetPose()
    {
        foreach (KeyValuePair<Transform, PoseData> pose in startPoses)
        {
            if (pose.Key == null)
            {
                continue;
            }

            pose.Key.localPosition = pose.Value.LocalPosition;
            pose.Key.localRotation = pose.Value.LocalRotation;
        }
    }

    private void SetLocalRotation(Transform target, Quaternion rotation)
    {
        if (target == null)
        {
            return;
        }

        target.localRotation = Quaternion.Slerp(target.localRotation, rotation, Time.deltaTime * animationSpeed);
    }

    private void SetLocalRotation(List<Transform> targets, Quaternion rotation)
    {
        foreach (Transform target in targets)
        {
            SetLocalRotation(target, rotation);
        }
    }

    private void SetLocalPosition(Transform target, Vector3 offset)
    {
        if (target == null || !startPoses.TryGetValue(target, out PoseData pose))
        {
            return;
        }

        target.localPosition = Vector3.Lerp(
            target.localPosition,
            pose.LocalPosition + offset,
            Time.deltaTime * animationSpeed
        );
    }

    private void SetLocalPosition(List<Transform> targets, Vector3 offset)
    {
        foreach (Transform target in targets)
        {
            SetLocalPosition(target, offset);
        }
    }

    private void SetPlayerCameraActive(bool isActive)
    {
        if (mainCamera != null)
        {
            mainCamera.enabled = !isActive;
        }

        if (playerCamera != null)
        {
            playerCamera.enabled = isActive;
        }
    }

    private struct PoseData
    {
        public PoseData(Vector3 localPosition, Quaternion localRotation)
        {
            LocalPosition = localPosition;
            LocalRotation = localRotation;
        }

        public Vector3 LocalPosition { get; }
        public Quaternion LocalRotation { get; }
    }
}
