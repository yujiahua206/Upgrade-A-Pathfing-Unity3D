using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
public class AStarPathFinding: MonoBehaviour {

	Grid grid;
	public bool ShowPathFindingTime;
	PathRequestManager requestManger;


	void Awake () {
		requestManger = GetComponent<PathRequestManager> ();
		//grid = GetComponent<Grid>();
	}


	public void StartFindPath(Vector3 startPos,Vector3 endPos){
		StartCoroutine (PathFinding (startPos, endPos));

	}
	IEnumerator PathFinding(Vector3 StartPos,Vector3 TargetPos) {
		Stopwatch sw = new Stopwatch ();
		sw.Start ();
		Vector3[] waypoints = new Vector3[0];
		bool PathSuccess = false;
		grid = GetComponent<Grid>();
		Node StartNode=grid.GetNodePositionFromWorld(StartPos);
		Node TargetNode = grid.GetNodePositionFromWorld (TargetPos);

		if (StartNode.walkable && TargetNode.walkable) {

			//List<Node> openList = new List<Node> ();
			Heap<Node> openList = new Heap<Node> (grid.MaxSize);
			HashSet<Node> closeList = new HashSet<Node> ();
			openList.Add (StartNode);

			while (openList.Count > 0) {
				//use loop to find the minium fCost instead of heap

				/*Node currentNode = openList [0];
			 
			for (int i = 1; i < openList.Count; i++) {
				if (openList [i].fCost < currentNode.fCost || (openList [i].fCost == currentNode.fCost && openList [i].hCost < currentNode.hCost)) {
					currentNode = openList [i];
				}

			}*/
				//Node currentNode = openList [0];
				//openList.Remove (currentNode);

				Node currentNode = openList.RemoveFirst ();
				closeList.Add (currentNode);

				if (currentNode == TargetNode) {
					if (ShowPathFindingTime) {
						sw.Stop ();
						print ("Path Found: " + sw.ElapsedMilliseconds + " ms");
					}
					PathSuccess = true;
 
					break;
				}

				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					if (!neighbour.walkable || closeList.Contains (neighbour)) {
						continue;
					} 

					int MoveCostToNeighbour = currentNode.gCost + GetMoveCost (currentNode, neighbour)+neighbour.movePenality;


					if (MoveCostToNeighbour < neighbour.gCost || !openList.Contains (neighbour)) {
						neighbour.gCost = MoveCostToNeighbour;
						neighbour.hCost = GetManHattanDiatance (neighbour, TargetNode);
						//neighbour.hCost = GetMoveCost (neighbour, TargetNode);
						neighbour.parent = currentNode;

						if (!openList.Contains (neighbour)) {
							openList.Add (neighbour);
						} else {
							openList.Update (neighbour);
						}
					}
				}
			}
			yield return null;
			if (PathSuccess) {
				waypoints = getPath (StartNode, TargetNode);
				closeList.Clear ();
			}
			requestManger.FinishedProcessPath (waypoints, PathSuccess);
		} else {
			print ("Start Node or End Node is unwalkable.");
		}
	}


	Vector3[] getPath(Node StartNode,Node TargetNode){
		List<Node> Path = new List<Node> ();
		Node currentNode = TargetNode;

		while (currentNode != StartNode) {
			//print (currentNode.worldPosition);
			Path.Add (currentNode);
			currentNode = currentNode.parent;
		}
		Vector3[] waypoints = SimplifyPath (Path);

		Array.Reverse (waypoints);

		return waypoints;
	}

	Vector3[] SimplifyPath(List<Node> path){
		List<Vector3> waypoints = new List<Vector3> ();
	

		for (int i = 1; i < path.Count; i++) {

			waypoints.Add (path [i].worldPosition);
		}
			
		return waypoints.ToArray();

	}

	int GetMoveCost(Node A,Node B){
		int distX = Mathf.Abs(A.gridX - B.gridX);
		int distY = Mathf.Abs(A.gridY - B.gridY);
		/*if (distX > distY) {
			return 14 * distY + 10 * (distX - distY);
		} else {
			return 14 * distX + 10 * (distY - distX);
		}*/

		return distX > distY ? 10 * (distX - distY) + 14 * (distY) : 10 * (distY - distX) + 14 * distX;
	}
	int GetManHattanDiatance(Node A,Node B){
		int distX = Mathf.Abs(A.gridX - B.gridX);
		int distY = Mathf.Abs(A.gridY - B.gridY);

		return distX + distY;
	}
}
