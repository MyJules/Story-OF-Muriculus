using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class MovableBox : MonoBehaviour, IGrabbable
{
    [SerializeField]
    private GameObject grabPosition;
    private Rigidbody2D rigidbody;
    private GameObject startParent;
    private Collider2D collider;
    private Vector3 defaultScale;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        startParent = gameObject.transform.parent.gameObject;
        defaultScale = transform.localScale;
        collider = GetComponent<Collider2D>();
    }
    public void Grab()
    {
        collider.enabled = false;
        rigidbody.bodyType = RigidbodyType2D.Kinematic;
        transform.parent = grabPosition.transform;
        transform.position = grabPosition.transform.position;
    }
    public void Release()   
    {
        float thrust = 600f;
        int direction;
        if (transform.parent.rotation.y >= 0)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }
        Vector2 releaseVelocity = new Vector2(direction, 1);
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        transform.parent = startParent.transform;
        rigidbody.AddForce(releaseVelocity * thrust, ForceMode2D.Impulse);
        collider.enabled = true;
        transform.rotation = Quaternion.identity;
        transform.localScale = defaultScale;
    }
}
