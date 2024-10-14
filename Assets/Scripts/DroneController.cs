using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    // Parametri fisici del drone
    public float throttlePower = 10f; // Potenza per la spinta verticale
    public float maxSpeed = 15f; // Velocità massima
    public float forwardAcceleration = 5f; // Accelerazione avanti/indietro
    public float strafeAcceleration = 5f; // Accelerazione laterale
    public float yawSpeed = 10f; // Velocità di rotazione (yaw)
    public float pitchSpeed = 10f; // Velocità di inclinazione (pitch)
    public float rollSpeed = 10f; // Velocità di rotazione laterale (roll)

    private Rigidbody rb;

    private Vector3 inputTranslation;
    private float inputYaw;
    private float inputPitch;
    private float inputRoll;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;  // Simula la gravità realistica del drone
    }

    private void Update()
    {
        // Leggi input tastiera per traslazione
        inputTranslation = new Vector3(
            Input.GetAxis("Horizontal"), // A/D per muoversi lateralmente (Strafing)
            0, // L'input verticale sarà gestito separatamente
            Input.GetAxis("Vertical") // W/S per andare avanti/indietro
        );

        // Input per salire e scendere
        if (Input.GetKey(KeyCode.Space))
        {
            inputTranslation.y = 1; // Salire
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            inputTranslation.y = -1; // Scendere
        }

        // Input per rotazione (yaw) con i tasti J (sinistra) e L (destra)
        if (Input.GetKey(KeyCode.J))
        {
            inputYaw = -1; // Ruota a sinistra
        }
        else if (Input.GetKey(KeyCode.L))
        {
            inputYaw = 1; // Ruota a destra
        }
        else
        {
            inputYaw = 0; // Nessuna rotazione
        }

        // Input per inclinazione (pitch) con i tasti I (inclinazione verso l'alto) e K (inclinazione verso il basso)
        if (Input.GetKey(KeyCode.I))
        {
            inputPitch = 1; // Inclinazione verso l'alto
        }
        else if (Input.GetKey(KeyCode.K))
        {
            inputPitch = -1; // Inclinazione verso il basso
        }
        else
        {
            inputPitch = 0; // Nessuna inclinazione
        }

        // Input per rotazione laterale (roll) con i tasti Q (sinistra) e E (destra)
        inputRoll = Input.GetKey(KeyCode.Q) ? -1 : (Input.GetKey(KeyCode.E) ? 1 : 0);
    }

    private void FixedUpdate()
    {
        // Applica movimento traslazione
        Vector3 moveForce = Vector3.zero;

        // Spinta verticale (salire e scendere)
        if (inputTranslation.y != 0)
        {
            moveForce += Vector3.up * throttlePower * inputTranslation.y;
        }

        // Movimento avanti/indietro e laterale (traslazione)
        Vector3 forwardMovement = transform.forward * inputTranslation.z * forwardAcceleration;
        Vector3 strafeMovement = transform.right * inputTranslation.x * strafeAcceleration;
        Vector3 desiredVelocity = forwardMovement + strafeMovement;

        // Limitare la velocità massima
        if (desiredVelocity.magnitude > maxSpeed)
        {
            desiredVelocity = desiredVelocity.normalized * maxSpeed;
        }

        // Applica la forza di traslazione
        rb.AddForce(desiredVelocity, ForceMode.Acceleration);

        // Applica la forza verticale per salire/scendere
        rb.AddForce(Vector3.up * throttlePower * inputTranslation.y, ForceMode.Acceleration);

        // Rotazione (yaw, pitch, roll)
        rb.AddTorque(Vector3.up * yawSpeed * inputYaw, ForceMode.Acceleration); // Ruota sullo yaw
        rb.AddTorque(transform.right * pitchSpeed * inputPitch, ForceMode.Acceleration); // Inclinazione pitch
        rb.AddTorque(transform.forward * rollSpeed * inputRoll, ForceMode.Acceleration); // Rotazione laterale roll
    }
}
