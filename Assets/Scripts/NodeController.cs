using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{	
	[HideInInspector]
	public int num;
    public NodeType nodeType;
	Animator animator;
	Material material;
	ParticleSystem particles;
	AudioSource audioSource;	

	NodeGlyphController nodeGlyph;

	int faces = 0;

	void Awake()
	{
        animator = GetComponentInChildren<Animator>();
        material = GetComponentInChildren<MeshRenderer>().material;
        particles = GetComponentInChildren<ParticleSystem>();
        nodeGlyph = GetComponentInChildren<NodeGlyphController>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Initialise(int nodeNum, Vector3 newPosition, NodeType newNodeType)
	{
		num = nodeNum;
		name = string.Format("node{0}", num);
		nodeType = newNodeType;
		transform.localPosition = newPosition;
        ResetState();
		UpdateProperties();
	}

	void UpdateProperties()
	{
		// node
		if (nodeType.overrideMaterial != null)
		{
            GetComponentInChildren<MeshRenderer>().material = nodeType.overrideMaterial;
            material = GetComponentInChildren<MeshRenderer>().material;
		}

        if (nodeType.overrideModel != null)
        {
            GetComponentInChildren<MeshFilter>().mesh = nodeType.overrideModel.GetComponentInChildren<MeshFilter>().sharedMesh;
        }


		material.color = nodeType.color;


		// particles
		var main = particles.main;
        main.startColor = nodeType.color;
	} 	


	public void ResetState()
	{
        faces = 0;
        nodeGlyph.Off();
	}

	public void OnPointed()
	{
		animator.SetBool("Pointed", true);
	}

    public void OnDePointed()
    {
        animator.SetBool("Pointed", false);
    }


	public void OnSelected()
	{
        animator.SetBool("Selected", true);
        audioSource.Play();
    }

    public void OnDeSelected()
    {
        animator.SetBool("Selected", false);
    }


	public void OnCreateEdge()
	{
        particles.Play();
    }

	public void OnCreateFace()
	{
		faces++;
		if (!nodeGlyph.isOn)
		{
			nodeGlyph.On();
		}
    }

	public void OnRemoveEdge()
	{

	}

	public void OnRemoveFace()
	{
        if (--faces == 0)
		{
            nodeGlyph.Off();
		}
		else if (faces < 0)
		{
			Debug.LogWarning("Negative connections... something wrong");
			faces = 0;
            nodeGlyph.Off();
		}
	}

}
