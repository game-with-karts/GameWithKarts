using UnityEngine;
using UnityEditor;
using System;

public class EditorUIElements {
    private static UnityEngine.Object LoadPrefabFromFile(string filename) {
        UnityEngine.Object loadedObject = AssetDatabase.LoadAssetAtPath(filename, typeof(GameObject));
        if (loadedObject == null) {
            throw new Exception("Prefab not found. Make sure it's in the Assets/Prefabs/UI Elements folder and check the file name.");
        }
        return loadedObject;
    }

    private static void Add(string name) {
        string assetPath = $"Assets/Prefabs/UI Elements/{name}.prefab";
        UnityEngine.Object prefab = LoadPrefabFromFile(assetPath);
        Selection.activeObject = PrefabUtility.InstantiatePrefab(prefab as GameObject);
    }

    [MenuItem("GameObject/GWK UI/Button", false, 2)] static void AddButton() => Add("Button");

    [MenuItem("GameObject/GWK UI/Checkbox", false, 2)] static void AddCheckbox() => Add("Checkbox");

    [MenuItem("GameObject/GWK UI/Choicebox", false, 2)] static void AddChoicebox() => Add("Choicebox");

    [MenuItem("GameObject/GWK UI/Number Input", false, 2)] static void AddNumberInputBox() => Add("NumberInputBox");

    [MenuItem("GameObject/GWK UI/Slider", false, 2)] static void AddSlider() => Add("Slider");
    
    [MenuItem("GameObject/GWK UI/Slideshow", false, 2)] static void AddSlideshow() => Add("Slideshow");
}