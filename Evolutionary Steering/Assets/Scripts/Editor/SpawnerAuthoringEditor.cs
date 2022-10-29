using UnityEditor;

[CustomEditor(typeof(PositionFactoryData))]
public class PositionFactoryDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var thisObject = target as PositionFactoryData;

        DisplayCommonSettings();

        switch (thisObject.factoryType)
        {
            case PositionFactoryEnum.Square: DisplaySquareSettings(); break;
            case PositionFactoryEnum.Circle: DisplayCircleSettings(); break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayCommonSettings()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("factoryType"));
    }

    private void DisplaySquareSettings()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("bounds"));
    }

    private void DisplayCircleSettings()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxRadius"));
    }
}
