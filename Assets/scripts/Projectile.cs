using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Projectile : MonoBehaviour {

    public void Setup(Vector3 shootDir, float force, float angularVelocity = -80.0f) {
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(shootDir * force, ForceMode2D.Impulse);
        rigidbody2D.angularVelocity = angularVelocity;
        //Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        // Physics Hit Detection
        Character target = collider.GetComponent<Character>();
        if (target != null) {
            target.TakeDamage(1);
        }
        DestroyProjectileServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyProjectileServerRpc(){
        Destroy(gameObject);
    } 

}
