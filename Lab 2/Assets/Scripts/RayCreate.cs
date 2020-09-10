using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class RayCreate : MonoBehaviour {

  private float rayLength = 5f;
  private LineRenderer rayLine;

	// Use this for initialization
	void Start () {
    rayLine = gameObject.AddComponent<LineRenderer>();
    rayLine.startWidth = .05f;
    rayLine.endWidth = .05f;
    rayLine.enabled = false;
  }

	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        if (SteamVR_Input._default.inActions.Ray.GetStateDown(SteamVR_Input_Sources.RightHand) || SteamVR_Input._default.inActions.Ray.GetState(SteamVR_Input_Sources.RightHand)) {

            rayLine.enabled = true;
            rayLine.SetPosition(0, gameObject.transform.position);
            rayLine.SetPosition(1, gameObject.transform.position + (gameObject.transform.forward * rayLength));

            if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, rayLength))
            {
                if (hit.collider.tag == "balloon") {
                    Destroy(hit.collider.gameObject);
                }
            }
        }
        else if (SteamVR_Input._default.inActions.Ray.GetStateUp(SteamVR_Input_Sources.RightHand)) {
          rayLine.enabled = false;
        }
    }

}
