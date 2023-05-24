using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private List<Transform> _movementPath;
    [SerializeField] private float _aggroRate = 0.1f;
    [SerializeField] private float _minAggroRangeMulti = 1f;
    [SerializeField] private float _maxAggroRangeMulti = 10f;
    private GameObject _player;
    private bool _followPlayer;
    private NavMeshAgent _navMeshAgent;
    private SphereCollider _trigger;
    private int _pathIndex;

    private void Start()
    {
        _trigger = GetComponent<SphereCollider>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player");
    }

    private void FixedUpdate()
    {
        UpdateTriggerRadius();

        if (_followPlayer)
        {
            _navMeshAgent.destination = _player.transform.position;
        }
        else if (_movementPath.Count > 0)
        {
            if (Mathf.Abs((transform.position - _movementPath[_pathIndex].position).normalized.y) < 0.9f)
                _navMeshAgent.destination = _movementPath[_pathIndex].position;
            else
                _pathIndex = (_pathIndex + 1) % _movementPath.Count;
        }
    }

    private void UpdateTriggerRadius()
    {
        if (isPlayerVisible())
        {
            _trigger.radius += _aggroRate;
        }
        else
        {
            _trigger.radius -= _aggroRate;
        }

        _trigger.radius = Mathf.Clamp(_trigger.radius, _minAggroRangeMulti, _maxAggroRangeMulti);
    }

    private bool isPlayerVisible()
    {
        Vector3 rayDirection = (_player.transform.position - transform.position);
        rayDirection.y = 0f;

        Ray ray = new Ray(transform.position, rayDirection);
        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData) && hitData.transform.gameObject == _player)
        {
            return true;
        }

        return false;
    }

    private void Awake()
    {
        foreach (Transform point in _movementPath)
        {
            point.SetParent(null, true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _followPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _followPlayer = false;
        }
    }
}
