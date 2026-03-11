using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

// [RequireComponent(typeof(PlayerInput))]

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class PlayerControls : MonoBehaviour
{
    public GameObject mainRotor;
    public GameObject secondaryRotor;
    
    // Engine Control Vars
    public float maxEngineSpeed = 7000.0f;
    private float currentEngineSpeed = 0.0f;
    public bool engineActive = false;
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
    
    // private PlayerInput playerInput;
    private Helicopter_Actions helicopterActions;
    private InputAction throttleAction;
    private InputAction cyclicAction;
    private InputAction collectiveAction;
    private InputAction pedalAction;
    private InputAction engineAction;
    
    private Rigidbody rb;
    private AudioSource audioSource;

    void Awake()
    {
        helicopterActions = new Helicopter_Actions();
        throttleAction = helicopterActions.Player.Throttle;
        cyclicAction = helicopterActions.Player.Cyclic;
        collectiveAction = helicopterActions.Player.Collective;
        pedalAction = helicopterActions.Player.Pedals;
        engineAction = helicopterActions.Player.Engine;
    }
    private void OnEnable()
    {
        helicopterActions.Enable();
    }
    private void OnDisable()
    {
        helicopterActions.Disable();
    }
    private void OnDestroy()
    {
        helicopterActions.Dispose();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (engineAction.triggered)
        {
            engineActive = !engineActive;
            Debug.Log("Engine active = " + engineActive);
        }

        if (mainRotorActive)
        {
            mainRotor.transform.rotation *= Quaternion.Euler(0, maxMainRotorVelocity * currentEngineSpeed * Time.deltaTime, 0);
        }
    }

    // FixedUpdate is used to calculate things that cannot be dependent on FPS 
    private void FixedUpdate()
    {
        var throttle = throttleAction.ReadValue<float>();
        var cyclic = cyclicAction.ReadValue<float>();
        var collective = collectiveAction.ReadValue<float>();
        var pedal = pedalAction.ReadValue<float>();
        

        if (engineActive)
        {
            currentEngineSpeed = Mathf.Clamp(currentEngineSpeed + throttle * Time.fixedDeltaTime, 0.0f, 1.2f);
            while (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            Debug.Log("Engine Speed = " + currentEngineSpeed);
        }
        else
        {
            currentEngineSpeed -= Time.deltaTime;
        }
    }
}
