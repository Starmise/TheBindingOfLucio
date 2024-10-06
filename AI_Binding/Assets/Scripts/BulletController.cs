using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    public GameObject shooter;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelateDelay());

        int bulletLayer = gameObject.layer;

        // Se comprueban las capas, porque desde el inspector parecen estar siengo ignoradas.
        if (bulletLayer == LayerMask.NameToLayer("BulletPlayer"))
        {
            Physics2D.IgnoreLayerCollision(bulletLayer, LayerMask.NameToLayer("Player"));
            Physics2D.IgnoreLayerCollision(bulletLayer, LayerMask.NameToLayer("BulletEnemy"));
        }
        else if (bulletLayer == LayerMask.NameToLayer("BulletEnemy"))
        {
            Physics2D.IgnoreLayerCollision(bulletLayer, LayerMask.NameToLayer("Enemy"));
            Physics2D.IgnoreLayerCollision(bulletLayer, LayerMask.NameToLayer("BulletPlayer"));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }

    IEnumerator DelateDelay()
    {
        yield return new WaitForSeconds(lifeTime*2);
        Destroy(gameObject);
    }
}
