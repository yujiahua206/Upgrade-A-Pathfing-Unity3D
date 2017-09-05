using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
	
	public LayerMask unWalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	public float nodeDiameter;
	public TerrainType[] walkableRegions;
	public Transform player;
	LayerMask walkableMask;
	Dictionary<int,int> walkregionsDictionary=new Dictionary<int,int>();

	public bool DisplayGrid;

	Node[,] grid; 

	int gridSizeX,gridSizeY;
	 
	void Awake(){
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

		foreach(TerrainType region in walkableRegions){
			walkableMask.value = walkableMask | region.terrianMask.value;
			walkregionsDictionary.Add ((int)(Mathf.Log (region.terrianMask.value, 2)), region.terrianPenalty);
			//print (walkableMask.value);
		}

		CreateGrid ();
	}

	public int MaxSize{
		get{ 
			return gridSizeX * gridSizeY;
		}
	}

	void CreateGrid(){
		grid = new Node[gridSizeX, gridSizeY];
		Vector3 worldBottonLeft = transform.position - Vector3.right * gridSizeX / 2 - Vector3.forward * gridSizeY / 2;

		for (int x = 0; x < gridSizeX; x++) {
			for (int y = 0; y < gridSizeY; y++) {
				Vector3 worldPoint = worldBottonLeft + Vector3.right * (x * nodeDiameter+nodeRadius) + Vector3.forward * (y * nodeDiameter+nodeRadius);
				bool walkable = !(Physics.CheckSphere (worldPoint, nodeRadius,unWalkableMask));
				int movementPenalty = 0;
				if (walkable) {
					Ray ray = new Ray (worldPoint + Vector3.up * 80, Vector3.down);
					RaycastHit hit;
					if (Physics.Raycast (ray, out hit, 100, walkableMask)) {
						walkregionsDictionary.TryGetValue (hit.collider.gameObject.layer, out movementPenalty);

						switch (movementPenalty) 
						{
						case 20:
							worldPoint.y = 8;
							break;
						case 30:
							worldPoint.y = 20;
							break;
						case 40:
							worldPoint.y = 35;
							break;
						case 50:
							worldPoint.y = 45;
							break;

						}

					}
				}
				grid [x, y] = new Node (walkable, worldPoint, x, y,movementPenalty);
			}
		}
	}


	public Node GetNodePositionFromWorld(Vector3 worldPosition){
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

		percentX = Mathf.Clamp01 (percentX);
		percentY = Mathf.Clamp01 (percentY);

		int x = Mathf.RoundToInt ((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt ((gridSizeY - 1) * percentY);

		return grid [x, y];
	}


	public List<Node> GetNeighbours(Node node){
		List<Node> neighbours = new List<Node> ();
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0) {
					continue;
				}
				int neighbourX = node.gridX + x;
				int neighbourY = node.gridY + y;
				if (neighbourX > 0 && neighbourX < gridSizeX && neighbourY > 0 && neighbourY < gridSizeY) {
					neighbours.Add (grid [neighbourX, neighbourY]);
				}
			}
		}

		return neighbours;
	}

	public List<Node> path = new List<Node> ();
	void OnDrawGizmos(){
		
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));

		if (grid != null) {

			if (DisplayGrid){
			
				foreach (Node n in grid) {
					Gizmos.color = (n.walkable) ? Color.white : Color.red; 
					Node playerNode = GetNodePositionFromWorld (player.position);
					if (playerNode == n) {
						Gizmos.color = Color.green;
					}
					if (path.Contains (n))
						Gizmos.color = Color.blue;
					if (n.worldPosition.x == -15)
						Debug.Log ("Cube Location" + n.worldPosition);
				
					Gizmos.DrawCube (n.worldPosition, (Vector3.one) * (nodeDiameter));
				}
			}
		}
	}

	[System.Serializable]
	public class TerrainType{
		public LayerMask terrianMask;
		public int terrianPenalty;
	}


}
