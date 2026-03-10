using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CubeAgentRays : Agent
{
    public Transform Target;
    public float speedMultiplier = 0.5f;
    public float rotationMultiplier = 5f;

    public override void OnEpisodeBegin()
    {
        // Reset agent positie en rotatie
        this.transform.localPosition = new Vector3(0, 0.5f, 0);
        this.transform.localRotation = Quaternion.identity;

        // Reset target naar een willekeurige plek op het platform (bijv. tussen -4 en 4)
        Target.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Alleen de eigen positie observeren (3 observaties)
        // De target wordt geobserveerd via de Ray Perception Sensor component in de Inspector
        sensor.AddObservation(this.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Acties uitlezen (ContinuousActions)
        float moveInput = actionBuffers.ContinuousActions[0]; // Vooruit/Achteruit
        float rotateInput = actionBuffers.ContinuousActions[1]; // Rotatie rond Y-as

        // Bewegen
        Vector3 move = transform.forward * moveInput * speedMultiplier;
        transform.position += move;

        // Roteren
        transform.Rotate(0f, rotateInput * rotationMultiplier, 0f);

        // Beloningen berekenen
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Target bereikt
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Van het platform gevallen?
        if (this.transform.localPosition.y < 0)
        {
            SetReward(-1.0f); // Straf voor vallen
            EndEpisode();
        }
    }

    // Hiermee kun je zelf met de pijltjestoetsen of WASD testen
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
    }
}