using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameScript game;

    void OnCollisionEnter2D(Collision2D collision)
    {
        var c = collision.collider.GetComponent<DontTouchComponent>();
        if (c != null)
        {
            game.OnFailed();
        }
    }
}
