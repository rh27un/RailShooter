using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType { None = 0, Walking, Sprinting, Crouching, Sliding };

[RequireComponent(typeof(CharacterController))]
public class FPSCharacter : MonoBehaviour
{

	public bool dontDestroyOnLoad = false;
	public float yKillPlane;
	public float xKillPlane;
	//movement properties
	[Header("Speed Properties")]
	public float speed = 3f;
	public float sprintSpeed = 6f;
	public float crouchSpeed = 1.5f;
	public float maxSlideSpeed = 9f;
	public float aimedSpeed = 1.5f;
	public float slideDeceleration = 9f;

	[Header("Jump Properties")]
	public int jumps = 1;
	public float jumpSpeed = 10f;
	public float gravity = 15f;
	public float terminalVelocity = 20f;

	//camera properties
	[Header("Camera Properties")]
	public bool invertLook = false;
	public float baseSensitivity;
	public float sensitivity = 3f;
	public float maxAngle = 70f;
	public float minAngle = -70f;
	public bool strafeLean = false;
	public float strafeLeanSpeed = 1f;
	public float strafeLeanAmount = 1f;
	public float trainFriction = 0.5f;

	//FOV properties
	[Header("FOV Properties")]
	public bool FOVKick;
	public float walkFOV = 90f;
	public float sprintFOV = 100f;
	public float kickSpeed = 2f;

	[Header("Crouch Properties")]
	public float walkHeight = 2f;
	public float crouchHeight = 1.5f;
	public float slideHeight = 1f;

	[Header("Gun Properties")]
	public GameObject gun;
	public Transform aimed;
	public Transform unaimed;
	public float aimedFOV = 60f;
	public float zoomSpeed = 4f;
	public float gunSpeed = 0.2f;
	public float dragSpeed = 0.2f;

	public bool aiming;
	public MoveType moveType;
	protected GameObject wall;
	protected bool wallRunning;
	protected int jumpsLeft;
	protected float curJump = 0f;
	protected bool canJump = true;
	protected float slideSpeed;
	[SerializeField]
	protected Vector3 moveDirection = Vector3.zero;
	protected CharacterController controller;
	protected Camera cameraComponent;
	protected Transform cameraTransform;
	protected float curSpeed;
	protected float curLean;
	protected Vector3 curTrainFriction;
	protected TrainController train;
	protected bool IsGrounded => controller.isGrounded;

	protected Pause pause;
	// Use this for initialization
	void Awake()
	{
		if (dontDestroyOnLoad)
		{
			DontDestroyOnLoad(gameObject);
		}
		controller = GetComponent<CharacterController>();

		cameraComponent = Camera.main;
		cameraTransform = cameraComponent.transform;


		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		jumpsLeft = jumps;

		moveType = MoveType.Walking;
		RaycastHit hit;
		if (Physics.Raycast(cameraComponent.transform.position, cameraComponent.transform.forward, out hit))
		{
			aimed.LookAt(hit.point);
			gun.transform.LookAt(hit.point);
		}

		pause = GameObject.FindGameObjectWithTag("GameController").GetComponent<Pause>();
		train = pause.GetComponent<TrainController>();
		//var options = GameObject.Find("Options").GetComponent<Options>();

		walkFOV = PlayerPrefs.GetFloat("FieldOfView");
		sprintFOV = walkFOV + 10f;
		invertLook = Convert.ToBoolean(PlayerPrefs.GetInt("IsInverted"));
		sensitivity = baseSensitivity * PlayerPrefs.GetFloat("Sensitivity");
	}

	// Update is called once per frame
	void Update()
	{
		//apply movement to character controller
		controller.Move(CharacterMove() * Time.deltaTime);
		if(!pause.isPause)
			CameraLook();
		if(transform.position.y <= yKillPlane)
		{
			GetComponent<PlayerHealth>().Die();
		}
		if(transform.position.x >= xKillPlane)
		{
			GetComponent<PlayerHealth>().Die();
		}
	}

