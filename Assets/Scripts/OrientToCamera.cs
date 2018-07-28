using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientToCamera : MonoBehaviour
{

    Camera cam;

    public static float smooth = 0.2F;
    private float xVelocity, yVelocity = 0.0F;

    // Use this for initialization
    void Start()
    {
		cam = Camera.main;

        Vector3 diff = cam.transform.position - transform.position;
        transform.eulerAngles = Quaternion.LookRotation(diff).eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        Orient();
    }


    void Orient()
    {
        Vector3 diff = cam.transform.position - transform.position;
        Vector3 targetEuler = Quaternion.LookRotation(diff).eulerAngles;

        float xAngle = Mathf.SmoothDampAngle(transform.eulerAngles.x, targetEuler.x, ref xVelocity, smooth);
        float yAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetEuler.y, ref yVelocity, smooth);

        transform.eulerAngles = new Vector3(xAngle, yAngle, transform.eulerAngles.z);
    }

}
