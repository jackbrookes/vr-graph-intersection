using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "ProblemSpec")]
public class ProblemSpec : ScriptableObject
{   
    [Range(0, 10)]
    public int extraEdges = 1;
    [Range(0, 1)]
    public float blockerPortion = 0.5f;
    public NodeReplacementSpec replacements;
    public List<ShapeSpec> shapeSpecs = new List<ShapeSpec>();

    public List<PositionedShape> ToShapes()
    {
        if (shapeSpecs.Count == 0)
            Debug.LogError("0 shapes specified in problem specification!");

        var shapes = shapeSpecs.Select(
            s => new PositionedShape(
                ShapeTopography.FromShapeName(s.shapeName),
                s.nodeType
                )
        ).ToList();


        // generate all posible shape -> node indeces
        List<int[]> indeces = new List<int[]>();
        for (int i = 0; i < shapes.Count; i++)
        {
            for (int j = 0; j < shapes[i].nodeTypes.Length; j++)
            {
                indeces.Add(new int[]{i, j});
            }
        }

        // shuffle indeces
        indeces.Shuffle();

        // apply replacements
        for (int i = 0; i < replacements.numReplacements; i++)
        {
            int shapeIndex = indeces[i][0];
            int nodeIndex = indeces[i][1];

            var shape = shapes[shapeIndex];
            shape.nodeTypes[nodeIndex] = replacements.nodeType;
        }
        


        return shapes;
    
    }

}

[Serializable]
public struct NodeReplacementSpec
{
    public NodeType nodeType;
    public int numReplacements;
}