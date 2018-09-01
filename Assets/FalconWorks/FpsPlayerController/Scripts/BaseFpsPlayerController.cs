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
	[SerializeField] protected float m_PlayerDashMoveSpeed = 6.0f;
	/// <summary> 
	/// 
	/// </summary>
	[SerializeField] protected float m_PlayerGravity = 1.0f;
	/// <summary> 
	/// プレイヤーのジャンプ力
	/// </summary>
	[SerializeField] protected float m_PlayerJumpPower = 20.0f;
	/// <summary> 
	/// プレイヤーのホバリング力
	/// </summary>
	[SerializeField] protected float m_PlayerHoveringPower = 3.0f;
	/// <summary> 
	/// プレイヤーの最大ホバリング力
	/// </summary>
	[SerializeField] protected float m_MaxPlayerHoveringPower = 6.0f;
	[Header("*Player Rotation Setting")]
	/// <summary> 
	/// 
	/// </summary>
	[SerializeField] protected float m_PlayerRotateSpeed = 60.0f;
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
	protected float m_PlayerAddHoveringPower = 0.0f;
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
	/// <summary>
	/// ホバリングモードフラグ
	/// </summary>
	protected bool m_IsHovering;
	/// <summary>
	/// ホバリングモードフラグ
	/// </summary>
	public bool IsHovering {
		get { return m_IsHovering; }
		protected set { m_IsHovering = value; }
	}
	/// <summary>
	/// ダッシュフラグ
	/// </summary>
	protected bool m_DashModeFlag;
	/// <summary>
	/// 移動ボタンを押下中フラグ
	/// </summary>
	protected bool m_MoveButtonPushingFlag;
	/// <summary>
	/// 最初に移動ボタンが押されてからの経過時間
	/// </summary>
	protected float m_MoveButtonPushingElapsedTime;
	/// <summary>
	/// 次に移動ボタンが押されるまでの時間
	/// </summary>	
	protected float m_nextMoveButtonDownTime = 1.0f;
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

		dashModeFlagUpdate();

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
	/// ダッシュモードフラグ更新処理
	/// </summary>
	protected virtual void dashModeFlagUpdate(){

		if(!m_DashModeFlag){

			/*--------------
			* 通常時
			--------------*/

			#if UNITY_EDITOR
			if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W) ||
			 Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S)){
			#else
			if(CrossPlatformInputManager.GetButtonDown("PlayerMoveForward") ||
			 CrossPlatformInputManager.GetButtonDown("PlayerMoveBack") ||
			 CrossPlatformInputManager.GetButtonDown("PlayerMoveLeft") ||
			 CrossPlatformInputManager.GetButtonDown("PlayerMoveRight")){
			#endif
				if(!m_MoveButtonPushingFlag){
					//移動ボタン押下1回目
					m_MoveButtonPushingFlag = true;
					m_MoveButtonPushingElapsedTime = 0f;
				}else{
					//移動ボタン押下2回目
					if(m_MoveButtonPushingElapsedTime <= m_nextMoveButtonDownTime){
						m_DashModeFlag = true;
					}
				}
			}

		}else{

			/*--------------
			* ダッシュモード時
			--------------*/

			#if UNITY_EDITOR
			if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) &&
			 !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S)){
			#else
			if(!CrossPlatformInputManager.GetButton("PlayerMoveForward") &&
			 !CrossPlatformInputManager.GetButton("PlayerMoveBack") &&
			 !CrossPlatformInputManager.GetButton("PlayerMoveLeft") &&
			 !CrossPlatformInputManager.GetButton("PlayerMoveRight")){
			#endif
				m_MoveButtonPushingFlag = false;
				m_DashModeFlag = false;
			}

		}

		//最初の移動ボタンを押していれば時間計測
		if(m_MoveButtonPushingFlag){
			//時間計測
			m_MoveButtonPushingElapsedTime += Time.deltaTime;

			if(m_MoveButtonPushingElapsedTime > m_nextMoveButtonDownTime){
				m_MoveButtonPushingFlag = false;
			}
		}

	}

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
		if(CrossPlatformInputManager.GetButton("PlayerRotateLeft")){
		#endif
			m_PlayerNewAngle.y -= m_PlayerRotateSpeed * Time.deltaTime;
		}

		#if UNITY_EDITOR
		if(Input.GetKey(KeyCode.RightArrow)){
		#else
		if(CrossPlatformInputManager.GetButton("PlayerRotateRight")){
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

		if(m_DashModeFlag){
			m_PlayerMoveVelocity *= m_PlayerDashMoveSpeed; //移動スピード
		}else{
			m_PlayerMoveVelocity *= m_PlayerMoveSpeed; //移動スピード
		}

		if(IsHovering){

			//ホバリングと落下処理 =========================
			#if UNITY_EDITOR
			if (Input.GetKey(KeyCode.RightShift)){
			#else
			if (CrossPlatformInputManager.GetButton("PlayerJump")){
			#endif
				m_PlayerAddHoveringPower += m_PlayerHoveringPower;
				if(m_PlayerAddHoveringPower > m_MaxPlayerHoveringPower){
					m_PlayerAddHoveringPower = m_MaxPlayerHoveringPower;
				}
			}else{
				m_PlayerAddHoveringPower -= m_PlayerHoveringPower * 0.3f;
			}

			if(m_PlayerAddHoveringPower < 0.0f){
				m_PlayerAddHoveringPower = 0.0f;
			}else{
				m_PlayerMoveVelocity.y += m_PlayerAddHoveringPower;
			}

			m_PlayerMoveVelocity.y -= m_PlayerGravity * 3.0f; //重力

		}else{

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

		}

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

		// Debug.Log("MOB- isGrounded : "+isGrounded);

	}

	/// <summary>
	/// ホバリング機能ON・OFF関数
	/// </summary>
	/// <param name="_flag">ホバリングモードフラグ</param>
	public virtual void chgHoveringMode(bool _flag){
		IsHovering = _flag;
	}
	#endregion

	// --------
	#region インナークラス
	#endregion

}
