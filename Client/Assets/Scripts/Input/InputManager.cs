using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class InputManager : MonoBehaviour {
	private Vector2 targetDirection;
	private bool isTurbo = false;
    private SnakeState playerSnake;
    private GameWorld gameWorld;


	private ControlScheme activeControlScheme;
	public FollowFingerControlScheme followFingerControlScheme;
	public JoystickControlScheme joystickControlScheme;
	public AccelerometerControlScheme accelerometerControlScheme;


    void Start ()
    {
		SetControlScheme();
    }


	void Update () 
	{
		GetReferences();
		UpdateInput();
		//        if (VRSettings.enabled) {
		//			UpdateVRInput();
		//        } else {
		//			UpdateInput();
		//        }
	}


	void GetReferences()
	{
		this.playerSnake = GameObject.FindGameObjectWithTag("Player").GetComponent<SnakeState>();
		this.gameWorld = GameObject.FindGameObjectWithTag("World").GetComponent<GameWorld>();
	}


	void SetControlScheme()
	{
		int controlSchemeId = PlayerProfile.Instance().ControlScheme;

		switch (controlSchemeId) {
		case 0:
			ActivateControlScheme(followFingerControlScheme);
			break;
		case 1:
			ActivateControlScheme(joystickControlScheme);
			break;
		case 2:
			ActivateControlScheme(accelerometerControlScheme);
			break;
		default:
			ActivateControlScheme(followFingerControlScheme);
			break;
		}
	}


	void ActivateControlScheme(ControlScheme inputScheme)
	{
		inputScheme.gameObject.SetActive(true);
		activeControlScheme = inputScheme;
	}


	void UpdateVRInput()
	{
		// Virtual reality input
		// Move towards where we're looking
		Ray lookRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
		float dist;
		if (this.gameWorld.GetWorldPlane().Raycast(lookRay, out dist)) {
			Vector3 lookWorldSpacePos = lookRay.GetPoint(dist);
			Vector2 lookVec = lookWorldSpacePos - this.playerSnake.transform.position;
			targetDirection = lookVec * 1000;
		}
	}


	void UpdateInput()
	{
		targetDirection = activeControlScheme.TargetDirection();
		isTurbo = activeControlScheme.IsTurbo();
	}


	public Vector2 TargetDirection() {
		return targetDirection;
	}


	public bool IsTurbo() {
		return isTurbo;
	}
}
