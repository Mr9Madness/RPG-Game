using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    Rigidbody rigidBody;
    BoxCollider capsuleCollider;

    [SerializeField]
    PhysicMaterial zFriction;
    [SerializeField]
    PhysicMaterial mFriction;

    Transform cam;

    [SerializeField]
    float speed = 0.8f;
    [SerializeField]
    float turnSpeed = 3;
    [SerializeField]
    float jumpPower = 5;

    Vector3 directionPos;
    Vector3 storPos;

    float horizontal;
    float vertical;
    bool jumpInput;
    bool onGround;

    private void Start()
    {
        rigidBody = GetComponent< Rigidbody >();
        cam = Camera.main.transform;
        capsuleCollider = GetComponent< BoxCollider >();
        onGround = true;
    }
    private void Update()
    {
        HandleFriction();
    }
    private void FixedUpdate()
    {
        if( onGround )
        {
            horizontal = Input.GetAxis( "Horizontal" );
            vertical = Input.GetAxis( "Vertical" );
        }
        jumpInput = Input.GetButtonDown( "Jump" );

        storPos = cam.right;

        Vector3 ding = ( ( storPos * horizontal ) + ( cam.forward * vertical ) ) * speed / Time.fixedDeltaTime;
        rigidBody.AddForce( new Vector3( ding.x, 0, ding.z ) );

        if ( onGround )
        {
            if( jumpInput && onGround )
                rigidBody.AddForce( Vector3.up * jumpPower, ForceMode.Impulse );
        }
        directionPos = transform.position + ( storPos * horizontal ) + ( cam.forward * vertical );
        Vector3 dir = directionPos - transform.position;
        dir.y = 0;

        if( horizontal != 0 || vertical != 0 )
        {
            float angle = Quaternion.Angle( transform.rotation, Quaternion.LookRotation( dir ) );

            if ( angle != 0 )
                rigidBody.rotation = Quaternion.Slerp( transform.rotation, Quaternion.LookRotation( dir ), turnSpeed * Time.fixedDeltaTime );
        }
    }
    private void OnCollisionEnter( Collision other )
    {
        if ( other.gameObject.tag == "Ground" )
        {
            onGround = true;
            rigidBody.drag = 5;
        }
    }
    private void OnCollisionExit( Collision other )
    {
        if( other.gameObject.tag == "Ground" )
        {
            onGround = false;
            rigidBody.drag = 5;
        }
    }
    private void HandleFriction()
    {
        if ( horizontal == 0 && vertical == 0 )
            capsuleCollider.material = mFriction;
        else
            capsuleCollider.material = zFriction;
    }
}