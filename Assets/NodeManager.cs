using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public NodeController nodePrefab;

    public List<NodeController> nodes { get; private set; }

    void Awake()
    {
        nodes = new List<NodeController>();
    }

    public void CreateFromPositionedNode(int num, PositionedNode pn)
    {
        NodeController node = Instantiate(nodePrefab, transform);
        node.Initialise(num, pn.position, pn.type);
        nodes.Add(node);
    }

    public void ResetAll()
    {
        nodes.ForEach(n => n.ResetState());
    }

    public void DestroyAll()
    {
        nodes.ForEach(n => Destroy(n));
	nodes.Clear();
    }

}
