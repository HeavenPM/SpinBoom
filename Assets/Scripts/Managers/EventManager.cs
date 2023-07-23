using UnityEngine.Events;

public static class EventManager
{
    public static event UnityAction<int> ReelStartedRotating;
    public static event UnityAction LastReelStopped;
    public static event UnityAction SpinButtonPressed;
    public static event UnityAction IncreaseBetButtonPressed;
    public static event UnityAction ReduceBetButtonPressed;
    public static event UnityAction MakeIncreaseBetButtonUnavailable;
    public static event UnityAction MakeReduceBetButtonUnavailable;
    public static event UnityAction MakeSpinButtonUnavailable;
    public static event UnityAction MakeSpinButtonAvailable;
    public static event UnityAction EndOfSpin;
    public static event UnityAction SetMaxBet;
    public static event UnityAction ToggleSoundState;
    public static event UnityAction ToggleMusicState;

    public static void OnReelStartedRotating(int id) => ReelStartedRotating?.Invoke(id);
    public static void OnLastReelStopped() => LastReelStopped?.Invoke();
    public static void OnSpinButtonPressed() => SpinButtonPressed?.Invoke();
    public static void OnIncreaseBetButtonPressed() => IncreaseBetButtonPressed?.Invoke();
    public static void OnReduceBetButtonPressed() => ReduceBetButtonPressed?.Invoke();
    public static void OnMakeIncreaseBetButtonUnavailable() => MakeIncreaseBetButtonUnavailable?.Invoke();
    public static void OnMakeReduceBetButtonUnavailable() => MakeReduceBetButtonUnavailable?.Invoke();
    public static void OnMakeSpinButtonUnavailable() => MakeSpinButtonUnavailable?.Invoke();
    public static void OnMakeSpinButtonAvailable() => MakeSpinButtonAvailable?.Invoke();
    public static void OnEndOfSpin() => EndOfSpin?.Invoke();
    public static void OnSetMaxBet() => SetMaxBet?.Invoke();
    public static void OnToggleSoundState() => ToggleSoundState?.Invoke();
    public static void OnToggleMusicState() => ToggleMusicState?.Invoke();
}
