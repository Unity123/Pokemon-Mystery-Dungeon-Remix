using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon : MonoBehaviour
{
    Animator animator;
    public TurnManager turns;
    List<string> moveSequence = new List<string>();
    Rigidbody2D rb2D;
    public float moveSpeed;
    Vector3 currentPos = Vector3.zero;
    bool moving = false;
    public bool isMyTurn = true;
    public int blockingLayer;
    RaycastHit2D hit;
    BoxCollider2D boxCollider;
    Vector2 direction = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        turns.pokemon.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (isMyTurn && !moving)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                if (Input.GetAxis("Vertical") > 0)
                {
                    moveSequence.Add("WalkUpRight");
                }
                else if (Input.GetAxis("Vertical") < 0)
                {
                    moveSequence.Add("WalkDownRight");
                }
                else
                {
                    moveSequence.Add("WalkRight");
                }
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                if (Input.GetAxis("Vertical") > 0)
                {
                    moveSequence.Add("WalkUpLeft");
                }
                else if (Input.GetAxis("Vertical") < 0)
                {
                    moveSequence.Add("WalkDownLeft");
                }
                else
                {
                    moveSequence.Add("WalkLeft");
                }
            }
            else if (Input.GetAxis("Vertical") > 0)
            {
                moveSequence.Add("WalkUp");
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                moveSequence.Add("WalkDown");
            }
        }
        if (moveSequence.Count > 0 && !moving && isMyTurn)
        {
            Walk(moveSequence[0]);
        }
    }

    void Walk(string direction_string)
    {
        ChangeDirection(direction_string);

        Vector2 start = transform.position;
        Vector2 end = start + direction;

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            StartCoroutine(Walkment(end));
            animator.Play(gameObject.name + direction_string);
            isMyTurn = false;
            return;
        }
        else
        {
            moveSequence.RemoveAt(0);
            animator.Play(gameObject.name + direction_string);
            return;
        }
    }

    void ChangeDirection(string direction_string)
    {
        switch (direction_string)
        {
            case "WalkUp":
                direction = Vector2.up;
                break;
            case "WalkDown":
                direction = Vector2.down;
                break;
            case "WalkLeft":
                direction = Vector2.left;
                break;
            case "WalkRight":
                direction = Vector2.right;
                break;
            case "WalkDownLeft":
                direction = Vector2.down + Vector2.left;
                break;
            case "WalkDownRight":
                direction = Vector2.down + Vector2.right;
                break;
            case "WalkUpLeft":
                direction = Vector2.up + Vector2.left;
                break;
            case "WalkUpRight":
                direction = Vector2.up + Vector2.right;
                break;
        }
    }

    IEnumerator Walkment(Vector3 end)
    {
        moving = true;
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, moveSpeed * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }

        currentPos = end;
        moveSequence.RemoveAt(0);
        moving = false;
    }
}