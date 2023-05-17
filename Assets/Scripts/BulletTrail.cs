using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]

public class BulletTrail : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _timer = 0f;
    [SerializeField] private float _lifetime = 0.25f;
    // Start is called before the first frame update
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Color newColor = Color.Lerp(_lineRenderer.startColor, new Color(255, 255, 255, 0), _timer / _lifetime);
        _lineRenderer.startColor = newColor;
        _lineRenderer.endColor = newColor;
        _timer += Time.deltaTime;

        if (_timer > _lifetime) 
        {
            Destroy(this.gameObject);
        }
    }
}
