using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "NodeType")]
public class NodeType : ScriptableObject
{

    public string typeName;
    public Color color;
    public Material overrideMaterial = null;
    public GameObject overrideModel = null;

}
