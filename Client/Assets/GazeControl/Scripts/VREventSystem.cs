using UnityEngine;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This scripts allows interactions with objects in the scene based on a gaze.
/// This class casts a ray into the scene and if it finds a VRIneractive item it 
/// calls the events for the item to use.
/// need help? contact us: babilinapps.com
/// </summary>

namespace GazeInput{

public class VREventSystem : MonoBehaviour
    {
     
        public event Action<RaycastHit> OnRaycasthit;                   // This event is called every frame that the user's gaze is over a collider.

       
   
	[Tooltip("The Scale of the crosshair")]
	[SerializeField]
		private float scale = .5f; 
    [Tooltip("Layers excluded from the raycast.")]
    [SerializeField]
        private LayerMask _exclusionLayers;
    [Tooltip("Should the cursor grow, when interacting with an object?")]
    [SerializeField]
    public bool  growAnim = true;
        [Tooltip("Use the ‘Auto Click Timer’ to press the item.")]
    [SerializeField]
        public bool autoClick = true;
    [Tooltip("How long does the user have to look at an item before it's clickable")]
    [SerializeField]
         public float clickDelay = 1;
    [Tooltip("Adds ‘VR Interactive Item’ script to all UI objects.")]
    [SerializeField]
         private bool AutoAddVRInteract = true;
    [Tooltip("How far into the scene the ray is cast.")]
    [SerializeField]
        private float _rayLength = 500f;
    [Tooltip("Time that has to pass to click event.")]
    public  float autoClickTime = 1;
    [Tooltip("show crosshair at all times.")]
    public bool classicCursor = true;

	public enum CrosshairType { Radial,RadialSimple, Simple, Shrink, };
	[Tooltip("Which fill animation to play")]
	public CrosshairType fillAnimation; 



        private PointerEventData eventSystem;                           // Is used to send simple events
		private VRInteractiveItem _CurrentInteractible;                //The current interactive item
        private VRInteractiveItem _LastInteractible;                   //The last interactive item
        private bool clicking;                            // calls action if time has passed or button is pressed.
        private GameObject cursor;                        // The Cursor that shows in the middle of the screen.
        private Image _imageFill;
        private float lookTime;                            //current amout of time that the user is looking at the object.
        private Transform _camera;                         // Stores the camera transform.
		private GameObject crosshairCanvas; 			// Stores the canvas that has the crossHair attached;
		private GameObject crosshairFillObj;			// Stores the crosshairfill 
        public bool cardboardTrigger;
		private  Vector3 crosshairLocation;

        public static Ray gazeRay;                        // stores the ray that hits UI objects.
        public delegate void Hover();
        public static event Hover OnHover;
        public delegate void Deselect();
        public static event Deselect OnDeselect;

	//used for Animation
	private Animator fillAnimator;
	private Animator crosshairAnimator;

        public static VREventSystem curEventSystem;
        
    // TODO Utility for other classes to get the current interactive item
    public VRInteractiveItem CurrentInteractible
        {
            get { return _CurrentInteractible; }
        }


        void Awake()
        {
       

            if (curEventSystem == null)
            {
                curEventSystem = this;
            }else
            {
                this.enabled = false;
            }

        
    }

        void Start()
        {
            //Gets the main Camera
            _camera = Camera.main.transform;
            //Gets active event system
            eventSystem = new PointerEventData(EventSystem.current);

     	   UnityEngine.VR.InputTracking.Recenter(); // recenters the VR input
			SetUp ();

            // ******************************************
          // Uncomment for cardboard V1 trigger support 
         //   cardboardTrigger = Cardboard.SDK.Triggered;
		// ******************************************

        }

