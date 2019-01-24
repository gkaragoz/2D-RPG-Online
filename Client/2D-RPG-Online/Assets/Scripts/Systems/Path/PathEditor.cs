using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathEditor : MonoBehaviour {

    public enum Direction {
        Normal,
        Reversed
    }

    [Header("Initialization")]
    public bool isRectTransform = false;
    public Color rayColor = Color.white;
    public List<Transform> allPaths = new List<Transform>();

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private bool _reversed = false;
    [SerializeField]
    [Utils.ReadOnly]
    private Transform[] _array;

    private void Start() {
        _reversed = false;
    }

    private void OnDrawGizmos() {
        Gizmos.color = rayColor;

        FillPathList();

        for (int ii = 0; ii < allPaths.Count; ii++) {
            Vector3 currentPosition = allPaths[ii].position;
            if (ii > 0) {
                Vector3 previousPosition = allPaths[ii - 1].position;
                Gizmos.DrawLine(previousPosition, currentPosition);
                Gizmos.DrawWireSphere(currentPosition, 0.3f);
            }
        }
    }

    public Vector3 GetPathPoint(int index) {
        return allPaths[index].position;
    }

    public Vector3 GetRandomPathPoint() {
        return allPaths[Random.Range(0, allPaths.Count)].position;
    }

    public int GetRandomPathIndex() {
        return Random.Range(0, allPaths.Count);
    }

    public void SelectDirection(Direction direction) {
        switch (direction) {
            case Direction.Normal:
            _reversed = false;
            break;
            case Direction.Reversed:
            _reversed = true;
            break;
            default:
            _reversed = false;
            break;
        }

        FillPathList();
    }

    public void FillPathList() {
        if (isRectTransform) {
            _array = GetComponentsInChildren<RectTransform>();
        } else {
            _array = GetComponentsInChildren<Transform>();
        }

        allPaths = new List<Transform>();

        if (_reversed) {
            for (int ii = _array.Length - 1; ii >= 0; ii--) {
                if (_array[ii] != this.transform) {
                    allPaths.Add(_array[ii]);
                }
            }
        } else {
            foreach (Transform pathPoint in _array) {
                if (pathPoint != this.transform) {
                    allPaths.Add(pathPoint);
                }
            }
        }
    }

}

[CustomEditor(typeof(PathEditor))]
public class PathEditorDrawer : Editor {

    public override void OnInspectorGUI() {
        PathEditor script = (PathEditor)target;

        GUI.backgroundColor = Color.green;

        if (GUILayout.Button("Redraw Path")) {
            script.FillPathList();

            EditorWindow view = EditorWindow.GetWindow<SceneView>();
            view.Repaint();
        }

        base.OnInspectorGUI();
    }

}