using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour {

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance {get; private set;}

    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;
    private int successfulRecipeAmount;
    private int failedRecipeAmount;

    private void Awake() {
        Instance = this;


        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update() {
        spawnRecipeTimer-= Time.deltaTime;
        if(spawnRecipeTimer<=0f) {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if(GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax){
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0,recipeListSO.recipeSOList.Count)];
                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawned?.Invoke(this,EventArgs.Empty);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject){
        for(int i=0;i<waitingRecipeSOList.Count;i++){
            RecipeSO waitingRecipeSO =waitingRecipeSOList[i];

            if(waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count) {
                //Has same number of ingredient
                bool plateContentMatchesRecipe = true;
                foreach(KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
                    //cycling through ingredient in recipe
                    bool ingredientFound = false;
                    foreach(KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
                        //cycling through ingredient in plate
                        if(recipeKitchenObjectSO == plateKitchenObjectSO){
                            ingredientFound = true;
                            break;
                        }
                    }
                    if(!ingredientFound) {
                        //this recipe doen not matches to the plate
                        plateContentMatchesRecipe = false;
                        break;
                    }
                }
                if(plateContentMatchesRecipe){
                    //player delevered correct recipe

                    successfulRecipeAmount++;
                    waitingRecipeSOList.RemoveAt(i);

                    OnRecipeSuccess?.Invoke(this,EventArgs.Empty);
                    OnRecipeCompleted?.Invoke(this,EventArgs.Empty);
                    return;
                }
            }
        }
        //no matches found
        //player does not delivered correct recipe

        failedRecipeAmount++;
        OnRecipeFailed?.Invoke(this,EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList() {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulDeliveredRecipesCount() {
        return successfulRecipeAmount;
    }

    public int GetFailedDeliveryCount() {
        return failedRecipeAmount;
    }
}
