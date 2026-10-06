using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Effectable , IMoveable
{
    [SerializeField] string PLAYER_IDLE_ANIM = "PlayerIdle";
    [SerializeField] string PLAYER_RUN_ANIM = "PlayerRun";
    

    Rigidbody2D rb;
    [SerializeField]RandomMapGenerator mapGenerator;
    Camera cam;
    [SerializeField] float moveSpeed;
    [SerializeField] Vector2 movementVector;
    Vector3 towardsMouse;
    public AnimationManager animManager;


    private void Awake()
    {
        RandomMapGenerator.EMapGenerationFinished += OnSpawn;
    }

    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = FindFirstObjectByType<RandomMapGenerator>();
        animManager = GetComponent<AnimationManager>();
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(activeEffects.Count);
        movementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        towardsMouse = (cam.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        if (animManager != null)
        {
            if (rb.linearVelocity == Vector2.zero)
            {
                animManager.PlayAnim(PLAYER_IDLE_ANIM);
            }
            else { animManager.PlayAnim(PLAYER_RUN_ANIM); }
        }

        
    }

    private void FixedUpdate()
    {
        Move();
        if (rb.linearVelocity.x > 0.1f) { transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); }
        else if (rb.linearVelocity.x < -0.1f) { transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0)); }
    }

    public void OnSpawn()
    {
        transform.position = mapGenerator.WalkerStartPositions.AtIndex<Vector2Int>(0).ToV3();
    }

    public void Move()
    {
        // get the maxSpeed
        Vector2 targetSpeed = movementVector * moveSpeed;

        // get the difference b/w current and max speed
        Vector2 speedDif = targetSpeed - rb.linearVelocity;
        rb.AddForce(speedDif, ForceMode2D.Impulse); // impulse feels more snappy but FORCE feels more floaty

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(towardsMouse+transform.position,1f);
    }

    private void OnDestroy()
    {
        RandomMapGenerator.EMapGenerationFinished -= OnSpawn;
    }

    public void SetMoveSpeed(float val)
    {
        moveSpeed = val;
    }
    public float GetMoveSpeed() => moveSpeed;
}
