using Patterns.Command.Customization.BodyMaterial;
using UnityEditor;
using UnityEngine;

namespace Patterns.Command.Customization.Editor
{
    [CustomEditor(typeof(MaterialFeature))]
    public sealed class MaterialFeatureEditor : UnityEditor.Editor
    {
        private SerializedProperty _targetsProperty;
        private SerializedProperty _collectionRootProperty;
        private SerializedProperty _sourceMaterialProperty;

        private void OnEnable()
        {
            _targetsProperty = serializedObject.FindProperty("_targets");
            _collectionRootProperty = serializedObject.FindProperty("_collectionRoot");
            _sourceMaterialProperty = serializedObject.FindProperty("_sourceMaterial");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(10);

            Transform collectionRoot = _collectionRootProperty.objectReferenceValue as Transform;
            Material sourceMaterial = _sourceMaterialProperty.objectReferenceValue as Material;

            if (collectionRoot == null)
            {
                EditorGUILayout.HelpBox(
                    "Assign Collection Root before collecting paint targets.",
                    MessageType.Info);
            }

            if (sourceMaterial == null)
            {
                EditorGUILayout.HelpBox(
                    "Assign Source Material before collecting paint targets.",
                    MessageType.Info);
            }

            using (new EditorGUI.DisabledScope(collectionRoot == null || sourceMaterial == null))
            {
                if (GUILayout.Button("Collect Paint Targets"))
                {
                    CollectTargets(collectionRoot, sourceMaterial);
                }
            }
        }

        private void CollectTargets(Transform collectionRoot, Material sourceMaterial)
        {
            serializedObject.Update();

            Renderer[] renderers = collectionRoot.GetComponentsInChildren<Renderer>(true);

            _targetsProperty.ClearArray();

            int targetCount = 0;

            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.sharedMaterials;

                for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
                {
                    if (materials[materialIndex] != sourceMaterial)
                    {
                        continue;
                    }

                    AddTarget(renderer, materialIndex);
                    targetCount++;
                }
            }

            serializedObject.ApplyModifiedProperties();

            MaterialFeature materialFeature = (MaterialFeature)target;

            EditorUtility.SetDirty(materialFeature);
            PrefabUtility.RecordPrefabInstancePropertyModifications(materialFeature);

            if (targetCount == 0)
            {
                Debug.LogWarning(
                    $"No material slots using {sourceMaterial.name} were found under {collectionRoot.name}.",
                    materialFeature);

                return;
            }

            Debug.Log(
                $"Collected {targetCount} paint material slots under {collectionRoot.name}.",
                materialFeature);
        }

        private void AddTarget(Renderer renderer, int materialIndex)
        {
            int index = _targetsProperty.arraySize;

            _targetsProperty.InsertArrayElementAtIndex(index);

            SerializedProperty targetProperty = _targetsProperty.GetArrayElementAtIndex(index);

            targetProperty.FindPropertyRelative("_renderer").objectReferenceValue = renderer;
            targetProperty.FindPropertyRelative("_materialIndex").intValue = materialIndex;
        }
    }
}