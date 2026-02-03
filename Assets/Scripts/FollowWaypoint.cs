using UnityEngine;

public class FollowWaypoint : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;
    public float rotSpeed = 5f;
    public float waypointDistance = 1f;
    public bool loop = true;

    private int currentWaypoint = 0;

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];

        // Doelpositie op dezelfde hoogte houden
        Vector3 targetPos = target.position;
        targetPos.y = transform.position.y;

        // Richting berekenen
        Vector3 direction = targetPos - transform.position;

        // Smooth draaien
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                rotSpeed * Time.deltaTime
            );
        }

        // Bewegen naar waypoint
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        // Check of waypoint bereikt of voorbij gereden
        Vector3 toWaypoint = targetPos - transform.position;

        if (toWaypoint.magnitude < waypointDistance ||
            Vector3.Dot(transform.forward, toWaypoint) < 0)
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
