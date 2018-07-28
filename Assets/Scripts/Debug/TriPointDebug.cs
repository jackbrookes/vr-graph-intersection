using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TriPointDebug : MonoBehaviour
{

	public Transform p0, p1, p2;

	public Transform e0, e1;

	Vector3? intersectPoint;

	public bool intersect;

    // Update is called once per frame
    void Update()
    {
		// tri
		Debug.DrawLine(p0.position, p1.position, Color.red);
        Debug.DrawLine(p0.position, p2.position, Color.red);
        Debug.DrawLine(p1.position, p2.position, Color.red);


        // edge
        Debug.DrawLine(e0.position, e1.position, Color.blue);

		// algorithm
		intersect = TriTriOverlap.TriEdgeIntersect(p0.position, p1.position, p2.position, e0.position, e1.position, out intersectPoint);

    }


	void OnDrawGizmos()
	{
		if (intersectPoint != null)
		{
			// intersect
			Gizmos.color = Color.cyan;
			Gizmos.DrawSphere((Vector3) intersectPoint, .01f);
		}
	}



}
