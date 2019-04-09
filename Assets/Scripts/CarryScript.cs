using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] Sprite[] sprites;
    [Range(0.0f, 1.0f)] public float moveTolerance;
    [SerializeField] float flyRate;
    [SerializeField] float flySpeed;
    [SerializeField] float flyClamp;
    [SerializeField] GameObject soul;
    [SerializeField] GameObject[] deathPrefabs;

    private bool isCarried;
    private bool isDead;
    private SpriteRenderer render;
    private Vector2 flyVelocity;
    private TopDownFollowCamera cameraMovement;
    private float flyTimer;
    private Rigidbody2D rb;
    private new Collider2D collider;
    private MapGenerator map;
    private float carryTime = 0;
    private Queue<Vector2> previousPositions;

    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
        cameraMovement = Camera.main.GetComponent<TopDownFollowCamera>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        map = GameObject.FindObjectOfType<MapGenerator>();
        previousPositions = new Queue<Vector2>();
    }

    void Update()
    {
        //Check what cell you are on
        Cell landedOn = Cell.NONE;
        if (transform.position.x > 0 && transform.position.y > 0 && transform.position.x < map.worldWidth && transform.position.y < map.worldHeight)
            landedOn = map.GetCell(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

        if (isCarried)
        {
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
            float movement = Mathf.Abs(Input.GetAxis("Mouse X"));
            if (movement > moveTolerance)
            {
                render.sprite = sprites[2];
                render.flipX = Input.GetAxis("Mouse X") > 0;
            }
            else if (movement < moveTolerance - .2f)
            {
                render.sprite = sprites[1];
                render.flipX = false;
            }
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

                if (landedOn == Cell.WATER || transform.position.x < 0 || transform.position.y < 0 || transform.position.x > map.worldWidth || transform.position.y > map.worldHeight)
                    Die(DeathReason.DROWN);
            }
        }

        if (landedOn == Cell.BURNING_BUSH || landedOn == Cell.BURNING_GRASS)
            Die(DeathReason.BURN);
    }

    void OnMouseDown()
    {
        // Start carry
        if (isCarried) return;
        isCarried = true;
        render.sprite = sprites[1];
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
        render.sprite = sprites[3];
        render.flipX = false;
        collider.offset = new Vector2(0, 0.9f);
        StartCoroutine(SetSpriteInSeconds(sprites[0], .25f));
    }

    public void Die(DeathReason reason)
    {
        if (isDead) return;
        isDead = true;
        Instantiate(soul, transform.position + Vector3.down * 1.5f, Quaternion.identity);
        Debug.Log("I died from " + reason);

        GameObject go = Instantiate(deathPrefabs[(int)reason], transform.position, Quaternion.identity);

        if (go.GetComponent<Animator>())
            Destroy(go, go.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);

        if (reason == DeathReason.SMASH)
            go.transform.Translate(Vector3.up * 1f);

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(map.ReloadGame());
    }

    private IEnumerator SetSpriteInSeconds(Sprite s, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        render.sprite = s;
    }
}
