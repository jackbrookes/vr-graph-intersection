using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EdgeCounter : MonoBehaviour
{

	public GraphLogicManager graphLogicManager; 
	public Text countText;

	int _maxEdges;
	public int maxEdges { get; private set; }

	public int count { get; private set;}

	public int remaining { get { return maxEdges - count; } }

	public void EdgeCountChange(int change)
	{
		count += change;
        UpdateIndicator();
	}


    public void ResetCounter(int newMaxEdges)
	{
        maxEdges = newMaxEdges;
		count = 0;
        UpdateIndicator();
	}


	public void OnCreateWithNoneRemaining()
	{
		Debug.Log("No remaining moves!");
	}

	void UpdateIndicator()
	{
		countText.text = remaining.ToString();
	}



}
