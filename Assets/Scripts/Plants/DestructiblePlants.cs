using UnityEngine;
using System;
public class DestructiblePlants : MonoBehaviour
{

    public event EventHandler OnDestructibleTakeDamage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Sword>())
        {
            OnDestructibleTakeDamage?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);


        }
    }

}
