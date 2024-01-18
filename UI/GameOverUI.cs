using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
    [SerializeField] private TextMeshProUGUI failedDeliveryText;
    [SerializeField] private Button restartButton;

    private void Awake() {
        restartButton.onClick.AddListener(()=>{
            Loader.Load(Loader.Scene.GameScene);
        });
    }

    private void Start() {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        Hide();
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e) {
        if(GameManager.Instance.IsGameOver()){
            Show();
            recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulDeliveredRecipesCount().ToString();
            failedDeliveryText.text = DeliveryManager.Instance.GetFailedDeliveryCount().ToString();
        }
        else{
            Hide();
        }
    }

    private void Show(){
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
