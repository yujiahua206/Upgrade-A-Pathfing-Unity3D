using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class Units : MonoBehaviour {

	public Transform target,target1;
	public float speed;
	Vector3[] path;
	int targetIndex;

	const float UpdatePathTheshold=0.0f;
	const float PathUpdateTime = 0.5f;

	public bool DisplayPath;

	float timeSpent;

	void Start(){
		StartCoroutine (GenerateNewpath());
	}

	void Update(){

		timeSpent += Time.deltaTime;
		string line = transform.position.x + "," + transform.position.z + "," + transform.position.y;
		//print (line);

		if (timeSpent>0.2) {
			WritePathToFile ("path.txt", line, true);
			timeSpent = 0;
		}

	}


	IEnumerator GenerateNewpath(){
		if (Time.timeSinceLevelLoad < 0.3f) {
			yield return new WaitForSeconds (.3f);
		}

		PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);
		Vector3 OldTargetPosition = target.position;
		while (true) {
			yield return new WaitForSeconds (PathUpdateTime);

			/*if ((transform.position-target.position).magnitude<5.0) {
				PathRequestManager.RequestPath (transform.position, target1.position, OnPathFound);
			}*/

			if ((target.position - OldTargetPosition).sqrMagnitude > UpdatePathTheshold) {
				PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);
				OldTargetPosition = target.position;
			}
		}
	}

	public void OnPathFound(Vector3[] newPath,bool pathSuccessful){
		if (pathSuccessful) {
			path = newPath;
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");

		}
		//path [path.Length - 1] = target.position;
	}

	public  void WritePathToFile(string FileName,string Line,bool ifappend){


		StreamWriter sw=new StreamWriter(FileName,ifappend);
		sw.WriteLine(Line);
		sw.Flush();
		sw.Close();
	}

	IEnumerator FollowPath(){
		Vector3 currentWaypoint = path [0];
		timeSpent += Time.deltaTime;

		while (true) {
			if (transform.position == currentWaypoint) {
				targetIndex++;
				if (targetIndex >= path.Length) {
					yield break;
				}
				currentWaypoint = path [targetIndex];
			}
			transform.position = Vector3.MoveTowards (transform.position, currentWaypoint, speed*Time.deltaTime);
		
			//print (transform.position);
			yield return null;
		}
	}

	public void OnDrawGizmos(){
		if (path != null&&DisplayPath) {
			for (int i = targetIndex; i < path.Length; i++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube (path[i], new Vector3(5,5,5));
				if (i == targetIndex) {
					Gizmos.DrawLine (transform.position, path[i]);
				} else {
					Gizmos.DrawLine (path[i-1], path [i]);
				}
			}
		}
	}
}
