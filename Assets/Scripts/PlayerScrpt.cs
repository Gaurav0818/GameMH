using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerScrpt : MonoBehaviour
{
    // movement is be based on char controller
    
    private CharacterController _charController;
    

    private void Start()
    {
        _charController = GetComponent<CharacterController>();
    }
    
    private void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        Vector3 movement = new Vector3();
        _charController.Move(movement);
    }

    private void OnAnimatorMove()
    {
        throw new NotImplementedException();
    }
}
