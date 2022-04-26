using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TileYoneticisi : MonoBehaviour
{
    public bool highlit;
    public Material mainMaterial;
    public Material highlight;
    public GameObject board;

    // Start is called before the first frame update
    private void Start()
    {
        highlight = board.GetComponent<GridYoneticisi>().StartPoints;
    }

    public void SetDefaultColour(Material Default)
    {
        mainMaterial = Default;
    }

    public void HighlightTile()
    {
        gameObject.GetComponent<SpriteRenderer>().material = highlight;
        highlit = true;
    }

    public void ResetHighlight()
    {
        gameObject.GetComponent<SpriteRenderer>().material = mainMaterial;
        highlit = false;
    }
}