using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemyPatrolPath
{
    [Header("Waypoints")]
    public Transform[] Waypoints;

    [Header("Movement")]
    public float MoveSpeed = 2f;
    public float StoppingDistance = 0.5f;
    public bool Loop = true;

    int _currentIndex = 0;

    public float CurrentSpeed => MoveSpeed;

    public void Tick(EnemyController enemy)
    {
        if (Waypoints == null || Waypoints.Length == 0)
            return;

        NavMeshAgent agent = enemy.Stats.NavAgent;

        if (!agent) return;

        agent.speed = MoveSpeed;

        Transform target = Waypoints[_currentIndex];
        agent.SetDestination(target.position);

        float distance = Vector3.Distance(enemy.transform.position, target.position);

        if (distance <= StoppingDistance)
        {
            AdvanceIndex();
        }
    }

    void AdvanceIndex()
    {
        if (Loop)
        {
            _currentIndex = (_currentIndex + 1) % Waypoints.Length;
        }
        else
        {
            _currentIndex = Mathf.Min(_currentIndex + 1, Waypoints.Length - 1);
        }
    }

    public void ResetPath()
    {
        _currentIndex = 0;
    }
}