using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class GraphLogicManager : MonoBehaviour
{

    FaceCreator faceCreator;

    public UnityEvent onSolvedPuzzle;

    [Header("Special rules")]
    public NodeType blankNode;


    List<EdgeController> edges = new List<EdgeController>();

	void Awake()
	{
        faceCreator = GetComponentInChildren<FaceCreator>();
	}



    public void DestroyEdges()
    {
        edges.ForEach(e => Destroy(e));
        edges.Clear();
    }


    /// <summary>
    /// Assesses plausibility of a potential edge based on node types.
    /// </summary>
    /// <param name="node1"></param>
    /// <param name="node2"></param>
    /// <returns></returns>
    public bool IsValidEdge(NodeController node1, NodeController node2)
    {
        return (node1.nodeType == blankNode) ||
                (node2.nodeType == blankNode) ||
                (node1.nodeType == node2.nodeType);
    }


	public bool EdgeExists(NodeController node1, NodeController node2, out EdgeController foundEdge)
	{
        foundEdge = null;
        foreach (var edge in edges)
		{
            if (edge.ContainsBoth(node1, node2))
            {
                foundEdge = edge;
                return true;
            }
		}
		return false;
	}

    public void AddEdge(EdgeController newEdge)
    {
        edges.Add(newEdge);
    }


    public void UpdateEdgeStates(IEnumerable<EdgeController> edgesToUpdate)
    {
        foreach (EdgeController edge in edgesToUpdate)
        {
            // check if the edges are no longer connected to a face
            if (!faceCreator.EdgeInFaces(edge))
            {
                edge.SetEdgeState(EdgeState.NoFace);
            }
        }

    }

    public bool AssessEdgeIntersection(EdgeController newEdge)
    {
        return faceCreator.AssessEdgeIntersection(newEdge);
    }

    public List<NodeController[]> CheckForNewFaces(EdgeController newEdge, out bool edgeCreatesIntersection)
    {
        // Get all edges that contain either the start or end node from newEdge,
        // and is not this edge
        var connectedEdges = edges.Where(
            e => ((e != this) && (GetConnection(e, newEdge) != null))
            ).ToList();

        int n = connectedEdges.Count;
        // now check all pairings with the new edges

        edgeCreatesIntersection = false;

        List<NodeController[]> facesToCreate = new List<NodeController[]>();
        
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                List<EdgeController> potentialTriangle = new List<EdgeController>()
                {
                    newEdge,
                    connectedEdges[i],
                    connectedEdges[j]
                };

                // if they share exactly 3 nodes, the three edges form a triangle.
                List<NodeController> distinctNodes = DistinctNodesInSubGraph(potentialTriangle).ToList();
                Debug.LogFormat("Asessing triangle: {0}-{1}-{2}", distinctNodes.Select(node => node.name).ToArray());
                if (distinctNodes.Count() == 3)
                {
                    Debug.Log("Assessing triangle intersection");
                    // now we know the new edge created a new triangle, we must test if the new triangle would intersect with other existing triangles.
                    edgeCreatesIntersection = faceCreator.AssessFaceIntersection(
                        distinctNodes[0],
                        distinctNodes[1],
                        distinctNodes[2]
                        );

                    if (edgeCreatesIntersection)
                    {
                        // one intersection invalidates the edge
                        facesToCreate.Clear();
                        return facesToCreate;
                    }
                    else 
                    {

                        facesToCreate.Add(
                            new NodeController[]{
                                distinctNodes[0],
                                distinctNodes[1],
                                distinctNodes[2]
                            }
                        );

                        Debug.Log("Successful triangle");
                        
                        // continue to assess, since a single node can form multiple triangles

                    }
                }               

            }
        }

        if (facesToCreate.Count > 0)
        {
            foreach (var newFace in facesToCreate)
            {
                faceCreator.CreateFace(
                    newFace[0],
                    newFace[1],
                    newFace[2]
                    );
            }
        }

        return facesToCreate;
        
    }

    public void RemoveEdge(EdgeController edgeToRemove, out List<NodeController[]> removedFaces)
    {
        removedFaces = faceCreator.RemoveFacesByEdge(edgeToRemove);
        Destroy(edgeToRemove.gameObject);
        edges.Remove(edgeToRemove);
    }


    public IEnumerable<EdgeController> GetAllConnectedEdges(NodeController node)
    {
        return edges
            .Where(edge => edge.Contains(node));
    }

    public void CheckIfSolved()
    {
        
    }


    /// <summary>
    /// Gets a connecting node of two edges. If none, returns null.
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    public static NodeController GetConnection(EdgeController v1, EdgeController v2)
    {
        if (v1.startNode == v2.startNode)
            return v1.startNode;
        else if (v1.startNode == v2.endNode)
            return v1.startNode;
        else if (v1.endNode == v2.startNode)
            return v1.endNode;
        else if (v1.endNode == v2.endNode)
            return v1.endNode;
        else
            return null;
    }


    public static IEnumerable<NodeController> DistinctNodesInSubGraph(IEnumerable<EdgeController> subGraph)
    {
        return subGraph
            .SelectMany(v => v.nodes)
            .Distinct();
    }

    public IEnumerable<EdgeController> DistinctEdgesInTriangles(List<NodeController[]> triangles)
    {
        List<NodeController> allNodesInFaces;
        if (triangles.Count() == 1)
        {
            allNodesInFaces = triangles[0].ToList();
        }
        else
        {
            allNodesInFaces = triangles
                .SelectMany(nodes => nodes)
                .Distinct()
                .ToList();
        }

        var allEdges = allNodesInFaces
            .SelectMany(node => GetAllConnectedEdges(node));
        
        return allEdges
            .Distinct()
            .Where(edge => allNodesInFaces.Contains(edge.startNode) && allNodesInFaces.Contains(edge.endNode));   
    }

}
