using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KbMouseController : MonoBehaviour, IInputController
{

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

    }

    bool IInputController.SelectIsPressed()
    {
        return Input.GetMouseButtonDown(0);
    }

    bool IInputController.SelectIsUnpressed()
    {
        return Input.GetMouseButtonUp(0);
    }
}
