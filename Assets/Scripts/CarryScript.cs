using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public enum DeathReason
{
    SMASH = 0,
    DROWN = 1,
    BURN = 2,
    ELECTRICITY = 3
}

// Author: Iaroslav Titov (c)
public class CarryScript : MonoBehaviour
{
    [SerializeField] float flyRate;
    [SerializeField] float flySpeed;
    [SerializeField] float flyClamp;
    [SerializeField] float speed;
    [SerializeField] GameObject soul;
    [SerializeField] GameObject[] deathPrefabs;

    private bool isCarried;
    private bool isDead;
    private SpriteRenderer render;
    private Vector2 flyVelocity;
    private TopDownFollowCamera cameraMovement;
    private float flyTimer;
    private float idleTimer;
    private Rigidbody2D rb;
    private new Collider2D collider;
    private MapGenerator map;
    private float carryTime = 0;
    private Queue<Vector2> previousPositions;
    private Animator anim;
    private Vector2Int? target = null;
    private ScoreScript score;
    private DeathCountScript death;
    private Achievements achievement;

    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
        cameraMovement = Camera.main.GetComponent<TopDownFollowCamera>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        map = GameObject.FindObjectOfType<MapGenerator>();
        previousPositions = new Queue<Vector2>();
        anim = GetComponent<Animator>();
        score = GameObject.FindObjectOfType<ScoreScript>();
        death = GameObject.FindObjectOfType<DeathCountScript>();
        achievement = new Achievements();
    }

    void Update()
    {
        //Check what cell you are on
        Cell landedOn = Cell.NONE;
        if (transform.position.x > 0 && transform.position.y > 0 && transform.position.x < map.worldWidth && transform.position.y < map.worldHeight)
            landedOn = map.GetCell(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

        anim.SetBool("SeeTarget", target != null);
        if (isCarried)
        {
            idleTimer = 0;
            target = null;
            carryTime += Time.deltaTime;

            previousPositions.Enqueue(rb.position);
            if (previousPositions.Count > 3) previousPositions.Dequeue();

            //Check if out of bounds
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.back * 10, Vector3.forward);
            if (hit && hit.collider.tag == "AntiCarry")
            {
                OnMouseUp();
                return;
            }

            // Set position to mouse
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.down * 2;
            pos.z = 0;
            rb.position = pos;
            collider.offset = new Vector2(0, 1.2f);

            // Change sprite based on drag
            anim.SetFloat("SpeedX", Input.GetAxis("Mouse X"));
        }
        else // when not carried
        {
            if (flyTimer > -.1f) // Fly
            {
                flyTimer -= Time.deltaTime;
                rb.velocity = flyVelocity;
                if (flyTimer <= 0)
                    Land();
            }
            else // Idle on ground
            {
                rb.velocity = Vector2.zero;
                rb.rotation = 0;
                idleTimer += Time.deltaTime;

                if (target == null && idleTimer > 3)
                {
                    target = map.FindClosestCarrot(transform.position, 10);
                    if (target == null)
                    {
                        anim.SetTrigger("Bored");
                        idleTimer = -1 * anim.GetCurrentAnimatorClipInfo(0).Length;
                    }
                }
                else
                if (target != null)
                {
                    rb.velocity = ((Vector2)(target - rb.position + Vector2.right * 0.5f + Vector2.up)).normalized * speed;
                    anim.SetFloat("SpeedX", rb.velocity.x);
                    anim.SetFloat("SpeedY", rb.velocity.y);

                    if (((Vector2)(target - rb.position + Vector2.right * 0.5f + Vector2.up)).magnitude < 0.1f)
                    {
                        anim.SetTrigger("NearCarrot");
                        idleTimer = -1 * anim.GetCurrentAnimatorClipInfo(0).Length;

                        Vector2Int v = (Vector2Int)target;
                        StartCoroutine(ClickOnTile(v, .5f));
                        target = null;
                    }
                }

                if (landedOn == Cell.WATER || transform.position.x < 0 || transform.position.y < 0 || transform.position.x > map.worldWidth || transform.position.y > map.worldHeight)
                    Die(DeathReason.DROWN);
            }
        }

        if (landedOn == Cell.BURNING_BUSH || landedOn == Cell.BURNING_GRASS)
            Die(DeathReason.BURN);
    }

    private IEnumerator ClickOnTile(Vector2Int position, float wait)
    {
        yield return new WaitForSeconds(wait);

        map.ClickOnTile(position.x, position.y);
        score.AddScore(10);
    }

    void OnMouseDown()
    {
        // Start carry
        if (isCarried) return;
        isCarried = true;
        anim.SetBool("Carried", isCarried);
        cameraMovement.enabled = false;
        previousPositions.Clear();
    }

    private void OnMouseUp() // Send guy flying after mouse up
    {
        if (!isCarried) return;


        //Check for death on click
        if (carryTime < .1f && rb.velocity.magnitude < 1f)
        {
            Die(DeathReason.SMASH);
            return;
        }

        isCarried = false;
        carryTime = 0;
        cameraMovement.enabled = true;
        flyVelocity = Vector2.ClampMagnitude((rb.position - previousPositions.Peek()), flyClamp) * flySpeed;
        flyTimer = flyRate * flyVelocity.magnitude;
        if (flyTimer <= 0)
            Land();
    }

    private void Land()
    {
        render.flipX = false;
        collider.offset = new Vector2(0, 0.9f);
        anim.SetBool("Carried", isCarried);
    }

    public void Die(DeathReason reason)
    {
        if (isDead) return;
        isDead = true;
        death.AddDeath(reason);
        Instantiate(soul, transform.position + Vector3.down * 1.5f, Quaternion.identity);
        Debug.Log("I died from " + reason);
        score.Deactivate();
        death.Deactivate();
        achievement.checkDeathAchievements();

        GameObject go = Instantiate(deathPrefabs[(int)reason], transform.position, Quaternion.identity);

        if (go.GetComponent<Animator>())
            Destroy(go, go.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);

        if (reason == DeathReason.SMASH)
            go.transform.Translate(Vector3.up * 1f);

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        map.StartCoroutine(map.ReloadGame());
        Destroy(gameObject);
    }
}
