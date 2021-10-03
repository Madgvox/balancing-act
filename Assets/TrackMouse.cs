using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMouse : MonoBehaviour {

    [SerializeField]
    public Rigidbody2D rigidbody;

    [SerializeField]
    public HingeJoint2D hinge;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

	internal void OnLose () {
        hinge.enabled = false;
	}

	internal void OnPlay () {
        hinge.enabled = true;
	}
}
