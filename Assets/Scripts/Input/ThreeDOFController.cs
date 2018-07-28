using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component used for GearVR control
/// </summary>
public class ThreeDOFController : MonoBehaviour, IInputController, IPhysicalController
{
	
	Vector2 prevTouchPoint;
    OVRInput.Controller activeController;

	void Start()
	{
        
	}

    // Update is called once per frame
    void Update()
    {
        Vector2 touchPoint = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, activeController);
    }

    void IInputController.Initialise()
    {
        activeController = (this as IPhysicalController).GetActiveController();
    }


    bool IInputController.SelectIsPressed()
    {
        return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, activeController);
    }

    bool IInputController.SelectIsUnpressed()
    {
        return OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, activeController);
    }

    OVRInput.Controller IPhysicalController.GetActiveController()
    {
        return OVRInput.GetActiveController();
    }

}
