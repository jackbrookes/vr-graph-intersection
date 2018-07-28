using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoBehaviour
{

    public PointerController pointer;

    public ControlMethod controlMethod;

	[HideInInspector]
    public IInputController inputController;


    [Header("VR controller")]
    public GameObject vrCameraRig;
    public Transform vrHand;

    [Header("KbMouse controller")]
    public GameObject flyingCam;
    public Transform mouseHand;


    // Use this for initialization
    void Awake()
    {
        switch (controlMethod)
		{
			case ControlMethod.KbMouse:
                inputController = (IInputController) gameObject.AddComponent<KbMouseController>();
                pointer.Initialise(inputController, mouseHand);
                flyingCam.gameObject.SetActive(true);
                vrCameraRig.gameObject.SetActive(false);
				break;

            case ControlMethod.SixDof:
                inputController = (IInputController) gameObject.AddComponent<SixDOFController>();
                pointer.Initialise(inputController, vrHand);
                flyingCam.gameObject.SetActive(false);
                vrCameraRig.gameObject.SetActive(true);
                break;

            case ControlMethod.ThreeDof:
                inputController = (IInputController) gameObject.AddComponent<ThreeDOFController>();
                pointer.Initialise(inputController, vrHand);
                flyingCam.gameObject.SetActive(false);
                vrCameraRig.gameObject.SetActive(true);
                break;
		}

        inputController.Initialise();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
