using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeState : MonoBehaviour 
{
    public enum MeleeSubstate
    {
        BasicAttack,
        Dash,
        AreaAttack
    }

    private MeleeSubstate _meleeSubstate = MeleeSubstate.BasicAttack; 
    private List<MeleeSubstate> _substateHistory = new List<MeleeSubstate>();
    private BossEnemy owner;
    private int basicAttackCount = 0;
    private int dashAttackCount = 0;
    private bool SubstateEntered = false;
    public Collider2D attackRangeCollider; // Reference to the 2D Box Collider
    public Animator animator;

    public GameObject player;
    public void OnEnter()
    {
        OnEnter();
        Setup();
    }

    public void Setup()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player GameObject with tag 'Player' not found.");
            return;
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component is missing on the GameObject.");
        }

        if (owner == null)
        {
            owner = GetComponent<BossEnemy>();
            if (owner == null)
            {
                Debug.LogError("BossEnemy component is missing on the GameObject.");
                return;
            }
        }

        // Ensure the collider is correctly assigned
        if (attackRangeCollider == null)
        {
            Debug.LogError("No Collider2D assigned for attack range.");
        }
        else if (!attackRangeCollider.isTrigger)
        {
            Debug.LogWarning("Collider2D for attack range is not set as a trigger.");
        }
    }

    public void Update()
    {
        if (animator.GetBool("IsDead") != true)
        {
            switch (_meleeSubstate)
            {
                case MeleeSubstate.BasicAttack:
                    HandleBasicAttack(owner);
                    break;

                case MeleeSubstate.Dash:
                    HandleDashAttack(owner);
                    break;

                case MeleeSubstate.AreaAttack:
                    HandleAreaAttack(owner);
                    break;
            }
        }
    }

    private void HandleBasicAttack(BossEnemy bossOwner)
    {
        if (IsPlayerInRange())
        {
            if (!SubstateEntered)
            {
                SubstateEntered = true;
                StartCoroutine(BasicAttack());
            }

            if (basicAttackCount >= 2)
            {
                TransitionToSubstate(MeleeSubstate.Dash);
            }
        }
    }

    private void HandleDashAttack(BossEnemy bossOwner)
    {
        if (IsPlayerInRange())
        {
            if (!SubstateEntered && basicAttackCount >= 2) 
            {
                SubstateEntered = true;
                StartCoroutine(DashAttack());
            }

            if (dashAttackCount >= 2)
            {
                TransitionToSubstate(MeleeSubstate.AreaAttack);
            }
        }
    }

    private void HandleAreaAttack(BossEnemy bossOwner)
    {
        if (IsPlayerInRange())
        {
            if (!SubstateEntered && dashAttackCount >= 2)
            {
                SubstateEntered = true;
                StartCoroutine(AreaAttack());
            }

            TransitionToSubstate(MeleeSubstate.BasicAttack);
            basicAttackCount = 0;
            dashAttackCount = 0;
        }
    }

    private void TransitionToSubstate(MeleeSubstate newSubstate)
    {
        _substateHistory.Add(_meleeSubstate);
        _meleeSubstate = newSubstate;
        SubstateEntered = false;
    }

    private bool IsPlayerInRange()
    {
        if (attackRangeCollider == null || player == null)
        {
            Debug.LogWarning("Attack range collider or player GameObject is not assigned.");
            return false;
        }

        // Check if the player's collider is overlapping with the attack range
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            Debug.LogWarning("Player does not have a Collider2D component.");
            return false;
        }

        return attackRangeCollider.IsTouching(playerCollider);
    }

    IEnumerator BasicAttack()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned.");
            yield break;
        }

        animator.SetBool("IsMoving", false);
        animator.SetBool("IsAttacking", true);
        animator.Play("Attack_Melee");
        if (player.TryGetComponent<Health>(out Health health))
        {
            health.DamagePlayer(1); 
        }
        yield return new WaitForSeconds(1);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsMoving", true);
        basicAttackCount++;
        SubstateEntered = false; // Reset flag
    }

    IEnumerator DashAttack()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned.");
            yield break;
        }

        animator.SetBool("IsMoving", false);
        animator.SetBool("IsAttacking", true);
        animator.Play("DashMelee");
        if (player.TryGetComponent<Health>(out Health health))
        {
            health.DamagePlayer(1); 
        }
        yield return new WaitForSeconds(2);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsMoving", true);
        dashAttackCount++;
        SubstateEntered = false; // Reset flag
    }

    IEnumerator AreaAttack()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned.");
            yield break;
        }

        animator.SetBool("IsMoving", false);
        animator.SetBool("IsAttacking", true);
        animator.Play("AreaMelee");
        if (player.TryGetComponent<Health>(out Health health))
        {
            health.DamagePlayer(1); 
        }
        yield return new WaitForSeconds(2);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsMoving", true);
        SubstateEntered = false; // Reset flag
    }
}
