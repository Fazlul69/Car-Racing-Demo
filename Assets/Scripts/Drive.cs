using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public WheelCollider wc;
    public float torque = 200;
    public GameObject wheel;
    // Start is called before the first frame update
    void Start()
    {
        wc = this.GetComponent<WheelCollider>();
    }

    void Go(float accel)
    {
        accel = Mathf.Clamp(accel, -1, 1);
        float thrustTorque = accel * torque;
        wc.motorTorque = thrustTorque;

        Quaternion quaternion;
        Vector3 position;
        wc.GetWorldPose(out position, out quaternion);
        wheel.transform.position = position;
        wheel.transform.rotation = quaternion;
    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        Go(a);
    }
}
