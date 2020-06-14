using UnityEngine;

namespace com.PROS.SalvationLand
{
    public class CameraController : MonoBehaviour
    {
        public static Transform FocusTo;
        public static CameraController Instance;

        [Header("Components")] public Transform cameraTranslatePivot;
        public Transform cameraRotatePivot;
        public Transform cameraGroup;
        public Transform pointer;
        [Header("Parameters")] public Rect area;
        public float distance;
        public bool isXAxisReversed;
        public bool isYAxisReversed;
        public float mousePositionX;
        public float mousePositionY;
        public float moveVelocityX;
        public float moveVelocityY;
        public float rotationX;
        public float rotationY;
        public float sensitivityMouseScrollWheel;
        public float sensitivityX;
        public float sensitivityY;

        private bool m_IsMoveEnabled = true;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetMouseButton(2))
            {
                rotationX = Mathf.Clamp(
                    isXAxisReversed
                        ? rotationX - Input.GetAxis("Mouse Y") * sensitivityY
                        : rotationX + Input.GetAxis("Mouse Y") * sensitivityY, 10f, 90f);
                rotationY = isYAxisReversed
                    ? rotationY - Input.GetAxis("Mouse X") * sensitivityX
                    : rotationY + Input.GetAxis("Mouse X") * sensitivityX;
            }

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * sensitivityMouseScrollWheel, 5f,
                30f);
            mousePositionX = Input.mousePosition.x;
            mousePositionY = Input.mousePosition.y;
            if (mousePositionX < 1f)
            {
                moveVelocityX = Mathf.Clamp(moveVelocityX - Time.deltaTime * 50f, -8f, 0f);
            }
            else if (mousePositionX > Screen.width - 2f)
            {
                moveVelocityX = Mathf.Clamp(moveVelocityX + Time.deltaTime * 50f, 0f, 8f);
            }
            else
            {
                moveVelocityX = Mathf.Sign(moveVelocityX) *
                                Mathf.Clamp(Mathf.Abs(moveVelocityX) - Time.deltaTime * 50f, 0f, 8f);
            }

            if (mousePositionY < 1f)
            {
                moveVelocityY = Mathf.Clamp(moveVelocityY - Time.deltaTime * 50f, -8f, 0f);
            }
            else if (mousePositionY > Screen.height - 2f)
            {
                moveVelocityY = Mathf.Clamp(moveVelocityY + Time.deltaTime * 50f, 0f, 8f);
            }
            else
            {
                moveVelocityY = Mathf.Sign(moveVelocityY) *
                                Mathf.Clamp(Mathf.Abs(moveVelocityY) - Time.deltaTime * 50f, 0f, 8f);
            }
        }

        private void LateUpdate()
        {
            cameraTranslatePivot.localRotation = Quaternion.Lerp(cameraTranslatePivot.localRotation,
                Quaternion.Euler(0f, rotationY, 0f), Time.deltaTime * 6f);
            cameraRotatePivot.localRotation = Quaternion.Lerp(cameraRotatePivot.localRotation,
                Quaternion.Euler(rotationX, 0f, 0f), Time.deltaTime * 6f);
            cameraGroup.localPosition = new Vector3(0f, 0f,
                Mathf.Lerp(cameraGroup.localPosition.z, -distance, Time.deltaTime * 6f));
            if ((Input.GetKey(KeyCode.Space) || Input.GetKeyDown(KeyCode.Space)))
            {
                Focus();
            }
            else if (m_IsMoveEnabled)
            {
                Vector3 nextLocalPosition = cameraTranslatePivot.localPosition +
                                            (cameraTranslatePivot.right * moveVelocityX +
                                             cameraTranslatePivot.forward * moveVelocityY) * distance * Time.deltaTime *
                                            0.1f;
                nextLocalPosition.x = Mathf.Clamp(nextLocalPosition.x, area.x, area.x + area.width);
                nextLocalPosition.z = Mathf.Clamp(nextLocalPosition.z, area.y, area.y + area.height);
                cameraTranslatePivot.localPosition = nextLocalPosition;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            m_IsMoveEnabled = hasFocus;
        }

        public void Focus()
        {
            if (FocusTo != null)
            {
                cameraTranslatePivot.position = FocusTo.position;
                pointer.SetParent(FocusTo);
                pointer.localPosition = Vector3.zero;
            }
        }
    }
}