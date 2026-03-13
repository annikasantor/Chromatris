using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class Piece : MonoBehaviour
{
    private AudioSource _source;
    [SerializeField] private AudioClip _movePiece;
    [SerializeField] private AudioClip _pieceLand;
    [SerializeField] private AudioClip _rotatePiece;
    
    public Board board {get; private set;}
    public ScoreManager scoreManager {get; private set;}
    public TetrominoData data {get; private set;}
    public Vector3Int[] cells {get; private set;}
    public Vector3Int position {get; private set;}
    public int rotationIndex {get; private set;}

    public float stepTime = 1f;
    public float lockDelay = 0.3f;
    
    private float lockTime;
    
    private bool hasLocked = false;

    private float timer;
    
    private float _movementInterval = 0.15f;
    private float _movementTimer = 0.0f;
    
    public static bool _inCoroutine = false;

    private float _originalStepTime;
    
    [SerializeField] private TMP_Text _timeSlowedUI;

    //public static int Score;
    IEnumerator PowerUp()
    {
        //Debug.Log("In coroutine");
        _inCoroutine = true;
        //bool addScore = ScoreManager.addScore;

        //if (addScore)
        //{
        //    Score++;
        //}
        
        _originalStepTime = stepTime;
        //Debug.Log(_originalStepTime);
        stepTime = 10f;
        _timeSlowedUI.enabled = true;
        //_countdownText.enabled = true;
        
        yield return new WaitForSeconds(10);
        stepTime = _originalStepTime;
        _timeSlowedUI.enabled = false;
        _inCoroutine = false;
        //_countdownText.enabled = false;
    }

    private int time;

    //[SerializeField] private TMP_Text _countdownText;

    //private int countdown = 10;

    //public void Countdown()
   // {
   //     countdown--;
   //     _countdownText.text = countdown.ToString();

    //    if (countdown == 0)
  //      {
   //         _countdownText.text = 10.ToString();
   //     }
   // }
    
    
    
    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        rotationIndex = 0;

        //Score = 0;

        //_countdownText.enabled = false;
        _timeSlowedUI.enabled = false;
        //Debug.Log(stepTime);
        lockTime = 0f;

        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    public void CheckCoroutine()
    {
        if (_inCoroutine == false)
        {
            //Debug.Log("4 lines cleared");
            StartCoroutine(PowerUp());
            //Countdown();
        }
    }
    
    private void Start()
    {
        _source = GetComponent<AudioSource>();

    }
    private void Update()
    {
        board.Clear(this);
        
        lockTime += Time.deltaTime;
        
        bool _isMoving = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S);
        
        if (hasLocked == true)
        {
            stepTime *= 0.99f;
            
            hasLocked = false;
            //Debug.Log(hasLocked);
        }
        
        if (GameBehavior.Instance.GameMode == Utilities.GameState.Play)
        {

            HandleMoveInputs();
            
            if (_isMoving)
            {
                _source.PlayOneShot(_movePiece);
            }

            if (_movementTimer < _movementInterval)
            {
                _movementTimer += Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                _source.PlayOneShot(_rotatePiece);
                Rotate(-1);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                _source.PlayOneShot(_rotatePiece);
                Rotate(1);
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _source.PlayOneShot(_pieceLand);
                HardDrop();
                hasLocked = true;
                int linesCleared = Board.linesCleared;
                if (linesCleared == 4)
                {
                    CheckCoroutine(); 
                }
                //Debug.Log(hasLocked);
            }

            if (timer >= stepTime)
            {
                Step();
                timer -= stepTime;
            }

            board.Set(this);
            
            timer += Time.deltaTime;
        }
    }

    public void HandleMoveInputs()
    {
        
        if (_movementTimer < _movementInterval)
        {
            return;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Move(Vector2Int.right);
        }

        if (Input.GetKey(KeyCode.S))
        {
            Move(Vector2Int.down);
        }
    }

    private void Step()
    {
        Move(Vector2Int.down);

        if (lockTime >= lockDelay)
        {
            Lock();
            hasLocked = true;
            
            int linesCleared = Board.linesCleared;
            if (linesCleared == 4)
            {
                CheckCoroutine(); 
            }
            //Debug.Log(hasLocked);
        }
    }
    private void Lock()
    {
        board.Set(this);
        board.ClearLines();
        board.SpawnPiece();
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        if (valid)
        {
            position = newPosition;
            lockTime = 0f;
            _movementTimer = 0f;
        }

        return valid;
    }

    private void Rotate(int direction)
    {
        int originalRotationIndex = rotationIndex;
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);
        
        
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotationIndex;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                    
            }
            cells[i] = new Vector3Int(x, y, 0);
        } 
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }
        
        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }
        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }
    
    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        { 
            return min + (input - min) % (max - min);
        }
    }
}
