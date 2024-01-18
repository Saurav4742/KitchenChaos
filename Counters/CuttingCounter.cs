using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress {

    public static EventHandler OnAnyCut;

    new public static void ResetStaticData() {
        OnAnyCut = null;
    }

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    [SerializeField] CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;

    public override void Interact(Player player) {
        if(!HasKitchenObject()) {
            //There is no Kitchen Object Here
            if(player.HasKitchenObject()){
                //player carrying something
                if(HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())){
                    //carried kichen object can be cut
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress=0;

                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                        progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressLimit
                    });
                }
            }
        }
        else {
            //There is Kitchen Object Here
            if(player.HasKitchenObject()){
                //player carrying kitchen object
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    //player is holding a plate
                    if(plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())){
                        GetKitchenObject().DestroySelf();
                    }
                }
            } else {
                //Player not carrying kitchen Object
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player) {
        KitchenObjectSO inputKitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();
        if(HasKitchenObject() && HasRecipeWithInput(inputKitchenObjectSO)){
            cuttingProgress++;

            OnCut?.Invoke(this,EventArgs.Empty);
            OnAnyCut?.Invoke(this,EventArgs.Empty);
            
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressLimit
            });

            if(cuttingProgress >= cuttingRecipeSO.cuttingProgressLimit){
                //there is kitchen object here with recipe which has been cut
                KitchenObject kitchenObject = GetKitchenObject();
                //get output from recipe
                KitchenObjectSO outputKitchenObjectSO = GetCuttingRecipeSOWithInput(kitchenObject.GetKitchenObjectSO()).output;
                //destroy input
                kitchenObject.DestroySelf();
                //spawn output
                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO,this);
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }

    public KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if(cuttingRecipeSO != null){
            return cuttingRecipeSO.output;
        }
        else return null;
    }

    public CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach(CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray){
            if(cuttingRecipeSO.input == inputKitchenObjectSO){
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}
