using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerLook : MonoBehaviourPun
{
    [Header("Camera Variables")]
    [SerializeField] private float _sensX;
    [SerializeField] private float _sensY;
    [SerializeField] private float _mouseX;
    [SerializeField] private float _mouseY;
    [SerializeField] private float _multiplier = 0.01f;
    [SerializeField] private float _xRot;
    [SerializeField] private float _yRot;

    [Header("Camera GameObject")]
    [SerializeField] private Camera _cam;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        if (!photonView.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
        else
        {
            _cam = GetComponentInChildren<Camera>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.visible = true;
        }
        
        if (photonView.IsMine)
        {
            CameraInput();
            
            _cam.transform.localRotation = Quaternion.Euler(_xRot,0,0);
            transform.rotation = Quaternion.Euler(0, _yRot, 0);
        }
    }

    void CameraInput()
    {
        _mouseX = Input.GetAxisRaw("Mouse X");
        _mouseY = Input.GetAxisRaw("Mouse Y");

        _yRot += _mouseX * _sensX * _multiplier;
        _xRot -= _mouseY * _sensY * _multiplier;

        _xRot = Mathf.Clamp(_xRot, 15f, 15f);
    }
}
