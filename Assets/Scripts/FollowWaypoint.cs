using UnityEngine;

public class FollowWaypoint : MonoBehaviour
{
    public Transform[] waypoints;   // Alle punten: Start -> Path -> End
    public float speed = 5f;
    public float rotSpeed = 5f;
    public float waypointDistance = 1f;
    public bool loop = true;         // Zet uit als je wil stoppen bij End

    private int currentWaypoint = 0;

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];

        // Richting naar waypoint
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        // Smooth rotation
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                Time.deltaTime * rotSpeed
            );
        }

        // Voorwaartse beweging
        transform.Translate(0, 0, speed * Time.deltaTime);

        // Check afstand tot waypoint
        if (Vector3.Distance(transform.position, target.position) < waypointDistance)
        {
            currentWaypoint++;

            if (currentWaypoint >= waypoints.Length)
            {
                if (loop)
                    currentWaypoint = 0;
                else
                    enabled = false; // stopt bij End
            }
        }
    }
}
