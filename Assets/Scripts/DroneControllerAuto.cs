using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class DroneAgent : Agent
{
    public float throttlePower = 10f; // Potenza per la spinta verticale
    public float yawSpeed = 60f; // Velocità di rotazione (yaw)
    public float pitchSpeed = 45f; // Velocità di inclinazione (pitch)
    public float rollSpeed = 45f; // Velocità di rotazione laterale (roll)
    public float forwardForce = 20f; // Forza per muoversi avanti/indietro

    private Rigidbody rb;
    private Vector3 startingPosition;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        startingPosition = transform.position; // Salva la posizione iniziale del drone
    }

    public override void OnEpisodeBegin()
    {
        // Reset dell'agente e dell'ambiente all'inizio di ogni episodio
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startingPosition;
        transform.rotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Osservazioni: puoi aggiungere variabili come la velocità e la posizione del drone
        sensor.AddObservation(transform.localPosition); // Posizione del drone
        sensor.AddObservation(rb.velocity); // Velocità attuale del drone
        sensor.AddObservation(rb.angularVelocity); // Velocità angolare del drone
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Azioni ricevute dal modello di apprendimento
        float moveUp = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f); // Azione per spinta verticale
        float yaw = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f); // Azione per rotazione yaw
        float pitch = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f); // Azione per inclinazione pitch (avanti/indietro)
        float roll = Mathf.Clamp(actions.ContinuousActions[3], -1f, 1f); // Azione per rotazione roll (laterale)

        // Applica la forza verticale
        rb.AddForce(Vector3.up * throttlePower * moveUp, ForceMode.Acceleration);

        // Applica le rotazioni
        rb.AddTorque(Vector3.up * yawSpeed * yaw, ForceMode.Acceleration); // Rotazione sullo yaw
        rb.AddTorque(transform.right * pitchSpeed * pitch, ForceMode.Acceleration); // Inclinazione pitch (avanti/indietro)
        rb.AddTorque(transform.forward * rollSpeed * roll, ForceMode.Acceleration); // Rotazione roll (laterale)

        // Spinta avanti/indietro in base al pitch
        Vector3 forwardMovement = transform.forward * pitch * forwardForce;
        rb.AddForce(forwardMovement, ForceMode.Acceleration); // Applica la forza in avanti/indietro

        Vector3 rollMovement = transform.right * roll * rollSpeed;
        rb.AddForce(rollMovement, ForceMode.Acceleration); // Applica la forza in avanti/indietro

        // Ricompense e penalità
        float distanceToGround = transform.position.y;
       /* if (distanceToGround < 0.5f)
        {
            SetReward(-1f); // Penalizza se il drone è troppo vicino al suolo
            EndEpisode();
        }
        */
    }

     private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            // Quando tocca il nemico, notifica l'EnvironmentController
            SetReward(-1f); // Penalizza se il drone è troppo vicino al suolo
            EndEpisode();
        }

        else if (collision.gameObject.CompareTag("wall"))
        {
            // Quando tocca un muro, notifica l'EnvironmentController
            SetReward(-0.5f); // Penalizza se il drone è troppo vicino al suolo
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("top"))
        {
            // Quando tocca un muro, notifica l'EnvironmentController
            SetReward(-0.2f); // Penalizza se il drone è troppo vicino al suolo
            EndEpisode();
        }
        else
        {
            SetReward(0.4f); // Ricompensa positiva per rimanere in aria
        }

         
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Implementazione di controlli manuali per testare l'agente
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetKey(KeyCode.Space) ? 1f : (Input.GetKey(KeyCode.LeftControl) ? -1f : 0f); // Spinta verticale
        continuousActions[1] = Input.GetKey(KeyCode.J) ? -1f : (Input.GetKey(KeyCode.L) ? 1f : 0f); // Rotazione yaw
        continuousActions[2] = Input.GetKey(KeyCode.I) ? 1f : (Input.GetKey(KeyCode.K) ? -1f : 0f); // Inclinazione pitch (avanti/indietro)
        continuousActions[3] = Input.GetKey(KeyCode.Q) ? -1f : (Input.GetKey(KeyCode.E) ? 1f : 0f); // Rotazione roll (laterale)
    }
}

