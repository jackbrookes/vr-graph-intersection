using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EdgeCreator : MonoBehaviour
{
    public EdgeController edgePrefab;
    GraphLogicManager graphLogicManager;
    FaceCreator faceCreator;

    public Transform graph;
    public Transform edgesTransform;
    LineRenderer lineRenderer;
    NodeController startNode;
    EdgeCounter counter;
    
    void Awake()
    {
        faceCreator = graph.GetComponentInChildren<FaceCreator>();
        graphLogicManager = graph.GetComponentInChildren<GraphLogicManager>();
        lineRenderer = GetComponent<LineRenderer>();
        counter = GetComponent<EdgeCounter>();
    }
    void Start()
    {
        CancelGhostEdge();
    }


    void LateUpdate()
    {
        if (creating)
            lineRenderer.material.mainTextureOffset = new Vector2(Time.time / 5f, 0);
    }

    public bool creating
    {
        get { return startNode != null; }
    }



    public void PointNode(NodeController pointedNode)
    {
        if (creating)
        {
            bool isValid = graphLogicManager.IsValidEdge(startNode, pointedNode);
            if (isValid)
            {
                DrawGhostEdge(startNode, pointedNode);
            }
        }
    }

    public void DePointNode()
    {
        CancelGhostEdge();
    }

    void DrawGhostEdge(NodeController node1, NodeController node2)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(
            new Vector3[] {
                node1.transform.position,
                node2.transform.position
             });
    }

    void CancelGhostEdge()
    {
        // set to empty array
        lineRenderer.positionCount = 0;
        lineRenderer.SetPositions(new Vector3[] { });
    }

    public void SelectNode(NodeController selectedNode)
    {
        if (!creating)
        {
            startNode = selectedNode;
        }
    }

	/// <summary>
	/// Modified the graph, creates or removes edge if valid. 
	/// </summary>
	/// <param name="joinNode">New node we are using to modify</param>
    public Outcome AttemptModification(NodeController joinNode, out object outcomeData)
    {   
        outcomeData = null;
        if (!creating)
			return Outcome.None;
			
		EdgeController foundEdge;
		if (graphLogicManager.EdgeExists(startNode, joinNode, out foundEdge))
		{
            Debug.LogFormat("Removing edge between {0} and {1}", startNode.name, joinNode.name);
            List<NodeController[]> removedFaces;
            graphLogicManager.RemoveEdge(foundEdge, out removedFaces);
            counter.EdgeCountChange(-1);
            if (removedFaces.Count > 0)
            {
                outcomeData = new object[] { foundEdge, removedFaces } ;
                return Outcome.RemovedEdgeAndFaces;
            }
            else
            {
                return Outcome.RemovedEdge;
            }
		}

        if (counter.remaining <= 0)
        {
            counter.OnCreateWithNoneRemaining();
            return Outcome.NoEdgesRemaining;
        }

		if (graphLogicManager.IsValidEdge(startNode, joinNode))
		{
            Debug.LogFormat("Creating edge between {0} and {1}", startNode.name, joinNode.name);

            EdgeController newEdge = Create(startNode, joinNode);

            // first check if this edge intersects a face directly
            bool edgeCreatesEdgeIntersection;
            edgeCreatesEdgeIntersection = graphLogicManager.AssessEdgeIntersection(newEdge);

            if (edgeCreatesEdgeIntersection)
            {
                Debug.Log("Edge invalid! Creates edge intersection.");
                Destroy(newEdge.gameObject);
                return Outcome.IntersectingEdge;
            }

            // if it doesnt, check if it creates a face which creates an intersection
            bool edgeCreatesFaceIntersection;
            var newFaces = graphLogicManager.CheckForNewFaces(newEdge, out edgeCreatesFaceIntersection);

            if (edgeCreatesFaceIntersection)
            {
                Debug.Log("Edge invalid! Creates face intersection.");
                Destroy(newEdge.gameObject);
                return Outcome.IntersectingFace;
            }
            else
            {
                Debug.Log("Adding edge to graph");
                graphLogicManager.AddEdge(newEdge);
                counter.EdgeCountChange(1);     
                if (newFaces.Count > 0)
                {
                    outcomeData = new object[]{ newEdge, newFaces };
                    return Outcome.CreatedEdgeAndFaces;
                }
                else
                {
                    outcomeData = newEdge;
                    return Outcome.CreatedEdge;
                }
            }
		}
        else
        {
            return Outcome.InvalidNodePair;
        }
    }


    public EdgeController Create(NodeController n0, NodeController n1)
    {
        Vector3 midPoint = (n0.transform.position + n1.transform.position) / 2f;
        EdgeController newEdge = Instantiate(edgePrefab, midPoint, Quaternion.identity, edgesTransform);
        newEdge.Initialise(n0, n1);
        return newEdge;
    }

    public void CancelCreation()
    {
        startNode = null;
    }

}


public enum Outcome
{
    None, CreatedEdge, CreatedEdgeAndFaces, RemovedEdge, RemovedEdgeAndFaces, IntersectingFace, IntersectingEdge, InvalidNodePair, NoEdgesRemaining
}