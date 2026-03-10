using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CubeAgentPush : Agent
{
    public Transform TargetBlock;
    public Transform GreenZone;
    public float speedMultiplier = 0.5f;
    public float rotationMultiplier = 5f;

    private Rigidbody blockRb;

    public override void Initialize()
    {
        blockRb = TargetBlock.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset Agent
        this.transform.localPosition = new Vector3(0, 0.5f, 0);
        this.transform.localRotation = Quaternion.identity;

        // Reset Blok (Target)
        blockRb.linearVelocity = Vector3.zero;
        blockRb.angularVelocity = Vector3.zero;
        TargetBlock.localPosition = new Vector3(Random.Range(-2f, 2f), 0.5f, Random.Range(-2f, 2f));

        // Zone blijft op vaste plek staan (zoals eerder afgesproken)
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 1. Positie Agent (3)
        sensor.AddObservation(this.transform.localPosition);
        // 2. Positie van het Blok (3) - De agent moet weten waar het blok is om te duwen
        sensor.AddObservation(TargetBlock.localPosition);

        // Totaal Vector Observation Space Size in Inspector moet nu op 6 staan!
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float moveInput = actionBuffers.ContinuousActions[0];
        float rotateInput = actionBuffers.ContinuousActions[1];

        transform.Translate(transform.forward * moveInput * speedMultiplier, Space.World);
        transform.Rotate(0f, rotateInput * rotationMultiplier, 0f);

        // Straf voor vallen
        if (this.transform.localPosition.y < 0 || TargetBlock.localPosition.y < 0)
        {
            SetReward(-1.0f);
            EndEpisode();
        }

        // Kleine negatieve reward per stap om snelheid te stimuleren
        AddReward(-1f / MaxStep);
    }

    // We checken nu of het BLOKJE de zone raakt, niet de agent
    public void Scored()
    {
        SetReward(1.0f);
        EndEpisode();
    }
}