using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class GraphGenerator
{
    public static bool debug;

    public static bool AttemptPosition(ref List<PositionedShape> shapes, int iterations, GridSystem grid, float blockerPortion)
    {
        for (int i = 0; i < iterations; i++)
        {
            foreach (var shape in shapes)
                shape.RepositionRandomlyInGrid(grid);

            List<Vector3[]> blockers = GenerateBlockers(shapes, blockerPortion);


            bool solvable = TestIfSolvable(shapes);
            if (solvable)
            {
                if (debug)
                    Debug.Log(i);
                return true;
            }
            grid.Reset();
        }
        if (debug) Debug.Log("Could not solve!");
        return false;
    }


    static List<Vector3[]> GenerateBlockers(List<PositionedShape> shapes, float portion)
    {

        List<Vector3[]> blockers = new List<Vector3[]>();

        foreach (var shapeGroup in shapes.GroupBy(s => s.originalType))
        {
            if (shapeGroup.Count() == 1)
            {
                // get pairs
                IEnumerable<IEnumerable<PositionedShape>> pairs = Extensions.GetPermutations(shapeGroup, 2);

                foreach (IEnumerable<PositionedShape> pair in pairs)
                {
                    var pairAsList = pair.ToArray();
                    PositionedShape shape0 = pairAsList[0];
                    PositionedShape shape1 = pairAsList[1];

                    // TODO
                    // GetLeftConnectingTriangles

                }

            }
        }


        return blockers;
    }

    static IEnumerable<Vector3[]> GetLeftConnectingTriangles(PositionedShape s0, PositionedShape s1)
    {
        IEnumerable<Vector3[]> nodePairs = Extensions
            .GetPermutations(s0.nodes, 2)
            .Select(p => p.ToArray());

        return s1.nodes
            .SelectMany(n => PairsAndOneToTriangles(nodePairs, n));
    }


    static IEnumerable<Vector3[]> PairsAndOneToTriangles(IEnumerable<Vector3[]> pairs, Vector3 one)
    {
        return pairs
            .Select(p => new Vector3[]{ p[0], p[1], one });
    }

    static bool TestIfSolvable(List<PositionedShape> shapes)
    {   
        IEnumerable<Vector3[]> tris = shapes.SelectMany(x => x.ToPositionedTriangles());

        // get pairs of triangles
        IEnumerable<IEnumerable<Vector3[]>> pairs = Extensions.GetPermutations(tris, 2);

        foreach (IEnumerable<Vector3[]> pair in pairs)
        {
            var pairList = pair.ToList();
            bool overlaps = TriTriOverlap.TriTriIntersect(
                pairList[0][0], // triangle 0, point 0
                pairList[0][1], // triangle 0, point 1
                pairList[0][2], // etc
                pairList[1][0],
                pairList[1][1],
                pairList[1][2]
            );

            if (debug)
            {
                float w = 3;
                Debug.DrawLine(pairList[0][0], pairList[0][1], Color.red, w);
                Debug.DrawLine(pairList[0][0], pairList[0][2], Color.red, w);
                Debug.DrawLine(pairList[0][1], pairList[0][2], Color.red, w);

                Debug.DrawLine(pairList[1][0], pairList[1][1], Color.blue, w);
                Debug.DrawLine(pairList[1][0], pairList[1][2], Color.blue, w);
                Debug.DrawLine(pairList[1][1], pairList[1][2], Color.blue, w);

            }

            if (overlaps)
                return false;
        }

        IEnumerable<Vector3[]> intersectors = shapes.SelectMany(x => x.ToPositionedIntersectors());

        // add some additional intersectors - lowers the number of solutions
        


        // now, at least one triangle must intersect with each intersector
        foreach (Vector3[] intersector in intersectors)
        {
            bool atLeastOneOverlap = false;
            foreach (Vector3[] tri in tris)
            {
                bool overlaps = TriTriOverlap.TriTriIntersect(
                    tri[0],
                    tri[1],
                    tri[2], 
                    intersector[0],
                    intersector[1],
                    intersector[2]
                );

                if (overlaps)
                {
                    atLeastOneOverlap = true;
                    break;
                }
            }
            // one of the intersectors didnt overlap, fail!
            if (!atLeastOneOverlap) return false;
        }


        // all the tests passed, it must be solvable
        return true;

    }

}


public class ShapeTopography
{
    public int numNodes;
    public int[,] triangles;
    public int[,] intersectors;

    static Dictionary<ShapeName, ShapeTopography> shapeMapping = new Dictionary<ShapeName, ShapeTopography>()
    {
        {
            ShapeName.Triangle, new ShapeTopography(
                3,
                new int[,] { { 0, 1, 2 } }
                )
        },
        {
            ShapeName.Square, new ShapeTopography(
                4,
                new int[,] { { 0, 1, 2 }, { 2, 3, 0 } }
                )
        },
        {
            ShapeName.Bow, new ShapeTopography(
                5,
                new int[,] { { 0, 1, 2 }, { 2, 3, 4 } }
                )
        },
        {
            ShapeName.Trapezuim, new ShapeTopography(
                5,
                new int[,] { { 0, 1, 2 }, { 1, 2, 3 }, { 2, 3, 4 } }
                )
        },
        {
            ShapeName.Chain, new ShapeTopography(
                7,
                new int[,] { { 0, 1, 2 }, { 2, 3, 4 }, { 4, 5, 6 } },
                new int[,] { { 1, 2, 3 }, { 3, 4, 5 } }
                )
        }
    };

    public ShapeTopography(int numNodes, int[,] triangles, int[,] intersectors = null)
    {
        if (intersectors == null) this.intersectors = new int[0,0];
        else this.intersectors = intersectors;

        this.numNodes = numNodes;
        this.triangles = triangles;
    }

    public static ShapeTopography FromShapeName(ShapeName shapeName)
    {
        return shapeMapping[shapeName];
    }

}


public class PositionedShape
{
    public int[,] triangles;
    public int[,] intersectors;
    public Vector3[] nodes;
    public NodeType[] nodeTypes;
    public NodeType originalType;

    public PositionedShape(ShapeTopography shape, NodeType nodeType)
    {
        originalType = nodeType;

        // initialise nodes
        nodes = new Vector3[shape.numNodes];
        triangles = shape.triangles;
        intersectors = shape.intersectors;
        this.nodeTypes = new NodeType[shape.numNodes];

        for (int i = 0; i < shape.numNodes; i++)
            this.nodeTypes[i] = nodeType;

    }


    public void RepositionRandomlyInGrid(GridSystem grid)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = grid.Sample();
        }
    }

    public List<Vector3[]> ToPositionedTriangles()
    {
        return Position(triangles);
    }

    public List<Vector3[]> ToPositionedIntersectors()
    {
        return Position(intersectors);
    }

    List<Vector3[]> Position(int[,] triangleSet)
    {
        int len = triangleSet.GetLength(0);

        List<Vector3[]> positionedTriangles = new List<Vector3[]>(len);

        for (int i = 0; i < len; i++)
        {
            positionedTriangles.Add(new Vector3[] {
                nodes[triangleSet[i, 0]],
                nodes[triangleSet[i, 1]],
                nodes[triangleSet[i, 2]]
            });
        }

        return positionedTriangles;
    }


    public void SetNodeTypeAt(int index, NodeType newNodeType)
    {
        nodeTypes[index] = newNodeType;
    }

}


[System.Serializable]
public class ShapeSpec
{
    public NodeType nodeType;
    public ShapeName shapeName;

}


public enum ShapeName
{
    Triangle, Square, Bow, Trapezuim, Chain
}
