using UnityEngine;

public class TankMovement : MonoBehaviour
{
	public int m_PlayerNumber = 1;              // どのタンクがどのプレイヤーに属するかを識別するために使用
	public float m_Speed = 12f;                 // タンクが前後に移動する速さ
	public float m_TurnSpeed = 180f;            // タンクが1秒間に何度回転するか
	public AudioSource m_MovementAudio;         // エンジン音の再生に使用されるオーディオソースへの参照（砲撃オーディオソースとは異なることに注意）
	public AudioClip m_EngineIdling;            // タンクが動かないときに再生するオーディオ
	public AudioClip m_EngineDriving;           // タンクが動いているときに再生するオーディオ
	public float m_PitchRange = 0.2f;           // エンジン音のピッチを変化させる量

    
	private string m_MovementAxisName;          // 前後の動きの入力軸の名前
	private string m_TurnAxisName;              // 回転の入力軸の名前
	private Rigidbody m_Rigidbody;              // タンクを動かすのに使用する参照
	private float m_MovementInputValue;         // 動きの現在の入力値
	private float m_TurnInputValue;             // 回転の現在の入力値
	private float m_OriginalPitch;              // シーン開始時のオーディオソースのピッチ


	private void Awake ()
	{
		m_Rigidbody = GetComponent<Rigidbody> ();
	}


	private void OnEnable ()
	{
		// タンクがオンの時、キネマティックでないことを確認
		m_Rigidbody.isKinematic = false;

		// 入力値もリセット
		m_MovementInputValue = 0f;
		m_TurnInputValue = 0f;
	}


	private void OnDisable ()
	{
		// タンクがオフの時、キネマティックに設定
		m_Rigidbody.isKinematic = true;
	}


	private void Start ()
	{
		// 軸の名前はプレイヤー番号に基づく
		m_MovementAxisName = "Vertical" + m_PlayerNumber;
		m_TurnAxisName = "Horizontal" + m_PlayerNumber;

		// オーディオソースの元のピッチを格納
		m_OriginalPitch = m_MovementAudio.pitch;
	}


	private void Update ()
	{
		//両方の入力軸の値を保存
		m_MovementInputValue = Input.GetAxis (m_MovementAxisName);
		m_TurnInputValue = Input.GetAxis (m_TurnAxisName);

		EngineAudio ();
	}


	private void EngineAudio ()
	{
		// タンクが動いているかを判断
		if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f)
		{
			if (m_MovementAudio.clip == m_EngineDriving)
			{
				// 止まっている音を再生
				m_MovementAudio.clip = m_EngineIdling;
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
				m_MovementAudio.Play ();
			}
		}
		else
		{
			if (m_MovementAudio.clip == m_EngineIdling)
			{
				// 移動している音お再生
				m_MovementAudio.clip = m_EngineDriving;
				m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
				m_MovementAudio.Play();
			}
		}
	}


	private void FixedUpdate ()
	{
		// リジッドボディの位置と向きを FixedUpdateで調整します。
		Move ();
		Turn ();
	}


	/**
	 * タンクを移動させる
	 */
	private void Move ()
	{
		// 入力、スピード、フレーム間の時間に基づいて、タンクが向いている方向に量のベクトルを作成
		Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

		// この動きをリジッドボディの位置に適用
		m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
	}


	/*
	 * タンクを回転させる
	 */
	private void Turn ()
	{
		// 入力、スピード、フレーム間の時間に基づいて、回転する度数を決定
		float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

		// それを y 軸の回転に設定
		Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

		// この回転をリジッドボディの回転に適用
		m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
	}
}