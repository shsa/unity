using UnityEngine;
using Unity.Entities;
using Unity.Transforms;


/// <summary>
/// Handles player shooting
/// </summary>
public class PlayerWeapon : MonoBehaviour
{

    [Header("Specs")]

    // time between shots
    [SerializeField] private float rateOfFire = 0.15f;

    // where the weapon's bullet appears
    [SerializeField] private Transform muzzleTransform = null;

    // GameObject prefab 
    [SerializeField] private GameObject bulletPrefab = null;

    [Header("Effects")]
    [SerializeField] private AudioSource soundFXSource = null;
    // reference to the current World's EntityManager
    private EntityManager entityManager;

    // prefab converted into an entityPrefab
    private Entity bulletEntityPrefab;

    // timer until weapon and shoot again
    private float shotTimer;

    // is the fire button held down?
    private bool isFireButtonDown;
    public bool IsFireButtonDown { get { return isFireButtonDown; } set { isFireButtonDown = value; } }

    protected virtual void Start()
    {
        // get reference to current EntityManager
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // create entity prefab from the game object prefab, using default conversion settings
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        bulletEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulletPrefab, settings);
    }

    public void FireBulletNonECS()
    {
        // instantiates a GameObject prefab to fire a bullet
        GameObject instance = Instantiate(bulletPrefab, muzzleTransform.position, muzzleTransform.rotation, null);

        // plays one-shot sound (pew pew pew!)
        soundFXSource?.Play();
    }

    public virtual void FireBullet()
    {
        // create an entity based on the entity prefab
        Entity bullet = entityManager.Instantiate(bulletEntityPrefab);

        // set it to the muzzle angle and position
        entityManager.SetComponentData(bullet, new Translation { Value = muzzleTransform.position });
        entityManager.SetComponentData(bullet, new Rotation { Value = muzzleTransform.rotation });

        // plays one-shot sound (pew pew pew!)
        soundFXSource?.Play();
    }

    protected virtual void Update()
    {
        // ignore if the player is dead
        //if (GameManager.IsGameOver())
        //{
        //    return;
        //}

        // count up to the next time we can shoot
        shotTimer += Time.deltaTime;
        if (shotTimer >= rateOfFire && isFireButtonDown)
        {
            // fire and reset the timer
            //FireBullet();
            shotTimer = 0f;
        }
    }
}
