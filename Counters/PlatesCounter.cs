using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlatesCounter : BaseCounter {
    
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;
    [SerializeField] private float spawnPlateTimerLimit;
    [SerializeField] private int platesSpawnedAmountLimit;
    private float spawnPlateTimer;
    private int platesSpawnedAmount;



    private void Update() {
        spawnPlateTimer += Time.deltaTime;
        if(spawnPlateTimer > spawnPlateTimerLimit) {
            spawnPlateTimer = 0f;

            if(GameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountLimit) {
                platesSpawnedAmount++;

                OnPlateSpawned?.Invoke(this,EventArgs.Empty);
            }

        }
    }

    public override void Interact(Player player) {
        if(!player.HasKitchenObject()) {
            if(platesSpawnedAmount>0){
                platesSpawnedAmount--;
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO,player);
                OnPlateRemoved?.Invoke(this,EventArgs.Empty);
            }
        }
    }

}
