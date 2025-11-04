using UnityEngine;
using System.Collections;

public enum AnimalState { Idle, Wander, Graze, Flee }

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class AnimalAgent : MonoBehaviour
{
    [Header("Config / References")]
    public AnimalStats  stats;
    public AnimalSenses senses;
    public Transform    threat;          // usually the player

    protected CharacterController _cc;
    protected AnimalState _state;
    protected Coroutine _fsm;

    // --------- tunable virtual hooks (species can override) ---------
    protected virtual float WanderYawRangeDeg => 90f;    // ±deg per sec jitter base
    protected virtual float GrazeYawJitterDeg => 10f;    // small head sway during graze
    protected virtual float ObstacleAvoidYawDeg => 120f; // quick turn when obstacle/cliff
    protected virtual float FleeSpeedMultiplier => 1.0f; // species may flee faster/slower
    // ----------------------------------------------------------------

    protected virtual void Awake()
    {
        _cc = GetComponent<CharacterController>();
        if (senses == null) senses = GetComponent<AnimalSenses>();
        if (senses != null && senses.eyes == null) senses.eyes = transform;

        if (threat == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player) threat = player.transform;
        }
    }

    protected virtual void OnEnable()  { _fsm = StartCoroutine(FSM()); }
    protected virtual void OnDisable() { if (_fsm != null) StopCoroutine(_fsm); }

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

    // ------------------------- States -------------------------
    protected virtual IEnumerator Idle()
    {
        float t = Random.Range(stats.minIdleTime, stats.maxIdleTime);
        while (t > 0f)
        {
            if (ShouldFlee()) { _state = AnimalState.Flee; yield break; }
            t -= Time.deltaTime;
            yield return null;
        }
        _state = (Random.value < stats.grazeChance) ? AnimalState.Graze : AnimalState.Wander;
    }

    protected virtual IEnumerator Wander()
    {
        float dur = Random.Range(2f, 4f);
        float t = 0f;
        float yaw = Random.Range(-WanderYawRangeDeg, WanderYawRangeDeg);

        while (t < dur)
        {
            if (ShouldFlee()) { _state = AnimalState.Flee; yield break; }

            // avoid obstacle/cliff with a quick turn
            if (senses.CliffAhead() || senses.ObstacleAhead())
                transform.Rotate(0f, ObstacleAvoidYawDeg * Time.deltaTime, 0f);
            else
                transform.Rotate(0f, yaw * Time.deltaTime * 0.5f, 0f);

            _cc.SimpleMove(transform.forward * stats.walkSpeed);
            t += Time.deltaTime;
            yield return null;
        }
        _state = AnimalState.Idle;
    }

    protected virtual IEnumerator Graze()
    {
        float dur = Random.Range(2f, 5f);
        float t = 0f;

        while (t < dur)
        {
            if (ShouldFlee()) { _state = AnimalState.Flee; yield break; }

            // gentle sway
            transform.Rotate(0f, Random.Range(-GrazeYawJitterDeg, GrazeYawJitterDeg) * Time.deltaTime, 0f);
            t += Time.deltaTime;
            yield return null;
        }
        _state = AnimalState.Idle;
    }

    protected virtual IEnumerator Flee()
    {
        float dur = Random.Range(stats.fleeDurationRange.x, stats.fleeDurationRange.y);
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
                        transform.rotation, target, stats.turnSpeedDegPerSec * Time.deltaTime);
                }
            }

            if (!(senses.CliffAhead() || senses.ObstacleAhead()))
                _cc.SimpleMove(transform.forward * (stats.runSpeed * FleeSpeedMultiplier));

            t += Time.deltaTime;
            yield return null;
        }
        _state = AnimalState.Idle;
    }
    // ---------------------------------------------------------

    // *** FIX: make this virtual so subclasses (e.g., PigAgent) can override ***
    protected virtual bool ShouldFlee()
    {
        return (stats != null && senses != null && threat != null && senses.SeeTarget(threat));
    }
}

// ===================== Pig (species) in SAME file =====================
public class PigAgent : AnimalAgent
{
    // Pigs never flee
    protected override bool ShouldFlee() => false;

    // Optional tweaks
    protected override float WanderYawRangeDeg => 80f;
    protected override float GrazeYawJitterDeg => 12f;
    protected override float ObstacleAvoidYawDeg => 140f;
    protected override float FleeSpeedMultiplier => 1.0f;
}
// ======================================================================
