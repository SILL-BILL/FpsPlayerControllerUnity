using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class BaseFpsPlayerController : MonoBehaviour {

	// --------
	#region インスペクタ設定用フィールド
	/// <summary> 
	/// キャラクターコントローラ
	/// </summary>
	[SerializeField] protected CharacterController m_CharacterController;
	/// <summary> 
	/// プレイヤーのトランスフォーム
	/// </summary>
	[SerializeField] protected Transform m_PlayerTransform;
	/// <summary> 
	/// プレイヤーの主観用カメラのトランスフォーム
	/// </summary>
	[SerializeField] protected Transform m_CameraTransform;
	[Header("*Player Movement Setting")]
	/// <summary> 
	/// 
	/// </summary>
	[SerializeField] protected float m_PlayerMoveSpeed = 3.0f;
	/// <summary> 
	/// 
	/// </summary>
	[SerializeField] protected float m_PlayerGravity = 6.4f;
	[Header("*Player Rotation Setting")]
	/// <summary> 
	/// 
	/// </summary>
	[SerializeField] protected float m_PlayerRotateSpeed = 60.0f;
	/// <summary> 
	/// 
	/// </summary>
	[SerializeField] protected float m_PlayerJumpPower = 120.0f;
	[Header("*Camera Rotation Setting")]
	/// <summary> 
	/// 
	/// </summary>
	[SerializeField] protected float m_CameraHorizontalRotateSpeed = 60.0f;
	/// <summary> 
	/// 
	/// </summary>
	[SerializeField] protected float m_CameraVerticalRotateSpeed = 60.0f;
	/// <summary> 
	/// 
	/// </summary>
	[SerializeField] protected float m_CameraUpAngleMax  = 270.0f;
	/// <summary> 
	/// 
	/// </summary>
	[SerializeField] protected float m_CameraDownAngleMax = 90.0f;
	/// <summary> 
	/// 
	/// </summary>
	[SerializeField] protected float m_CameraLeftAngleMax = 270.0f;
	/// <summary> 
	/// 
	/// </summary>
	[SerializeField] protected float m_CameraRightAngleMax = 90.0f;
	#endregion

	// --------
	#region メンバフィールド
	/// <summary> 
	/// 
	/// </summary>
	protected Vector3 m_PlayerMoveVelocity;
	/// <summary> 
	/// 
	/// </summary>
	protected Vector3 m_PlayerMoveDetection;
	/// <summary> 
	/// 
	/// </summary>
	protected Vector3 m_PlayerNewAngle;
	/// <summary> 
	/// 
	/// </summary>
	protected float m_PlayerAddJumpPower = 0.0f;
	/// <summary> 
	/// 
	/// </summary>
	protected Vector3 m_GroundRayPos;
	/// <summary> 
	/// 
	/// </summary>
	protected float m_GroundRayDistance;
	/// <summary> 
	/// 
	/// </summary>
	protected bool m_IsGrounded;
	/// <summary> 
	/// 
	/// </summary>
	public bool isGrounded {
		get { return m_IsGrounded; }
		protected set { m_IsGrounded = value; }
	}
	/// <summary> 
	/// 
	/// </summary>
	protected Vector3 m_CameraNewAngle;
	protected bool m_JumpButtonPushingFlag;
	#endregion

	// --------
	#region MonoBehaviourメソッド
	/// <summary> 
	/// 初期化処理
	/// </summary>
	protected virtual void Awake() {

	}
	/// <summary> 
	/// 開始処理
	/// </summary>
	protected virtual void Start () {

	}
	/// <summary> 
	/// 更新処理
	/// </summary>
	protected virtual void Update () {

		groundRayUpdate();

		cameraRotateUpdate();

		playerRotateUpdate();

		playerMoveUpdate();

	}

	/// <summary> 
	/// 更新処理
	/// </summary>
	protected virtual void FixedUpdate(){

	}

	/// <summary> 
	/// 更新処理
	/// </summary>
	protected virtual void LateUpdate(){

	}

	protected virtual void OnTriggerEnter(Collider other) {
		Debug.Log("きた");
	}
	#endregion

	// --------
	#region メンバメソッド
	/// <summary>
	/// プレイヤー主観用カメラの回転処理
	/// </summary>
	protected virtual void cameraRotateUpdate(){
		m_CameraNewAngle　= new Vector3(
			m_CameraTransform.localEulerAngles.x,
			m_CameraTransform.localEulerAngles.y,
			m_CameraTransform.localEulerAngles.z
		);

		//上方向に回転
		#if UNITY_EDITOR
		if(Input.GetKey(KeyCode.UpArrow)){
		#else
		if (CrossPlatformInputManager.GetButton("PlayerCameraRotateUp")){
		#endif
			m_CameraNewAngle.x -= m_CameraVerticalRotateSpeed * Time.deltaTime;
			if(m_CameraNewAngle.x > m_CameraDownAngleMax && m_CameraNewAngle.x < m_CameraUpAngleMax){
				m_CameraNewAngle.x = m_CameraUpAngleMax;
			}
		}

		//下方向に回転
		#if UNITY_EDITOR
		if(Input.GetKey(KeyCode.DownArrow)){
		#else
		if (CrossPlatformInputManager.GetButton("PlayerCameraRotateDown")){
		#endif
			m_CameraNewAngle.x += m_CameraVerticalRotateSpeed * Time.deltaTime;
			if(m_CameraNewAngle.x < m_CameraUpAngleMax && m_CameraNewAngle.x > m_CameraDownAngleMax){
				m_CameraNewAngle.x = m_CameraDownAngleMax;
			}
		}
		//左方向に回転
		if (CrossPlatformInputManager.GetButton("PlayerCameraRotateLeft")){
			m_CameraNewAngle.y -= m_CameraHorizontalRotateSpeed * Time.deltaTime;
			if(m_CameraNewAngle.y > m_CameraRightAngleMax && m_CameraNewAngle.y < m_CameraLeftAngleMax){
				m_CameraNewAngle.y = m_CameraLeftAngleMax;
			}
		}
		//右方向に回転
		if (CrossPlatformInputManager.GetButton("PlayerCameraRotateRight")){
			m_CameraNewAngle.y += m_CameraHorizontalRotateSpeed * Time.deltaTime;
			if(m_CameraNewAngle.y < m_CameraLeftAngleMax && m_CameraNewAngle.y > m_CameraRightAngleMax){
				m_CameraNewAngle.y = m_CameraRightAngleMax;
			}
		}

		m_CameraTransform.localEulerAngles = m_CameraNewAngle;
	}

	/// <summary>
	/// プレイヤー回転処理
	/// </summary>
	protected virtual void playerRotateUpdate(){
		Vector3 m_PlayerNewAngle = new Vector3(
			m_PlayerTransform.localEulerAngles.x,
			m_PlayerTransform.localEulerAngles.y,
			m_PlayerTransform.localEulerAngles.z
		);


		#if UNITY_EDITOR
		if(Input.GetKey(KeyCode.LeftArrow)){
		#else
		if (CrossPlatformInputManager.GetButton("PlayerRotateLeft")){
		#endif
			m_PlayerNewAngle.y -= m_PlayerRotateSpeed * Time.deltaTime;
		}

		#if UNITY_EDITOR
		if(Input.GetKey(KeyCode.RightArrow)){
		#else
		if (CrossPlatformInputManager.GetButton("PlayerRotateRight")){
		#endif
			m_PlayerNewAngle.y += m_PlayerRotateSpeed * Time.deltaTime;
		}

		m_PlayerTransform.localEulerAngles = m_PlayerNewAngle;
	}

	/// <summary>
	/// プレイヤー移動処理
	/// </summary>
	protected virtual void playerMoveUpdate(){

		m_PlayerMoveVelocity = Vector3.zero;
		m_PlayerMoveDetection = Vector3.zero;

		//前進
		#if UNITY_EDITOR
		if(Input.GetKey(KeyCode.W)){
		#else
		if(CrossPlatformInputManager.GetButton("PlayerMoveForward")){
		#endif
			m_PlayerMoveDetection.z += 1.0f;
		}

		//後退
		#if UNITY_EDITOR
		if (Input.GetKey(KeyCode.S)){
		#else
		if(CrossPlatformInputManager.GetButton("PlayerMoveBack")){
		#endif
			m_PlayerMoveDetection.z -= 1.0f;
		}

		//左平行移動
		#if UNITY_EDITOR
		if (Input.GetKey(KeyCode.A)){
		#else
		if(CrossPlatformInputManager.GetButton("PlayerMoveLeft")){
		#endif
			m_PlayerMoveDetection.x -= 1.0f;
		}

		//右平行移動
		#if UNITY_EDITOR
		if (Input.GetKey(KeyCode.D)){
		#else
		if(CrossPlatformInputManager.GetButton("PlayerMoveRight")){
		#endif
			m_PlayerMoveDetection.x += 1.0f;
		}

		m_PlayerMoveVelocity = this.transform.TransformDirection(m_PlayerMoveDetection);
		m_PlayerMoveVelocity *= m_PlayerMoveSpeed; //移動スピード

		//ジャンプと落下処理 =========================
		if (!isGrounded){
			m_PlayerAddJumpPower -= m_PlayerGravity; //重力
			m_PlayerMoveVelocity.y += m_PlayerAddJumpPower;
		}else{
			m_PlayerMoveVelocity.y -= m_PlayerGravity; //重力
		}

#if UNITY_EDITOR
		if (!m_JumpButtonPushingFlag && Input.GetKeyDown(KeyCode.RightShift)){
#else
		if(!m_JumpButtonPushingFlag && CrossPlatformInputManager.GetButtonDown("PlayerJump")){
#endif
			if(isGrounded){
				m_PlayerAddJumpPower = m_PlayerJumpPower;
				m_PlayerMoveVelocity.y += m_PlayerAddJumpPower;
			}
			m_JumpButtonPushingFlag = true;
#if UNITY_EDITOR
		}else if(Input.GetKeyUp(KeyCode.RightShift)){
#else
		}else if(CrossPlatformInputManager.GetButtonUp("PlayerJump")){
#endif
			m_JumpButtonPushingFlag = false;
		}
		// =========================================

		m_CharacterController.Move(m_PlayerMoveVelocity * Time.deltaTime);

	}

	/// <summary>
	/// 着地判定
	/// </summary>
	protected virtual void groundRayUpdate(){

		RaycastHit hit;
		m_GroundRayPos = m_PlayerTransform.position + m_CharacterController.center;
		m_GroundRayDistance = (m_CharacterController.height / 2) - (m_CharacterController.radius / 2);

		// Debug.Log("MOB- m_GroundRayPos : " + m_GroundRayPos);
		// Debug.Log("MOB- m_GroundRayDistance : " + m_GroundRayDistance);

		if(Physics.SphereCast(m_GroundRayPos, m_CharacterController.radius, m_PlayerTransform.TransformDirection(Vector3.down), out hit, m_GroundRayDistance)){
			if(!isGrounded){

				m_PlayerTransform.position = new Vector3(
					m_PlayerTransform.position.x,
					hit.point.y + (m_CharacterController.height / 2),
					m_PlayerTransform.position.z
				);

				isGrounded = true;
			}
		}else{
			isGrounded = false;
		}

		Debug.Log("MOB- isGrounded : "+isGrounded);

	}
#endregion

	// --------
#region インナークラス
#endregion

}
