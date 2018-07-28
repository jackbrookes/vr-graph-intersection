using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	public FaceCreator faceCreator;
	public GraphLogicManager graph;
	public EdgeCreator edgeCreator;
	public NodeManager nodeManager;
	public EdgeCounter counter;

	Level loadedLevel;

	[InspectorButton("Solve")]
	public bool _solve;

	public void LoadLevel(Level level)
	{
        loadedLevel = level;		
		int i = 0;

		foreach (PositionedNode posNode in level.nodes)
		{
            nodeManager.CreateFromPositionedNode(i++, posNode);
		}

        faceCreator.InitialiseFaces(nodeManager.nodes);
        counter.ResetCounter(level.maxEdges);
	}

    public void DestroyLevel()
	{
		faceCreator.ResetFaces();
		graph.DestroyEdges();
		nodeManager.DestroyAll();		
	}


	public void Solve()
	{
		if (loadedLevel != null)
        {
            graph.DestroyEdges();
            nodeManager.ResetAll();

			int numSolutionTris = loadedLevel.solution.Count;

			foreach (Int3 face in loadedLevel.solution)
			{
                int v0 = face.a;
                int v1 = face.b;
                int v2 = face.c;

				NodeController n0 = nodeManager.nodes[v0];
                NodeController n1 = nodeManager.nodes[v1];
                NodeController n2 = nodeManager.nodes[v2];

				
				EdgeController edge01 = edgeCreator.Create(n0, n1);
                EdgeController edge02 = edgeCreator.Create(n0, n2);
				EdgeController edge12 = edgeCreator.Create(n1, n2);
				
				graph.AddEdge(edge01);
                graph.AddEdge(edge02);
                graph.AddEdge(edge12);
            }

        }
	}

}
