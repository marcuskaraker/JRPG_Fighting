using UnityEngine;
using UnityEngine.Events;

namespace MK.ThirdPerson
{
    [System.Serializable]
    public class InputKeys
    {
        // This is a class so that the variables can have default values.

        public string horizontalInputID = "Horizontal";
        public string verticalInputID = "Vertical";
        public string jumpInputID = "Jump";
        public string interactInputID = "Fire1";
        public string zoomInInputID = "Fire2";
        public string zoomOutInputID = "Fire3";
        public string scrollWheelInputID = "Mouse ScrollWheel";
    }

    public class ThirdPersonInput : MonoBehaviour
    {
        public Movement target;
        public ThirdPersonCamera cameraController;

        public float movementInputMultiplier = 1.2f;
        public bool useController = false;

        public InputKeys inputKeys;

        [Header("Zoom")]
        public Vector2 inputZoomLimits = new Vector2(-5, 5);
        public float inputZoomSpeed = 1f;
        public float inputZoomValue = 0.0f;
        public float globalZoomValue = 0.0f;

        public UnityEvent onInteractDown;
        public UnityEvent onInteractUp;

        private bool controllerActive;

        private void Awake()
        {
            target = target.GetComponent<Movement>();
            controllerActive = Input.GetJoystickNames().Length > 0;
        }

        private void Update()
        {
            float moveX = Input.GetAxis(inputKeys.horizontalInputID) * movementInputMultiplier;
            float moveZ = Input.GetAxis(inputKeys.verticalInputID) * movementInputMultiplier;

            Vector3 cameraBasedMovement = 
                (cameraController.transform.forward * moveZ) +
                (cameraController.transform.right * moveX);

            Vector3 movement = Vector3.ClampMagnitude(cameraBasedMovement, 1f);
            
            MovementInput(movement);

            // Zoom
            if (useController && controllerActive)
            {
                ZoomUpdateController();
            }
            else
            {
                ZoomUpdateMouse();
            }

            cameraController.zoomValue = globalZoomValue + inputZoomValue;
        }

        private void MovementInput(Vector3 movement)
        {
            target.Move(movement);

            if (Input.GetButtonDown(inputKeys.jumpInputID))
            {
                target.Jump();
            }

            if (Input.GetButtonDown(inputKeys.interactInputID))
            {
                onInteractDown.Invoke();
            }

            if (Input.GetButtonUp(inputKeys.interactInputID))
            {
                onInteractUp.Invoke();
            }
        }

        private void ZoomUpdateController()
        {
            bool zoomInButton = Input.GetButton(inputKeys.zoomInInputID);
            bool zoomOutButton = Input.GetButton(inputKeys.zoomOutInputID);
            if (zoomInButton && zoomOutButton)
            {
                inputZoomValue = 0.0f;
            }
            else if (zoomInButton)
            {
                inputZoomValue += inputZoomSpeed * Time.deltaTime;
                inputZoomValue = Mathf.Clamp(inputZoomValue, inputZoomLimits.x, inputZoomLimits.y);
            }
            else if (zoomOutButton)
            {
                inputZoomValue -= inputZoomSpeed * Time.deltaTime;
                inputZoomValue = Mathf.Clamp(inputZoomValue, inputZoomLimits.x, inputZoomLimits.y);
            }
        }

        private void ZoomUpdateMouse()
        {
            inputZoomValue += Input.GetAxis(inputKeys.scrollWheelInputID) * inputZoomSpeed;
            inputZoomValue = Mathf.Clamp(inputZoomValue, inputZoomLimits.x, inputZoomLimits.y);
        }
    }
}
