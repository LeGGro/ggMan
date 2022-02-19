using Photon.Pun;
using UnityEngine;

// Positions:
// Spawn tm pos (5f,20f)
// pwTetrPos (15f,16f)
// his pos (0f,0f)
//
public class Game1 : MonoBehaviour
{

    #region //// Increasing Difficulty Params ////
    public int currentLevel = 0;
    private int numLinesCleared = 0;
    public float fallSpeed = 1f;
    #endregion
    #region //// Grid Params ////
    public static int gridWidth = 10;
    public static int gridHeight = 20;
   
    public static Transform[,] grid = new Transform[gridWidth, gridHeight];
    #endregion
    #region //// New Tetrominoes Params ////
    private GameObject previewNextTetromino;
    private GameObject nextTetromino;
    public bool isGameOver = false;
    private bool gameIsStarted = false;
    public int queue = 0;
    private Vector2 previewTetrominoPosition = new Vector2(13f, 16f);
    #endregion
    #region //// Audio Params ////
    public AudioClip rowDelete;
    private AudioSource audioSource;
    #endregion
    PhotonView Ph;
    
    private void Start()
    {
        // Ph = GetComponent<PhotonView>();
        //if (!Ph.IsMine)
        // { return; }
        SpawnNextTetroFromList();
        audioSource = GetComponent<AudioSource>();
    }
    public void Update()
    {
        UpdateLevel();
        UpdateSpeed();
    }
    public void LoadGameOver()
    {
        isGameOver = true;

    }

    #region ==== Grid Transformation ====
    public Transform GetTransformAtGridPosition(Vector2 pos)
    {
        if (pos.y > gridHeight - 1)
        {
            return null;

        }
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }
    public void UpdateGrid(Tetramino1 tetromino)
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            for (int x = 0; x < gridWidth; ++x)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == tetromino.transform)
                    {
                        grid[x, y] = null;

                    }
                }
            }
            foreach (Transform mino in tetromino.transform)
            {
                Vector2 pos = Rounding(/*mino.transform.parent.localPosition+ */ mino.position/*.localPosition*/);
                if (pos.y < gridHeight)
                {
                    grid[(int)pos.x, (int)pos.y] = mino;
                }

            }

        }

    }
    public bool CheckIsAboveGrid(Tetramino1 Tetramino1)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            foreach (Transform mino in Tetramino1.transform)
            {
                Vector2 pos = Rounding(mino.position);
                if (pos.y > gridHeight - 1)
                {
                    return true;
                }
            }


        }
        return false;
    }
    public bool IsInsideGrid(Vector2 pos) => ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    public Vector2 Rounding(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }
    #endregion
    #region ==== Rows Deleting ====
    public bool IsFullRowAt(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        return true;
    }
    public void DeleteMinoAt(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }
    public void MoveRowDown(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);

            }
        }
    }
    public void MoveAllRowsDown(int y)
    {
        for (int i = y; i < gridHeight; ++i)
        {
            MoveRowDown(i);
        }
    }
    public void DeleteRow()
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            if (IsFullRowAt(y))
            {
                DeleteMinoAt(y);
                MoveAllRowsDown(y + 1);
                --y;
            }

        }
        RowDeleteSound();
        numLinesCleared++;
    }
    #endregion
    #region ==== Audio Methods ====
    public void RowDeleteSound()
    { audioSource.PlayOneShot(rowDelete); }
    #endregion
    #region ==== Spawning New Tetrominoes ====
    public void SpawnNextTetroFromList()
    {
        if (!isGameOver)
        {
            if (!gameIsStarted)
            {
                gameIsStarted = true;
                nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(rando.mass[queue])),
                    /*parent.transform.position +*/ /*new Vector3(55f, 20f, 0)*/new Vector3(5f, 20f, 0),
                    Quaternion.identity);
                queue++;
                previewNextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(rando.mass[queue])),
                    previewTetrominoPosition,
                    Quaternion.identity);
                previewNextTetromino.GetComponent<Tetramino1>().enabled = false;
                queue++;
            }
            else 
            {
                previewNextTetromino.transform.localPosition = new Vector2(5f, 20f);
                nextTetromino = previewNextTetromino;
                nextTetromino.GetComponent<Tetramino1>().enabled = true;

                previewNextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(rando.mass[queue])),
                    previewTetrominoPosition,
                    Quaternion.identity);
                queue++;
                previewNextTetromino.GetComponent<Tetramino1>().enabled = false;
            }


            
        }

    }
    public void SpawnNextTetromino()
    {

        //if (Ph.IsMine)
        //{
        if (!isGameOver)
        {

            //GameObject nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector3(5f, 20f, 0f), Quaternion.identity, parent.transform);
            nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino()), /*parent.transform.position +*/ /*new Vector3(55f, 20f, 0)*/new Vector3(5f, 20f, 0), Quaternion.identity);
            //Debug.Log("index 1 spawned");
        }
        // }
        //nextTetromino.transform.SetParent(parent.transform);
    }
    string GetRandomTetromino(int randomTetr)
    {
        string randomTetrName = "T Tetramino1";
        switch (randomTetr)
        {
            case 1:
                randomTetrName = "T Tetramino1";
                break;
            case 2:
                randomTetrName = "J Tetramino1";
                break;
            case 3:
                randomTetrName = "L Tetramino1";
                break;
            case 4:
                randomTetrName = "I Tetramino1";
                break;
            case 5:
                randomTetrName = "O Tetramino1";
                break;
            case 6:
                randomTetrName = "Z Tetramino1";
                break;
            case 7:
                randomTetrName = "S Tetramino1";
                break;

        }
        return randomTetrName;
    }
    string GetRandomTetromino()
    {
        int randomTetr = Random.Range(1, 8);
        string randomTetrName = "T Tetramino1";
        switch (randomTetr)
        {
            case 1:
                randomTetrName = "T Tetramino1";
                break;
            case 2:
                randomTetrName = "J Tetramino1";
                break;
            case 3:
                randomTetrName = "L Tetramino1";
                break;
            case 4:
                randomTetrName = "I Tetramino1";
                break;
            case 5:
                randomTetrName = "O Tetramino1";
                break;
            case 6:
                randomTetrName = "Z Tetramino1";
                break;
            case 7:
                randomTetrName = "S Tetramino1";
                break;

        }
        return randomTetrName;
    }
    #endregion
    #region ==== Increasing Difficulty Params ====
    public void UpdateLevel()
    {
        currentLevel = numLinesCleared / 10;
    }
    public void UpdateSpeed()
    {
        fallSpeed = 1f - ((float)currentLevel * 0.1f);
    }
    #endregion
}
