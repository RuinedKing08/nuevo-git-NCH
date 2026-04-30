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
            return;
        }

        CurrentState = startingState;
        CurrentState.EnterState();
    }

    public void ChangeState(IEnemyState newState)
    {        
        if (CurrentState != null)
            CurrentState.ExitState();

        CurrentState = newState;

        if (CurrentState != null)
            CurrentState.EnterState();
    }
}
