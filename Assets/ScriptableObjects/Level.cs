using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : ScriptableObject
{

	public PositionedNode[] nodes;
	public int maxEdges = 0;
    public List<Int3> solution;

}

[System.Serializable]
public struct PositionedNode
{
	public NodeType type;
	public Vector3 position;
}

[System.Serializable]
public class Int3
{
	[SerializeField]
    private int[] values;

	public Int3(int a, int b, int c)
	{
		values = new int[]{ a, b, c };
	} 

	public int a
	{
		get { return values[0]; }
		set { values[0] = value; }
	}

    public int b
    {
        get { return values[1]; }
        set { values[1] = value; }
    }

    public int c
    {
        get { return values[2]; }
        set { values[2] = value; }
    }

}