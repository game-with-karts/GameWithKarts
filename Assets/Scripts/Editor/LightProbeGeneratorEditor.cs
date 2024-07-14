using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LightProbeGenerator))]
public class LightProbeGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Generate")) {
            (serializedObject.targetObject as LightProbeGenerator).Generate();
        }
    }
}