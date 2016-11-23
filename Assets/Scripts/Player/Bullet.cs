using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public float damage;

    void Update()
    {     
        //Destroy Bullet
        Destroy(gameObject, 3.0f); //Destroy after time 
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag != ("PlayerBullet"))
        {
            Destroy(gameObject);
        }
    }
}
