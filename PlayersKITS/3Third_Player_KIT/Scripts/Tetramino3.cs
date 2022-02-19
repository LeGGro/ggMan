using Photon.Pun;
using UnityEngine;

public class Tetramino3 : MonoBehaviourPunCallbacks
{
    #region //// Fall Params ////
    private float fallSpeed;
    private float fallTime;
    #endregion
    #region //// Smooth Move Params ////
    private float continiousVerticalSpeed = 0.05f;
    private float continiousHorizontalSpeed = 0.1f;
    private float buttonDownWaitMax = 0.2f;

    private float verticalTimer = 0f;
    private float horizontalTimer = 0;
    private float buttonDownTimerHorizontal = 0;
    private float buttonDownTimerVertical = 0;

    private bool movedImmediateHorizontal = false;
    private bool movedImmediateVertical = false;
    #endregion
    #region //// Rotation Params ////
    public Vector3 rotatePoint;
    public bool allowRotation = true;
    public bool limitRotation = false;
    #endregion
    #region //// Audio Params ////
    public AudioClip moveSound;
    public AudioClip fallSound;

    private AudioSource audioSource;
    #endregion
    // PhotonView ph;
    void Start()
    {
        fallTime = Time.time;
        //ph = GetComponent<PhotonView>();
        fallSpeed = GameObject.Find("ThirdPlayer").GetComponent<Game3>().fallSpeed;
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        //if (!ph.IsMine)
        //{ return; }
        CheckInputs();
    }
    public void CheckInputs()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            movedImmediateHorizontal = false;
            buttonDownTimerHorizontal = 0;
            horizontalTimer = 0;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            verticalTimer = 0;
            movedImmediateVertical = false;
            buttonDownTimerVertical = 0;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }
        if (Input.GetKey(KeyCode.DownArrow) || Time.time - fallTime >= fallSpeed)
        {
            MoveDown();
        }

    }
    #region ==== Move Methods ====
    public void MoveLeft()
    {
        if (movedImmediateHorizontal)
        {
            if (buttonDownTimerHorizontal < buttonDownWaitMax)
            {
                buttonDownTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continiousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }
        if (!movedImmediateHorizontal)
        { movedImmediateHorizontal = true; }

        horizontalTimer = 0;

        transform.position += new Vector3(-1f, 0f, 0f);
        if (isPositionValid())
        {
            FindObjectOfType<Game3>().UpdateGrid(this);
            PlayMoveSound();
        }
        else transform.position += new Vector3(1f, 0f, 0f);
    }
    public void MoveRight()
    {

        
        if (movedImmediateHorizontal)
        {
            if (buttonDownTimerHorizontal < buttonDownWaitMax)
            {
                buttonDownTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continiousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }
        if (!movedImmediateHorizontal)
        { movedImmediateHorizontal = true; }

        horizontalTimer = 0;

        transform.position += new Vector3(1f, 0f, 0f);
        if (isPositionValid())
        {
            FindObjectOfType<Game3>().UpdateGrid(this);
            PlayMoveSound();
        }
        else transform.position += new Vector3(-1f, 0f, 0f);
    }
    public void MoveDown()
    {

        if (movedImmediateVertical)
        {
            if (buttonDownTimerVertical < buttonDownWaitMax)
            {
                buttonDownTimerVertical += Time.deltaTime;
                return;
            }
            if (verticalTimer < continiousVerticalSpeed)
            {
                verticalTimer += Time.deltaTime;
                return;
            }
        }
        if (!movedImmediateVertical)
        { movedImmediateVertical = true; }
        verticalTimer = 0;

        transform.position += new Vector3(0f, -1f, 0f);
        if (isPositionValid())
        {
            FindObjectOfType<Game3>().UpdateGrid(this);
            PlayFallSound();
        }
        else
        {
            transform.position += new Vector3(0f, 1f, 0f);
            FindObjectOfType<Game3>().DeleteRow();
            if (FindObjectOfType<Game3>().CheckIsAboveGrid(this))
            {
                FindObjectOfType<Game3>().LoadGameOver();


            }
            enabled = false;

            //FindObjectOfType<Game3>().SpawnNextTetromino();
            FindObjectOfType<Game3>().SpawnNextTetroFromList();
        }
        fallTime = Time.time;
    }
    public void Rotate()
    {
        if (allowRotation)
        {
            if (limitRotation)
            {
                if (transform.rotation.eulerAngles.z >= 90)
                {
                    transform.RotateAround(transform.TransformPoint(rotatePoint), new Vector3(0f, 0f, 1f), -90f);
                }
                else { transform.RotateAround(transform.TransformPoint(rotatePoint), new Vector3(0f, 0f, 1f), 90f); }
            }
            else
            {
                transform.RotateAround(transform.TransformPoint(rotatePoint), new Vector3(0f, 0f, 1f), 90f);

            }

        }
        if (isPositionValid())
        {
            FindObjectOfType<Game3>().UpdateGrid(this);
        }
        else
        {
            if (limitRotation)
            {
                if (transform.rotation.eulerAngles.z >= 90)
                { transform.RotateAround(transform.TransformPoint(rotatePoint), new Vector3(0f, 0f, 1f), -90f); }
                else
                { transform.RotateAround(transform.TransformPoint(rotatePoint), new Vector3(0f, 0f, 1f), 90f); }
            }
            else { transform.RotateAround(transform.TransformPoint(rotatePoint), new Vector3(0f, 0f, 1f), -90f); }
        }
    }

    bool isPositionValid()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<Game3>().Rounding(mino.position - new Vector3(60f,0,0));
            if (!FindObjectOfType<Game3>().IsInsideGrid(pos))
            {
                return false;
            }


            if (FindObjectOfType<Game3>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game3>().GetTransformAtGridPosition(pos).parent != transform)
            { return false; }
        }
        return true;

    }

    #endregion
    #region ==== Audio Methods ====
    public void PlayMoveSound()
    { audioSource.PlayOneShot(moveSound); }

    public void PlayFallSound()
    { audioSource.PlayOneShot(fallSound); }
    #endregion

}
