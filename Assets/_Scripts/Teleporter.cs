using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Vector2Int teleportToPosition;
    bool activated = true;
    [SerializeField] bool keepMomentum;

    void Teleport(GameObject obj)
    {
        if (!activated) return;
        if (!keepMomentum)
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null) { rb.linearVelocity = Vector2.zero; }
        }
        obj.transform.position = teleportToPosition.ToV3();
        Debug.Log("Teleport");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if(collision.CompareTag("Player"))
        Teleport(collision.gameObject);
    }
}
