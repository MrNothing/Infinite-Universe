using UnityEngine;
using System.Collections;

public class CharaterManeger : MonoBehaviour {
	
	public int ibtnNum = 10;
	public int ibtnInitPosY = 50;
	
	public float GravityForce = -1.3f;
	public float JumpHeight = 2.0f;
	public bool CanJump = false;
	public bool Jump = false;
	public float hSliderValue = 0.0F;
	
	
	private int ibtnPositionX = 0;
	private int ibtnPositionY = 50;
	private int ibtnPositionSizeX = 100;
	private int ibtnPositionSizeY = 30;
	
	
	
	
	// Use this for initialization
	void Start () {
		
		
		
		CanJump = false;
		
		ibtnPositionX = (int)Screen.width - 120;
			
		animation.wrapMode = WrapMode.Loop; 
		animation.Play("idle01");
			
	}
	
	// Update is called once per frame
	void Update () {
		
		Physics.gravity = new Vector3(0, GravityForce, 0);
		Debug.Log(rigidbody.velocity.y);
		
		transform.eulerAngles = new Vector3(0, hSliderValue, 0);
		
		
		if (CanJump == true && Jump == true)
        {
			JumpLogic();
			
			CanJump = false;
			animation.CrossFade("jump", 0.1f);
			animation.wrapMode = WrapMode.Default;
            
        }
		
	}
	
	void OnCollisionEnter(Collision Wall)
    {
        if (Wall.gameObject.tag == "Wall") 
        {
            CanJump = true;
			animation.wrapMode = WrapMode.Loop;
            animation.CrossFade("idle01",0.1f);
        }
    }
	
	
	void OnGUI()
	{
		
		 
		
		if (GUI.Button(new Rect(Screen.width / 2 - (ibtnPositionSizeX * 2), Screen.height - 70, ibtnPositionSizeX, ibtnPositionSizeY), "RIGHT"))
		{
			hSliderValue += 45;
			
		}
		if (GUI.Button(new Rect(Screen.width / 2 + ibtnPositionSizeX, Screen.height - 70, ibtnPositionSizeX, ibtnPositionSizeY), "LEFT"))
		{
			
			hSliderValue -=45;
		}
		
		
		if (GUI.Button(new Rect(ibtnPositionX, ibtnInitPosY + (0 * ibtnPositionY), ibtnPositionSizeX, ibtnPositionSizeY), "Idle"))
		{
			animation.CrossFade("idle01", 0.1f);
			
		}
		
		if (GUI.Button(new Rect(ibtnPositionX, ibtnInitPosY + (1 * ibtnPositionY), ibtnPositionSizeX, ibtnPositionSizeY), "Walk"))
		{
			animation.CrossFade("walk", 0.1f);
			animation.wrapMode = WrapMode.Loop;
	    	
		}
	
		
		if (GUI.Button(new Rect(ibtnPositionX, ibtnInitPosY + (2 * ibtnPositionY), ibtnPositionSizeX, ibtnPositionSizeY), "Jump"))
		{
			Jump = true;
		}
		else
			Jump = false;
		
		if (GUI.Button(new Rect(ibtnPositionX, ibtnInitPosY + (3 * ibtnPositionY), ibtnPositionSizeX, ibtnPositionSizeY), "Run"))
		{
	    	animation.CrossFade("run", 0.1f);
			animation.wrapMode = WrapMode.Loop;
		}
		
		if (GUI.Button(new Rect(ibtnPositionX, ibtnInitPosY + (4 * ibtnPositionY), ibtnPositionSizeX, ibtnPositionSizeY), "Run2"))
		{
	    	animation.CrossFade("run2", 0.1f);
			animation.wrapMode = WrapMode.Loop;
		}
		
		if (GUI.Button(new Rect(ibtnPositionX, ibtnInitPosY + (5 * ibtnPositionY), ibtnPositionSizeX, ibtnPositionSizeY), "Touch Arm"))
		{
	    	animation.CrossFade("touchArm", 0.1f);
			animation.wrapMode = WrapMode.Default;
		}
		
		if (GUI.Button(new Rect(ibtnPositionX, ibtnInitPosY + (6 * ibtnPositionY), ibtnPositionSizeX, ibtnPositionSizeY), "Touch Breast"))
		{
	    	animation.CrossFade("touchBreast", 0.1f);
			animation.wrapMode = WrapMode.Default;
		}
		
		
		if (GUI.Button(new Rect(ibtnPositionX, ibtnInitPosY + (7 * ibtnPositionY), ibtnPositionSizeX, ibtnPositionSizeY), "Touch Foot"))
		{
	    	animation.CrossFade("touchFoot", 0.1f);
			animation.wrapMode = WrapMode.Default;
		}
		
		
		if (GUI.Button(new Rect(ibtnPositionX, ibtnInitPosY + (8 * ibtnPositionY), ibtnPositionSizeX, ibtnPositionSizeY), "Touch Head"))
		{
	    	animation.CrossFade("touchHead", 0.1f);
			animation.wrapMode = WrapMode.Default;
		}
		
		
		if (GUI.Button(new Rect(ibtnPositionX, ibtnInitPosY + (9 * ibtnPositionY), ibtnPositionSizeX, ibtnPositionSizeY), "Touch Hip"))
		{
	    	animation.CrossFade("touchHip", 0.1f);
			animation.wrapMode = WrapMode.Default;
		}
		
		
		
		
	}
	
	  void JumpLogic()
    {
        rigidbody.AddForce(Vector3.up * JumpHeight);
    }
	
	
}