        void SetUp()
        {

            if (AutoAddVRInteract)
                FindGameObjectsWithLayer(LayerMask.NameToLayer("UI"));



            crosshairCanvas = Instantiate(Resources.Load("Crosshair_Canvas")) as GameObject;
            crosshairCanvas.transform.SetParent(Camera.main.transform);
            crosshairCanvas.transform.localPosition = Vector3.forward;
            crosshairCanvas.transform.localScale = new Vector3(scale, scale, scale);

            crosshairCanvas.transform.rotation = Quaternion.Euler(Vector3.zero);
            cursor = GameObject.FindGameObjectWithTag("Crosshair");
            switch (fillAnimation)
            {
                case CrosshairType.Simple:
                    crosshairFillObj = Instantiate(Resources.Load("Radial0")) as GameObject;
                    crosshairFillObj.transform.SetParent(cursor.transform);
                    crosshairFillObj.transform.localPosition = Vector3.zero;
                    crosshairFillObj.transform.localScale = Vector3.one;
                    _imageFill = crosshairFillObj.GetComponent<Image>();
                    break;

                case CrosshairType.RadialSimple:
                    crosshairFillObj = Instantiate(Resources.Load("Radial 2")) as GameObject;
                    crosshairFillObj.transform.SetParent(cursor.transform);
                    crosshairFillObj.transform.localPosition = Vector3.zero;
                    crosshairFillObj.transform.localScale = Vector3.one;
                    break;

                case CrosshairType.Radial:
                    crosshairFillObj = Instantiate(Resources.Load("Radial 1")) as GameObject;
                    crosshairFillObj.transform.SetParent(cursor.transform);
                    crosshairFillObj.transform.localPosition = Vector3.zero;
                    crosshairFillObj.transform.localScale = Vector3.one;

                    break;

                case CrosshairType.Shrink:
                    crosshairFillObj = Instantiate(Resources.Load("Radial 3")) as GameObject;
                    crosshairFillObj.transform.SetParent(cursor.transform);
                    crosshairFillObj.transform.localPosition = Vector3.zero;
                    crosshairFillObj.transform.localScale = Vector3.one;

                    break;
            }

            crosshairAnimator = cursor.GetComponentInChildren<Animator>();
         
           fillAnimator = crosshairFillObj.GetComponent<Animator>();


            if (!growAnim) // stops animations from the main crosshair
                crosshairAnimator.speed = 0;

        }


        private void Update()
        {
		
		if (crosshairFillObj) {
			if (fillAnimator) {
				fillAnimator.speed = 2 / autoClickTime; 
			}
                if (!growAnim) // stops animations from the main crosshair
                    crosshairAnimator.speed = 0;
                else
                crosshairAnimator.speed = 1 / clickDelay;
		}

      

        if (!autoClick) {
          //  Cursor.lockState = CursorLockMode.Locked;
          if(crosshairFillObj)
			crosshairFillObj.SetActive (false);

			if (Input.GetKeyDown(KeyCode.Mouse0) || cardboardTrigger)
                {
                HandleClick();
            }

        }
        else
        {
        //    Cursor.lockState = CursorLockMode.Confined;
			if(_imageFill)
			_imageFill.fillAmount = lookTime-clickDelay / autoClickTime;
			
        }
        EyeRaycast();

		//places the cursor on the active object
		if (cursor != null) {
			if (cursor.activeSelf)
				PlaceCursor ();
		}
        }
        private void EyeRaycast()
        {
            // Create a ray that points forwards from the camera.
            gazeRay = new Ray(_camera.position, _camera.forward);
            RaycastHit hit;

            // Do the raycast forweards to see if we hit an interactive item
            if (Physics.Raycast(gazeRay, out hit, _rayLength, ~_exclusionLayers))
            {
                VRInteractiveItem interactible = hit.collider.GetComponent<VRInteractiveItem>(); //attempt to get the VRInteractiveItem on the hit object
                _CurrentInteractible = interactible;
                crosshairFillObj.SetActive(true);
                //Sets the mouse positon at the colided point
                eventSystem.position = hit.point;

                // If we hit an interactive item and it's not the same as the last interactive item, then call Over
                if (interactible && interactible != _LastInteractible)
                {
                
                    DeactiveLastInteractible();
                }

            //We see if we are hovering over an item and then call  the action
            if (interactible && interactible == _LastInteractible)
            {
                HandleHover();
                cursor.SetActive(true);
            }

                //We update the last interactible item
                _LastInteractible = interactible;


                if (OnRaycasthit != null)
                    OnRaycasthit(hit);
            }
            else
            {
                _CurrentInteractible = null;
                crosshairFillObj.SetActive(false);
                // Nothing was hit, deactive the last interactive item.
                DeactiveLastInteractible();
             if (cursor && !classicCursor)
                    cursor.SetActive(false);
            }
        }


        //Desctivates last 
        private void DeactiveLastInteractible()
        {
            lookTime = 0;
         
            if (_LastInteractible == null)
                return;

            ExecuteEvents.Execute(_LastInteractible.gameObject, eventSystem, ExecuteEvents.deselectHandler);
          
            _LastInteractible = null;
            if (OnDeselect != null)
            {
                OnDeselect();
            }
        }

        // Deselects all 
        private void HandleDeselect()
        {
            lookTime = 0;
            if (_LastInteractible == null)
                return;
            ExecuteEvents.Execute(_CurrentInteractible.gameObject, eventSystem, ExecuteEvents.deselectHandler);
            _LastInteractible = _CurrentInteractible;
       
            if(OnDeselect != null)
            {
                OnDeselect();
            }
        }