	Vector3 CharacterMove()
	{
		RaycastHit floorHit;
		if(Physics.Raycast(transform.position, Vector3.down, out floorHit, 1.1f))
		{
			if(floorHit.collider.tag == "Train")
			{
				curTrainFriction = Vector3.zero;
			}
			else
			{
				
				curTrainFriction.x = Mathf.Clamp(curTrainFriction.x + trainFriction * train.GetCurSpeed() * Time.deltaTime, 0f, train.GetCurSpeed());
			}
		}
		//Accept Movement Inputs
		moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
		moveDirection = transform.TransformDirection(moveDirection);

		moveDirection = moveDirection * SpeedMod();

		if (Input.GetButton("Fire2"))
		{
			if (moveType != MoveType.Sprinting)
			{
				if (!aiming)
				{
					aiming = true;
				}

				if (cameraComponent.fieldOfView > aimedFOV)
				{
					cameraComponent.fieldOfView = Mathf.Clamp(cameraComponent.fieldOfView - zoomSpeed * Time.deltaTime, aimedFOV, walkFOV);
				}
				if (gun.transform.position != aimed.position)
				{
					gun.transform.position = Vector3.Lerp(gun.transform.position, aimed.position, gunSpeed);
					gun.transform.rotation = Quaternion.Lerp(gun.transform.rotation, aimed.rotation, gunSpeed);
				}
				if (moveType != MoveType.Sliding && controller.isGrounded)
					moveDirection = moveDirection / 2;
			}
		}
		else
		{
			if (aiming)
			{
				aiming = false;
			}
			if (cameraComponent.fieldOfView < walkFOV)
			{
				cameraComponent.fieldOfView = Mathf.Clamp(cameraComponent.fieldOfView + zoomSpeed * Time.deltaTime, aimedFOV, walkFOV);
			}
			if (gun.transform.position != unaimed.position)
			{
				gun.transform.position = Vector3.Lerp(gun.transform.position, unaimed.position, 0.1f);
				gun.transform.rotation = Quaternion.Lerp(gun.transform.rotation, unaimed.rotation, 0.1f);
			}
		}

		if (!IsGrounded)
		{
			if (IsBonking())
			{
				curJump = -10;
			}
			if (!wallRunning)
			{
				curJump = Mathf.Clamp(curJump - gravity * Time.deltaTime, -terminalVelocity, jumpSpeed);
			}
			else
			{
				curJump = Mathf.Clamp((curJump - gravity * Time.deltaTime) / 2, -terminalVelocity, jumpSpeed);
			}
		}
		else
		{
			if (jumpsLeft != jumps)
				jumpsLeft = jumps;
			if (!canJump)
				canJump = true; //landing automatically allows jumping
			curJump = -1;
		}
		//Accept jump input
		if (Input.GetButtonDown("Jump"))
		{   //use canJump bool instead of controller.isGrounded to allow for double jumps & walljumps
			// but fuck you i'm not doing double jumping anymore
			if (IsGrounded)
			{
				curJump = jumpSpeed;
				jumpsLeft--;
				if (jumpsLeft == 0)
				{
					canJump = false;
				}
			}
		}



		moveDirection.y = curJump;
		moveDirection += curTrainFriction;
		curSpeed = moveDirection.magnitude;
		return moveDirection;
	}

