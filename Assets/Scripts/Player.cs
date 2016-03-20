using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    private Rigidbody2D myRigidbody;

    private Animator myAnimator;

    [SerializeField]
    private float movementSpeed;

    private bool attack;

    private bool facingRight;

    private bool slider;

    [SerializeField]
    Transform[] groundPoints;

    [SerializeField]
    float groundRadius;

    [SerializeField]
    LayerMask whatIsGround;

    bool isGrounded;

    bool jump;

    bool jumpAttack;

    [SerializeField]
    bool airControl;

    [SerializeField]
    float jumpForce;

    // Use this for initialization
    void Start()
    {
        facingRight = true;
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");

        isGrounded = IsGrounded();

        HandleMovement(horizontal);
        Flip(horizontal);
        HandleAttacks();
        HandleLayers();
        ResetValues();
    }

    void HandleMovement(float horizontal)
    {
        if (myRigidbody.velocity.y < 0)
        {
            myAnimator.SetBool("land", true);
        }
        if (!myAnimator.GetBool("slide") && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && (isGrounded || airControl))
        {
            myRigidbody.velocity = new Vector2(horizontal * movementSpeed, myRigidbody.velocity.y);
        }

        if (isGrounded && jump)
        {
            isGrounded = false;
            myRigidbody.AddForce(new Vector2(0, jumpForce));
            myAnimator.SetTrigger("jump");
        }

        if (slider && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("ani_slide"))
        {
            myAnimator.SetBool("slide", true);
        }
        else if (!this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("ani_slide"))
        {
            myAnimator.SetBool("slide", false);
        }


        myAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }

    void HandleAttacks()
    {
        if (attack && isGrounded && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            myAnimator.SetTrigger("attack");
            myRigidbody.velocity = Vector2.zero;
        }

        if (jumpAttack && !isGrounded && !this.myAnimator.GetCurrentAnimatorStateInfo(1).IsName("ani_jumpAttack"))
        {
            myAnimator.SetBool("jumpAttack", true);
        }

        if (!jumpAttack && !this.myAnimator.GetCurrentAnimatorStateInfo(1).IsName("ani_jumpAttack"))
        {
            myAnimator.SetBool("jumpAttack", false);
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            attack = true;
            jumpAttack = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            slider = true;
        }
    }

    void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;

            Vector3 theScale = transform.localScale;

            theScale.x *= -1;

            transform.localScale = theScale;
        }

    }

    void ResetValues()
    {
        attack = false;
        slider = false;
        jump = false;
        jumpAttack = false;
    }

    bool IsGrounded()
    {
        if (myRigidbody.velocity.y <= 0)
        {
            foreach( Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);
                for(int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != gameObject)
                    {
                        myAnimator.ResetTrigger("jump");
                        myAnimator.SetBool("land", false);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void HandleLayers()
    {
        if (!isGrounded)
        {
            myAnimator.SetLayerWeight(1, 1);
        }
        else
        {
            myAnimator.SetLayerWeight(1, 0);
        }
    }
}
