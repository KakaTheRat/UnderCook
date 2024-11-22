using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody rb;

    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Camera playerCamera;
    private Vector2 lookInput;
    private float pitch = 0f;

    [Header("Raycast Interactable Items Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;

    private InteractableObjects[] interactableObjects;
    private Animator animator;
    private GameObject underWiewItem;
    private GameObject holdingItem;
    private UIManager uiManager;
    private bool controller = false;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
        interactableObjects = FindObjectsOfType<InteractableObjects>();
        uiManager = FindObjectOfType<UIManager>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        controller = context.control.device is Gamepad;
        lookInput = context.ReadValue<Vector2>();
    }

    void FixedUpdate(){
        HandleMovement();
        HandleCamera();
        HandleHover();
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        move = transform.TransformDirection(move);
        Vector3 targetVelocity = move * moveSpeed;

        if(!rb.isKinematic){
            rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
        }

        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;
        animator.SetFloat("Speed", currentSpeed);
    }

    private void HandleCamera()
    {
        float multipy = 0.2f;
        if(controller){multipy = 2f;Debug.Log("Controller");}
        if(!rb.isKinematic){
            float yaw = lookInput.x * mouseSensitivity * multipy;
            transform.Rotate(Vector3.up * yaw);
        }

        pitch -= lookInput.y * mouseSensitivity * multipy;
        pitch = Mathf.Clamp(pitch, -80f, 80f);
        playerCamera.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }



    private void HandleHover()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance,interactableLayer))
        {
            Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.green);
            InteractableObjects item = hit.collider.GetComponent<InteractableObjects>();
            if (item != null && item.GetCanBeInteracted())
            {
                item.ActivateOutline(true);
                uiManager.SetInteractText(item.GetInteractrionText());
                uiManager.ToggleInteractText(true);
                if(underWiewItem != hit.collider.gameObject){
                    underWiewItem = hit.collider.gameObject;
                }
                DisableAllOutlines(item);
                return;
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);
        }
        DisableAllOutlines();
    }

    public GameObject GetHoldingItem(){
        return holdingItem;
    }

    private void DisableAllOutlines(InteractableObjects Ignored = null)
    {
       foreach (var outinteractableObject in interactableObjects)
       {    
            if(Ignored != outinteractableObject){
                outinteractableObject.ActivateOutline(false);
            }
       }
       if(Ignored == null){
            underWiewItem = null;
            uiManager.ToggleInteractText(false);
       }
    }

    public void HoldItem(GameObject itemToHold){
        holdingItem = itemToHold;
        holdingItem.transform.SetParent(GameObject.FindGameObjectWithTag("HoldingPlaceHolder").transform);
        string itemName = holdingItem.GetComponent<IngredientManager>().GetIngredientName();
        holdingItem.transform.localPosition =  new Vector3(-0.00384f, 0.00214f, -0.00331f);
        if(itemName == "Cucumber" || itemName == "Nori" || itemName == "Tentacle"){
            holdingItem.transform.localRotation =  Quaternion.Euler(12.029f,-75.593f, 157.392f);
        }else{
            holdingItem.transform.localRotation =  Quaternion.Euler(12.029f,-75.593f, 61.671f);
        }
        animator.SetBool("Holding", true);
        foreach(InteractableObjects interactableObject in interactableObjects){
            interactableObject.SetInteractText();
        }
    }

    public void OnInteract(InputAction.CallbackContext context){
        if(context.phase == InputActionPhase.Performed){
            if(underWiewItem == null){Debug.Log("Nothing to interact with"); return;}
            InteractableObjects item = underWiewItem.GetComponent<InteractableObjects>();
            if(item.GetCanBeInteracted()){
                item.Interact();
            } 
        }
    }

    public void DestroyHoldingItem(){
        Destroy(holdingItem);
        ReleaseItem();
    }

    public void ReleaseItem(){
        holdingItem = null;
        animator.SetBool("Holding", false);
    }

    public void Static(bool _static){
        if(_static){
            rb.isKinematic = true;
            return;
        }
        rb.isKinematic = false;
    }

    public void PauseTime(InputAction.CallbackContext context){
        if(context.phase == InputActionPhase.Performed){
            if(Time.timeScale == 0f){
                Time.timeScale = 1f;
                return;
            }
            Time.timeScale = 0f;
        }
    }

    public void ToggleCutAnim(bool _bool){
        if(_bool){
            animator.SetTrigger("CutStart");
            return;
        }
        animator.SetTrigger("CutEnd");
    }

    public void Emote(){
        string[] triggerNames = {"Hello", "Chockbar"};
        animator.SetTrigger(triggerNames[Random.Range(0,triggerNames.Length)]);
    }
}
