using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible to has some utility function to help the game system.
/// </summary>
public class Utils : MonoBehaviour {

    /// <summary>
    /// This attribute is for making variables as a readOnly on inspector.
    /// </summary>
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

}
