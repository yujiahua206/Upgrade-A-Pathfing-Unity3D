 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>{

	public bool walkable;
	public Vector3 worldPosition;
	public int gCost;
	public int hCost;
	public int gridX, gridY;
	public int movePenality;
	public Node parent;
	int heapIndex;

	public Node(bool _walkable, Vector3 _worldPosition,int _gridX,int _gridY,int _Penality){
	
		walkable = _walkable;
		worldPosition = _worldPosition;
		gridX = _gridX;
		gridY = _gridY;
		movePenality = _Penality;

	}


	public int fCost{
		get{
			return gCost + hCost;
		}
	}
	public int HeapIndex{
		get{ 
			return heapIndex;
		}
		set{ 
			heapIndex = value;
		}
	}

	public  int CompareTo(Node other){
		int result = fCost.CompareTo (other.fCost);
		if (result == 0) {
			result = hCost.CompareTo (other.hCost);
		}

		return -result;
	}

}
