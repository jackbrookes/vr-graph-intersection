using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FaceCreator : MonoBehaviour
{
    Mesh mesh;

    public List<NodeController[]> currentTriangles = new List<NodeController[]>();

    // Use this for initialization
    void Awake()
    {
        // set mesh to a new, empty mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void ResetFaces()
    {
        InitialiseFaces(new List<NodeController>());
    }

    public void InitialiseFaces(IEnumerable<NodeController> nodes)
    {
        currentTriangles.Clear();

        mesh.vertices = nodes
            .Select(node => node.transform.localPosition)
            .ToArray();
    }


    public void CreateFace(NodeController node1, NodeController node2, NodeController node3)
    {
        currentTriangles.Add(
            new NodeController[] {
                node1, node2, node3
            }
        );

        // stored as int array, 3 consecutive vertices encode a triangle
        var newTriangles = new int[currentTriangles.Count * 6];

        // keep old triangles
        mesh.triangles.CopyTo(newTriangles, 0);

        // clockwise and counter clockwise - double sided triangle
        new int[] { node1.num, node2.num, node3.num, node1.num, node3.num, node2.num }
            .CopyTo(newTriangles, mesh.triangles.Length);

        mesh.triangles = newTriangles;
    }

    public void RemoveFace(NodeController[] triangle)
    {
        int triIndex = currentTriangles.IndexOf(triangle);
        currentTriangles.RemoveAt(triIndex);

        int nodeIndex = triIndex * 6;

        var tempTriangles = mesh.triangles.ToList();
        // remove 6 consecutive nodes from the mesh.triangles array
        tempTriangles
            .RemoveRange(nodeIndex, 6);
        
        mesh.triangles = tempTriangles.ToArray();
    }


    /// <summary>
    /// Removes all faces associated with an edge
    /// </summary>
    /// <param name="edge"></param>
    public List<NodeController[]> RemoveFacesByEdge(EdgeController edge)
    {
        List<NodeController[]> trianglesToRemove = currentTriangles
            .Clone()
            .Where(tri => TriangleContainsEdge(tri, edge))
            .ToList();

        trianglesToRemove
            .ForEach(tri => RemoveFace(tri));

        return trianglesToRemove;
    }




    public bool AssessEdgeIntersection(EdgeController edge)
    {        
        foreach (var tri in currentTriangles)
        {

            Vector3 centroid = (
            tri[0].transform.position +
            tri[1].transform.position +
            tri[2].transform.position) / 3f;

            // shift all points by an epsilon inwards, to make sure points aren't erronously said to 
            // overlap due to floating point errors.

            Vector3 pos1 = Vector3.MoveTowards(
                tri[0].transform.position, centroid, .01f
            );

            Vector3 pos2 = Vector3.MoveTowards(
                tri[1].transform.position, centroid, .01f
            );

            Vector3 pos3 = Vector3.MoveTowards(
                tri[2].transform.position, centroid, .01f
            );

            Vector3? intersectionPoint = null;
            bool intersects = TriTriOverlap.TriEdgeIntersect(
                pos1,
                pos2,
                pos3,
                edge.startNode.transform.position,
                edge.endNode.transform.position,
                out intersectionPoint
            );

            if (intersects)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Returns true if triangle created from three nodes intersects with any other triangles 
    /// </summary>
    /// <param name="node1"></param>
    /// <param name="node2"></param>
    /// <param name="node3"></param>
    /// <returns></returns>
    public bool AssessFaceIntersection(NodeController node1, NodeController node2, NodeController node3)
    {

        Vector3 centroid = (
            node1.transform.position +
            node2.transform.position +
            node3.transform.position) / 3f;

        // shift all points by an epsilon inwards, to make sure points aren't erronously said to 
        // overlap due to floating point errors.
        
        Vector3 pos1 = Vector3.MoveTowards(
            node1.transform.position, centroid, .01f
        );

        Vector3 pos2 = Vector3.MoveTowards(
            node2.transform.position, centroid, .01f
        );

        Vector3 pos3 = Vector3.MoveTowards(
            node3.transform.position, centroid, .01f
        );


        foreach (var triangle in currentTriangles)
        {
            bool intersects = TriTriOverlap.TriTriIntersect(
                pos1,
                pos2,
                pos3,
                triangle[0].transform.position,
                triangle[1].transform.position,
                triangle[2].transform.position
            );

            if (intersects)
                return true;
        }

        return false;
    }

    public bool EdgeInFaces(EdgeController edge)
    {
        foreach (var tri in currentTriangles)
        {
            if (edge.ContainsBoth(tri[0], tri[1]) ||
                edge.ContainsBoth(tri[0], tri[2]) ||
                edge.ContainsBoth(tri[1], tri[2]))
                return true;
        }
        return false;
    }


    public static bool TriangleContainsEdge(NodeController[] triangle, EdgeController edge)
    {
        if (edge.ContainsBoth(triangle[0], triangle[1]))
            return true;

        if (edge.ContainsBoth(triangle[0], triangle[2]))
            return true;

        if (edge.ContainsBoth(triangle[1], triangle[2]))
            return true;

        return false;
    }

}


