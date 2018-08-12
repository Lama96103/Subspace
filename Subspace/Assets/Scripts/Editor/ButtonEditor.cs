using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(ButtonController))]
    public class ButtonEditor : Editor
    {
        bool showRpcs = false;
        private void OnEnable()
        {
            ButtonController button = (ButtonController)target;

        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            showRpcs = EditorGUILayout.Foldout(showRpcs, "RPCs List:");

            EditorGUILayout.Space();

            if (showRpcs)
            {

                List<string> myList = PhotonNetwork.PhotonServerSettings.RpcList;

                for (int i = 0; i < myList.Count; i++)
                {
                    EditorGUILayout.TextField(i.ToString(), myList[i]);
                }
            }
        }

    }

}