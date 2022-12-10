using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ControllerInput : MonoBehaviour, IListener {
    public delegate void InputEvent(Vector2 direction);
    public static event InputEvent inputEvent;

    public static ControllerInput Instance;
    public GameObject rangeAttack;

	Player Player;

    [Header("Button")]
    public GameObject btnJump;
    public GameObject btnRange;

    [Header("---PARACHUTE---")]
    public Button parachuteBtn;

    CanvasGroup canvasGroup;

    GameplayControl controls;

    private void Awake()
    {
        Instance = this;
        controls = new GameplayControl();
        controls.PlayerControl.Jump.started += ctx => Jump();
        controls.PlayerControl.Jump.canceled += ctx => JumpOff();

        controls.PlayerControl.Melee.started += ctx => MeleeAttack();
        controls.PlayerControl.Throw.started += ctx => RangeAttack();

        controls.PlayerControl.Parachute.started += ctx => Parachute(true);
        controls.PlayerControl.Parachute.canceled += ctx => Parachute(false);

        controls.PlayerControl.MoveLeft.started += ctx => MoveLeft();
        controls.PlayerControl.MoveLeft.canceled += ctx => StopMove();

        controls.PlayerControl.MoveRight.started += ctx => MoveRight();
        controls.PlayerControl.MoveRight.canceled += ctx => StopMove();
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            StopMove();

        controls.PlayerControl.Enable();
    }

    private void OnDisable()
	{
		controls.PlayerControl.Disable();

	}

    public void TurnJump(bool isOn)
    {
        btnJump.SetActive(isOn);
    }

    public void TurnRange(bool isOn)
    {
        btnRange.SetActive(isOn);
    }

    void Start () {
        canvasGroup = GetComponent<CanvasGroup>();

        Player = FindObjectOfType<Player> ();
		if(Player==null)
			Debug.LogError("There are no Player character on scene");
    }

	void Update(){

        if (isMovingRight)
            MoveRight();
        else if (isMovingLeft)
            MoveLeft();

        parachuteBtn.interactable = GameManager.Instance.Player.canUseParachute();
    }

    bool isMovingLeft, isMovingRight;
	
	public void MoveLeft(){
        if (GameManager.Instance.State == GameManager.GameState.Playing)
        {
            Player.MoveLeft();
            isMovingLeft = true;
            if (inputEvent != null)
                inputEvent(Vector2.left);
        }
	}

	public void MoveRight(){
        if (GameManager.Instance.State == GameManager.GameState.Playing)
        {
            Player.MoveRight();
            isMovingRight = true;
            if (inputEvent != null)
                inputEvent(Vector2.right);
        }
	}

	public void FallDown(){
        if (GameManager.Instance.State == GameManager.GameState.Playing)
        {
            Player.FallDown();
            if (inputEvent != null)
                inputEvent(Vector2.down);
        }
	}


	public void StopMove(){
        if (GameManager.Instance.State == GameManager.GameState.Playing)
        {
            Player.StopMove();
            isMovingLeft = false;
            isMovingRight = false;
            if (inputEvent != null)
                inputEvent(Vector2.zero);
        }
	}

	public void Jump (){
		if (GameManager.Instance.State == GameManager.GameState.Playing)
			Player.Jump ();
	}

	public void JumpOff(){
		if (GameManager.Instance.State == GameManager.GameState.Playing)
			Player.JumpOff ();
	}

	public void RangeAttack(){
        if (GameManager.Instance.State == GameManager.GameState.Playing)
        {
            Player.RangeAttack();
        }
	}

    public void MeleeAttack()
    {
        if (GameManager.Instance.State == GameManager.GameState.Playing)
            Player.MeleeAttack();
    }

    public void Parachute(bool useParachute)
    {
        if (GameManager.Instance.State == GameManager.GameState.Playing)
        {
            GameManager.Instance.Player.SetParachute(useParachute);
        }
    }

    public void NoHoldingNormalBullet()
    {
    
    }

    public void IPlay()
    {

    }

    public void ISuccess()
    {

    }

    public void IPause()
    {

    }

    public void IUnPause()
    {

    }

    public void IGameOver()
    {
       
    }

    public void IOnRespawn()
    {

    }

    public void IOnStopMovingOn()
    {

    }

    public void IOnStopMovingOff()
    {

    }

    public void ShowController(bool show)
    {
        if (show)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1;
        }
        else
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
        }
    }
}
