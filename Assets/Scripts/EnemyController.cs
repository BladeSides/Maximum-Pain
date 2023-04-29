using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{

    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private float _timer = 0f;
    [SerializeField] private float _timePerAIUpdate = 0.5f;
    [SerializeField] private float _minimumDistance = 7f;
    [SerializeField] private float _wanderRadius = 5f;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _viewDistance = 15f;
    [SerializeField] private Vector3 _target;
    private enum State 
    { 
        Unaware,
        Following
    }

    [SerializeField] private State _state = State.Following;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        _playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        _timer += Time.deltaTime;

        if (_timer > _timePerAIUpdate)
        {
            UpdateAI();
            _timer = 0f;
        }
    }

    private void UpdateAI()
    {
        if (_state == State.Unaware)
        {
            if ((_playerTransform.position - this.transform.position).sqrMagnitude < _viewDistance*_viewDistance)
            {
                _navMeshAgent.isStopped = false;
                if ((Physics.Raycast(this.transform.position, (_playerTransform.position - this.transform.position).normalized,
                    out RaycastHit hit, _viewDistance) && hit.transform.tag == "Player"))
                {
                    _state = State.Following;
                    Vector2 randomWanderCircle = Random.insideUnitCircle * _wanderRadius;
                    _target = (_playerTransform.position - this.transform.position).normalized
                        * _wanderRadius + _playerTransform.position + new Vector3(randomWanderCircle.x, 0, randomWanderCircle.y); ;
                    _navMeshAgent.SetDestination(_target);
                    print("Hit");
                }
            }
            else
            {
                _navMeshAgent.isStopped = true;
            }
        }
        if (_state == State.Following)
        {
            if ((_playerTransform.position - this.transform.position).sqrMagnitude < _viewDistance * _viewDistance)
            {
                _navMeshAgent.isStopped = false;
                if ((Physics.Raycast(this.transform.position, (_playerTransform.position - this.transform.position).normalized,
                    out RaycastHit hit, _viewDistance) && hit.transform.tag == "Player"))
                {
                    _state = State.Following;
                    Vector2 randomWanderCircle = Random.insideUnitCircle * _wanderRadius;
                    _target = -(_playerTransform.position - this.transform.position).normalized
                        * _wanderRadius + _playerTransform.position + new Vector3(randomWanderCircle.x,0, randomWanderCircle.y);
                    _navMeshAgent.SetDestination(_target);
                    print("Hit");
                }
            }
            else
            {
                _navMeshAgent.isStopped = true;
                _state = State.Unaware;
            }
        }
    }
}
