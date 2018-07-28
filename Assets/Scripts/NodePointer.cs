using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NodePointer : MonoBehaviour
{

    [SerializeField]
    NodeController previouslyPointedNode;
    
    [SerializeField]
    NodeController previouslySelectedNode;

    [Header("References")]
    public EdgeCreator edgeCreator;
    public GraphLogicManager graphLogicManager;

    PointerFeedack feedback;

    void Awake()
    {
        feedback = GetComponent<PointerFeedack>();
    }


    public void RegisterPoint(NodeController newlyPointedNode)
    {
        if (newlyPointedNode != previouslyPointedNode)
        {

            if (previouslyPointedNode != null)
            {
                previouslyPointedNode.OnDePointed();
                edgeCreator.DePointNode();
            }

            if (newlyPointedNode != null)
            {
                Debug.LogFormat("Pointed at {0}", newlyPointedNode.name);
                newlyPointedNode.OnPointed();
                edgeCreator.PointNode(newlyPointedNode);
            }               
   
        }

        previouslyPointedNode = newlyPointedNode;
    
    }


    public void RegisterSelect(NodeController newlySelectedNode)
    {
        if (newlySelectedNode != null)
        {
            newlySelectedNode.OnSelected();
            edgeCreator.SelectNode(newlySelectedNode);
        }

        previouslySelectedNode = newlySelectedNode;
    }

    public void RegisterJoin(NodeController joinNode)
    {
        if (joinNode != previouslySelectedNode && joinNode != null)
        {
            object outcomeData;
            Outcome outcome = edgeCreator.AttemptModification(joinNode, out outcomeData);
            switch (outcome)
            {
                case Outcome.CreatedEdgeAndFaces:

                    object[] edgeAndFaces = (object[]) outcomeData;

                    EdgeController createdEdge = (EdgeController) edgeAndFaces[0];
                    List<NodeController[]> createdFaces = (List<NodeController[]>) edgeAndFaces[1];

                    createdEdge.nodes
                        .ForEach(n => n.OnCreateEdge());

                    createdFaces
                        .SelectMany(face => face)
                        .ToList()
                        .ForEach(node => node.OnCreateFace());

                    var distinctEdges = graphLogicManager.DistinctEdgesInTriangles(createdFaces).ToList();
                    distinctEdges.ForEach(e => e.SetEdgeState(EdgeState.Face));

                    break;
                case Outcome.CreatedEdge:
                    EdgeController newEdge = (EdgeController) outcomeData;
                    joinNode.OnCreateEdge();
                    previouslySelectedNode.OnCreateEdge();
                    newEdge.SetEdgeState(EdgeState.NoFace);
                    break;
                case Outcome.RemovedEdgeAndFaces:

                    object[] removedEdgesAndFaces = (object[]) outcomeData;

                    EdgeController removedEdge = (EdgeController) removedEdgesAndFaces[0];
                    List<NodeController[]> removedFaces = (List<NodeController[]>) removedEdgesAndFaces[1];

                    removedFaces
                        .SelectMany(face => face)
                        .ToList()
                        .ForEach(node => node.OnRemoveFace());

                    removedEdge.nodes
                        .ForEach(n => n.OnRemoveEdge());

                    IEnumerable<EdgeController> edgesToUpdate = graphLogicManager.DistinctEdgesInTriangles(removedFaces);

                    graphLogicManager.UpdateEdgeStates(edgesToUpdate);

                    break;
                case Outcome.RemovedEdge:
                    joinNode.OnRemoveEdge();
                    previouslySelectedNode.OnRemoveEdge();
                    break;
                case Outcome.IntersectingFace:
                case Outcome.IntersectingEdge:
                    feedback.FeedbackIntersectingFace();
                    break;
                case Outcome.InvalidNodePair:
                    feedback.FeedbackInvalidPair();
                    break;
            }
        }
        RegisterDeSelect();
        edgeCreator.DePointNode();
    }


    
    public void RegisterDeSelect()
    {
        if (previouslySelectedNode != null)
        {
            edgeCreator.CancelCreation();
            previouslySelectedNode.OnDeSelected();
        }
        
        previouslySelectedNode = null;
    }

}
