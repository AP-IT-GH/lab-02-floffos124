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

        // Alleen horizontale richting
        Vector3 targetPos = target.position;
        targetPos.y = transform.position.y;

        Vector3 direction = targetPos - transform.position;

        // Rotatie
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                rotSpeed * Time.deltaTime
            );
        }

        // Beweging zonder vliegen
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        // Check waypoint bereikt
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
