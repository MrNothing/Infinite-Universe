using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PhysicsCharacterMotor : CharacterMotor {

  //  public float runSpeed = 3.0f;
  //  public float walkSpeed = 5.0f;
    public bool jumpingReachedApex;
    public float maxRotationSpeed = 270;
	public bool useCentricGravity = false;
	public LayerMask groundLayers;
	public Transform gravityCenter;
    public Vector3 desiredUp;

	void Awake () {
		rigidbody.freezeRotation = true;
		rigidbody.useGravity = false;
	}
	
	private void AdjustToGravity() {
		int origLayer = gameObject.layer;
		gameObject.layer = 2;
		
		Vector3 currentUp = transform.up;
				
		float damping = Mathf.Clamp01(Time.deltaTime*5);
		
		RaycastHit hit;
		
		desiredUp = Vector3.zero;
        /*for (int i=0; i<8; i++) {
            Vector3 rayStart =
                transform.position
                    + transform.up
                    + Quaternion.AngleAxis(360 * i / 8.0f, transform.up)
                        * (transform.right * 0.5f)
					+ desiredVelocity*0.2f;
           
            Debug.DrawRay(rayStart, transform.up * -5, Color.red);
           // Debug.Log(Physics.Raycast(rayStart, transform.up*-48, out hit, 3.0f, groundLayers));
			if ( Physics.Raycast(rayStart, transform.up*-5, out hit, 10.0f, groundLayers.value) ) {
               
                desiredUp += hit.normal;
               	}
		}*/

		desiredUp = (transform.position - gravityCenter.position).normalized;

		desiredUp = (currentUp+desiredUp).normalized;
		Vector3 newUp = (currentUp+desiredUp*damping).normalized;
		
		float angle = Vector3.Angle(currentUp,newUp);
		if (angle>0.01) {
			Vector3 axis = Vector3.Cross(currentUp,newUp).normalized;
			Quaternion rot = Quaternion.AngleAxis(angle,axis);
			transform.rotation = rot * transform.rotation;
		}
		
		gameObject.layer = origLayer;
	}
	
	private void UpdateFacingDirection() {
		// Calculate which way character should be facing
		float facingWeight = desiredFacingDirection.magnitude;
		Vector3 combinedFacingDirection = (
			transform.rotation * desiredMovementDirection * (1-facingWeight)
			+ desiredFacingDirection * facingWeight
		);
		combinedFacingDirection = Util.ProjectOntoPlane(combinedFacingDirection, transform.up);
		combinedFacingDirection = alignCorrection * combinedFacingDirection;
		
		if (combinedFacingDirection.sqrMagnitude > 0.1f) {
			Vector3 newForward = Util.ConstantSlerp(
				transform.forward,
				combinedFacingDirection,
				maxRotationSpeed*Time.deltaTime
			);
			newForward = Util.ProjectOntoPlane(newForward, transform.up);
			//Debug.DrawLine(transform.position, transform.position+newForward, Color.yellow);
			Quaternion q = new Quaternion();
			q.SetLookRotation(newForward, transform.up);
			transform.rotation = q;
		}
	}
	
	private void UpdateVelocity() {
		Vector3 velocity = rigidbody.velocity;
		if (grounded) velocity = Util.ProjectOntoPlane(velocity, transform.up);
		
		if (grounded) {
			// Apply a force that attempts to reach our target velocity
			Vector3 velocityChange = (desiredVelocity - velocity);
			if (velocityChange.magnitude > maxVelocityChange) {
				velocityChange = velocityChange.normalized * maxVelocityChange;
			}
			rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
		
			// Jump
			if (canJump && Input.GetButton("Jump") && grounded) {
				rigidbody.velocity = velocity + transform.up * Mathf.Sqrt(2 * jumpHeight * gravity);
				jumping = true;
			}
		}
		
		// Apply downwards gravity
		rigidbody.AddForce(transform.up * -gravity * rigidbody.mass);
       
		grounded = false;
	}


    void OnCollisionEnter(Collision coll)
    {
       
        if (coll.gameObject.tag == "World")
             SendMessage("DidLand");
    }


    void OnCollisionStay()
    {
        grounded = true;        
        jumping = false;
        jumpingReachedApex = false;
      
    }
	
	void FixedUpdate () {
		if (useCentricGravity) AdjustToGravity();


		//UpdateFacingDirection();
		
		//UpdateVelocity();

      /*  if (!jumpingReachedApex && transform.rigidbody.velocity.y <= 0.0f)
        {
            jumpingReachedApex = true;
        }
      */
	}


}
