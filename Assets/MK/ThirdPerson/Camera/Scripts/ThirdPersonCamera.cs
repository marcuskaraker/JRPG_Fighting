using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace MK.ThirdPerson
{
    /// <summary>
    /// A third-person camera controller.
    /// </summary>
    public class ThirdPersonCamera : MonoBehaviour
    {
        public Transform target;
        public Transform zoomer;
        public Transform pitcher;

        [Header("Settings")]
        public float zoomValue = 0.0f;
        public float zoomSpeed = 10f;
        public float rotationSpeed = 10f;

        [System.NonSerialized] public Vector3 offset;
        [System.NonSerialized] public Camera mainCamera;

        private Vector3 zoomerBasePos;

        private void Awake()
        {
            mainCamera = GetComponentInChildren<Camera>();
            zoomerBasePos = zoomer.transform.localPosition;
        }

        private void Start()
        {
            offset = transform.position - target.position;
        }

        private void LateUpdate()
        {
            transform.position = target.position + offset;

            Vector3 zoomPos = zoomerBasePos + zoomer.transform.forward * zoomValue;
            zoomer.transform.localPosition = Vector3.Lerp(zoomer.transform.localPosition, zoomPos, Time.deltaTime * zoomSpeed);
        }

        /// <summary>
        /// Rotates the camera horizontally with the defined rotationSpeed based on <param name="value">value</param>.
        /// <para>To rotate vertically use <see cref="RotateVertical"></see>.</para>
        /// </summary>
        /// <param name="value">An input value between -1 and 1.</param>
        public void RotateHorizontal(float value)
        {
            transform.Rotate(Vector3.up, value * Time.deltaTime * rotationSpeed);
        }

        /// <summary>
        /// Rotates the camera vertically with the defined rotationSpeed based on <param name="value">value</param>.
        /// <para>To rotate horizontally use <see cref="RotateHorizontal"></see>.</para>
        /// </summary>
        /// <param name="value">An input value between -1 and 1.</param>
        public void RotateVertical(float value)
        {
            pitcher.Rotate(pitcher.right, value * Time.deltaTime * rotationSpeed);
        }
    }
}
