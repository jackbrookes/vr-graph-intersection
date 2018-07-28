using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeController : MonoBehaviour
{

	LineRenderer[] lineRenderers;

	public GameObject noFaceLine, faceLine;
	[Space]
    public NodeController startNode, endNode;


	public List<NodeController> nodes {
		get { return new List<NodeController>(){ startNode, endNode }; }
	}

	void Awake()
	{
		lineRenderers = GetComponentsInChildren<LineRenderer>();
	}

	public void Initialise(NodeController node1, NodeController node2)
	{
		if (node1 == node2)
		{
			Destroy(gameObject);
			throw new System.Exception("Can't create edge between two identical nodes");
		}

		foreach (var lineRenderer in lineRenderers)
		{
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(
                new Vector3[] {
                transform.InverseTransformPoint(node1.transform.position),
                transform.InverseTransformPoint(node2.transform.position)
                    });
        }

        startNode = node1;
		endNode = node2;
		name = string.Format("{0}-{1}", startNode.name, endNode.name);
	}


	public NodeController GetOther(NodeController node)
	{
		if (node == startNode)
			return endNode;
		else if (node == endNode)
			return startNode;
		else
			throw new System.Exception("Node doesn't exist for this edge");
	}
	

	public void SetEdgeState(EdgeState edgeState)
	{
		switch (edgeState)
		{
			case EdgeState.Face:
                faceLine.SetActive(true);
                noFaceLine.SetActive(false);
				break;
			case EdgeState.NoFace:
                faceLine.SetActive(false);
                noFaceLine.SetActive(true);
				break;
		}
	}


	/// <summary>
	/// Returns true if edge contains this node.
	/// </summary>
	/// <param name="node"></param>
	/// <returns></returns>
	public bool Contains(NodeController node)
	{
		return node == startNode || node == endNode;
	}

	/// <summary>
	/// Returns true if edge consists of both supplied nodes.
	/// </summary>
	/// <param name="node1"></param>
	/// <param name="node2"></param>
	/// <returns></returns>
	public bool ContainsBoth(NodeController node1, NodeController node2)
	{
		return (node1 == startNode && node2 == endNode) || (node1 == endNode && node2 == startNode);
	}



}


public enum EdgeState
{
	NoFace, Face
}