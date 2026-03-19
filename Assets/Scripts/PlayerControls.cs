using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;
// using System;
// using UnityEngine.InputSystem.Controls;
// [RequireComponent(typeof(PlayerInput))]

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class PlayerControls : MonoBehaviour
{
    [Header("Rotor Game Objects")]
    [SerializeField] GameObject mainRotor;
    [SerializeField] GameObject secondaryRotor;
    
    [Header("Engine Control Vars")]
    public float maxEngineSpeed = 7000.0f;
    public bool engineActive = false;
    [SerializeField] [Range(0f, 1f)] float currentEngineSpeed = 0f;
    [Header("Main Rotor Vars")]
    public bool mainRotorActive = true;
    public float maxMainRotorTorque = 2000.0f;
    [SerializeField] float currentMainRotorTorque;
    [Header("Secondary Rotor Vars")]
    public bool secondaryRotorActive = true;
    public float maxSecondaryRotorTorque = 400.0f;
    [SerializeField] [Range(0f, 1f)] float currentSecondaryRotorTorque = 0.0f;
    [Header("Velocity Vars")]
    public float maxMainRotorVelocity = 10000.0f;
    public float maxSecondaryRotorVelocity = 10000.0f;
    [Header("Control Modifiers")]
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
        // Audio Source
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = 0.0f;
        audioSource.Play();
        
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (engineActive && currentEngineSpeed > 0)
        {
            // audio source's pitch 
            audioSource.pitch = 0.25f + currentEngineSpeed * 0.750f;
        }

        if (engineAction != null && engineAction.WasPressedThisFrame())
        {
            engineActive = !engineActive;
            Debug.Log($"Engine active = {engineActive}");
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
            Debug.Log("Engine Speed = " + currentEngineSpeed);
        }
        else
        {
            currentEngineSpeed = Mathf.Max(0f, currentEngineSpeed - Time.fixedDeltaTime);
        }

        if (mainRotorActive)
        {
            currentMainRotorTorque = collective * maxMainRotorTorque * currentEngineSpeed * forwardRotorTorqueModifier;
            Debug.Log("Main Rotor Torque = " + currentMainRotorTorque); 
            rb.AddRelativeForce(Vector3.up * currentMainRotorTorque);
        }
    }
}
