using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Camera playerCamera;
    public Transform holdPoint;
    
    [Header("Movement")]
    public float moveSpeed = 4f;
    
    [Header("Mouse Look")]
    public float mouseSensitivity = 150f;
    public float maxLookAngle = 85f;
    
    [Header("Raycast")]
    public float interactRange = 3f;
    public LayerMask interactableMasks;

    public string holdingMaskId;
    public bool isHoldingMask;
    
    private GameObject _holdingObject;
    private MaskShelf _lookingMaskShelf;
    private Customer _lookingCustomer;
    
    private float _xRotation;

    private void Update()
    {
        if (!GameManager.instance.isGameStarted) return;
        
        // Pause
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!GameManager.instance.isPaused) GameManager.instance.PauseGame();
            else GameManager.instance.ResumeGame();
        }
        if (GameManager.instance.isPaused) return;
        HandleMovement();
        HandleInteraction();
    }

    private void LateUpdate()
    {
        if (!GameManager.instance.isGameStarted) return;
        HandleMouseLook();
    }

    private void HandleMovement()
    {
        // WASD input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        // Mouse look
        if (GameManager.instance.isPaused) return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -maxLookAngle, maxLookAngle);

        playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    
    private void HandleInteraction()
    {
        LookForInteractable();
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Nếu đang dialogue -> E dùng để Skip/Continue
            TypewriterManager typewriter = GameManager.instance.typewriter;
            if (typewriter.IsInDialogue())
            {
                if (typewriter.IsTyping()) typewriter.SkipTypingText();
                else if (typewriter.CanContinue()) typewriter.ContinueMessageText();
                return;
            }
            
            // Nếu không dialogue -> xử lý tương tác bình thường
            if (_lookingMaskShelf != null)
            {
                CheckForPickupMask(_lookingMaskShelf);
            }

            if (_lookingCustomer != null)
            {
                if (!_lookingCustomer.isTalked)
                    TalkToCustomer(_lookingCustomer);
                else if (isHoldingMask && !_lookingCustomer.isServed)
                    GiveMaskToCustomer(_lookingCustomer);
            }
        }
    }
    
    private void LookForInteractable()
    {
        _lookingMaskShelf = null;
        _lookingCustomer = null;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableMasks))
        {
            MaskShelf maskShelf = hit.collider.GetComponent<MaskShelf>();
            if (maskShelf != null)
            {
                _lookingMaskShelf = maskShelf;
                GameManager.instance.ui.ShowTooltip(maskShelf.maskData.maskTooltip);
                return;
            }

            Customer customer = hit.collider.GetComponent<Customer>();
            if (customer != null)
            {
                _lookingCustomer = customer;
                return;
            }
        }
        GameManager.instance.ui.HideTooltip();
    }
    
    private void CheckForPickupMask(MaskShelf maskShelf)
    {
        // Nếu đang cầm mask rồi
        if (isHoldingMask && _holdingObject != null)
        {
            // Nếu đang cầm đúng loại mask của kệ này thì coi như trả lại
            if (maskShelf.CheckForMaskName(holdingMaskId))
            {
                DestroyHoldingMask();   
            }
            return;
        }
        
        GameObject maskObject = Instantiate(maskShelf.maskData.maskPrefab, holdPoint.position, Quaternion.identity);
        maskObject.transform.SetParent(holdPoint);
        maskObject.transform.localPosition = Vector3.zero;
        maskObject.transform.localRotation = Quaternion.identity;
        
        Collider col = maskObject.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        holdingMaskId = maskShelf.maskData.maskId;
        isHoldingMask = true;
        _holdingObject = maskObject;
    }

    private void DestroyHoldingMask()
    {
        Destroy(_holdingObject);
        _holdingObject = null;
            
        isHoldingMask = false;
        holdingMaskId = "";
    }

    private void TalkToCustomer(Customer customer)
    {
        GameManager.instance.customerManager.TalkToCustomer(customer);
        customer.isTalked = true;
    }

    private void GiveMaskToCustomer(Customer customer)
    {
        bool success = customer.CheckForRequiredMask(holdingMaskId);
        DestroyHoldingMask();
        GameManager.instance.customerManager.GiveMaskToCustomer(customer, success);
    }
}