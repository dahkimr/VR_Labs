using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PickUpController : MonoBehaviour {

	// code modified from HTV Vive tutorial https://www.raywenderlich.com/792-htc-vive-tutorial-for-unity 

	private GameObject objClldr;
	private GameObject objInHand;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (SteamVR_Input._default.inActions.PickUp.GetStateDown(SteamVR_Input_Sources.RightHand)) {
			
			if (objClldr) {
				objInHand = objClldr;
				objClldr = null;
				FixedJoint jnt = gameObject.AddComponent<FixedJoint>();
				jnt.breakForce = 20000;
				jnt.breakTorque = 20000;
				jnt.connectedBody = objInHand.GetComponent<Rigidbody>();
			}
		}
		else if (SteamVR_Input._default.inActions.PickUp.GetStateUp(SteamVR_Input_Sources.RightHand)) {
			if (objInHand) {
				if (GetComponent<FixedJoint>()) {
					GetComponent<FixedJoint>().connectedBody = null;
					Destroy(GetComponent<FixedJoint>());
					objInHand.GetComponent<Rigidbody>().velocity = SteamVR_Input.__actions_default_in_Pose.GetVelocity(SteamVR_Input_Sources.RightHand);
					objInHand.GetComponent<Rigidbody>().angularVelocity = SteamVR_Input.__actions_default_in_Pose.GetAngularVelocity(SteamVR_Input_Sources.RightHand);
				}
				objInHand = null;
			}
		}
	}

	public void OnTriggerEnter(Collider obj) {
		if (!objClldr && obj.GetComponent<Rigidbody>()) {
			objClldr = obj.gameObject;
		}
	}
    public void OnTriggerStay(Collider obj) {
        if (!objClldr && obj.GetComponent<Rigidbody>()) {
            objClldr = obj.gameObject;
        }
    }
    public void OnTriggerExit(Collider obj) {
		if (objClldr) {
			objClldr = null;
        }
	}
}
