using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CubeAgentPushShaped : Agent
{
    public Transform TargetBlock;
    public Transform GreenZone;
    public float speedMultiplier = 0.5f;
    public float rotationMultiplier = 5f;

    private Rigidbody blockRb;
    private float lastDistance;

    public override void Initialize()
    {
        blockRb = TargetBlock.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset Agent
        this.transform.localPosition = new Vector3(0, 0.5f, 0);
        this.transform.localRotation = Quaternion.identity;

        // Reset Blok
        blockRb.linearVelocity = Vector3.zero;
        blockRb.angularVelocity = Vector3.zero;
        TargetBlock.localPosition = new Vector3(Random.Range(-2f, 2f), 0.5f, Random.Range(-2f, 2f));

        // Start afstand meten tussen blok en zone
        lastDistance = Vector3.Distance(TargetBlock.localPosition, GreenZone.localPosition);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition); // 3
        sensor.AddObservation(TargetBlock.localPosition);    // 3
        sensor.AddObservation(GreenZone.localPosition);      // 3
        // Totaal: 9 observaties (Space Size in Inspector moet naar 9!)
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float moveInput = actionBuffers.ContinuousActions[0];
        float rotateInput = actionBuffers.ContinuousActions[1];

        transform.Translate(transform.forward * moveInput * speedMultiplier, Space.World);
        transform.Rotate(0f, rotateInput * rotationMultiplier, 0f);

        // --- SHAPED REWARD LOGICA ---
        float currentDistance = Vector3.Distance(TargetBlock.localPosition, GreenZone.localPosition);

        // Als het blokje dichterbij de zone komt, geef een kleine bonus
        if (currentDistance < lastDistance)
        {
            AddReward(0.001f);
        }
        else
        {
            AddReward(-0.001f); // Straf als het blokje verder weg gaat
        }
        lastDistance = currentDistance;

        // Straf voor vallen (houdt het binnen de -1 limiet)
        if (this.transform.localPosition.y < 0 || TargetBlock.localPosition.y < 0)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public void Scored()
    {
        // Maximale beloning bij succes
        SetReward(1.0f);
        EndEpisode();
    }
}