using System;
using UnityEngine;

public class Utils : MonoBehaviour {

    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class ReadOnlyAttribute : PropertyAttribute { }
#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : UnityEditor.PropertyDrawer {
        public override void OnGUI(Rect rect, UnityEditor.SerializedProperty prop, GUIContent label) {
            bool wasEnabled = GUI.enabled;
            GUI.enabled = false;
            UnityEditor.EditorGUI.PropertyField(rect, prop);
            GUI.enabled = wasEnabled;
        }
    }
#endif

    public static bool IsValid(params string[] value) {
        for (int ii = 0; ii < value.Length; ii++) {
            if (value[ii] == string.Empty || value[ii] == null || value[ii] == "NaN") {
                return false;
            }
        }
        return true;
    }

}
