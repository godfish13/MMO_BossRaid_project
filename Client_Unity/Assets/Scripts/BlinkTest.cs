using Assets.PixelFantasy.Common.Scripts;
using Assets.PixelFantasy.PixelMonsters.Common.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkTest : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            Blink(gameObject);
            Debug.Log("Blink");
        }
    }

    private static Material _baseMaterial;
    private static Material _blinkMaterial;

    public void Blink(GameObject go)
    {
        if (_baseMaterial == null) _baseMaterial = go.GetComponent<SpriteRenderer>().sharedMaterial;
        if (_blinkMaterial == null) _blinkMaterial = new Material(Shader.Find("GUI/Text Shader"));
        
        StartCoroutine(BlinkCoroutine(go));
    }

    private IEnumerator BlinkCoroutine(GameObject go)
    {
        go.GetComponent<SpriteRenderer>().material = _blinkMaterial;

        yield return new WaitForSeconds(0.1f);

        go.GetComponent<SpriteRenderer>().material = _baseMaterial;
    }
}
