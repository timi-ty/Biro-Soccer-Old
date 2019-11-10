using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenComponentBehaviour : MonoBehaviour
{
    PenBehaviour myPen;
    Rigidbody penComponentBody;
    [HideInInspector]
    public Collider myCollider;
    Vector3 startPoint;
    Quaternion startRotation;
    bool comingInHot;

    void Start()
    {
        myPen = transform.parent.GetComponent<PenBehaviour>();
        penComponentBody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        startPoint = transform.localPosition;
        startRotation = transform.localRotation;
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other) {
        if(!other.transform.parent.Equals(transform.parent) 
            && other.transform.tag.Equals("Player") && comingInHot){
            other.rigidbody.velocity += penComponentBody.velocity * myPen.impactPower;
            Debug.Log(other.transform.parent + ": " + other.transform.tag);
            comingInHot = false;
            CancelInvoke("CoolDown");

            StartCoroutine("ApplyRecoil", penComponentBody.velocity);
        }
    }

    public void Flick(Vector3 flickForce){
        penComponentBody.velocity = new Vector3(flickForce.x, 0, flickForce.z) * myPen.maxSpeed;
        comingInHot = true;
        Invoke("CoolDown", myPen.powerGasTank);
    }

    void CoolDown(){
        comingInHot = false;
    }

    IEnumerator ApplyRecoil(Vector3 fromDirection){
        float lastSpeed = penComponentBody.velocity.magnitude;
        for(int i = 0; i < 20; i++){
            lastSpeed = penComponentBody.velocity.magnitude;
            penComponentBody.velocity -= fromDirection * myPen.recoilPower * Time.fixedDeltaTime;
            if(penComponentBody.velocity.magnitude > lastSpeed){
                Debug.Log("Aborted recoil application after " + i + " frames");
                break;
            }
            yield return null;
        }
    }

    void Respawn(){
        Debug.Log("Respawning...");
        penComponentBody.isKinematic = true;
        Invoke("JumpToTable", 0.1f);
    }

    void ResumePhysics(){
        penComponentBody.isKinematic = false;
    }
    void JumpToTable(){
        transform.localPosition = startPoint + new Vector3(0, 1, 0);
        transform.localRotation = startRotation;
        Invoke("ResumePhysics", 0.5f);
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag.Equals("OutBound")){
            Respawn();
        }
    }
}