        //Hover Action
        private void HandleHover()
        {
            if (_CurrentInteractible == null || clicking == true)
                return;

            if (autoClick) // Does the AutoClick
                HandleAutoClick();

            if (OnHover != null) // Alows other scripts to subscribe to the  hover event.
                OnHover();

          ExecuteEvents.Execute(_CurrentInteractible.gameObject, eventSystem, ExecuteEvents.selectHandler);


        }

        //Key Up
        private void HandleUp()
        {
            if (_CurrentInteractible != null)
            {
               
                ExecuteEvents.Execute(_CurrentInteractible.gameObject, eventSystem, ExecuteEvents.pointerUpHandler);
            }
        }

        //Key Down
        private void HandleDown()
        {
            if (_CurrentInteractible != null)
            {
              
              ExecuteEvents.Execute(_CurrentInteractible.gameObject, eventSystem, ExecuteEvents.pointerDownHandler);
            }
        }

        //Click action ( button down)
        private void HandleClick()
        {
      
            if (_CurrentInteractible != null)
            {
               
                if (clicking == false)
                {
                    StartCoroutine(preformClick());
                    clicking = true;
                }
            }

        }

        //Makes sure that the press is rendered
        IEnumerator preformClick()
        {
     
         yield return new WaitForEndOfFrame();
            if (_CurrentInteractible)
                ExecuteEvents.Execute(_CurrentInteractible.gameObject, eventSystem, ExecuteEvents.deselectHandler);
          yield return new WaitForEndOfFrame();
            if (_CurrentInteractible)
                ExecuteEvents.Execute(_CurrentInteractible.gameObject, eventSystem, ExecuteEvents.pointerDownHandler);
          yield return new WaitForEndOfFrame();
            if (_CurrentInteractible)
                ExecuteEvents.Execute(_CurrentInteractible.gameObject, eventSystem, ExecuteEvents.pointerUpHandler);
        yield return new WaitForEndOfFrame();
            if (_CurrentInteractible)
                ExecuteEvents.Execute(_CurrentInteractible.gameObject, eventSystem, ExecuteEvents.pointerClickHandler);
            clicking = false;
            HandleDeselect();

        }

        //Preforms the look to click action
        private void HandleAutoClick()
        {
        if (lookTime > clickDelay)
        {
			if (fillAnimation != CrosshairType.Simple && autoClick) {
				crosshairFillObj.SetActive (true);
                    if(growAnim)
				crosshairAnimator.Play ("Start");
                    else
                    {
                        crosshairAnimator.Stop();
                    }
			}


            if (lookTime > autoClickTime+clickDelay)
            {
				cursor.SetActive (false);

                HandleClick();
                lookTime = 0;


            }
            else
            {
                lookTime = lookTime + Time.deltaTime;
            }
        }else
        {
            lookTime = lookTime + Time.deltaTime;
			if (fillAnimation != CrosshairType.Simple) {
				crosshairFillObj.SetActive (false);
			}
        }
        }


    //This can be called in other scripts
    public float CursorDistance()
        {
            if (_LastInteractible != null)
                return Vector3.Distance(_LastInteractible.transform.position, _camera.transform.position);
            else
                return 1;
        }

        //Gets the active VREventSystem usefull for public bools
        public static VREventSystem GetCurrent()
        {
            return curEventSystem;
        }


       public static void FindGameObjectsWithLayer (int layer) {
			GameObject[] goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        for (int i = 0; i < goArray.Length; i++)
			{ if (goArray[i].layer == layer){
					foreach (Transform trans in goArray[i].GetComponentsInChildren( typeof(Transform), true )) {
						if ( !trans.GetComponent<VRInteractiveItem> () &&
						 ( trans.GetComponent<Toggle> () ||  trans.GetComponent<Button> () ||  trans.GetComponent<Slider> ()
						 ||  trans.GetComponent<Scrollbar> () ||  trans.GetComponent<EventTrigger> () ||  trans.GetComponent<Dropdown> ())) {
							trans.gameObject.AddComponent<VRInteractiveItem> ();
						}
					}
			}
        }
 
    }

	void PlaceCursor(){
		if (_CurrentInteractible != null) {
       
                float distance = Vector3.Distance(Camera.main.transform.position, _CurrentInteractible.transform.position);
                RaycastHit thing;
                if (Physics.Raycast(gazeRay, out thing))
                {
                    crosshairLocation = thing.point;
                }

            
				
					cursor.transform.localScale = new Vector3 (distance, distance);
			
				cursor.transform.position = crosshairLocation;
		
		} else {
             
				cursor.transform.localPosition = new Vector3 (0, 0, 1);
				cursor.transform.localScale = Vector3.one;
				cursor.transform.LookAt (Camera.main.transform);

		}

	}
        void DisableEvents()
        {
           curEventSystem.enabled = false;
        }
    }
}

    

