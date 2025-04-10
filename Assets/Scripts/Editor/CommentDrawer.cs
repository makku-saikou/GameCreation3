// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_04_10
// Description:
// -------------------------------------------------

using Common.Attribute;
using UnityEditor;
using UnityEngine;

// namespace Editor
// {
    [CustomPropertyDrawer(typeof(CommentAttribute))]
    public class CommentDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CommentAttribute commentAttribute = (CommentAttribute)attribute;
            EditorGUI.LabelField(position, commentAttribute.Comment);
            position.y += EditorGUIUtility.singleLineHeight + 5;
            position.height = EditorGUIUtility.singleLineHeight + 5;
            EditorGUI.PropertyField(position, property, label, true);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
// }