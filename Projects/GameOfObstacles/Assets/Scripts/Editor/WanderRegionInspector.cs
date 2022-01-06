using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Renders a wire cube representing the WanderRegion.
[CustomEditor(typeof(WanderRegion))]
public class WanderRegionInspector : Editor
{
    private WanderRegion Target
    {
        get
        {
            return (WanderRegion) target;
        }
    }
    private const float BoxHeight = 10f;
    
    void OnSceneGUI()
    {
        Handles.color = Color.white;
        Handles.DrawWireCube(Target.transform.position + (Vector3.up * BoxHeight * .5f), new Vector3(Target.size.x, BoxHeight, Target.size.z));
    }

}
