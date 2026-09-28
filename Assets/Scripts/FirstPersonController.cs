using UnityEngine;
using UnityEngine.InputSystem;

namespace ValorantPractice
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonController : MonoBehaviour
    {
        // Hipfire coefficient from the supplied VALORANT sensitivity reference.
        private const float DegreesPerMouseCount = 0.07f;

        public enum MovementEquipment
        {
            Gun,
            Knife
        }

        [Header("View")]
        [SerializeField] private Transform cameraPivot;
        [Tooltip("VALORANT Sensitivity: Aim scale. At 800 DPI, 1 = approximately 16.33 cm/360. Hipfire only.")]
        [SerializeField, Min(0f)] private float mouseSensitivity = 1f;
        [SerializeField, Range(1f, 89.9f)] private float pitchLimit = 89f;

        [Header("Movement (1 Unity unit = 1 metre)")]
        [Tooltip("Speed profile only: 1 = gun, 3 = knife. No weapon models yet.")]
        [SerializeField] private MovementEquipment equipment = MovementEquipment.Gun;
        [Tooltip("Unscoped running speed for Vandal, Phantom and Sheriff.")]
        [SerializeField, Min(0f)] private float gunSpeed = 5.4f;
        [SerializeField, Min(0f)] private float knifeSpeed = 6.75f;
        [Tooltip("Prototype gravity; not a measured VALORANT value.")]
        [SerializeField, Min(0f)] private float gravity = 20f;

        private CharacterController controller;
        private InputAction moveAction;
        private InputAction lookAction;
        private float yaw;
        private float pitch;
        private float verticalSpeed;
        private bool hasFocus = true;
        private bool hasCapturedCursor;

        public float MoveSpeed => equipment == MovementEquipment.Knife ? knifeSpeed : gunSpeed;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (cameraPivot == null || cameraPivot == transform || !cameraPivot.IsChildOf(transform))
            {
                Debug.LogError("Assign a child camera pivot to FirstPersonController.", this);
                enabled = false;
                return;
            }

            yaw = transform.eulerAngles.y;
            pitch = Mathf.Clamp(Mathf.DeltaAngle(0f, cameraPivot.localEulerAngles.x), -pitchLimit, pitchLimit);
            hasFocus = Application.isFocused;

            moveAction = new InputAction("Move", InputActionType.Value);
            moveAction.AddCompositeBinding("2DVector(mode=0)")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            lookAction = new InputAction("Look", InputActionType.Value, "<Mouse>/delta");
        }

        private void OnEnable()
        {
            moveAction?.Enable();
            lookAction?.Enable();
        }

        private void OnDisable()
        {
            moveAction?.Disable();
            lookAction?.Disable();
            verticalSpeed = 0f;
            ReleaseCursor();
        }

        private void OnDestroy()
        {
            moveAction?.Dispose();
            lookAction?.Dispose();
        }

        private void OnApplicationFocus(bool focused)
        {
            hasFocus = focused;
            if (!focused)
                ReleaseCursor();
        }

        private void Update()
        {
            if (!hasFocus || !controller.enabled || Time.timeScale == 0f)
                return;

            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            {
                ReleaseCursor();
                return;
            }

            if (!hasCapturedCursor || Cursor.lockState != CursorLockMode.Locked)
            {
                hasCapturedCursor = false;
                if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    hasCapturedCursor = true;
                }

                // Discard the click frame's delta to avoid a jump when recapturing.
                return;
            }

            if (keyboard != null)
            {
                if (keyboard.digit1Key.wasPressedThisFrame)
                    equipment = MovementEquipment.Gun;
                else if (keyboard.digit3Key.wasPressedThisFrame)
                    equipment = MovementEquipment.Knife;
            }

            // Mouse delta is already accumulated displacement, not velocity.
            // DPI is already reflected in the device input; do not multiply it again.
            Vector2 look = lookAction.ReadValue<Vector2>() * (mouseSensitivity * DegreesPerMouseCount);
            yaw = Mathf.Repeat(yaw + look.x, 360f);
            pitch = Mathf.Clamp(pitch - look.y, -pitchLimit, pitchLimit);
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

            Vector2 input = Vector2.ClampMagnitude(moveAction.ReadValue<Vector2>(), 1f);
            Vector3 velocity = (transform.right * input.x + transform.forward * input.y) * MoveSpeed;
            if (controller.isGrounded && verticalSpeed < 0f)
                verticalSpeed = -2f;

            verticalSpeed -= gravity * Time.deltaTime;
            velocity.y = verticalSpeed;
            CollisionFlags collisions = controller.Move(velocity * Time.deltaTime);
            if ((collisions & CollisionFlags.Below) != 0 && verticalSpeed < 0f)
                verticalSpeed = -2f;
        }

        private void ReleaseCursor()
        {
            if (!hasCapturedCursor)
                return;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            hasCapturedCursor = false;
        }
    }
}
