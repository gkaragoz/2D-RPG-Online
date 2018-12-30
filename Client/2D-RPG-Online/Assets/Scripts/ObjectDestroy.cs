using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible to destroy an object.
/// </summary>
public class ObjectDestroy : MonoBehaviour {

    /// <summary>
    /// Destroy a object after a particular time.
    /// </summary>
    /// <remarks>
    /// Default time is zero.
    /// </remarks>
    /// <param name="time"></param>
    public void Destroy(float time = 0f) {
        Destroy(this.gameObject, time);
    }

}
