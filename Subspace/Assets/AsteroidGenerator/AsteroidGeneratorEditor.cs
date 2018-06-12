using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AsteroidGenerator))]
public class AsteroidGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AsteroidGenerator myScript = (AsteroidGenerator)target;
        if (GUILayout.Button("Generator Asteroid"))
        {
            myScript.GenerateMeshInEditor();
        }

        if (GUILayout.Button("Save Asteroid"))
        {
            myScript.SaveAsteroid();
        }
    }
}
