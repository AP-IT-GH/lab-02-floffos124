using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class ObelixAgent : Agent
{
    private bool hasMenhir = false;

    public GameObject menhirPrefab;
    public GameObject destinationPrefab;
    private GameObject spawnedMenhir;
    private GameObject spawnedDestination;

    // Voor afstandsberekening
    private float lastDistance;

    public override void OnEpisodeBegin()
    {
        hasMenhir = false;

        if (spawnedMenhir != null) Destroy(spawnedMenhir);
        if (spawnedDestination != null) Destroy(spawnedDestination);

        // Spawn logica (behouden zoals je had)
        Vector3 randomPosMenhir = new Vector3(Random.Range(-4f, 4f), 1f, Random.Range(-4f, 4f));
        spawnedMenhir = Instantiate(menhirPrefab, randomPosMenhir + transform.parent.position, Quaternion.identity, transform.parent);

        Vector3 randomPosDest = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
        spawnedDestination = Instantiate(destinationPrefab, randomPosDest + transform.parent.position, Quaternion.identity, transform.parent);

        this.transform.localPosition = new Vector3(0, 0.5f, 0);

        // Initialiseer afstand tot de eerste target (de menhir)
        lastDistance = Vector3.Distance(transform.position, spawnedMenhir.transform.position);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 1. Draagt hij de menhir? (1 observation)
        sensor.AddObservation(hasMenhir ? 1f : 0f);

        // 2. Waar is het doelwit ten opzichte van Obelix? (3 observations)
        Vector3 targetPos = hasMenhir ? spawnedDestination.transform.position : spawnedMenhir.transform.position;
        sensor.AddObservation((targetPos - transform.position).normalized);

        // Totaal Space Size in Unity nu: 4! (Pas dit aan in je Behavior Parameters)
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // --- Beweging Logica ---
        int moveAction = actionBuffers.DiscreteActions[0];
        Vector3 move = Vector3.zero;
        if (moveAction == 1) move = transform.forward;
        else if (moveAction == 2) move = -transform.forward;
        transform.Translate(move * Time.deltaTime * 5f, Space.World);

        int rotateAction = actionBuffers.DiscreteActions[1];
        float rotation = 0f;
        if (rotateAction == 1) rotation = -1f;
        else if (rotateAction == 2) rotation = 1f;
        transform.Rotate(Vector3.up, rotation * Time.deltaTime * 150f);

        // --- Slimme Beloningen ---

        // 1. Val-straf
        if (transform.localPosition.y < 0)
        {
            SetReward(-1.0f);
            EndEpisode();
        }

        // 2. Afstands-beloning (Heuristiek)
        Vector3 targetPos = hasMenhir ? spawnedDestination.transform.position : spawnedMenhir.transform.position;
        float currentDistance = Vector3.Distance(transform.position, targetPos);

        if (currentDistance < lastDistance)
        {
            AddReward(0.001f); // Kleine aanmoediging voor de juiste richting
        }
        else
        {
            AddReward(-0.001f); // Kleine straf voor weglopen
        }
        lastDistance = currentDistance;

        // 3. Tijdstraf (behouden)
        if (MaxStep > 0) AddReward(-1f / MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? 2 : 0);
        discreteActions[1] = Input.GetKey(KeyCode.A) ? 1 : (Input.GetKey(KeyCode.D) ? 2 : 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Menhir") && !hasMenhir)
        {
            hasMenhir = true;
            AddReward(0.5f); // Beloning voor oppakken verhoogd
            collision.gameObject.SetActive(false);
            // Reset afstandsberekening voor het nieuwe doel (Destination)
            lastDistance = Vector3.Distance(transform.position, spawnedDestination.transform.position);
        }

        if (collision.gameObject.CompareTag("Destination") && hasMenhir)
        {
            SetReward(1.5f); // Bonus voor voltooiing
            EndEpisode();
        }
    }
}