using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]


public class PlayerControls : MonoBehaviour
{
    public GameObject mainRotor;
    public GameObject secondaryRotor;
    
    // Engine Control Vars
    public float maxEngineSpeed = 7000.0f;
    private float currentEngineSpeed = 0.0f;
    public bool engineActive = true;
    // Main Rotor Vars
    public bool mainRotorActive = true;
    public float maxMainRotorTorque = 10000.0f;
    private float currentMainRotorTorque = 0.0f;
    // Secondary Rotor Vars
    public bool secondaryRotorActive = true;
    public float maxSecondaryRotorTorque = 10000.0f;
    private float currentSecondaryRotorTorque = 0.0f;
    // Velocity Vars
    public float maxMainRotorVelocity = 10000.0f;
    public float maxSecondaryRotorVelocity = 10000.0f;
    // Control Modifiers
    public float forwardRotorTorqueModifier = 1.0f;
    public float sidewayRotorTorqueModifier = 1.0f;
    
    private PlayerInput playerInput;
    private InputAction throttleAction;
    private InputAction cyclicAction;
    private InputAction collectiveAction;
    private InputAction pedalAction;
    
    private Rigidbody rb;
    private AudioSource audioSource;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    // FixedUpdate is used to calculate things that cannot be dependent on FPS 
    private void FixedUpdate()
    {
    }
}
