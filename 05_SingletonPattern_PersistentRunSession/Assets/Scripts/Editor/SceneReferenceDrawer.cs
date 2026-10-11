using Patterns.Singleton.Levels;
using UnityEditor;
using UnityEngine;

namespace Patterns.Singleton.Editor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public sealed class SceneReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty assetProperty = property.FindPropertyRelative("_sceneAsset");
            SerializedProperty pathProperty = property.FindPropertyRelative("_scenePath");

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, assetProperty, label);

            string scenePath = assetProperty.objectReferenceValue == null ? string.Empty : AssetDatabase.GetAssetPath(assetProperty.objectReferenceValue);

            if (pathProperty.stringValue != scenePath)
            {
                pathProperty.stringValue = scenePath;
            }

            EditorGUI.EndProperty();
        }
    }
}
