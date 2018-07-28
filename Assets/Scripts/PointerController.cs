using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerController : MonoBehaviour {

	public bool showLine = true;
	LineRenderer lineRenderer;
	NodePointer nodePointer;

	float lineLength = 3f;
	float pointerDistance = 100f;

	IInputController inputController;

	void Start()
	{
		// instances
		nodePointer = GetComponent<NodePointer>();
		lineRenderer = GetComponent<LineRenderer>();
	}

	public void Initialise(IInputController input, Transform newParent)
	{
		transform.parent = newParent;
        transform.localPosition = Vector3.zero;
		inputController = input;
	}

	void Update()
	{
        CastPointer();
	}


	void CastPointer()
	{
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
		bool pointedAtSomething = Physics.Raycast(transform.position, fwd, out hit, pointerDistance);

		// check pressed button
        bool pressed =  inputController.SelectIsPressed();
        bool unPressed = inputController.SelectIsUnpressed();

		if (pressed)
            Debug.Log("Pressed");

        // nodes - pointing/selecting
        if (pointedAtSomething)
		{
            NodeController pointedNode = hit.collider.GetComponent<NodeController>();
            nodePointer.RegisterPoint(pointedNode);
            if (pressed)
			{
                Debug.LogFormat("Selected node {0}", pointedNode.name);
                nodePointer.RegisterSelect(pointedNode);
            }
			if (unPressed)
			{
                nodePointer.RegisterJoin(pointedNode);
            }
		}
		else if (unPressed)
		{
            Debug.Log("Deselecting node");
            nodePointer.RegisterDeSelect();
		}
		else
		{
            nodePointer.RegisterPoint(null);
		}
       


		// line rendering
		if (showLine)
		{
			Vector3 endPoint = pointedAtSomething ? hit.point : transform.TransformPoint(Vector3.forward * lineLength);

            lineRenderer.SetPositions(
				new Vector3[]{transform.position, endPoint}
			);
		}

	}



}
