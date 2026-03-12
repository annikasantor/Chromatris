using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using TMPro;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    
    private AudioSource _source;
    [SerializeField] private AudioClip _lineClear;
    [SerializeField] private AudioClip _gameOver;
    
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
   
    public TetrominoData[] tetrominos;
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public Vector2Int boardSize = new Vector2Int(10, 20);
    
    private Utilities.GameState _gameMode;
    
    [SerializeField] private TMP_Text _pauseUI;

    public Utilities.GameState GameMode
    {
        get => _gameMode;

        set
        {
            _gameMode = value;
            _pauseUI.enabled = GameMode == Utilities.GameState.Pause;
        }
    }

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
        
        for (int i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }
    }

    private void Start()
    {
        GameMode = Utilities.GameState.Play;
        
        _source = GetComponent<AudioSource>();
        
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominos.Length);
        TetrominoData data = tetrominos[random];

        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition))
        {
             Set(activePiece);
        }
        else
        {
            GameOver();
        }

        Set(activePiece);
    }
    
    private void GameOver()
    {
        tilemap.ClearAllTiles();
        
        _source.PlayOneShot(_gameOver);
        
        SceneManager.LoadScene("GameOver");
    }
    
    
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }
    
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;
        
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                ScoreManager.instance.AddScore();
                _source.PlayOneShot(_lineClear);
            }
            else
            {
                row++;
            }
        }
    }
    
    private bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = Bounds.xMin; col < Bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }
        return true;
    }

    private void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (int col = Bounds.xMin; col < Bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);
                
                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }
            row++;
        }
    }
}
