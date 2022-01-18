using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StartingFadeOut : MonoBehaviour
{
    [SerializeField] private Image _fader;

    void Start()
    {
        _fader.color = new Color(.27f, .27f, .27f, 1);
        StartCoroutine(Fader());
    }

    private IEnumerator Fader()
    {
        yield return new WaitForSeconds(1);
                
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            _fader.color = new Color(.27f, .27f, .27f, i);
            yield return null;
        }
        Destroy(this.gameObject); 
    }


}
