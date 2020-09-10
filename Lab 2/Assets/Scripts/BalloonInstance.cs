using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonInstance : MonoBehaviour {

	private float maxHeight = 5f;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (gameObject.transform.position.y > maxHeight)
    {
       Destroy(gameObject);
    }
	}
}
