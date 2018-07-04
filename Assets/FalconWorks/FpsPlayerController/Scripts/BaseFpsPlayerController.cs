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
	[SerializeField] protected float m_PlayerRoateSpeed = 60.0f;
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
		if (CrossPlatformInputManager.GetButton("PlayerCameraRotateUp")){
			m_CameraNewAngle.x -= m_CameraVerticalRotateSpeed * Time.deltaTime;
			if(m_CameraNewAngle.x > m_CameraDownAngleMax && m_CameraNewAngle.x < m_CameraUpAngleMax){
				m_CameraNewAngle.x = m_CameraUpAngleMax;
			}
		}
		//下方向に回転
		if (CrossPlatformInputManager.GetButton("PlayerCameraRotateDown")){
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

		if (CrossPlatformInputManager.GetButton("PlayerRotateLeft")){
			m_PlayerNewAngle.y -= m_PlayerRoateSpeed * Time.deltaTime;
		}
		if (CrossPlatformInputManager.GetButton("PlayerRotateRight")){
			m_PlayerNewAngle.y += m_PlayerRoateSpeed * Time.deltaTime;
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
		if(CrossPlatformInputManager.GetButton("PlayerMoveForward")){
			m_PlayerMoveDetection.z += 1.0f;
		}
		//後退
		if(CrossPlatformInputManager.GetButton("PlayerMoveBack")){
			m_PlayerMoveDetection.z -= 1.0f;
		}
		//左平行移動
		if(CrossPlatformInputManager.GetButton("PlayerMoveLeft")){
			m_PlayerMoveDetection.x -= 1.0f;
		}
		//右平行移動
		if(CrossPlatformInputManager.GetButton("PlayerMoveRight")){
			m_PlayerMoveDetection.x += 1.0f;
		}

		m_PlayerMoveVelocity = this.transform.TransformDirection(m_PlayerMoveDetection);
		m_PlayerMoveVelocity *= m_PlayerMoveSpeed; //移動スピード

		//ジャンプ
		if(!m_JumpButtonPushingFlag && CrossPlatformInputManager.GetButtonDown("PlayerJump")){
			if(m_CharacterController.isGrounded){
				Debug.Log("JUMP");
				m_PlayerMoveVelocity.y += m_PlayerJumpPower;
			}
			m_JumpButtonPushingFlag = true;
		}else if(CrossPlatformInputManager.GetButtonUp("PlayerJump")){
			m_JumpButtonPushingFlag = false;
		}

		m_PlayerMoveVelocity.y -= m_PlayerGravity; //重力

		m_CharacterController.Move(m_PlayerMoveVelocity * Time.deltaTime);

	}
	#endregion

	// --------
	#region インナークラス
	#endregion

}
