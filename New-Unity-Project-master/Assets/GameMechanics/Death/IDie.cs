using UnityEngine;

public abstract class IDie : MonoBehaviour
{
    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
