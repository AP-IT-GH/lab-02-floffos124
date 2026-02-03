using UnityEngine;

public class WaypointTracker : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 6f;
    public float waypointDistance = 1f;
    public bool loop = true;

    private int currentWaypoint = 0;

    void Update()
    {
        if (waypoints.Length == 0) return;

        Vector3 targetPos = waypoints[currentWaypoint].position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < waypointDistance)
        {
            currentWaypoint++;

            if (currentWaypoint >= waypoints.Length)
            {
                if (loop)
                    currentWaypoint = 0;
                else
                    enabled = false;
            }
        }
    }
}
