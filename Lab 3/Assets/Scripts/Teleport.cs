using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Teleport : MonoBehaviour {

    public GameObject telCircle;
    public GameObject camRig;
    public Transform headTransform;

    private float rayLength = 25f;
    private LineRenderer rayLine;
    private Vector3 hitPoint;
    

    // Use this for initialization
    void Start() {

        telCircle.SetActive(false);

        rayLine = gameObject.AddComponent<LineRenderer>();
        rayLine.startWidth = .01f;
        rayLine.endWidth = .02f;
        rayLine.enabled = false;

    }

    // Update is called once per frame
    void Update() {
        RaycastHit hit;
        if (SteamVR_Input._default.inActions.Ray.GetState(SteamVR_Input_Sources.LeftHand)) {
            rayLine.enabled = true;
            rayLine.SetPosition(0, gameObject.transform.position);
            
            LayerMask telMask = LayerMask.GetMask("CanTeleport");

            if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, rayLength, telMask)) {
                    telCircle.SetActive(true);
                    telCircle.transform.position = hit.point + new Vector3(0f, 0.01f, 0.0f);
                    rayLine.SetPosition(1, hit.point);

                if (SteamVR_Input._default.inActions.Teleport.GetStateDown(SteamVR_Input_Sources.LeftHand)) {
                    FadeToBlack();
                    hitPoint = hit.point;
                    Invoke("TeleportMethod", .5f);
                    Invoke("FadeFromBlack", 1f);
                    
                }
            }
            else {
                rayLine.SetPosition(1, gameObject.transform.position + (gameObject.transform.forward * rayLength));
                telCircle.SetActive(false);
            }
                
        } else if (SteamVR_Input._default.inActions.Ray.GetStateUp(SteamVR_Input_Sources.LeftHand)) {
            rayLine.enabled = false;
            telCircle.SetActive(false);
        }
    }

    // worked with code from https://answers.unity.com/questions/1258342/steam-vr-fade-camera.html
    void FadeToBlack() {
        //set start color
        SteamVR_Fade.Start(Color.clear, 0f);
        //set and start fade to
        SteamVR_Fade.View(Color.black, .5f);
    }
    void FadeFromBlack() {
        //set start color
        SteamVR_Fade.Start(Color.black, 0f);
        //set and start fade to
        SteamVR_Fade.View(Color.clear, .5f);
    }

    void TeleportMethod() {
        // teleport
        telCircle.SetActive(false);
        rayLine.enabled = false;
        Vector3 diff = camRig.transform.position - headTransform.position;
        diff.y = 0;
        camRig.transform.position = new Vector3(hitPoint.x, camRig.transform.position.y, hitPoint.z) + diff;
    }
}
