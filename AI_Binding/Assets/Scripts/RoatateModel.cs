using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoatateModel : StateMachineBehaviour
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Rotate the model to face the player
        Vector3 direction = (animator.GetComponent<BossNavMesh>().player.position - animator.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

}
