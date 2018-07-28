using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.IO;

[ExecuteInEditMode]
public class GraphCreator : MonoBehaviour
{
    public ProblemSpec spec;

    [Header("Generate")]

	public string levelName = "level";

    [InspectorButton("Generate")]
    public bool _generate;


    [Header("Generate batch")]
    public int n = 10;
    public string levelNameFormat = "{0}";
    public string levelSetName = "level_set";

    [InspectorButton("GenerateMany")]
    public bool _generateMany;
    
    void Awake()
    {
        
    }


    public void Generate()
    {
        GridSystem grid = new GridSystem();

        List<PositionedShape> shapes = spec.ToShapes();

        // run solving algorithm
        bool solvable = GraphGenerator.AttemptPosition(ref shapes, 50000, grid, spec.blockerPortion);

        if (solvable)
        {
            MakeLevelFromShapes(shapes, spec.extraEdges, levelName);
        }

    }

    public void GenerateMany()
    {
        GridSystem grid = new GridSystem();

        List<PositionedShape> shapes = spec.ToShapes();
        
        LevelSet set = new LevelSet();

        for (int i = 0; i < n; i++)
        {
            // run solving algorithm
            bool solvable = GraphGenerator.AttemptPosition(ref shapes, 50000, grid, spec.blockerPortion);

            if (solvable)
            {
                Level level = MakeLevelFromShapes(shapes, spec.extraEdges, string.Format(levelNameFormat, i));
                set.levels.Add(level);
            }
        }

        string assetPath = Path.Combine("Assets/Objects/LevelSets", string.Format("{0}.asset", levelSetName));
        AssetDatabase.CreateAsset(set, assetPath);

    }


	public static Level MakeLevelFromShapes(List<PositionedShape> positionedShapes, int extraEdges, string levelName)
	{
        // initialise triangles

        int numTriangles = positionedShapes
            .Select(s => s.triangles.GetLength(0))
            .Sum();

        List<Int3> solution = new List<Int3>();

		// initialise nodes

        int numNodes = positionedShapes
			.Select(s => s.nodes.Length)
			.Sum();

        PositionedNode[] nodes = new PositionedNode[numNodes];

        int nodeNum = 0;
        int edges = 0;
		foreach (var shape in positionedShapes)
		{

            // generate the solution from triangles given in shapes
            for (int i = 0; i < shape.triangles.GetLength(0); i++)
            {
                solution.Add(
                    new Int3
                    (
                        shape.triangles[i, 0] + nodeNum,
                        shape.triangles[i, 1] + nodeNum,
                        shape.triangles[i, 2] + nodeNum
                    )
                );

                edges += 3;
            }

			// node creation
			for (int j = 0; j < shape.nodes.Length; j++)
			{
                
                PositionedNode node = new PositionedNode {
                    type = shape.nodeTypes[j],
                    position = shape.nodes[j]
                };

                nodes[nodeNum++] = node;
			}

            
		}
        
        Level level = new Level();
        level.nodes = nodes;
        level.maxEdges = edges + extraEdges;
        level.solution = solution;
        
        string assetPath = Path.Combine("Assets/Objects/Levels", string.Format("{0}.asset", levelName));
        AssetDatabase.CreateAsset(level, assetPath);

        return level;
    }

}