	void CameraLook()
	{
		transform.Rotate(new Vector3(0f, Input.GetAxis("Mouse X") * sensitivity, 0f));
		float camAngle = cameraTransform.rotation.eulerAngles.x;
		if (invertLook)
		{
			camAngle += Input.GetAxis("Mouse Y") * sensitivity;
		}
		else
		{
			camAngle -= Input.GetAxis("Mouse Y") * sensitivity;
		}
		gun.transform.position += new Vector3(Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity, Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity, 0f);
		camAngle = RotationClamp(camAngle, minAngle, maxAngle);
		if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.05f)
		{
			curLean = strafeLean ? Mathf.Clamp(curLean - Input.GetAxis("Horizontal") * strafeLeanSpeed * Time.deltaTime, -strafeLeanAmount, strafeLeanAmount) : 0f;
		}
		else
		{
			curLean = curLean * (1 - Time.deltaTime * 2);
		}
		cameraTransform.localRotation = Quaternion.Euler(new Vector3(camAngle, 0f, curLean));
	}

	public void Recoil(float recoilAmount) {
		transform.Rotate(new Vector3(0f, UnityEngine.Random.Range(-1f, 1f) * recoilAmount, 0f));
		float camAngle = cameraTransform.rotation.eulerAngles.x;
		{
			camAngle -= recoilAmount;
		}

		camAngle = RotationClamp(camAngle, minAngle, maxAngle);
		cameraTransform.localRotation = Quaternion.Euler(new Vector3(camAngle, 0f, 0f));
	}
	float RotationClamp(float _value, float _min, float _max)
	{
		if (_min > 0f)
		{
			return Mathf.Clamp(_value, _min, _max);
		}
		else
		{
			_min = 360f + _min;
			if (_value >= _min)
				return _value;
			if (_value <= _max)
				return _value;
			if (_value < _min && _value >= 180)
				return _min;
			if (_value > _max && _value < 180)
				return _max;
			else
				return _value;
		}
	}
	void OnTriggerEnter(Collider other)
	{
		if (moveType == MoveType.Walking || moveType == MoveType.Sprinting)
		{
			if (other.gameObject.tag == "Wall")
			{
				wallRunning = true;
				jumpsLeft = jumps + 1;
				canJump = true;
				wall = other.gameObject;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Wall")
			wallRunning = false;
		wall = null;
	}

	float SpeedMod()
	{
		switch (moveType)
		{
			case MoveType.Walking:

				if (FOVKick)
				{
					if (cameraComponent.fieldOfView > walkFOV)
					{
						cameraComponent.fieldOfView = Mathf.Clamp(cameraComponent.fieldOfView - kickSpeed * Time.fixedDeltaTime, walkFOV, sprintFOV);
					}
				}

				if (Input.GetButtonDown("Crouch"))
				{
					if (!wallRunning)
					{
						moveType = MoveType.Crouching;
						controller.height = crouchHeight;
					}
				}
				else if (moveDirection.magnitude > 0.9 && Input.GetButton("Sprint") && !aiming)
				{
					moveType = MoveType.Sprinting;
				}
				return speed;
			case MoveType.Sprinting:

				if (FOVKick)
				{
					if (cameraComponent.fieldOfView < sprintFOV)
					{
						cameraComponent.fieldOfView = Mathf.Clamp(cameraComponent.fieldOfView + kickSpeed * Time.fixedDeltaTime, walkFOV, sprintFOV);
					}
				}

				if (moveDirection.magnitude <= 0.9)
				{
					moveType = MoveType.Walking;
				}
				else if (Input.GetButtonDown("Crouch"))
				{
					if (!wallRunning)
					{
						moveType = MoveType.Sliding;
						controller.height = slideHeight;
						slideSpeed = maxSlideSpeed;
					}
				}
				return sprintSpeed;

			case MoveType.Crouching:
				if (FOVKick)
				{
					if (cameraComponent.fieldOfView > walkFOV)
					{
						cameraComponent.fieldOfView = Mathf.Clamp(cameraComponent.fieldOfView - kickSpeed * Time.fixedDeltaTime, walkFOV, sprintFOV);
					}
				}

				if (Input.GetButtonUp("Crouch"))
				{
					moveType = MoveType.Walking;
					controller.height = walkHeight;
				}
				return crouchSpeed;

			case MoveType.Sliding:
				if (Input.GetButtonDown("Sprint"))
				{
					moveType = MoveType.Sprinting;
					controller.height = walkHeight;
					return sprintSpeed;
				}
				if (slideSpeed > crouchSpeed)
				{
					slideSpeed = Mathf.Clamp(slideSpeed - slideDeceleration * Time.fixedDeltaTime, crouchSpeed, maxSlideSpeed);
				}
				else
				{
					if (Input.GetButton("Crouch"))
					{
						moveType = MoveType.Crouching;
						controller.height = crouchHeight;
					}
					else
					{
						moveType = MoveType.Walking;
						controller.height = walkHeight;
					}

				}
				return slideSpeed;
			case MoveType.None:
				Debug.LogWarning("No MoveType");
				return 0;
			default:
				return 0;
		}
	}
	public bool IsMoving()
	{
		return (curSpeed > 0f);
	}
	
	public bool IsBonking()
	{
		return Physics.Raycast(transform.position, Vector3.up, 1.1f);
	}
	public void SwitchWeapons(GameObject newGun, Vector3 gunStockpile, float gunFOV)
	{
		gun.transform.SetParent(null);
		gun.transform.position = gunStockpile;
		gun = newGun;
		gun.transform.SetParent(cameraTransform);
		gun.transform.position = unaimed.position;
		gun.transform.rotation = unaimed.rotation;
		aimedFOV = gunFOV;
	}
}
