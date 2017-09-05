using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour {

	public float viewRadius;
	public int viewAngle=360;
	public float meshResolution=10;
	public LayerMask obstacleMask;

	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();

	public MeshFilter viewMeshFilter;
	Mesh viewMesh;


	void Start(){
		viewMesh = new Mesh ();
		viewMesh.name="View Mesh";
		viewMeshFilter.mesh = viewMesh;
		StartCoroutine ("FindTargetWithDely", .2f);
	}

	void Update(){
		DrawFieldOfView ();
	}


	IEnumerator FindTargetWithDely(float dely){
		while(true){
			yield return new WaitForSeconds (dely);
			FindVisibleTargets ();
		}
	}



	void FindVisibleTargets(){
		visibleTargets.Clear ();
		Collider[] targetInViewRadius = Physics.OverlapSphere (transform.position, viewRadius, obstacleMask);

		for (int i = 0; i < targetInViewRadius.Length; i++) {
			Transform target = targetInViewRadius [i].transform;
			//Vector3 dirToTarget = (target.position - transform.position).normalized;
			//Vector3 dstToTarget = Vector3.Distance (transform.position, target.position);
			visibleTargets.Add(target);
		}
	}




	void DrawFieldOfView(){
		int stepCount = Mathf.RoundToInt (viewAngle * meshResolution);
		float stepAngleSize = viewAngle / stepCount;
		List<Vector3> viewPoints = new List<Vector3> ();
		for(int i=0;i<=stepCount;i++){
			float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
			ViewCastinfo newViewCast = ViewCast (angle);
			viewPoints.Add (newViewCast.point);
		}
		int vertexCount = viewPoints.Count;
		Vector3[] vertices=new Vector3[vertexCount];
		int[] trangles = new int[(vertexCount - 2) * 3];

		vertices [0] = Vector3.zero;

		for (int i = 0; i < vertexCount - 1; i++) {
			vertices [i + 1] = transform.InverseTransformPoint (viewPoints [i]);
			if (i < vertexCount - 2) {
				trangles [i * 3] = 0;
				trangles [i * 3 + 1] = i + 1;
				trangles [i * 3 + 2] = i + 2;
			}
		}

		viewMesh.Clear ();
		viewMesh.vertices = vertices;
		viewMesh.triangles = trangles;
		viewMesh.RecalculateNormals ();

	}
	ViewCastinfo ViewCast(float golableAngle){
		Vector3 dir = DirFromAngle (golableAngle, true);
		RaycastHit hit;

		if (Physics.Raycast (transform.position, dir, out hit, viewRadius, obstacleMask)) {
			return new ViewCastinfo (true, hit.point, hit.distance, golableAngle);
		} else {
			return new ViewCastinfo (false, transform.position + dir * viewRadius, viewRadius, golableAngle);
		}
	}


	public Vector3 DirFromAngle(float angleInDegrees,bool angleIsGolbal){
		if (!angleIsGolbal) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3 (Mathf.Sin (angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos (angleInDegrees * Mathf.Deg2Rad));
	}

	public struct ViewCastinfo{
		public bool hit;
		public Vector3 point;
		public float dst;
		public float angle;

		public ViewCastinfo(bool _hit,Vector3 _point,float _dst,float _angle){
			hit=_hit;
			point=_point;
			dst=_dst;
			angle=_angle;
		}
	}
}
