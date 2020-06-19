using UnityEngine;

public class KaleidoscopeAnimator : MonoBehaviour
{
    public KaleidoscopeSprite kaleidoscopeSprite = null;

    public class BallScript : MonoBehaviour
    {
        // Constant speed of the ball
        public float speed = 50f;

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

    [SerializeField]
    Texture2D _texture;
    public Texture2D texture {
        get {
            return _texture;
        }
        set {
            _texture = value;
            UpdateTexture();
        }
    }

    [SerializeField]
    Vector2Int _windowSize;
    public Vector2Int windowSize {
        get {
            return _windowSize;
        }
        set {
            _windowSize = value;
            UpdateWindow();
        }
    }

    [SerializeField]
    float _speed = 50;
    public float speed {
        get {
            return _speed;
        }
        set {
            _speed = value;
            var bs = window.GetComponent<BallScript>();
            bs.speed = value;
            bs.velocity = bs.velocity.normalized * bs.speed;
            var rb = window.GetComponent<Rigidbody2D>();
            rb.velocity = bs.velocity;
        }
    }

    [SerializeField]
    float _angularVelocity = 0;
    public float angularVelocity {
        get {
            return _angularVelocity;
        }
        set {
            _angularVelocity = value;
            var rb = window.GetComponent<Rigidbody2D>();
            rb.angularVelocity = value;
        }
    }

    public KaleidoscopeAnimator()
    {
        _windowSize = new Vector2Int(100, 100);
    }

    void Start()
    {
        UpdateTexture();
    }

    GameObject window;
    GameObject root;
    Texture2D internalTexture; // for GetPixels
    Texture2D windowTexture;
    Vector2Int lastWindowPosition;
    void UpdateTexture()
    {
        if (root != null)
        {
            Object.Destroy(root);
        }

        if (texture == null)
        {
            internalTexture = null;
            return;
        }

        root = new GameObject("KaleidoscopeTexture");
        root.transform.localPosition = Vector3.zero;
        root.transform.localScale = Vector3.one;
        internalTexture = new Texture2D(texture.width, texture.height, texture.format, false);
        //Graphics.ConvertTexture(texture, internalTexture);
        Graphics.CopyTexture(texture, internalTexture);
        var sprite = Sprite.Create(internalTexture, new Rect(0, 0, internalTexture.width, internalTexture.height), new Vector2(0, 0), 1);
        var sr = root.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;

        var rb = root.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        var c = root.AddComponent<CompositeCollider2D>();

        var borderWidth = 10;
        CreateBorder(root, new Rect(-borderWidth, -borderWidth, borderWidth, internalTexture.height + 2 * borderWidth), "l");
        CreateBorder(root, new Rect(internalTexture.width, -borderWidth, borderWidth, internalTexture.height + 2 * borderWidth), "r");
        CreateBorder(root, new Rect(-borderWidth, internalTexture.height, internalTexture.width + 2 * borderWidth, borderWidth), "t");
        CreateBorder(root, new Rect(-borderWidth, -borderWidth, internalTexture.width + 2 * borderWidth, borderWidth), "b");

        CreateWindow(root, new Vector3(internalTexture.width * 0.5f, internalTexture.height * 0.5f));
    }

    void CreateBorder(GameObject parent, Rect rect, string name)
    {
        //var obj = new GameObject(name);
        //obj.transform.SetParent(parent.transform);
        //obj.transform.localScale = Vector3.one;
        //obj.transform.localPosition = rect.center;
        var bc = parent.AddComponent<BoxCollider2D>();
        bc.size = rect.size;
        bc.offset = rect.center;
        bc.usedByComposite = true;

    }

    void CreateWindow(GameObject parent, Vector2 pos)
    {
        window = new GameObject("box");
        window.transform.SetParent(parent.transform);
        window.transform.localScale = Vector3.one;
        window.transform.localPosition = pos;

        var texture = Resources.Load<Texture2D>("arrow");
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);
        var sr = window.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 1;

        window.AddComponent<CircleCollider2D>();
        window.AddComponent<BoxCollider2D>();
        UpdateWindow();

        var rb = window.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.simulated = true;
        rb.angularVelocity = angularVelocity;

        window.AddComponent<BallScript>().speed = speed;

        lastWindowPosition = Vector2Int.left;
    }

    void UpdateWindow()
    {
        if (window != null)
        {
            var cc = window.GetComponent<CircleCollider2D>();
            cc.radius = windowSize.magnitude * 0.6f;

            var bc = window.GetComponent<BoxCollider2D>();
            bc.size = windowSize;

            windowTexture = new Texture2D(windowSize.x, windowSize.y);
        }
    }

    void Update()
    {
        if (internalTexture != null)
        {
            Vector2Int pos = Vector2Int.RoundToInt(window.transform.localPosition);
            if (pos != lastWindowPosition)
            {
                var colors = internalTexture.GetPixels(pos.x - windowSize.x / 2, pos.y - windowSize.y / 2, windowSize.x, windowSize.y);
                windowTexture.SetPixels(colors);
                windowTexture.Apply();
                lastWindowPosition = pos;

                if (kaleidoscopeSprite != null)
                {
                    kaleidoscopeSprite.texture = windowTexture;
                    kaleidoscopeSprite.rotation = window.transform.localEulerAngles;
                }
            }
        }
    }
}
