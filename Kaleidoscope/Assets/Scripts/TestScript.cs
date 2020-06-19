using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    public class BallScript : MonoBehaviour
    {
        // Constant speed of the ball
        public float speed = 1f;

        // Keep track of the direction in which the ball is moving
        public Vector2 velocity;

        Rigidbody2D rb;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            velocity = Random.insideUnitCircle.normalized * speed;
            rb.velocity = velocity;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            // Normal
            Vector3 N = col.contacts[0].normal;

            //Direction
            Vector3 V = velocity.normalized;

            // Reflection
            Vector3 R = Vector3.Reflect(V, N).normalized;

            // Assign normalized reflection with the constant speed
            var rb = this.GetComponent<Rigidbody2D>();
            velocity = new Vector2(R.x, R.y) * speed;
            rb.velocity = velocity; 
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        var mat = new PhysicsMaterial2D();
        mat.bounciness = 1;
        mat.friction = 0;

        var obj = new GameObject("texture");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        var texture = Resources.Load<Texture2D>("hT0XCMSooWBEY0");
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 1);
        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;

        var rb = obj.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        //rb.sharedMaterial = mat;
        var c = obj.AddComponent<CompositeCollider2D>();

        var borderWidth = 10;
        CreateBorder(obj, new Rect(-borderWidth, -borderWidth, borderWidth, texture.height + 2 * borderWidth), "l");
        CreateBorder(obj, new Rect(texture.width, -borderWidth, borderWidth, texture.height + 2 * borderWidth), "r");
        CreateBorder(obj, new Rect(-borderWidth, texture.height, texture.width + 2 * borderWidth, borderWidth), "t");
        CreateBorder(obj, new Rect(-borderWidth, -borderWidth, texture.width + 2 * borderWidth, borderWidth), "b");

        var box = CreateBox(obj, new Vector3(texture.width * 0.5f, texture.height * 0.5f), new Vector2(100, 100));
        rb = box.GetComponent<Rigidbody2D>();
        //rb.sharedMaterial = mat;
    }

    void CreateBorder(GameObject parent, Rect rect, string name)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent.transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = rect.center;
        var bc = obj.AddComponent<BoxCollider2D>();
        bc.size = rect.size;
        bc.offset = Vector2.zero;
        bc.usedByComposite = true;

    }

    GameObject CreateBox(GameObject parent, Vector2 pos, Vector2 size)
    {
        var box = new GameObject("box");
        box.transform.SetParent(parent.transform);
        box.transform.localScale = Vector3.one;
        box.transform.localPosition = pos;

        var texture = Resources.Load<Texture2D>("arrow");
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);
        var sr = box.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 1;

        var cc = box.AddComponent<CircleCollider2D>();
        cc.radius = (size * 0.5f).magnitude;

        var bc = box.AddComponent<BoxCollider2D>();
        bc.size = size;

        var rb = box.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        //rb.angularDrag = 0;
        //rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.simulated = true;
        //rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        box.AddComponent<BallScript>();

        return box;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
