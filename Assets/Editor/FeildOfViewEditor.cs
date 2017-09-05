using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FeildOfViewEditor : Editor {

	void OnSceneGUI(){
		FieldOfView fow = (FieldOfView)target;
		Handles.color = Color.white;
		Handles.DrawWireArc (fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius); 

		Handles.color = Color.red;

		foreach (Transform visiableTarget in fow.visibleTargets) {
			Handles.DrawLine (fow.transform.position, visiableTarget.position);
		}

	}
}
