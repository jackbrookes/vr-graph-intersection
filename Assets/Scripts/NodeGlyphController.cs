using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGlyphController : MonoBehaviour {


	SpriteRenderer spriteRenderer;
	public bool isOn { get { return spriteRenderer.enabled; } }

	void Awake()
	{
        spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void On()
	{
		spriteRenderer.enabled = true;
	}

	public void Off()
	{
        spriteRenderer.enabled = false;
	}


}
