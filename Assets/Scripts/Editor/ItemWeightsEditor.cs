using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemWeights))]
public class ItemWeightsEditor : Editor {
    ItemWeights weights;

    public void OnEnable() {
        weights = (ItemWeights)target;
    }

    public override void OnInspectorGUI() {
        GUILayoutOption option = GUILayout.Width(35);

        EditorGUILayout.BeginHorizontal(); {
            GUILayout.Label("Item", GUILayout.ExpandWidth(true));
            GUILayout.Label("1st", option);
            GUILayout.Label("2nd", option);
            GUILayout.Label("3rd", option);
            GUILayout.Label("4th", option);
            GUILayout.Label("5th", option);
            GUILayout.Label("6th", option);
            GUILayout.Label("7th", option);
            GUILayout.Label("8th", option);
        }
        EditorGUILayout.EndHorizontal();

        DrawUILine(Color.black);

        for (int i = 0; i < weights.records.Length; i++) {
            ItemWeightsRecord r = weights.records[i];
            EditorGUILayout.BeginHorizontal(); {
                GUILayout.Label(r.Name, GUILayout.ExpandWidth(true));
                r.firstPlace = EditorGUILayout.IntField(r.firstPlace, option);
                r.secondPlace = EditorGUILayout.IntField(r.secondPlace, option);
                r.thirdPlace = EditorGUILayout.IntField(r.thirdPlace, option);
                r.fourthPlace = EditorGUILayout.IntField(r.fourthPlace, option);
                r.fifthPlace = EditorGUILayout.IntField(r.fifthPlace, option);
                r.sixthPlace = EditorGUILayout.IntField(r.sixthPlace, option);
                r.seventhPlace = EditorGUILayout.IntField(r.seventhPlace, option);
                r.eighthPlace = EditorGUILayout.IntField(r.eighthPlace, option);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorUtility.SetDirty(weights);

    }

    public static void DrawUILine(Color color, int thickness = 2, int padding = 10) {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
        r.height = thickness;
        r.y += padding/2;
        r.x -= 2;
        EditorGUI.DrawRect(r, color);
    }

}