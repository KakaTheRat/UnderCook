using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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
    private GameObject underWiewItem = null;
    private GameObject holdingItem = null;

    public string deviceType;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Empèche la rotation du Rigidbody
        animator = GetComponent<Animator>();
        interactableObjects = FindObjectsOfType<InteractableObjects>();
    }

    // Fonction pour récupérer l'input de mouvement
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Fonction pour récupérer l'input de la souris/joystick pour la caméra
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        
        HandleCamera();
        HandleHover();
    }

    private void FixedUpdate()
    {
        HandleMovement(); // La gestion du mouvement physique se fait dans FixedUpdate
    }

    // Gestion du mouvement du joueur
    private void HandleMovement()
    {
        // Mouvement basé sur les entrées de déplacement
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y).normalized; // Normalize pour éviter des vitesses diagonales plus rapides
        move = transform.TransformDirection(move); // Convertit l'input en coordonnées mondiales
        Vector3 targetVelocity = move * moveSpeed;

        // Déplace le personnage en utilisant le Rigidbody (ajustement de la vitesse)
        if(!rb.isKinematic){
            rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z); // Garde la vitesse Y (gravité) intacte
        }

        // Mise à jour de l'animation en fonction de la vitesse du joueur
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;
        animator.SetFloat("Speed", currentSpeed);   // Modifie le paramètre 'Speed' dans l'Animator
    }

    // Gestion de la caméra pour une vue FPS
    private void HandleCamera()
    {
        if(!rb.isKinematic){
            // Rotation horizontale (yaw) du joueur
            float yaw = lookInput.x * mouseSensitivity;
            transform.Rotate(Vector3.up * yaw);
        }

        // Rotation verticale (pitch) de la caméra
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -80f, 80f); // Limite la vue verticale
        playerCamera.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }

    private void HandleHover()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance,interactableLayer))
        {
            Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.green);
            // Si on touche un objet avec OutlineController, on active l'outline
            InteractableObjects item = hit.collider.GetComponent<InteractableObjects>();
            if (item != null && item.GetCanBeInteracted())
            {
                item.ActivateOutline(true);
                if(underWiewItem != hit.collider.gameObject){
                    underWiewItem = hit.collider.gameObject;
                }
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

    private void DisableAllOutlines()
    {
       foreach (var outline in interactableObjects)
       {
           outline.ActivateOutline(false);
       }
       underWiewItem = null;
    }

    public void HoldItem(GameObject itemToHold){
        holdingItem = itemToHold;
        holdingItem.transform.SetParent(GameObject.FindGameObjectWithTag("HoldingPlaceHolder").transform);
        holdingItem.transform.localPosition =  new Vector3(-0.0003345405f, 0.002910723f, -0.004211193f);
        holdingItem.transform.localRotation =  Quaternion.Euler(12.029f,-75.593f, 61.671f);
        holdingItem.transform.localScale = new Vector3(1f, 1f,1f);
        animator.SetBool("Holding", true);
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

    public void GetDeviceType(InputAction.CallbackContext context){
        if(context.control.device.name == "Mouse" && deviceType != "Keyboard" && context.performed || context.control.device.name != "Mouse" && deviceType != context.control.device.name && context.performed){
            if(context.control.device.name == "Mouse"){
                deviceType = "Keyboard";
            }else{
                deviceType = context.control.device.name;
            }
            Debug.Log(deviceType);
        }
    }
}
