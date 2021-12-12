using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomCharacterController : MonoBehaviour
{
    [Serializable]
    public struct CharType
    { 
        public string name;
        public GameObject gameObject;
        public FootStepAndGroundCheck leftFoot;
        public FootStepAndGroundCheck rightFoot;
    }

    //public GameObject projectile;
    
    // state of char 
    public int state = 0;

    private int _jumpState = 0;
    // 0 for idle;
    // 1 for walk;
    // 2 for run;
    
    private CharacterController _charCtrl;
    private Animator _animator;
    private Rigidbody _rb;
    public float rotateSpeedHorizontal;
    public bool isGrounded;
    public bool jumpMode=false;
    public float jumpForce;
    public bool canLand=false;
    public float jumpForceHorizontalWalk;
    public float jumpForceHorizontalRun;
    private bool _applyJumpForce = false;
    public float jumpTime;
    public float jumpTimer;

    public bool shiftPressed=false;
    
    private bool _correctValues=true;

    public CharType[] charTypes;
    public int currentCharValue;
    private int _maxCharValue;
    
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");
    private static readonly int Landed = Animator.StringToHash("Landed");

    private void Start()
    {
        _charCtrl = charTypes[0].gameObject.GetComponent<CharacterController>();
        _animator = charTypes[0].gameObject.GetComponent<Animator>();
        _rb = charTypes[0].gameObject.GetComponent<Rigidbody>();
        _maxCharValue = charTypes.Length;
    }

    private void Update()
    {
        if (_correctValues)
        {
            _correctValues = SetParameters();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            print("left Click");
           // Vector3 x = new Vector3(projectile.transform.position.x, projectile.transform.position.y,
           //     projectile.transform.position.z + 10f);
           // projectile.transform.position=Vector3.Lerp(projectile.transform.position,x,5f);
        }
        
        ChangeViewWithCamera();

        MoveWithAnimation();

        isGrounded = CheckGround();
        
        bool jumpActive=false;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && jumpActive)
        {
            _applyJumpForce = true;
            jumpTimer = 0;
            jumpMode = true;
        }

        if (jumpTimer < jumpTime)
        {
            if (jumpMode)
                JumpVelocity();
            jumpTimer += Time.deltaTime;
        }

        

        if (_charCtrl.velocity.x == 0)
            state = 0;
        
        //print(_charCtrl.velocity.y);

    }

    #region - Jump / grounded-
    

    public void JumpVelocity()
    {
        if (_applyJumpForce)
        {
            if (state == 0)
                _jumpState = 0;
            if (state == 1)
                _jumpState = 1;
            if (state == 2)
                _jumpState = 2;
            
            _applyJumpForce = false;
        }
        
        if (_jumpState == 0)
        {
            _charCtrl.Move(new Vector3(0,jumpForce,0)*Time.deltaTime);
        }

        if (_jumpState == 1)
        {
            _charCtrl.Move(new Vector3(jumpForceHorizontalWalk, jumpForce, 0) * Time.deltaTime);
        }

        if (_jumpState == 2)
        {
            _charCtrl.Move(new Vector3(jumpForceHorizontalRun, jumpForce, 0) * Time.deltaTime);
        }



    }

    public bool CheckGround()
    {
        if (charTypes[currentCharValue].leftFoot.isGrounded)
            return true;
        
        if (charTypes[currentCharValue].rightFoot.isGrounded)
            return true;
        
        return false;
    }
    
    
    #endregion
    
    #region - Animation -
    
    
    private void MoveWithAnimation()
    {
        ForHorizontalAnimation();
        ForVerticalAnimation();
    }

    private void ForHorizontalAnimation()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _animator.SetBool(IsWalking,true);
            state = 1;
        }
        else
        {
            _animator.SetBool(IsWalking,false);
        }
        
        if(Input.GetKey(KeyCode.LeftShift))
        {
            shiftPressed = true;
            _animator.SetBool(IsRunning,true);
            state = 2;
        }
        else
        {
            shiftPressed = false;
            _animator.SetBool(IsRunning,false);
        }
    }

    private void ForVerticalAnimation()
    {
        if (_charCtrl.velocity.y >= 0.1 && jumpMode)
        {
            _animator.SetBool(IsJumping,true);
        }

        if (_charCtrl.velocity.y < 0.1 && jumpMode)
        {
            _animator.SetBool(IsJumping,false);
            _animator.SetBool(IsFalling,true);
            canLand = true;
        }
        
        if (jumpMode && isGrounded && canLand)
        {
            _animator.SetBool(IsFalling,false);
            _animator.SetBool(Landed,true);
            jumpMode = false;
            canLand = false;
        }
    }

    
    #endregion

    private void ChangeViewWithCamera()
    {
        transform.Rotate(0, Input.GetAxis("Mouse X")*rotateSpeedHorizontal, 0);
    }

    private bool SetParameters()
    {
        _charCtrl = charTypes[currentCharValue].gameObject.GetComponent<CharacterController>(); 
        _animator = charTypes[currentCharValue].gameObject.GetComponent<Animator>();
        _rb = charTypes[currentCharValue].gameObject.GetComponent<Rigidbody>();
        return false;
    }
}
