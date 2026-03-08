using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraPositionAngleCalculator))]
public class CameraPositionAngleCalculatorEditor : Editor
{
    private SerializedProperty lockHeight, lockLength, lockAngle;
    private SerializedProperty height, length, angle;

    private void OnEnable()
    {
        lockHeight = serializedObject.FindProperty("lockHeight");
        lockLength = serializedObject.FindProperty("lockLength");
        lockAngle  = serializedObject.FindProperty("lockAngle");
        height     = serializedObject.FindProperty("height");
        length     = serializedObject.FindProperty("length");
        angle      = serializedObject.FindProperty("angle");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawLockedField(lockHeight, height, "Height");
        DrawLockedField(lockLength, length, "Length");
        DrawLockedField(lockAngle,  angle,  "Angle");

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawLockedField(SerializedProperty lockProp, SerializedProperty valueProp, string label)
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUIUtility.labelWidth = 15f;
        EditorGUILayout.PropertyField(lockProp, new GUIContent(""), GUILayout.Width(30));

        EditorGUIUtility.labelWidth = 80f;
        using (new EditorGUI.DisabledScope(lockProp.boolValue))
            EditorGUILayout.PropertyField(valueProp, new GUIContent(label));

        EditorGUILayout.EndHorizontal();
        EditorGUIUtility.labelWidth = 0f;
    }
}