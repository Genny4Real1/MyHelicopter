using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerInput))]

public class Helicopter_Control : MonoBehaviour
{
    // Private constants
    const float maxEngineRPM = 15000f; // maximum engine speed in RPM
    const float engineMinPct = 0f; // minimum percentage of engine power (0% of max engine speed)
    const float engineMaxPct = 1.15f; // maximum percentage of engine power (115% of max engine speed, allowing for some overspeeding)
    const float engineMinSafePct = 0.85f; // minimum safe percentage of engine power (85% of max engine speed)
    const float engineMaxSafePct = 1.05f; // maximum safe percentage of engine power (105% of max engine speed, allowing for some overspeeding but not too much)
    // Main-to-tail rotor RPM ratio is 1:5 (max 300 RPM for main rotor, max 1500 RPM for tail rotor)
    const float maxMainRotorRPM = 300f; // maximum main rotor speed in RPM
    const float maxTailRotorRPM = 1500f; // maximum tail rotor



    // Initialize variables for the main rotor and tail rotor game objects
    [Header("Rotor Game Objects")]
    [SerializeField] GameObject mainRotor;
    [SerializeField] GameObject tailRotor;

    // Initialize variables for engine control
    [Header("Engine Control Variables")]
    [SerializeField] bool engineActive = true; // boolean for determining if the engine is on or off
    [SerializeField][Range(engineMinPct, engineMaxPct)] float enginePct = 0f; // percentage of engine power (minPct to maxPct, where 1.0f is 100% of max engine speed)

    [Header("Main Rotor Control Variables")]
    // Initialize variables for the main rotor
    [SerializeField] bool mainRotorActive = true; // boolean for determining if the main rotor is active
    [SerializeField] float maxCollectiveForce = 25000f; // in newtons, this is the maximum force that can be applied in the upward direction (for lift control)
    [SerializeField] float maxCyclicForce = 5000f; // in newtons, this is the maximum force that can be applied in the forward/backward and left/right directions (for pitch and roll control)

    [Header("Tail Rotor Control Variables")]
    // Initialize variables for the tail rotor
    [SerializeField] bool tailRotorActive = true; // boolean for determining if the tail rotor is active
    [SerializeField] float maxPedalForce = 15000f; // in newtons, this is the maximum force that can be applied in the yaw rotation direction (for yaw control)

    // Initialize variables for control input multipliers
    [Header("Control Input Multipliers")]
    [SerializeField][Range(0f, 1f)] float throttleInputMultiplier = 0.5f; // multiplier for throttle input (engine power control)
    [SerializeField][Range(0f, 1f)] float collectiveInputMultiplier = 0.5f; // multiplier for collective input (lift control)
    [SerializeField][Range(0f, 1f)] float cyclicInputMultiplier = 0.5f; // multiplier for cyclic input (pitch and roll control)
    [SerializeField][Range(0f, 1f)] float pedalInputMultiplier = 0.5f; // multiplier for pedal input (yaw control)

    // Private variables for reading player input
    private InputAction throttleAction; // reference to the throttle input action (used to read throttle input) // "Throttle" input action
    private InputAction cyclicAction; // reference to the cyclic input action (used to read cyclic input) // "Cyclic" input action
    private InputAction collectiveAction; // reference to the collective input action (used to read collective input) // "Collective" input action
    private InputAction pedalAction; // reference to the pedal input action (used to read pedal input) // "Pedal" input action

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Read controls
        PlayerInput playerInput = GetComponent<PlayerInput>();
        throttleAction = playerInput.actions["Throttle"];
        cyclicAction = playerInput.actions["Cyclic"];
        collectiveAction = playerInput.actions["Collective"];
        pedalAction = playerInput.actions["Pedal"];
    }

    // FixedUpdate is called at a fixed interval and is independent of the frame rate. Put physics code here.
    void FixedUpdate()
    {
        // Read control inputs
        float throttleInput = throttleAction.ReadValue<float>(); // read throttle input (range -1 to 1)
        Vector2 cyclicInput = cyclicAction.ReadValue<Vector2>(); // read cyclic input (x for left/right, y for forward/backward, range -1 to 1)
        float collectiveInput = collectiveAction.ReadValue<float>(); // read collective input (range -1 to 1)
        float pedalInput = pedalAction.ReadValue<float>(); // read pedal input (range -1 to 1)

        Debug.Log($"Throttle Input: {throttleInput}, Cyclic Input: {cyclicInput}, Collective Input: {collectiveInput}, Pedal Input: {pedalInput}");

        // Process control inputs and apply forces to the helicopter based on the inputs and the control variables
        // (This is where you would implement the physics calculations to determine how much force to apply to the main rotor and tail rotor based on the control inputs and the current state of the helicopter)
        if (engineActive)
        {
            enginePct = Mathf.Clamp(enginePct + (throttleInput * throttleInputMultiplier * Time.fixedDeltaTime), engineMinPct, engineMaxPct); // calculate engine percentage based on throttle input and multiplier, and clamp it to the min and max values
        }
        else
        {
            // Simulate engine slowdown when the engine is not active
            enginePct = Mathf.Clamp(enginePct - (throttleInputMultiplier * Time.fixedDeltaTime), engineMinPct, engineMaxPct); // if the engine is not active, decrease the engine percentage over time to simulate the engine slowing down
        }

        if (mainRotorActive)
        {
            float collectiveForce = collectiveInput * maxCollectiveForce * enginePct * collectiveInputMultiplier;
            GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * (GetComponent<Rigidbody>().mass * 9.81f * enginePct + collectiveForce)); // Add force to simulate hover

            Vector2 cyclicForce = cyclicInput * maxCyclicForce * enginePct * cyclicInputMultiplier;
            GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(-cyclicForce.y, 0, cyclicForce.x));
        }
        if (tailRotorActive)
        {
            float pedalForce = pedalInput * maxPedalForce * enginePct * pedalInputMultiplier;
            GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * pedalForce);
        }
    }

    // Update is called once per frame
    void Update()
    {

        GetComponent<AudioSource>().pitch = enginePct; // change the pitch of the audio source based on the engine percentage (this is a simple way to simulate the sound of the engine changing as it speeds up or slows down)
        if (mainRotorActive)
        {
            // Rotate the main rotor based on the engine percentage and the maximum main rotor velocity
            float mainRotorRPM = enginePct * maxMainRotorRPM; // calculate main rotor RPM
            mainRotor.transform.rotation *= Quaternion.Euler(0, mainRotorRPM * 6f * Time.deltaTime, 0); // rotate the main rotor (6f is used to convert RPM to degrees per second, 1RPM = 360 degrees per minute = 6 degrees per second)
        }
        if (tailRotorActive)
        {
            // Rotate the tail rotor based on the engine percentage and the maximum tail rotor velocity
            float tailRotorRPM = enginePct * maxTailRotorRPM; // calculate tail rotor RPM
            tailRotor.transform.rotation *= Quaternion.Euler(tailRotorRPM * 6f * Time.deltaTime, 0, 0); // rotate the tail rotor (6f is used to convert RPM to degrees per second, 1RPM = 360 degrees per minute = 6 degrees per second)
        }
    }
}
