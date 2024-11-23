using UnityEngine;
using UnityEngine.InputSystem;

public class InputTouchController : MonoBehaviour
{
    PlayerInput playerInput;

    InputAction touchPositionAction;
    InputAction touchPressAction;

    Vector2 touchPosition;

    Ray ray;
    RaycastHit hit;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        touchPositionAction = playerInput.actions["TouchPosition"];
        touchPressAction = playerInput.actions["TouchPress"];
    }

    private void Start()
    {
        touchPositionAction.performed += TouchPosition;
        touchPressAction.performed += TouchPressed;
    }

    private void Update()
    {
        //Debug.Log("touch pos : " +  touchPosition);
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        Debug.Log("touch press : " + context.ReadValueAsButton());
    }

    private void TouchPosition(InputAction.CallbackContext context)
    {
        touchPosition = context.ReadValue<Vector2>();

        ray = Camera.main.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                //Debug.Log("hit : " + hit.collider.gameObject.name);

                if (hit.collider.gameObject.TryGetComponent<Box>(out Box box))
                {
                    //Debug.Log("obatined box : " + box);

                    if (box.isSloted) return;

                    if (!box.PlaceBox())
                    {
                        Debug.Log("Box cannot be moved!!");
                    }
                }
            }
        }

        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

        //Debug.Log("touch pos : " + touchPosition);
    }

    private void OnGUI()
    {
        GUI.color = Color.red;
        GUI.Label(new Rect(0, 0, 200, 100), this.GetType().Name + " : " + touchPosition);
    }
}
