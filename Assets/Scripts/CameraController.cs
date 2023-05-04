using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private Camera _mainCamera;
    private float _offsetDistance;
    [SerializeField] private Transform _target;
    [SerializeField] private float _mouseSenstivity = 100f;
    [SerializeField] private float _slerpFactor = 0.01f;
    [SerializeField] private Quaternion _targetRotation;
    [SerializeField] private Transform _cameraHolder;

    [SerializeField] private float _angleX;
    [SerializeField] private float _angleY;
    [SerializeField] private float _deltaAngleX = 0;
    [SerializeField] private float _deltaAngleY = 0;

    [SerializeField] private float _maximumClipping = 10f;
    [SerializeField] private float _clipOffset = 1f;
    [SerializeField] private Vector3 _cameraCentreViewPort = new Vector3(0.5f, 0.5f, 0.05f);

    [SerializeField] private Vector2 _yLimits = new Vector2(-90, 90);

    [SerializeField] private float _clipLerp = 10f;
    [SerializeField] private Vector3 _mainCameraStartingLocalPosition;

    [SerializeField] private LayerMask _cameraCollision;
    [SerializeField] private Transform _cameraClipResetPosition;

    [SerializeField] private PlayerController _playerController;

    private Vector2 _mouseInput;
    private void Awake()
    {
        _mainCamera = GetComponent<Camera>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _offsetDistance = (_target.position - this.transform.position).magnitude;
        _targetRotation = this.transform.rotation;
        _mainCameraStartingLocalPosition = _mainCamera.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        _mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        _deltaAngleX = _mouseInput.x * Time.unscaledDeltaTime * _mouseSenstivity;
        _angleX += _deltaAngleX;
        
        float _testDeltaY = _mouseInput.y * Time.unscaledDeltaTime * _mouseSenstivity;
        if (_testDeltaY + _angleY < _yLimits.y && _testDeltaY + _angleY > _yLimits.x)
        {
            _deltaAngleY = _testDeltaY;
        }
        else if (_testDeltaY < 0)
        {
            _testDeltaY = _angleY - _yLimits.x;
        }
        else
        {
            _testDeltaY = _yLimits.y - _angleY;
        }
        _deltaAngleY = _testDeltaY;
        _angleY += _deltaAngleY;

        ClipCamera();
        //Mathf.Clamp(_angleY, _yLimits.x, _yLimits.y);
        //_targetRotation = Quaternion.Euler(_angleY, _angleX, 0);
}


    private void ClipCamera()
    {
        Vector3 cameraPosition = _mainCamera.ViewportToWorldPoint(_cameraCentreViewPort);
        if (Physics.Linecast(cameraPosition, _target.root.position, out RaycastHit hit,_cameraCollision))
        {
            float MoveDistance = 0f;
            if (Physics.Linecast(_target.root.position, cameraPosition, out RaycastHit hitBack, _cameraCollision))
            {
                MoveDistance = (_cameraHolder.transform.position - hitBack.transform.position).magnitude + _clipOffset;
                /*_cameraHolder.transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition
                    + (_target.root.position - transform.position).normalized * MoveDistance, _clipLerp * Time.deltaTime);*/
                _mainCamera.transform.localPosition += new Vector3(0, 0, _clipLerp * Time.unscaledDeltaTime * MoveDistance);
                //print("Clipped");
            }
            else 
            {
                if (Physics.Linecast(_target.root.position, _cameraClipResetPosition.position, _cameraCollision))
                {
                    return;
                }
                ClipBack();
            }

        }
        else
        {
            if (Physics.Linecast(_target.root.position, _cameraClipResetPosition.position, _cameraCollision))
            {
                return;
            }
            ClipBack();
        }
    }

    private void ClipBack()
    {
        _mainCamera.transform.localPosition = Vector3.Lerp(this.transform.localPosition, _mainCameraStartingLocalPosition, _clipLerp * Time.unscaledDeltaTime);
    }

    private void LateUpdate() //So that camera moves after player
    {
        if (_playerController._isShootDodging)
        {
            _cameraHolder.transform.Rotate(0, _deltaAngleX, 0);
        }
        else
        {
            transform.root.Rotate(0, _cameraHolder.localRotation.eulerAngles.y, 0); //rotate player towards where the camera was looking when shoot dodging
            _cameraHolder.localRotation = Quaternion.Euler(_cameraHolder.localRotation.eulerAngles.x, 0,0); //rotate camera back
            transform.root.Rotate(0, _deltaAngleX, 0); //Rotates player
        }
        _cameraHolder.transform.Rotate(-_deltaAngleY, 0, 0);
    }
}
