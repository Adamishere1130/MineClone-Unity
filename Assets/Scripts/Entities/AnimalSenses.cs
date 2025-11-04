using UnityEngine;

[DisallowMultipleComponent]
public class AnimalSenses : MonoBehaviour
{
    public AnimalStats stats;

    [Header("Masks")]
    public LayerMask obstacleMask;   // blocks / obstacles
    public LayerMask groundMask;     // ground / blocks

    [Header("References")]
    public Transform eyes;           // eye/head transform; can be self

    public bool SeeTarget(Transform target)
    {
        if (target == null || eyes == null || stats == null) return false;

        Vector3 dir = target.position - eyes.position;
        float dist = dir.magnitude;
        if (dist > stats.viewRadius) return false;

        float ang = Vector3.Angle(eyes.forward, dir);
        if (ang > stats.viewAngle * 0.5f) return false;

        // If ray hits something, verify it is the target
        if (Physics.Raycast(eyes.position, dir.normalized, out var hit, dist, obstacleMask))
            return hit.transform == target;

        return true;
    }

    public bool CliffAhead(float forward = 1.2f, float dropCheck = 1.2f)
    {
        Vector3 probe = transform.position + transform.forward * forward + Vector3.up * 0.1f;
        return !Physics.Raycast(probe, Vector3.down, dropCheck, groundMask);
    }

    public bool ObstacleAhead(float dist = 0.8f)
    {
        if (eyes == null) return false;
        return Physics.Raycast(eyes.position, eyes.forward, dist, obstacleMask);
    }

    public Vector3 Grounded(Vector3 pos)
    {
        if (Physics.Raycast(pos + Vector3.up * 3f, Vector3.down, out var hit, 6f, groundMask))
            return hit.point;
        return pos;
    }

    private void OnDrawGizmosSelected()
    {
        if (stats == null || eyes == null) return;

        Gizmos.color = new Color(1, 0.4f, 0.6f, 0.25f);
        Gizmos.DrawWireSphere(eyes.position, stats.viewRadius);

        // visualize FOV
        Vector3 left  = Quaternion.Euler(0, -stats.viewAngle * 0.5f, 0) * eyes.forward;
        Vector3 right = Quaternion.Euler(0,  stats.viewAngle * 0.5f, 0) * eyes.forward;
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(eyes.position, eyes.position + left  * stats.viewRadius);
        Gizmos.DrawLine(eyes.position, eyes.position + right * stats.viewRadius);
    }
}
