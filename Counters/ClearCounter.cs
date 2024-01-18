using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter {

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player) {
        if(!HasKitchenObject()) {
            //There is no Kitchen Object Here
            if(player.HasKitchenObject()){
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }
        else {
            //There is Kitchen Object Here
            if(player.HasKitchenObject()) {
                //player carrying something
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    //player is holding a plate
                    if(plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())){
                        GetKitchenObject().DestroySelf();
                    }
                }
                else {
                    //player is holding other than plate
                    if(GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                        //counter has plate
                        if(plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())){
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else{
                //Player not carrying kitchen Object
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}
