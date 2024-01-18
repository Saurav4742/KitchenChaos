using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress {

    public enum State {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs {
        public State state;
    }

    [SerializeField] FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] BurningRecipeSO[] burningRecipeSOArray;

    private State state;
    private float fryingTimer;
    private FryingRecipeSO fryingRecipeSO;
    private float burningTimer;
    private BurningRecipeSO burningRecipeSO;


    private void Start() {
        state = State.Idle;
    }
    private void Update() {

        if(HasKitchenObject()){
            switch(state) {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerLimit
                    });
                    if(fryingTimer>=fryingRecipeSO.fryingTimerLimit) {
                        //Fried
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output,this);
                        state = State.Fried;
                        burningTimer = 0f;
                        burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{
                            state=state
                        });
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                        progressNormalized = burningTimer / burningRecipeSO.burningTimerLimit
                    });
                    if(burningTimer>=burningRecipeSO.burningTimerLimit) {
                        //burned
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output,this);
                        state = State.Burned;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{
                            state=state
                        });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                            progressNormalized = 0f
                        });
                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }

    public override void Interact(Player player) {
        if(!HasKitchenObject()) {
            //There is no Kitchen Object Here
            if(player.HasKitchenObject()){
                //player carrying something
                if(HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())){
                    //carried kichen object can be fried
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    fryingTimer=0;
                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    state = State.Frying;
                    fryingTimer =0f;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{
                        state=state
                    });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                       progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerLimit
                    });
                }
            }
        }
        else {
            //There is Kitchen Object Here
            if(player.HasKitchenObject()){
                //player carrying something
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    //player is holding a plate
                    if(plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())){
                        GetKitchenObject().DestroySelf();
                        
                        state = State.Idle;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{
                            state=state
                        });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                            progressNormalized = 0f
                        });
                    }
                }

            } else {
                //Player not carrying kitchen Object
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{
                    state=state
                });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                    progressNormalized = 0f
                });
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }

    public KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if(fryingRecipeSO != null){
            return fryingRecipeSO.output;
        }
        else return null;
    }

    public FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach(FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray){
            if(fryingRecipeSO.input == inputKitchenObjectSO){
                return fryingRecipeSO;
            }
        }
        return null;
    }
    public BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach(BurningRecipeSO burningRecipeSO in burningRecipeSOArray){
            if(burningRecipeSO.input == inputKitchenObjectSO){
                return burningRecipeSO;
            }
        }
        return null;
    }

    public bool IsFried() {
        return state == State.Fried;
    }
}
