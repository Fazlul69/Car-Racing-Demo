﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public WheelCollider[] wc;
    public float torque = 200;
    public GameObject[] wheel;
    public float maxStreetAngle = 30;
    public float maxBreakTorque = 500;
    public AudioSource audioSkid;

    public Transform SkidTrailPrefab;
    Transform[] skidTrails = new Transform[4];

    public ParticleSystem smokePrefab;
    ParticleSystem[] skidSmoke = new ParticleSystem[4];

    public GameObject brakeLight;
    // Start is called before the first frame update
    void Start()
    {
        // wc = this.GetComponent<WheelCollider>();
        for (int i = 0; i < 4; i++)
        {
            skidSmoke[i] = Instantiate(smokePrefab);
            skidSmoke[i].Stop();
        }
        brakeLight.SetActive(false);
    }

    void Go(float accel, float steer, float brake)
    {
        accel = Mathf.Clamp(accel, -1, 1);
        steer = Mathf.Clamp(steer, -1, 1) * maxStreetAngle;
        brake = Mathf.Clamp(brake, 0, 1) * maxBreakTorque;

        if (brake != 0)
        {
            brakeLight.SetActive(true);
        }
        else { brakeLight.SetActive(false); }

        float thrustTorque = accel * torque;

        for (int i = 0; i < wc.Length; i++) {
            wc[i].motorTorque = thrustTorque;
            if (i < 2)
            {
                wc[i].steerAngle = steer;
            }
            else {
                wc[i].brakeTorque = brake;
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
        float b = Input.GetAxis("Jump");
        Go(a,s,b); 
        CheckForSKid();
    }

    void CheckForSKid() {
        int numSkiddding = 0;
        for (int i = 0; i < wc.Length; i++)
        {
            WheelHit wheelhit;
            wc[i].GetGroundHit(out wheelhit);

            if (Mathf.Abs(wheelhit.forwardSlip) >= .4f || Mathf.Abs(wheelhit.sidewaysSlip) >= .4f)
            {
                numSkiddding++;
                if (!audioSkid.isPlaying)
                {
                    audioSkid.Play();
                }
                StartSkidTrail(i);
                skidSmoke[i].transform.position = wc[i].transform.position - wc[i].transform.up * wc[i].radius;
                skidSmoke[i].Emit(1);
            }
            else {
                EndSkidTrail(i);
            }
        }
        if (numSkiddding == 0 && audioSkid.isPlaying)
        {
            audioSkid.Stop();
        }
        
    }

    public void StartSkidTrail(int i)
    {
        if (skidTrails[i] == null)
        {
            skidTrails[i] = Instantiate(SkidTrailPrefab);
        }
        skidTrails[i].parent = wc[i].transform;
        skidTrails[i].localRotation = Quaternion.Euler(90, 0, 0);
        skidTrails[i].localPosition = -Vector3.up * wc[i].radius;
    }

    public void EndSkidTrail(int i)
    {
        if (skidTrails[i] == null)
            return;
        Transform holder = skidTrails[i];
        skidTrails[i] = null;
        holder.parent = null;
        holder.rotation = Quaternion.Euler(90, 0, 0);
        Destroy(holder.gameObject, 15);
    }
}
