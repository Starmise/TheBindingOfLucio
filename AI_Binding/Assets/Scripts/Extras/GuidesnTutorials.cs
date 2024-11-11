using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidesnTutorials : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        // Recursos usados al investigar
        /*
         * Primer Parcial
         * https://discussions.unity.com/t/what-are-ienumerator-and-coroutine/143510/2
         * https://gamedev.stackexchange.com/questions/114121/most-efficient-way-to-convert-vector3-to-vector2
         * https://www.youtube.com/watch?v=EMMcgAgkipk&t=5s
         * https://www.youtube.com/watch?v=EWo3tAG-iAg
         * https://docs.unity3d.com/ScriptReference/Physics2D.Raycast.html
         * https://docs.unity3d.com/2022.3/Documentation/Manual/script-Physics2DRaycaster.html
         * https://discussions.unity.com/t/how-can-i-compare-colliders-layer-to-my-layermask-type-variable/66721
         * https://docs.unity3d.com/ScriptReference/ColorUtility.TryParseHtmlString.html
         * https://docs.unity3d.com/Manual/LayerBasedCollision.html
         * 
         * Segundo Parcial
         * https://youtu.be/HRX0pUSucW4?si=s5wpXSEkHMref0-t
         * https://github.com/h8man/NavMeshPlus/wiki/HOW-TO#bake-at-runtime
         * https://docs.unity3d.com/ScriptReference/Rigidbody2D-gravityScale.html //solo para comprobar
         * https://www.reddit.com/r/Unity3D/comments/izbvr7/navmesh_with_dynamically_created_obstacles/?rdt=63677
         * 
         */

        /*
         * Se intentó implementar obstáculos en movimiento, pero no se encontró nada en la
         * documentación o guías de NavMeshPlus. Este era el código que se intentó usar pero
         * no funcionó:
         * public NavMeshSurface2d navMeshSurface;
         * public Transform movingObstacle;
         * public float updateInt = 1f; 
         * 
         * private float timeSinceLastUpdate;
         * 
         * void Update()
         * {
         * timeSinceLastUpdate += Time.deltaTime;
         * 
         * if (timeSinceLastUpdate >= updateInt) {
         *  navMeshSurface.BuildNavMesh();
         *  timeSinceLastUpdate = 0f;
         * }
         * }
         */
    }
}
