using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour {

    private const string OPEN_CLOSE = "OpenClose";

    [SerializeField] ContainerCounter containerCounter;

    private Animator animator;
    
    private void Awake() {
        animator = GetComponent<Animator>();    
    }

    private void Start() {
        containerCounter.OnPlayerGrabbedObject +=  ContainerCounter_OnPlayerGrabbedObject;
        
    }

    private void ContainerCounter_OnPlayerGrabbedObject(object sender, EventArgs e) {
        animator.SetTrigger(OPEN_CLOSE);
    }
}
