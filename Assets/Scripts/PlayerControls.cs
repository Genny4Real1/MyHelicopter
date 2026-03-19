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
    // public float maxEngineSpeed = 2800.0f; // RPM
    public bool engineActive = false;
    [SerializeField] [Range(0f, 1f)] float currentEngineSpeed = 0f;
    [Header("Main Rotor Vars")]
    public bool mainRotorActive = true;
    public float maxMainRotorTorque = 2000.0f;
    public float maxCyclicForce = 1200f;
    [SerializeField] float currentMainRotorTorque;
    [SerializeField] Vector2 currentCyclicForce;
    [Header("Secondary Rotor Vars")]
    public bool secondaryRotorActive = true;
    public float maxSecondaryRotorTorque = 400.0f;
    [SerializeField] float currentSecondaryRotorTorque = 0.0f;
    [Header("Velocity Vars")]
    public float maxMainRotorVelocity = 10000.0f; // RPM
    public float maxSecondaryRotorVelocity = 10000.0f; // RPM
    [Header("Control Modifiers")]
    [SerializeField] float forwardRotorTorqueModifier = 1.0f;
    [SerializeField] float sidewayRotorTorqueModifier = 0.25f;
    [SerializeField] float cyclicInputModifier = 1.0f;
    
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
        }

        if (mainRotorActive)
        {
            mainRotor.transform.rotation *= Quaternion.Euler(0, maxMainRotorVelocity * currentEngineSpeed * Time.deltaTime * 6f, 0);
        }

        if (secondaryRotorActive)
        {
            secondaryRotor.transform.rotation *= Quaternion.Euler(0, (maxSecondaryRotorVelocity * currentEngineSpeed * 6f) * Time.deltaTime, 0);

        }
    }

    // FixedUpdate is used to calculate things that cannot be dependent on FPS 
    private void FixedUpdate()
    {
        var throttle = throttleAction.ReadValue<float>();
        var cyclic = cyclicAction.ReadValue<Vector2>();
        var collective = collectiveAction.ReadValue<float>();
        var pedal = pedalAction.ReadValue<float>();
        
        if (engineActive)
        {
            currentEngineSpeed = Mathf.Clamp(currentEngineSpeed + throttle * Time.fixedDeltaTime, 0.0f, 1.2f);
        }
        else
        {
            currentEngineSpeed = Mathf.Max(0f, currentEngineSpeed - Time.fixedDeltaTime);
        }

        if (mainRotorActive)
        {
            
            currentMainRotorTorque = collective * maxMainRotorTorque * currentEngineSpeed * forwardRotorTorqueModifier;
            rb.AddRelativeForce(Vector3.up * currentMainRotorTorque);

            currentCyclicForce = cyclic * (maxCyclicForce * currentEngineSpeed * cyclicInputModifier);
            rb.AddRelativeTorque(new Vector3(-currentCyclicForce.y, 0, currentCyclicForce.x));
        }
        
        if (secondaryRotorActive)
        {
            currentSecondaryRotorTorque = pedal * maxSecondaryRotorTorque * currentEngineSpeed * sidewayRotorTorqueModifier;
            rb.AddRelativeTorque(Vector3.up * currentSecondaryRotorTorque);
        }
    }
}
