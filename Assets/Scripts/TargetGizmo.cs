using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGizmo : MonoBehaviour
{
    public CameraController myCamera;
    MeshRenderer myMesh;
    public LineRenderer targetLine;
    public PenBehaviour playerPen;
    PenComponentBehaviour targetSubject;
    float screenSegment;
    bool _targeting;

    private void Start() {
        myMesh = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            StartTargeting(Input.mousePosition);
            Debug.Log("Target");
        }
        if(Input.GetMouseButton(0) && _targeting){
            Target();
        }
        else if(Input.GetMouseButtonUp(0) && _targeting){
            FinishTargeting();
        }
    }

    void StartTargeting(Vector2 targetOrigin){
        _targeting = true;
        myMesh.enabled = true;
        targetLine.enabled = true;
        screenSegment = Camera.main.pixelWidth/3;

        Vector3 vectorToCover = playerPen.cover.myCollider.bounds.center - Camera.main.transform.position;
        Vector3 vectorToMiddle = playerPen.middle.myCollider.bounds.center - Camera.main.transform.position;

        float deciderRotation = Quaternion.FromToRotation(vectorToMiddle, vectorToCover).eulerAngles.y;

        Debug.Log(deciderRotation);

        if(deciderRotation < 180){
            playerPen.rightComponent = playerPen.cover;
            playerPen.leftComponent = playerPen.tail;
        }
        else{
            playerPen.rightComponent = playerPen.tail;
            playerPen.leftComponent = playerPen.cover;
        }

        

        if(targetOrigin.x > screenSegment * 2){
            targetSubject = playerPen.rightComponent;
            Debug.Log("right press");
        }
        else if(targetOrigin.x > screenSegment){
            targetSubject = playerPen.middle;
            Debug.Log("middle press");
        }
        else{
            targetSubject = playerPen.leftComponent;
            Debug.Log("left press");
        }

        transform.position = targetSubject.myCollider.bounds.center;
        targetLine.SetPosition(0, transform.position);

        myCamera.ObservePoint(transform.position);
    }

    void Target(){
        Vector3 targetPos = NormalizeTargetPoint(Input.mousePosition) + targetLine.GetPosition(0);
        targetLine.SetPosition(1, targetPos);
    }

    void FinishTargeting(){
        _targeting = false;
        myMesh.enabled = false;
        targetLine.enabled = false;

        Vector3 flickForce = (targetLine.GetPosition(0) - targetLine.GetPosition(1));

        targetSubject.Flick(flickForce);

        myCamera.Reset();
    }

    Vector3 NormalizeTargetPoint(Vector2 targetPoint){
        if(targetSubject == playerPen.rightComponent){
            targetPoint.x = targetPoint.x - (screenSegment * 2);
        }
        else if(targetSubject == playerPen.middle){
            targetPoint.x = targetPoint.x - screenSegment;
        }
        float x = (targetPoint.x - (screenSegment / 2)) / (screenSegment / 4);
        float y = (targetPoint.y - (Camera.main.pixelHeight / 2)) / (Camera.main.pixelHeight / 6);

        float cameraOffsetAngle = Camera.main.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;

        float directAngle = Mathf.Atan2(x, y); //x comes before y because of Unity's clockwise angle convention

        float completePerspectiveAngle = cameraOffsetAngle + directAngle;

        float magnitude = new Vector2(x, y).magnitude;
        magnitude = Mathf.Clamp(magnitude, 0, 1.5f);

        return new Vector3(Mathf.Sin(completePerspectiveAngle), 
                    0, Mathf.Cos(completePerspectiveAngle)) * magnitude;
    }
}
