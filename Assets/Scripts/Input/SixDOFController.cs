using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SixDOFController : MonoBehaviour, IInputController, IPhysicalController
{

    OVRInput.Controller activeController;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        return OVRInput.Controller.RTouch;
    }
}
