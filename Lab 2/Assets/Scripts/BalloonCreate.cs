using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BalloonCreate : MonoBehaviour {

    public GameObject balloon;

	private GameObject ballInst;
	private Rigidbody ballInstRB;
	private bool holdingBall = false;
    private Vector3 offset = new Vector3(0.0f, 0.03f, 0.0f), growScale = new Vector3(1f, 1f, 1f), initBallScale = new Vector3(.04f,.06f,.04f);

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		//if (SteamVR_Input._default.inActions.Balloon.GetStateDown(SteamVR_Input_Sources.LeftHand) && !holdingBall)
    	if ((SteamVR_Input._default.inActions.Balloon.GetStateDown(SteamVR_Input_Sources.LeftHand) && !holdingBall) || (SteamVR_Input._default.inActions.Balloon.GetStateDown(SteamVR_Input_Sources.LeftHand) && ballInst == null))
        {
			ballInst = Instantiate(balloon, gameObject.transform.position + offset, Quaternion.identity);
			ballInstRB = ballInst.GetComponent<Rigidbody>();
			ballInst.GetComponent<ConstantForce>().relativeForce = new Vector3(0.0f,0.3f,0.0f);
			ballInstRB.isKinematic = true;
			holdingBall = true;
        }
		//if (holdingBall)
   	 	if (holdingBall && ballInst != null)
		{
			if(SteamVR_Input._default.inActions.Balloon.GetState(SteamVR_Input_Sources.LeftHand))
			{
                if (ballInst.transform.localScale.x < initBallScale.x * 2)
                    ballInst.transform.localScale += growScale * Time.deltaTime * 0.05f;
				ballInstRB.position = gameObject.transform.position + offset;
			}
			else if (SteamVR_Input._default.inActions.Balloon.GetStateUp(SteamVR_Input_Sources.LeftHand))
			{
				ballInstRB.isKinematic = false;
				holdingBall = false;
			}
		}
	}
}
