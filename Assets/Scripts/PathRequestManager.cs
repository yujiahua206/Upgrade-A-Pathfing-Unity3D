using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour {
	
	Queue<PathRequest> pathRequestsQuene = new Queue<PathRequest> ();
	PathRequest currentPathRequest;

	public static PathRequestManager  instance;

	AStarPathFinding pathfinding;
	bool isProcessingPath;


	void Awake(){
		instance = this;
		pathfinding = GetComponent<AStarPathFinding>();
	}


	public static void RequestPath(Vector3 pathStart,Vector3 pathEnd,Action<Vector3[], bool> callback){
		PathRequest newRequest = new PathRequest (pathStart, pathEnd, callback);
		instance.pathRequestsQuene.Enqueue (newRequest);
		instance.tryProcessNext ();
	}

	void tryProcessNext(){
		if (!isProcessingPath && pathRequestsQuene.Count > 0) {
			currentPathRequest = pathRequestsQuene.Dequeue ();
			isProcessingPath = true;
			pathfinding.StartFindPath (currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}
	public void FinishedProcessPath(Vector3[] path,bool success){
		currentPathRequest.callback (path, success);
		isProcessingPath = false;
		tryProcessNext ();
	}


	struct PathRequest{
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[],bool> callback;

		public PathRequest(Vector3 _start,Vector3 _end,Action<Vector3[],bool> _callback){
			pathStart=_start;
			pathEnd=_end;
			callback=_callback;
		}
	}
}
