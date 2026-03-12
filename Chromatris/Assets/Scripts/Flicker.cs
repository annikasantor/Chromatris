using System.Collections;
using UnityEngine;
using TMPro;
public class Flicker : MonoBehaviour
{
    [SerializeField] private float _flickerRate = 0.5f;
    
    private TMP_Text _text;

    private bool _inCoroutine = false;

    void Start()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (!_inCoroutine)
        {
            StartCoroutine(FlickerText()); 
        }
    }

    IEnumerator FlickerText()
    {
        while (true)
        {
            _inCoroutine = true;
            
            _text.enabled = !_text.enabled;
            
            yield return new WaitForSeconds(_flickerRate);
            
            _inCoroutine = false;
        }
    }
}