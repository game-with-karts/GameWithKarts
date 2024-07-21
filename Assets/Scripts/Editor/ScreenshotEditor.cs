using UnityEngine;
using UnityEditor;

// taken from
// https://kpprt.de/code-snippet/in-editor-screenshot-script-for-unity/

[CustomEditor(typeof(Screenshot))]
public class ScreenshotEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if(GUILayout.Button("Take Screenshot")) {
            ((Screenshot)serializedObject.targetObject).TakeScreenshot();
        }
    }
}