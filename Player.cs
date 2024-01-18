using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IKitchenObjectParent {
    
    public static Player Instance {get; private set;}

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake() {
        if(Instance != null){
            Debug.LogError("There is more then one Player instances");
        }
        Instance = this;
    }
    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
        if(!GameManager.Instance.IsGamePlaying()) return;

        if(selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender,System.EventArgs e) {
        if(!GameManager.Instance.IsGamePlaying()) return;
        
        if(selectedCounter != null) {
            selectedCounter.Interact(this);
        }
    }
    private void Update() {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void HandleInteractions() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x,0f,inputVector.y);

        if(moveDir != Vector3.zero){
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if(Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance,countersLayerMask)) {
            if(raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                //Has Clear Counter
                if(baseCounter != selectedCounter) {
                    SetSelectedCounter(baseCounter);
                }
            }
            else {
                SetSelectedCounter(null);
            }
        }
        else {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x,0f,inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position,transform.position + Vector3.up * playerHeight ,playerRadius,moveDir,moveDistance);
        
        if(!canMove){
            //can not move in moveDir
            //attempt to move in x dir
            Vector3 moveDirX = new Vector3(moveDir.x,0,0).normalized;
            canMove = (moveDir.x <-.5f || moveDir.x > .5f) && !Physics.CapsuleCast(transform.position,transform.position + Vector3.up * playerHeight ,playerRadius,moveDirX,moveDistance);
            if(canMove) {
                //can move in x dir
                moveDir = moveDirX;
            }
            else {
                //can not move in x dir
                //attempt to move in z dir
                Vector3 moveDirZ = new Vector3(0,0,moveDir.z).normalized;
                canMove = (moveDir.z <-.5f || moveDir.z > .5f) &&!Physics.CapsuleCast(transform.position,transform.position + Vector3.up * playerHeight ,playerRadius,moveDirZ,moveDistance);
                if(canMove) {
                    //can move in z dir
                    moveDir = moveDirZ;
                }
            }
        }
        if(canMove){
            transform.position += moveDir * moveDistance;
        } 

        float rotateSpeed = 10f;
        isWalking = moveDir != Vector3.zero;
        transform.forward = Vector3.Slerp(transform.forward,moveDir,Time.deltaTime * rotateSpeed);
        
    }
    private void SetSelectedCounter(BaseCounter selectedCounter){
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform(){
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject){
        this.kitchenObject = kitchenObject;

        if(kitchenObject != null) {
            OnPickedSomething?.Invoke(this,EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        kitchenObject = null;
    }

    public bool HasKitchenObject(){
        return kitchenObject != null;
    }
}

