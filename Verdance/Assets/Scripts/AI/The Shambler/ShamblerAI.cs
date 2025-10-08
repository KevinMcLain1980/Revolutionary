using UnityEngine;
using System.Collections;

public class ShamblerAI : MonoBehaviour
{
    private enum ShamblerState { Idle, Wander, Charge }

    [Header("Movement")]
    [SerializeField] private float wanderSpeed = 1f;
    [SerializeField] private float chargeSpeed = 4f;
    [SerializeField] private float lungeDelay = 0.5f;
    [SerializeField] private float minPaceDuration = 2f;
    [SerializeField] private float maxPaceDuration = 5f;

    [Header("Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask playerLayer;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    private ShamblerState currentState;
    private bool isDead = false;
    private bool isTwitching = false;
    private bool isStaggering = false;

    private void Start()
    {
        animator.SetTrigger("IsSpawning");
        currentState = ShamblerState.Idle;
        StartCoroutine(PaceRoutine());
    }

    private void Update()
    {
        if (isDead) return;

        DetectPlayer();

        switch (currentState)
        {
            case ShamblerState.Idle:
                rb.linearVelocity = Vector2.zero;
                break;

            case ShamblerState.Wander:
                rb.linearVelocity = new Vector2(wanderSpeed * Mathf.Sign(transform.localScale.x), rb.linearVelocity.y);
                break;

            case ShamblerState.Charge:
                Vector2 direction = (player.position - transform.position).normalized;
                rb.linearVelocity = new Vector2(direction.x * chargeSpeed, rb.linearVelocity.y);
                break;
        }

        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsTwitching", isTwitching);
        animator.SetBool("IsStaggering", isStaggering);
    }

    private IEnumerator PaceRoutine()
    {
        while (!isDead)
        {
            currentState = Random.value > 0.5f ? ShamblerState.Idle : ShamblerState.Wander;
            animator.SetTrigger(currentState == ShamblerState.Idle ? "IdleTrigger" : "WalkTrigger");

            float duration = Random.Range(minPaceDuration, maxPaceDuration);
            yield return new WaitForSeconds(duration);

            if (currentState == ShamblerState.Wander)
                FlipDirection();
        }
    }

    private void DetectPlayer()
    {
        if (currentState == ShamblerState.Charge) return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (hit != null && hit.transform == player)
        {
            StopAllCoroutines();
            StartCoroutine(ChargeRoutine());
        }
    }

    private IEnumerator ChargeRoutine()
    {
        currentState = ShamblerState.Charge;
        animator.SetTrigger("ChargeTrigger");
        yield return new WaitForSeconds(lungeDelay);
    }

    private void FlipDirection()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void TakeDamage()
    {
        if (isDead) return;

        animator.SetTrigger("IsHurt");
        isTwitching = Random.value > 0.5f;
        isStaggering = !isTwitching;
    }

    public void Die()
    {
        isDead = true;
        animator.SetBool("IsDead", true);
        rb.linearVelocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
