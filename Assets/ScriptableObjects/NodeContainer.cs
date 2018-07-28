using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "NodeContainer")]
public class NodeContainer : ScriptableObject
{

    public List<NodeType> nodeTypes;

}
