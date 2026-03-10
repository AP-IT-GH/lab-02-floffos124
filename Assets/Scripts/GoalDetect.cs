using UnityEngine;

public class GoalDetect : MonoBehaviour
{
    public CubeAgentPushShaped agent; // Sleep de Agent hierin in de Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zone"))
        {
            agent.Scored();
        }
    }
}