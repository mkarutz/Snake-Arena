using UnityEngine;
using System;
using UnityEngine.UI;


// This class should be added to any gameobject in the scene
// that should react to input based on the user's gaze.
// It contains events that can be subscribed to by classes that
// need to know about input specifics to this gameobject.

namespace GazeInput{
    [RequireComponent(typeof(BoxCollider))]
    public class VRInteractiveItem : MonoBehaviour
    {
    [SerializeField]
        private bool autoScale = true; // scale the Collider with the button.
    


        protected bool m_IsOver;


        public bool IsOver
        {
            get { return m_IsOver; }              // Is the gaze currently over this object?
        }

       
        void Start() // Scales the colider correctly on buttons
        {
            if (GetComponent<RectTransform>() != null && autoScale == true)
            {
            BoxCollider _col = transform.GetComponent<BoxCollider>();
                RectTransform Rtransform = GetComponent<RectTransform>();
            _col.size = Rtransform.rect.size;

            if (transform.parent.GetComponent<ScrollRect>() && GetComponent<Scrollbar>()) {
                _col.size = new Vector3(_col.size.x, transform.parent.GetComponent<RectTransform>().rect.size.y, _col.size.z);
            }
            }
        }

    void LateUpdate() {
        if (transform.Find("Dropdown List")) {
            Transform[] Children = transform.Find("Dropdown List").GetComponentsInChildren<Transform>();
            foreach (Transform _child in Children) {
                if (!_child.GetComponent<VRInteractiveItem>() && _child.GetComponent<Toggle>()) {
                    _child.gameObject.AddComponent<VRInteractiveItem>();
                }
            }

        }
    }
	}

}
