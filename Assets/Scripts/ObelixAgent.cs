using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;

public class ObelixAgent : Agent
{
    [Header("Prefabs")]
    public GameObject menhirPrefab;
    public GameObject destinationPrefab;

    [Header("Settings")]
    public int spawnAmount = 6;
    public float moveSpeed = 5f;
    public float turnSpeed = 150f;

    private bool hasMenhir = false;
    private int deliveredCount = 0;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    // Voor afstandsbeloningen
    private float lastDistance;

    public override void OnEpisodeBegin()
    {
        hasMenhir = false;
        deliveredCount = 0;

        foreach (var obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();

        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-4f, 4f), 1f, Random.Range(-4f, 4f));
            GameObject m = Instantiate(menhirPrefab, randomPos + transform.parent.position, Quaternion.identity, transform.parent);
            m.tag = "Menhir"; // Zorg dat de tag klopt
            spawnedObjects.Add(m);
        }

        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
            GameObject d = Instantiate(destinationPrefab, randomPos + transform.parent.position, Quaternion.identity, transform.parent);
            d.tag = "Destination"; // Zorg dat de tag klopt
            spawnedObjects.Add(d);
        }

        this.transform.localPosition = new Vector3(0, 0.5f, 0);
        this.transform.localRotation = Quaternion.identity;

        // Initialiseer eerste afstand
        UpdateLastDistance();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(hasMenhir ? 1f : 0f);
        sensor.AddObservation((float)deliveredCount / spawnAmount);
        // TOTAAL SPACE SIZE: 2
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        int moveAction = actionBuffers.DiscreteActions[0];
        int rotateAction = actionBuffers.DiscreteActions[1];

        // Straf voor inactiviteit
        if (moveAction == 0 && rotateAction == 0)
        {
            AddReward(-0.001f); // Kleine straf als hij niks doet
        }

        // Bewegen
        Vector3 move = Vector3.zero;
        if (moveAction == 1) move = transform.forward;
        else if (moveAction == 2) move = -transform.forward;
        transform.Translate(move * Time.deltaTime * moveSpeed, Space.World);

        // Draaien
        float rotation = 0f;
        if (rotateAction == 1) rotation = -1f;
        else if (rotateAction == 2) rotation = 1f;
        transform.Rotate(Vector3.up, rotation * Time.deltaTime * turnSpeed);

        // Shaped Reward: Afstand
        float currentDistance = GetDistanceToTarget();
        if (currentDistance < lastDistance)
        {
            AddReward(0.001f); // Bonus voor dichterbij komen
        }
        else if (currentDistance > lastDistance)
        {
            AddReward(-0.001f); // Straf voor verder weg gaan
        }
        lastDistance = currentDistance;

        // Tijdstraf
        AddReward(-1f / MaxStep);

        // Valstraf
        if (transform.localPosition.y < 0)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    private float GetDistanceToTarget()
    {
        GameObject closest = null;
        float minDist = Mathf.Infinity;
        string targetTag = hasMenhir ? "Destination" : "Menhir";

        // Zoek het dichtstbijzijnde actieve doelwit
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null && obj.activeInHierarchy && obj.CompareTag(targetTag))
            {
                float dist = Vector3.Distance(transform.position, obj.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = obj;
                }
            }
        }
        return minDist;
    }

    private void UpdateLastDistance()
    {
        lastDistance = GetDistanceToTarget();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Menhir") && !hasMenhir)
        {
            hasMenhir = true;
            AddReward(0.5f); // Iets hogere beloning voor oppakken
            collision.gameObject.SetActive(false);
            UpdateLastDistance(); // Reset afstand naar nieuwe target (Destination)
        }

        if (collision.gameObject.CompareTag("Destination") && hasMenhir)
        {
            hasMenhir = false;
            deliveredCount++;
            AddReward(0.8f);
            collision.gameObject.SetActive(false);
            UpdateLastDistance(); // Reset afstand naar nieuwe target (volgende Menhir)

            if (deliveredCount >= spawnAmount)
            {
                SetReward(2.0f); // Bonus voor alles klaar
                EndEpisode();
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? 2 : 0);
        discreteActions[1] = Input.GetKey(KeyCode.A) ? 1 : (Input.GetKey(KeyCode.D) ? 2 : 0);
    }
}