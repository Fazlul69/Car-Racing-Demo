using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public WheelCollider[] wc;
    public float torque = 200;
    public GameObject[] wheel;
    public float maxStreetAngle = 30;
    // Start is called before the first frame update
    void Start()
    {
       // wc = this.GetComponent<WheelCollider>();
    }

    void Go(float accel, float steer)
    {
        accel = Mathf.Clamp(accel, -1, 1);
        steer = Mathf.Clamp(steer, -1, 1) * maxStreetAngle;
        float thrustTorque = accel * torque;
        for (int i = 0; i < wc.Length; i++) {
            wc[i].motorTorque = thrustTorque;
            if (i < 2) {
                wc[i].steerAngle = steer;
            }
        

            Quaternion quaternion;
            Vector3 position;
            wc[i].GetWorldPose(out position, out quaternion);

            wheel[i].transform.position = position;
            wheel[i].transform.rotation = quaternion;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float s = Input.GetAxis("Horizontal");
        Go(a,s);
    }
}
