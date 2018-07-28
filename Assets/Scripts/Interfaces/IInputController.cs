using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputController
{
	void Initialise();

	bool SelectIsPressed();

	bool SelectIsUnpressed();
}



public interface IPhysicalController
{
    OVRInput.Controller GetActiveController();
}

public enum ControlMethod 
{
	SixDof, ThreeDof, KbMouse
}