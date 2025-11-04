using UnityEngine;
using System.Collections;

public enum AnimalState { Idle, Wander, Graze, Flee }

[RequireComponent(typeof(CharacterController))]
public class AnimalAgent : MonoBehaviour
{
    [Header("Config / References")]
    public AnimalStats stats;
    public AnimalSenses senses;
    public Transform threat;

    private CharacterController _cc;
    private AnimalState _state;
    private Coroutine _fsm;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        if (senses == null) senses = GetComponent<AnimalSenses>();
        if (senses != null && senses.eyes == null) senses.eyes = transform;

        // Auto find player by tag
        if (threat == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player) threat = player.transform;
        }
    }

    private void OnEnable()
    {
        _fsm = StartCoroutine(FSM());
    }

    private void OnDisable()
    {
        if (_fsm != null) StopCoroutine(_fsm);
    }

    IEnumerator FSM()
    {
        _state = AnimalState.Idle;
        while (true)
        {
            switch (_state)
            {
                case AnimalState.Idle:   yield return Idle();   break;
                case AnimalState.Wander: yield return Wander(); break;
                case AnimalState.Graze:  yield return Graze();  break;
                case AnimalState.Flee:   yield return Flee();   break;
            }
        }
    }

    IEnumerator Idle()
    {
        float t = Random.Range(stats.minIdleTime, stats.maxIdleTime);
        while (t > 0f)
        {
            if (ShouldFlee()) { _state = AnimalState.Flee; yield break; }
            t -= Time.deltaTime;
            yield return null;
        }
        _state = (Random.value < 0.6f) ? AnimalState.Wander : AnimalState.Graze;
    }

    IEnumerator Wander()
    {
        float dur = Random.Range(2f, 4f);
        float t = 0f;
        float yaw = Random.Range(-90f, 90f);

        while (t < dur)
        {
            if (ShouldFlee()) { _state = AnimalState.Flee; yield break; }

            transform.Rotate(0f, yaw * Time.deltaTime * 0.5f, 0f);

            if (!senses.CliffAhead() && !senses.ObstacleAhead())
                _cc.SimpleMove(transform.forward * stats.walkSpeed);

            t += Time.deltaTime;
            yield return null;
        }
        _state = AnimalState.Idle;
    }

    IEnumerator Graze()
    {
        float dur = Random.Range(2f, 5f);
        float t = 0f;

        while (t < dur)
        {
            if (ShouldFlee()) { _state = AnimalState.Flee; yield break; }

            transform.Rotate(0f, Random.Range(-10f, 10f) * Time.deltaTime, 0f);
            t += Time.deltaTime;
            yield return null;
        }
        _state = AnimalState.Idle;
    }

    IEnumerator Flee()
    {
        float dur = Random.Range(2f, 3f);
        float t = 0f;

        while (t < dur)
        {
            if (threat != null)
            {
                Vector3 away = (transform.position - threat.position);
                away.y = 0f;
                if (away.sqrMagnitude > 0.001f)
                {
                    Quaternion target = Quaternion.LookRotation(away.normalized, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        target,
                        stats.turnSpeedDegPerSec * Time.deltaTime);
                }
            }

            if (!senses.CliffAhead() && !senses.ObstacleAhead())
                _cc.SimpleMove(transform.forward * stats.runSpeed);

            t += Time.deltaTime;
            yield return null;
        }
        _state = AnimalState.Idle;
    }

    private bool ShouldFlee()
    {
        return (stats != null && senses != null && threat != null && senses.SeeTarget(threat));
    }
}
