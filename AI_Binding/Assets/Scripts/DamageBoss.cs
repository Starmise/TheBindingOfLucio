using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBoss : StateMachineBehaviour
{

    private BossNavMesh bossNavMesh;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossNavMesh = animator.GetComponent<BossNavMesh>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (bossNavMesh == null)
            return;

        if (Vector3.Distance(bossNavMesh.player.position, animator.transform.position) < 2f)
        {
            bossNavMesh.health--;
            Debug.Log("Boss health: " + bossNavMesh.health);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsHurt", false);
        if (bossNavMesh == null)
            return;

        if (bossNavMesh.health <= 0)
        {
            Debug.Log("Boss defeated!");
            animator.SetBool("IsDead", true);
        }
    }

}
