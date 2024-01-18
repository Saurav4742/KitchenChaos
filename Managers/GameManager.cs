using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance {get; private set;}

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    private enum State {
        WaitingToStart,
        CountddownToStart,
        GamePlaying,
        GameOver,
    }

    private State state;
    // private float WaitingToStartTimer = 1f;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 240f;
    private bool isGamePaused = false;

    private void Awake() {
        Instance = this;
        state = State.WaitingToStart;
    }

    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        if(state == State.WaitingToStart) {
            state = State.CountddownToStart;
            OnStateChanged?.Invoke(this,EventArgs.Empty);
        }
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        TogglePauseGame();
    }


    private void Update() {
        switch (state) {
            case State.WaitingToStart:
                // WaitingToStartTimer -= Time.deltaTime;
                // if(WaitingToStartTimer < 0f) {
                //     state = State.CountddownToStart;
                //     OnStateChanged?.Invoke(this,EventArgs.Empty);
                // }
                break;
            case State.CountddownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if(countdownToStartTimer < 0f) {
                    state = State.GamePlaying;
                    gamePlayingTimer = gamePlayingTimerMax;
                    OnStateChanged?.Invoke(this,EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if(gamePlayingTimer < 0f) {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this,EventArgs.Empty);
                }
                break;
            case State.GameOver: 
                break;
        }
    }

    public bool IsCountdownToStartActive() {
        return state == State.CountddownToStart;
    }

    public bool IsGamePlaying() {
        return state == State.GamePlaying;
    }

    public bool IsGameOver(){
        return state == State.GameOver;
    }

    public float GetCountdownToStartTimer() {
        return countdownToStartTimer;
    }

    public float GetGamePlayingTimeNormalized() {
        return 1 - (gamePlayingTimer/gamePlayingTimerMax);
    }

    public void TogglePauseGame() {
        isGamePaused = !isGamePaused;
        if(isGamePaused) {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this,EventArgs.Empty);
        } else {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this,EventArgs.Empty);
        }
    }
}
