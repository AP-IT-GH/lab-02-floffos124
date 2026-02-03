using UnityEngine;

public class FollowTracker : MonoBehaviour
{
    public Transform tracker;
    public float speed = 5f;
    public float rotSpeed = 5f;
    public float stopDistance = 0.5f; // Nieuw: voorkomt trillen/stoppen bovenop tracker

    void Update()
    {
        if (!tracker) return;

        Vector3 targetPos = tracker.position;
        targetPos.y = transform.position.y;

        Vector3 direction = targetPos - transform.position;
        float distance = direction.magnitude;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                rotSpeed * Time.deltaTime
            );
        }

        // Alleen bewegen als we nog niet bovenop de tracker staan
        if (distance > stopDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.deltaTime
            );
        }
    }
}