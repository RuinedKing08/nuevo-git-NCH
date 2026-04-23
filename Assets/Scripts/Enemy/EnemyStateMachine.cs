public interface IEnemyState
{
    void EnterState();
    void UpdateState();
    void FixedUpdateState();
    void ExitState();
}

public class EnemyStateMachine
{
    public IEnemyState CurrentState { get; private set; }

    public void Initialize(IEnemyState startingState)
    {
        if (startingState == null)
        {
            UnityEngine.Debug.LogWarning("EnemyStateMachine.Initialize: startingState es null. Inicialización ignorada.");
            return;
        }

        CurrentState = startingState;
        CurrentState.EnterState();
    }

    public void ChangeState(IEnemyState newState)
    {
        // Protección contra CurrentState nulo (evita NullReferenceException)
        if (CurrentState != null)
            CurrentState.ExitState();

        CurrentState = newState;

        if (CurrentState != null)
            CurrentState.EnterState();
    }
}
