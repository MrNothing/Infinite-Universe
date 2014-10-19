using UnityEngine;
using System;

public class WowCharacterController : MonoBehaviour
{
	public float jumpSpeed = 8.0f;
	public float jumpHeight = 20f;
	public float gravity = 20.0f;
	public float runSpeed = 8.0f;
	public float walkSpeed = 4.0f;
	public float rotateSpeed = 150.0f;
	
	public float dashSpeed = 30;
	public float dashDuration = 30;
	
	private bool grounded = false;
	private Vector3 moveDirection = Vector3.zero;
	Vector3 lastDirection=Vector3.zero;
	private bool isWalking = false;
	private string moveStatus = "idle";
	
	public bool flying=false;
	
	float jumping=0;
	public float waitBeforeJump=0;
	private Vector3 jumpDirection = Vector3.zero;
	
	public GameObject mainCamera;
	
		
	void Start()
	{
	
	}
	
	void Update ()
	{
		bool inWater = false;
		MeshBlender bloc = MeshIndexer.GetBlocAt(transform.position, 1);
		if(bloc!=null)
		{
			if(bloc.isWater)
				inWater = true;
		}
		// Only allow movement and jumps while grounded
		if(true)
		{
			moveDirection = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"))*Time.deltaTime;
			
			//if(Input.GetMouseButton(0) && Input.GetMouseButton(1))
			//	moveDirection = new Vector3(0,0,1);
			
			// if moving forward and to the side at the same time, compensate for distance
			// TODO: may be better way to do this?
			
			if(flying)
				moveDirection = mainCamera.transform.forward*Input.GetAxis("Vertical")+mainCamera.transform.right*Input.GetAxis("Horizontal");
			
			
			//moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= isWalking ? walkSpeed : runSpeed;
			
			if(inWater)
			{
				moveDirection*=0.5f;
			}
			
			moveStatus = "idle";
			if(moveDirection != Vector3.zero)
				moveStatus = isWalking ? "walking" : "running";
			
			if(Input.GetAxis("Horizontal")>0 && Input.GetAxis("Vertical")>0) {
				moveDirection *= .7f;
			}
			
				// Jump!
			if(Input.GetButton("Jump"))
			{
				if(grounded || inWater || flying)
				{
					jumpDirection = new Vector3(0, 1f, 0);
					
					jumping = jumpHeight;
				}
			}
			
		}
		if(jumping>0)
		{
			if(waitBeforeJump>0)
			{
				
			}
			else
			{
				moveDirection += jumpDirection*jumpSpeed*(jumping/jumpHeight)*((float)Time.deltaTime*30);
				jumping-=((float)Time.deltaTime*60);
			}
		}
		
		if(waitBeforeJump>0)
		{
			waitBeforeJump-=((float)Time.deltaTime*60);
		}
		
		
		
		// Allow turning at anytime. Keep the character facing in the same direction as the Camera if the right mouse button is down.
		if(true) {
			//print("rotating with right mouse..."+mainCamera.transform.eulerAngles.y);
			transform.eulerAngles = new Vector3(0, mainCamera.transform.eulerAngles.y, 0);
		} 
		else 
		{
			transform.Rotate(0,Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);
		}
		
		
		// Toggle walking/running with the T key
		if(Input.GetKeyDown("t"))
			isWalking = !isWalking;
		
		//Apply gravity
		if(jumping<=0 && !flying)
		{
			if(!inWater)
				moveDirection.y -= gravity * Time.deltaTime;
			else
				moveDirection.y -= gravity * Time.deltaTime*0.3f;
		}
		
		move(moveDirection);
			
		//if(moveDirection.magnitude>inertia.magnitude)
		//	inertia = moveDirection;
		
		//inertia = inertia*0.999f;
	}
	
	bool hitSides = false;
	
	void move(Vector3 direction)
	{
		try{
			CharacterController controller = (CharacterController)GetComponent("CharacterController");
		var flags = controller.Move(direction);
			grounded = (flags & CollisionFlags.Below) != 0;
			hitSides = (flags & CollisionFlags.Sides) !=0;
		}
		catch(Exception e)
		{
			
		}
	}

}