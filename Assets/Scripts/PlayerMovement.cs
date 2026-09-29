using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    private Rigidbody2D body;
    private Animator animator;
    private SpriteRenderer playerSprite;
    private BoxCollider2D box;
    private Transform playerTransform;

    [SerializeField] private float speed = 10;
    [SerializeField] private float jumpForce = 13;
    [SerializeField] private float shotgunForce = 13;
    [SerializeField] private float shotMovementLockTime = 0.5f;

    [SerializeField] private LayerMask ground;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform shotgunTransform;

    private float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    private float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    private float landingSoundCounter = -1f;

    private bool defaultInAirMovement = true;
    private bool noInAirMovement = false;
    private bool touchedGroundAfterShot = true;
    public bool canShoot = false;

    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource landSound;
    [SerializeField] private AudioSource shootSound;
    [SerializeField] private AudioSource reloadSound;
    [SerializeField] private AudioSource runSound;
    [SerializeField] private Animator shotgunAnimator;
    private enum animationState { idle, running, jumping, falling };

    // Start is called before the first frame update
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
        playerTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!PauseMenu.isPaused)
        {
            float movement = Input.GetAxis("Horizontal");

            // Arrow key movement on ground and in air
            HandleMovement(movement);

            // Jump when space button is pressed
            HandleJumping();

            // Shoot if player is in air, has touched gound since last shot and has clicked on screen
            ItemCollection itemCollection = playerSprite.GetComponent<ItemCollection>();
            canShoot = touchedGroundAfterShot && !IsGrounded() && itemCollection.ammoCount > 0;
            if (Input.GetMouseButtonDown(0) && canShoot)
            {
                itemCollection.ammoCount--;
                itemCollection.UpdateshotgunAmmo();
                StartCoroutine("Shoot");
            }

            // Animated player and weapon
            UpdateMovementAnimation(movement);
        }
    }

    void HandleMovement(float movement)
    {
        if (!noInAirMovement)
        {
            if (defaultInAirMovement)
            {
                body.velocity = new Vector2(movement * speed, body.velocity.y);
            }
            else // Move player with regard to current velocity -> prevents shot force from instantley being cancelled by movement
            {
                float newMovement = body.velocity.x + movement * speed;
                // Allow movement with arrow keys as long as it does not exceed max/min speed
                if (newMovement >= -speed && newMovement <= speed)
                {
                    body.velocity = new Vector2(newMovement, body.velocity.y);
                }
                else if (body.velocity.x > 0 && newMovement < 0 ||
                         body.velocity.x < 0 && newMovement > 0)
                {
                    Debug.Log("changed");
                    body.velocity = new Vector2(newMovement, body.velocity.y);
                    defaultInAirMovement = true; // Start default in air movement because user changed direction
                }
                else if (newMovement < -speed && body.velocity.x > -speed)
                {
                    body.velocity = new Vector2(-speed, body.velocity.y);
                    defaultInAirMovement = true; // Start default in air movement because user made a long arrow press
                }
                else if (newMovement > speed && body.velocity.x < speed)
                {
                    body.velocity = new Vector2(speed, body.velocity.y);
                    defaultInAirMovement = true; // Start default in air movement because user made a long arrow press
                }

                // allow player to only decrease velocity when it is bigger than max/min speed
                if (body.velocity.x < -speed && newMovement > body.velocity.x
                    || body.velocity.x > speed && newMovement < body.velocity.x)
                {
                    body.velocity = new Vector2(newMovement, body.velocity.y);
                }
            }

        }
    }

    private void HandleJumping()
    {
        // Allow player jump if it is a little after running off platform
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;

            if (landingSoundCounter < 0)
            {
                landSound.Play();
                landingSoundCounter = 0.2f;
            }
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            landingSoundCounter -= Time.deltaTime;
        }

        // Allow player jump if it is a little before reaching ground
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Check if the Jump button is pressed and the character is grounded
        if (jumpBufferCounter > 0f  && coyoteTimeCounter > 0)
        {
            body.velocity = new Vector2(body.velocity.x, jumpForce);
            jumpBufferCounter = 0f;
            jumpSound.Play();
        }

        // Check if the Jump button is released and the character is moving upwards while not shooting
        if (Input.GetButtonUp("Jump") && body.velocity.y > 0f && touchedGroundAfterShot)
        {
            // Reduce the upward for shorter jump
            body.velocity = new Vector2(body.velocity.x, body.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }
    }

    private bool IsGrounded()
    {
        // Moves a boxcast downward to see if it collides with ground layer
        return Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0f, Vector2.down, 0.1f, ground);
    }

    private IEnumerator Shoot()
    {
        noInAirMovement = true;
        touchedGroundAfterShot = false;
        shootSound.Play();
        shotgunAnimator.SetTrigger("Shoot");

        // Get the mouse and player positions
        Vector2 mousePosition = Input.mousePosition;
        Vector2 playerPosition = cam.WorldToScreenPoint(playerTransform.position);

        // Calculate direction of mouse and invert it for the direction to propell player
        Vector2 movementDir = (mousePosition - playerPosition).normalized;
        movementDir *= -1;
        body.velocity = new Vector2(movementDir.x * shotgunForce, movementDir.y * shotgunForce);

        // Lock movement for a short time after shot
        yield return new WaitForSeconds(shotMovementLockTime);
        noInAirMovement = false;

        // Wait untill grounded before setting default in air movement and letting player shoot again
        defaultInAirMovement = false;
        while (!IsGrounded())
        {
            yield return null;
        }
        reloadSound.Play();
        defaultInAirMovement = true;
        touchedGroundAfterShot = true;
    }

    private void UpdateMovementAnimation(float movement)
    {
        animationState state;

        if (movement > 0)
        {
            state = animationState.running;
            playerSprite.flipX = false;

            // Reset and angle weapon to direction player is running
            shotgunTransform.localScale = new Vector3(1, 1, 1);
            shotgunTransform.eulerAngles = new Vector3(0, 0, -20);
        }
        else if (movement < 0)
        {
            state = animationState.running;
            playerSprite.flipX = true;

            // Reset and angle weapon to direction player is running
            shotgunTransform.localScale = new Vector3(-1, 1, 1);
            shotgunTransform.eulerAngles = new Vector3(0, 0, 20);
        }
        else
        {
            state = animationState.idle;

            // Reset weapon sprite to direction player is facing when idle
            if(playerSprite.flipX)
            {
                shotgunTransform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                shotgunTransform.localScale = new Vector3(1, 1, 1);
            }
            shotgunTransform.eulerAngles = new Vector3(0, 0, 0);
        }

        if (body.velocity.y > 0.1f)
        {
            state = animationState.jumping;
            AimWeapon();

        }
        else if (body.velocity.y < -0.1f || !IsGrounded())
        {
            state = animationState.falling;
            AimWeapon();
        }

        animator.SetInteger("state", (int)state);
    }

    private void AimWeapon()
    {
        // Get the mouse and player positions
        Vector2 mousePosition = Input.mousePosition;
        Vector2 playerPosition = cam.WorldToScreenPoint(playerTransform.position);

        // Calculate aim direction and convert into angle in radians
        Vector2 aimDirection = (mousePosition - playerPosition).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        shotgunTransform.eulerAngles = new Vector3(0, 0, angle);

        // Move weapon sprite to direction of mouse
        Vector3 aimLocaleScale = new Vector3(1, 1, 1);
        if (angle > 90 || angle < -90)
        {
            aimLocaleScale.y = -1f;
            //playerSprite.flipX = true;  // Flip player sprite to aiming direction
        }
        else
        {
            aimLocaleScale.y = +1f;
            //playerSprite.flipX = false;  // Flip player sprite to aiming direction
        }
        shotgunTransform.localScale = aimLocaleScale;

    }
}