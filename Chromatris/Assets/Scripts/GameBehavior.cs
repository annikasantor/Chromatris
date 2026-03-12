using UnityEngine;

public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance;
    
    private AudioSource _source;
    [SerializeField] private AudioClip _menuSound;

    private Utilities.GameState _gameMode;

    public Utilities.GameState GameMode
    {
        get => _gameMode;

        set
        {
            _gameMode = value;
        }
    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
    }
    
    private void Start()
    {
        _source = GetComponent<AudioSource>();

        GameMode = Utilities.GameState.Play;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameMode = GameMode == Utilities.GameState.Play ? 
                Utilities.GameState.Pause : 
                Utilities.GameState.Play;
            
            _source.PlayOneShot(_menuSound);
        }
    }
}


