using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class MeshMerge : MonoBehaviour {

    public void BasicMerge() {
        Quaternion oldRot = transform.rotation;
        Vector3 oldPos = transform.position;

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();

        Debug.Log(name + " is combining " + filters.Length + " meshes!");

        Mesh finalMesh = new Mesh();

        CombineInstance[] combiners = new CombineInstance[filters.Length];

        for (int ii = 0; ii < filters.Length; ii++) {
            if (filters[ii].transform == transform) {
                continue;
            }

            combiners[ii].subMeshIndex = 0;
            combiners[ii].mesh = filters[ii].sharedMesh;
            combiners[ii].transform = filters[ii].transform.localToWorldMatrix;
        }

        finalMesh.CombineMeshes(combiners);

        GetComponent<MeshFilter>().sharedMesh = finalMesh;

        transform.rotation = oldRot;
        transform.position = oldPos;

        for (int ii = 0; ii < transform.childCount; ii++) {
            transform.GetChild(ii).gameObject.SetActive(false);
        }
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(MeshMerge))]
public class MeshMergeEditor : Editor {
    public MeshMerge meshMerge;

    public void OnEnable() {
        meshMerge = (MeshMerge)target;
    }

    public override void OnInspectorGUI() {
        GUI.backgroundColor = Color.green;
        GUILayout.Space(10f);
        GUILayout.Label("Mesh Merge");
        if (GUILayout.Button("Basic Merge")) {
            meshMerge.BasicMerge();
        }
        GUI.backgroundColor = Color.white;

        base.OnInspectorGUI();
    }
}
#endif
