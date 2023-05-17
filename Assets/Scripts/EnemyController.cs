using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    //Navmesh
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private float _timer = 0f;
    [SerializeField] private float _timePerAIUpdate = 0.1f;
    [SerializeField] private float _timePerUnawareUpdate = 0.1f;
    [SerializeField] private float _timePerFollowUpdate = 1f;
    [SerializeField] private float _wanderRadius = 5f;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _viewDistance = 15f;
    [SerializeField] private Vector3 _target;
    [SerializeField] private float _health = 100f;
    [SerializeField] private Transform _hostler;
    [SerializeField] private Gun _gun;
    public ParticleSystem bloodParticles;
    public Rigidbody rigidBody;    

    private enum State 
    { 
        Unaware,
        Following
    }

    [SerializeField] private State _state = State.Following;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        _playerTransform = GameObject.FindWithTag("Player").transform;
        rigidBody.isKinematic = true;
        _gun.isHeld = true;
    }

   /* private void Update()
    {
        //gun.transform.position = _hostler.transform.position;
    }*/
    private void FixedUpdate()
    {
        if (_playerTransform == null)
        {
            return;
        }
        _timer += Time.deltaTime;

        if (_state == State.Unaware)
        {
            _timePerAIUpdate = _timePerUnawareUpdate;
        }
        else
        {
            _timePerAIUpdate = _timePerFollowUpdate;
        }

        if (_timer > _timePerAIUpdate)
        {
            UpdateAI();
            _timer = 0f;
        }

        if (_state == State.Following)
        {
            _gun.Shoot(_gun.transform.position, "Enemy");
            this.transform.LookAt(new Vector3(_playerTransform.position.x, this.transform.position.y, _playerTransform.position.z));
        }
        if (_health <= 0)
        {
            _gun.transform.parent = null;
            _gun.isHeld = false;
            _gun.Owner = null;
            rigidBody.isKinematic = false;
            rigidBody.AddForce(this.transform.forward * -1, ForceMode.Impulse);

            Destroy(this);
            Destroy(_navMeshAgent);
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
                    out RaycastHit hit, _viewDistance) && hit.transform.CompareTag("Player")))
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
                    out RaycastHit hit, _viewDistance) && hit.transform.CompareTag("Player")))
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

    public void Damage(float damageAmount)
    {
        _health -= damageAmount;
    }
}
