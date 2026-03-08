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

        DrawTrianglePreview();
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
    
    private void DrawTrianglePreview()
    {
        EditorGUILayout.Space(10);

        CameraPositionAngleCalculator calc = (CameraPositionAngleCalculator)target;

        // Box
        float padding = 30f;
        float boxSize  = EditorGUIUtility.currentViewWidth - 40f;
        Rect boxRect = EditorGUILayout.GetControlRect(false, boxSize);
        boxRect.width  = boxSize;
        GUI.Box(boxRect, GUIContent.none);

        if (Event.current.type != EventType.Repaint) return;

        // Usable area inside padding
        float innerX = boxRect.x + padding;
        float innerY = boxRect.y + padding;
        float innerW = boxRect.width - padding * 2;
        float innerH = boxRect.height - padding * 2;

        // Normalise height/length to fit inside the inner rect while preserving ratio
        float h = Mathf.Max(calc.height, 0.001f);
        float l = Mathf.Max(calc.length, 0.001f);
        float scale = Mathf.Min(innerW / l, innerH / h);

        float scaledHeight = h * scale;
        float scaledLength = l * scale;

        // Centre the triangle in the inner rect
        float offsetX = innerX + (innerW - scaledLength) / 2f;
        float offsetY = innerY + (innerH - scaledHeight) / 2f;

        // Corners — bottom-left = right angle, bottom-right = theta, top-left = apex
        Vector2 bottomLeft = new Vector2(offsetX, offsetY + scaledHeight);
        Vector2 bottomRight = new Vector2(offsetX + scaledLength, offsetY + scaledHeight);
        Vector2 topLeft = new Vector2(offsetX, offsetY);

        // Triangle edges
        Handles.color = Color.white;
        Handles.DrawLine(bottomLeft,bottomRight); // length (adjacent)
        Handles.DrawLine(bottomLeft,topLeft);     // height (opposite)
        Handles.DrawLine(topLeft,bottomRight); // hypotenuse

        // Right angle marker
        float markerSize = 10f;
        Handles.DrawLine(bottomLeft + new Vector2(markerSize, 0),bottomLeft + new Vector2(markerSize, -markerSize));
        Handles.DrawLine(bottomLeft + new Vector2(0, -markerSize),bottomLeft + new Vector2(markerSize, -markerSize));

        // Angle arc (bottom right)
        float arcRadius = Mathf.Min(scaledLength, scaledHeight) * 0.1f;
        arcRadius = Mathf.Clamp(arcRadius, 10f, 30f);
        Handles.color = Color.yellow;
        Handles.DrawWireArc(bottomRight, Vector3.forward, (bottomLeft - bottomRight).normalized, calc.angle, arcRadius);

        // Labels
        GUIStyle labelStyle = new GUIStyle(EditorStyles.label) { fontSize = 10, normal = { textColor = Color.white } };
        GUIStyle valueStyle = new GUIStyle(EditorStyles.label) { fontSize = 9,  normal = { textColor = Color.cyan  } };

        Vector2 midBottom = (bottomLeft + bottomRight) / 2f;
        Vector2 midLeft = (bottomLeft + topLeft) / 2f;

        Handles.Label(midBottom + new Vector2(-20, 8), $"length: {calc.length:F2}", valueStyle);
        Handles.Label(midLeft + new Vector2(-42, -6), $"height:\n{calc.height:F2}", valueStyle);
        Handles.Label(bottomRight + new Vector2(-25 - arcRadius, -12), $"{calc.angle:F1}°", valueStyle);
        Handles.Label(topLeft + new Vector2(-20, -12), "Camera", labelStyle);
        Handles.Label(bottomRight + new Vector2(-5, 12), "Player", labelStyle);
    }
}