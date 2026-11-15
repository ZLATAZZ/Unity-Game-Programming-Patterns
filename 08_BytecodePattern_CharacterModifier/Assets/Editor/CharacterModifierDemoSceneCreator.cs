using System;
using System.IO;
using CharacterModifierBytecodeDemo.Character;
using CharacterModifierBytecodeDemo.Presentation;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CharacterModifierBytecodeDemo.Editor
{
    public static class CharacterModifierDemoSceneCreator
    {
        private const string SceneFolder = "Assets/CharacterModifierBytecodeDemo/Scenes";
        private const string ScenePath = SceneFolder + "/CharacterModifierDemo.unity";
        private const string DefaultTmpFontPath =
            "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";
        private const string ProgramFolder = "Assets/StreamingAssets/BytecodePrograms";

        [MenuItem("Tools/Bytecode Demo/Create Character Modifier Scene")]
        public static void CreateScene()
        {
            TMP_FontAsset font = ResolveFont();
            EnsureSceneFolder();

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera();
            CreateLight();
            CreateGround();

            CharacterModifierTarget target = CreateCharacter();

            CharacterMover mover = target.gameObject.AddComponent<CharacterMover>();
            mover.Configure(target);

            CharacterModifierController controller = CreateController();
            Canvas canvas = CreateCanvas();

            CreateHeader(canvas.transform, font);

            TextMeshProUGUI stateText = CreatePanelText(
                canvas.transform,
                font,
                "StateText",
                string.Empty,
                22.0f,
                new Vector2(30.0f, -130.0f),
                new Vector2(290.0f, 140.0f),
                new Vector2(0.0f, 1.0f));

            TextMeshProUGUI programText = CreatePanelText(
                canvas.transform,
                font,
                "ProgramText",
                "Last program: none",
                17.0f,
                new Vector2(30.0f, -285.0f),
                new Vector2(420.0f, 120.0f),
                new Vector2(0.0f, 1.0f));

            TextMeshProUGUI pathText = CreatePanelText(
                canvas.transform,
                font,
                "PathText",
                string.Empty,
                13.0f,
                new Vector2(30.0f, 118.0f),
                new Vector2(800.0f, 80.0f),
                new Vector2(0.0f, 0.0f));

            TextMeshProUGUI sourceText = CreateSourcePanel(canvas.transform, font);

            CreateProgramButtons(canvas.transform, font, controller);

            controller.Configure(target, stateText, programText, sourceText, pathText);

            CreateEventSystem();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeGameObject = target.gameObject;

            Debug.Log($"Character Modifier Bytecode demo scene created at {ScenePath}");
        }

        private static void CreateHeader(Transform parent, TMP_FontAsset font)
        {
            TextMeshProUGUI title = CreateText(
                parent,
                font,
                "Title",
                "Character Modifier — Stack Bytecode VM",
                30.0f,
                TextAlignmentOptions.Center);

            SetRect(
                title.rectTransform,
                new Vector2(0.5f, 1.0f),
                new Vector2(0.5f, 1.0f),
                new Vector2(0.0f, -35.0f),
                new Vector2(800.0f, 50.0f));

            TextMeshProUGUI subtitle = CreateText(
                parent,
                font,
                "Subtitle",
                "Programs are loaded from external .bcasm files, compiled to bytecode, then executed by the VM.",
                16.0f,
                TextAlignmentOptions.Center);

            SetRect(
                subtitle.rectTransform,
                new Vector2(0.5f, 1.0f),
                new Vector2(0.5f, 1.0f),
                new Vector2(0.0f, -78.0f),
                new Vector2(1000.0f, 40.0f));
        }

        private static TextMeshProUGUI CreateSourcePanel(Transform parent, TMP_FontAsset font)
        {
            GameObject panelObject = new GameObject("SourcePanel");
            panelObject.transform.SetParent(parent, false);

            Image panelImage = panelObject.AddComponent<Image>();
            panelImage.color = new Color(0.08f, 0.09f, 0.12f, 0.92f);

            RectTransform panelRect = panelObject.GetComponent<RectTransform>();

            SetRect(
                panelRect,
                new Vector2(1.0f, 1.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(-330.0f, -275.0f),
                new Vector2(610.0f, 340.0f));

            TextMeshProUGUI sourceText = CreateText(
                panelObject.transform,
                font,
                "SourceText",
                "External program source will appear here.",
                17.0f,
                TextAlignmentOptions.TopLeft);

            RectTransform textRect = sourceText.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(18.0f, 18.0f);
            textRect.offsetMax = new Vector2(-18.0f, -18.0f);

            sourceText.enableWordWrapping = false;
            sourceText.overflowMode = TextOverflowModes.Overflow;

            return sourceText;
        }

        private static TextMeshProUGUI CreatePanelText(
            Transform parent,
            TMP_FontAsset font,
            string name,
            string initialText,
            float fontSize,
            Vector2 anchoredPosition,
            Vector2 size,
            Vector2 anchor)
        {
            TextMeshProUGUI text = CreateText(
                parent,
                font,
                name,
                initialText,
                fontSize,
                TextAlignmentOptions.TopLeft);

            SetRect(
                text.rectTransform,
                anchor,
                anchor,
                anchoredPosition,
                size);

            return text;
        }

        private static void CreateProgramButtons(
            Transform parent,
            TMP_FontAsset font,
            CharacterModifierController controller)
        {
            string[] programPaths = Directory.GetFiles(ProgramFolder, "*.bcasm", SearchOption.TopDirectoryOnly);
            Array.Sort(programPaths, StringComparer.OrdinalIgnoreCase);

            if (programPaths.Length == 0)
            {
                throw new InvalidOperationException($"No .bcasm programs were found in {ProgramFolder}.");
            }

            const float spacing = 150.0f;
            const float width = 138.0f;

            float startX = -((programPaths.Length - 1) * spacing) * 0.5f;

            for (int i = 0; i < programPaths.Length; i++)
            {
                string fileName = Path.GetFileName(programPaths[i]);
                string displayName = ObjectNames.NicifyVariableName(Path.GetFileNameWithoutExtension(fileName));
                float x = startX + i * spacing;

                CreateButton(
                    parent,
                    font,
                    $"ProgramButton_{i}",
                    displayName,
                    fileName,
                    new Vector2(x, 38.0f),
                    new Vector2(width, 48.0f),
                    controller);
            }
        }

        private static CharacterModifierTarget CreateCharacter()
        {
            GameObject character = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            character.name = "BytecodeCharacter";
            character.transform.position = new Vector3(0.0f, 1.0f, 0.0f);

            return character.AddComponent<CharacterModifierTarget>();
        }

        private static CharacterModifierController CreateController()
        {
            GameObject controllerObject = new GameObject("CharacterModifierController");
            return controllerObject.AddComponent<CharacterModifierController>();
        }

        private static void CreateCamera()
        {
            GameObject cameraObject = new GameObject("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();

            cameraObject.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.055f, 0.065f, 0.085f);

            camera.transform.position = new Vector3(0.0f, 2.4f, -8.0f);
            camera.transform.LookAt(new Vector3(0.0f, 1.0f, 0.0f));
        }

        private static void CreateLight()
        {
            GameObject lightObject = new GameObject("Directional Light");
            Light light = lightObject.AddComponent<Light>();

            light.type = LightType.Directional;
            light.intensity = 1.2f;

            lightObject.transform.rotation = Quaternion.Euler(45.0f, -30.0f, 0.0f);
        }

        private static void CreateGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(1.2f, 1.0f, 1.2f);
        }

        private static Canvas CreateCanvas()
        {
            GameObject canvasObject = new GameObject("Canvas");

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280.0f, 720.0f);

            canvasObject.AddComponent<GraphicRaycaster>();

            return canvas;
        }

        private static TextMeshProUGUI CreateText(
            Transform parent,
            TMP_FontAsset font,
            string name,
            string value,
            float fontSize,
            TextAlignmentOptions alignment)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);

            TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
            text.font = font;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.text = value;
            text.color = Color.white;
            text.raycastTarget = false;

            return text;
        }

        private static Button CreateButton(
            Transform parent,
            TMP_FontAsset font,
            string name,
            string displayName,
            string programFileName,
            Vector2 anchoredPosition,
            Vector2 size,
            CharacterModifierController controller)
        {
            GameObject buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.16f, 0.18f, 0.24f, 0.98f);

            Button button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;

            RectTransform rect = buttonObject.GetComponent<RectTransform>();

            SetRect(
                rect,
                new Vector2(0.5f, 0.0f),
                new Vector2(0.5f, 0.0f),
                anchoredPosition,
                size);

            TextMeshProUGUI label = CreateText(
                buttonObject.transform,
                font,
                "Label",
                displayName,
                15.0f,
                TextAlignmentOptions.Center);

            RectTransform textRect = label.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(6.0f, 4.0f);
            textRect.offsetMax = new Vector2(-6.0f, -4.0f);
            label.enableWordWrapping = true;

            BytecodeProgramButton programButton = buttonObject.AddComponent<BytecodeProgramButton>();
            programButton.Configure(controller, programFileName, label, displayName);

            return button;
        }

        private static void CreateEventSystem()
        {
            GameObject eventSystemObject = new GameObject("EventSystem");

            eventSystemObject.AddComponent<EventSystem>();

            // New Input System only.
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        private static TMP_FontAsset ResolveFont()
        {
            TMP_FontAsset font = TMP_Settings.defaultFontAsset;

            if (font != null)
            {
                return font;
            }

            font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(DefaultTmpFontPath);

            if (font != null)
            {
                return font;
            }

            throw new InvalidOperationException(
                "TextMesh Pro Essential Resources are missing. " +
                "Use Window > TextMeshPro > Import TMP Essential Resources, then run the scene creator again.");
        }

        private static void SetRect(
            RectTransform rect,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
        }

        private static void EnsureSceneFolder()
        {
            Directory.CreateDirectory(SceneFolder);
            AssetDatabase.Refresh();
        }

    }
}
