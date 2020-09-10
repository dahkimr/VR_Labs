using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class ControllerLeft : MonoBehaviour {

    public Material selectMaterial;
    public GameObject button;
    public SteamVR_Action_Vibration hapticAction;

    public GameObject house, tree2, tree3, bench, canopy;

    private Vector3 houseStartScale, tree2StartScale, tree3StartScale, benchStartScale, canopyStartScale;
    private float halfDial;

    public GameObject obj, pos, scale;

    public static LineRenderer rayLine;
    public static GameObject highlightedObj;
    public static GameObject selectedObj;
    public static bool canPushBtn;
    public static bool canInteract = true;

    private bool thisCanInteract = false;

    private float rayLength = 50f;
    private Material[] newMatArr;
    private float buttonWidth;

    private Vector3 controllerStartPos, clldrStartPos;
    private Vector3 controllerStartRot, clldrStartRot;
    
    

    // Use this for initialization
    void Start() {
        rayLine = gameObject.AddComponent<LineRenderer>();
        rayLine.startWidth = .01f;
        rayLine.endWidth = .02f;
        rayLine.enabled = false;
        highlightedObj = null;
        selectedObj = null;
        buttonWidth = button.GetComponent<Collider>().bounds.size.y;

        halfDial = -7.6f;
        houseStartScale = house.transform.localScale;
        tree2StartScale = tree2.transform.localScale;
        tree3StartScale = tree3.transform.localScale;
        benchStartScale = bench.transform.localScale;
        canopyStartScale = canopy.transform.localScale;
    }

    // Update is called once per frame
    void Update() {

        RaycastHit hit;
        if (SteamVR_Input._default.inActions.Ray.GetState(SteamVR_Input_Sources.LeftHand)) {

            if (ControllerRight.rayLine.enabled != true) {
                rayLine.enabled = true;
                rayLine.SetPosition(0, gameObject.transform.position);

                LayerMask selMask = LayerMask.GetMask("CanSelect");

                if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, rayLength, selMask)) {
                    if (selectedObj != highlightedObj)
                        UnHighlightObject();
                    highlightedObj = hit.collider.gameObject;
                    rayLine.SetPosition(1, hit.point);
                    newMatArr = highlightedObj.GetComponent<MeshRenderer>().materials;
                    newMatArr[1] = selectMaterial;
                    hit.collider.gameObject.GetComponent<MeshRenderer>().materials = newMatArr;

                    if (SteamVR_Input._default.inActions.Select.GetStateDown(SteamVR_Input_Sources.LeftHand)) {
                        UnSelectObject();
                        selectedObj = highlightedObj;
                    }
                }
                else {
                    rayLine.SetPosition(1, gameObject.transform.position + (gameObject.transform.forward * rayLength));
                    if (selectedObj != highlightedObj)
                        UnHighlightObject();
                    if (SteamVR_Input._default.inActions.Select.GetStateDown(SteamVR_Input_Sources.LeftHand)) {
                        UnSelectObject();
                    }
                }
            }
        }
        else if (SteamVR_Input._default.inActions.Ray.GetStateUp(SteamVR_Input_Sources.LeftHand)) {
            if (rayLine.enabled == true) {
                rayLine.enabled = false;
                if (selectedObj != highlightedObj)
                    UnHighlightObject();
            }
        }

    }

    private void UnHighlightObject() {
        if (highlightedObj) {
            newMatArr = highlightedObj.GetComponent<MeshRenderer>().materials;
            newMatArr[1] = null;
            highlightedObj.GetComponent<MeshRenderer>().materials = newMatArr;
            highlightedObj = null;
        }
    }

    private void UnSelectObject() {
        if (selectedObj) {
            newMatArr = selectedObj.GetComponent<MeshRenderer>().materials;
            newMatArr[1] = null;
            selectedObj.GetComponent<MeshRenderer>().materials = newMatArr;
            selectedObj = null;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Rigidbody>() && canInteract) {
            canInteract = false;
            thisCanInteract = true;
            controllerStartPos = gameObject.transform.position;
            clldrStartPos = other.transform.position;
            canPushBtn = true;
            hapticAction.Execute(0, 1, 150, 75, SteamVR_Input_Sources.LeftHand);
            UpdateGui();
            switch (other.name) {
                case "Down Button":
                    PushButton(other, true);
                    break;
                case "Up Button":
                    PushButton(other, false);
                    break;
                case "Ball":
                    MoveJoyStick(other);
                    break;
                case "Dial":
                    SetDialPosition(other);
                    clldrStartPos = other.transform.position;
                    MoveDial(other);
                    break;

                default:
                    break;
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.GetComponent<Rigidbody>() && thisCanInteract) {
            //controllerStartRot = new Vector3(0.0f, gameObject.transform.eulerAngles.y, 0.0f);
            //Debug.Log("controllerstart.y: " + controllerStartRot.y);
            //clldrStartRot = other.transform.eulerAngles;
            hapticAction.Execute(0, 1, 150, 75, SteamVR_Input_Sources.LeftHand);
            UpdateGui();
            switch (other.name) {
                case "Down Button":
                    PushButton(other, true);
                    break;
                case "Up Button":
                    PushButton(other, false);
                    break;
                case "Ball":
                    MoveJoyStick(other);
                    break;
                case "Dial":
                    MoveDial(other);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<Rigidbody>() && thisCanInteract) {
            canPushBtn = false;
            switch (other.name) {
                case "Down Button":
                    other.transform.position = clldrStartPos;
                    break;
                case "Up Button":
                    other.transform.position = clldrStartPos;
                    break;
                case "Ball":
                    other.transform.position = clldrStartPos;
                    break;
                default:
                    break;
            }
            canInteract = true;
            thisCanInteract = false;
        }
    }

    private void PushButton(Collider other, bool downBtn) {
        float diffY = gameObject.transform.position.y - controllerStartPos.y;
        if (diffY >= -buttonWidth * 3 / 5 && diffY <= 0)
            other.transform.position = clldrStartPos + new Vector3(0.0f, diffY, 0.0f);
        if (canPushBtn && diffY <= -buttonWidth * 3 / 5) {
            if (downBtn) {
                MoveObj(new Vector3(0.0f, -0.2f, 0.0f));
            }
            else {
                MoveObj(new Vector3(0.0f, 0.2f, 0.0f));
            }
            canPushBtn = false;
        }
            
    }

    private void MoveObj(Vector3 offset) {
        if (selectedObj) {
            selectedObj.transform.position += offset;
        }
    }

    private void MoveJoyStick(Collider other) {
        Vector3 diff = gameObject.transform.position - controllerStartPos;
        diff.y = 0.0f;
        float bound = .11f;
        if (diff.x > -bound && diff.x < bound && diff.z > -bound && diff.z < bound)
            other.transform.position = clldrStartPos + diff;
        MoveObj(diff * 0.5f);
    }

    private void MoveDial(Collider other) {
        float diffZ = gameObject.transform.position.z - controllerStartPos.z;
        float tarZ = clldrStartPos.z + diffZ;
        if (tarZ <= -7.51 && tarZ >= -7.68) {
            other.transform.position = clldrStartPos + new Vector3(0.0f, 0.0f, diffZ);
            ScaleObj((other.transform.position.z - halfDial) * 0.5f);
        }
    }

    private void ScaleObj(float scale) {
        if (selectedObj) {
            Vector3 startScale;
            switch (selectedObj.name) {
                case "House Big":
                    startScale = houseStartScale;
                    break;
                case "Tree 2":
                    startScale = tree2StartScale;
                    break;
                case "Tree 3":
                    startScale = tree3StartScale;
                    break;
                case "Bench":
                    startScale = benchStartScale;
                    break;
                case "Canopy":
                    startScale = canopyStartScale;
                    break;
                default:
                    startScale = new Vector3(1f, 1f, 1f);
                    break;
            }
            selectedObj.transform.localScale = startScale + (new Vector3(1f, 1f, 1f) * scale);
        }
    }

    private void SetDialPosition(Collider other) {
        if (selectedObj) {
            Vector3 startScale;
            float dialPos;
            switch (selectedObj.name) {
                case "House Big":
                    startScale = houseStartScale;
                    break;
                case "Tree 2":
                    startScale = tree2StartScale;
                    break;
                case "Tree 3":
                    startScale = tree3StartScale;
                    break;
                case "Bench":
                    startScale = benchStartScale;
                    break;
                case "Canopy":
                    startScale = canopyStartScale;
                    break;
                default:
                    startScale = new Vector3(1f, 1f, 1f);
                    break;
            }
            dialPos = (selectedObj.transform.localScale.y - startScale.y) / 0.5f + halfDial;
            other.transform.position = new Vector3 (other.transform.position.x, other.transform.position.y, dialPos);

        }
    }

    private void UpdateGui() {
        if (selectedObj) {
            obj.GetComponent<Text>().text = selectedObj.name;
            pos.GetComponent<Text>().text = selectedObj.transform.position.ToString("n2");
            Vector3 startScale;
            switch (selectedObj.name) {
                case "House Big":
                    startScale = houseStartScale;
                    break;
                case "Tree 2":
                    startScale = tree2StartScale;
                    break;
                case "Tree 3":
                    startScale = tree3StartScale;
                    break;
                case "Bench":
                    startScale = benchStartScale;
                    break;
                case "Canopy":
                    startScale = canopyStartScale;
                    break;
                default:
                    startScale = new Vector3(1f, 1f, 1f);
                    break;
            }
            scale.GetComponent<Text>().text = "(" + (selectedObj.transform.localScale.x / startScale.x).ToString("n2") + ", " + (selectedObj.transform.localScale.y / startScale.y).ToString("n2") + ", " + (selectedObj.transform.localScale.z / startScale.z).ToString("n2") + ")";
        }
    }
}
