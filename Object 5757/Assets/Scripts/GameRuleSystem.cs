using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRuleSystem
{
    private GameObject[,] tiles;
    private GameObject[] highlightedTiles = new GameObject[4];

    public GameRuleSystem(GameObject[,] input)
    {
        tiles = input;
    }

    public void PreparePlayPieces(int roll, GameObject[] pieces)

    {
        ResetHighlights();

        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i].GetComponent<tasController>().CanWeMove(roll))
            {
                highlightedTiles[i] = tiles[pieces[i].GetComponent<tasController>().getX(), pieces[i].GetComponent<tasController>().getY()];
                highlightedTiles[i].GetComponent<TileYoneticisi>().HighlightTile();
            }
            if (roll == 6 && pieces[i].GetComponent<tasController>().CanWeStart())
            {
                highlightedTiles[i] = tiles[pieces[i].GetComponent<tasController>().getX(), pieces[i].GetComponent<tasController>().getY()];
                highlightedTiles[i].GetComponent<TileYoneticisi>().HighlightTile();
            }
        }
    }

    public void ResetHighlights()
    {
        for (int i = 0; i < highlightedTiles.Length; i++)
        {
            if (highlightedTiles[i])
            {
                highlightedTiles[i].GetComponent<TileYoneticisi>().ResetHighlight();
            }
        }

        highlightedTiles = new GameObject[4];
    }

    public int HowManyPiecesInGame(GameObject[] team)
    {
        int activePieces = 0;

        for (int i = 0; i < team.Length; i++)
        {
            if (team[i].GetComponent<tasController>().isInGame)
            {
                activePieces++;
            }
        }

        return activePieces;
    }

    public GameObject GetActivePiece(GameObject[] team)
    {
        GameObject Piece = new GameObject();

        for (int i = 0; i < team.Length; i++)
        {
            if (team[i].GetComponent<tasController>().isInGame)
            {
                Piece = team[i];
                break;
            }
        }

        return Piece;
    }

    public int[] GetTurnOrder(bool[] team)
    {
        int turncount = 0;

        for (int i = 0; i < team.Length; i++)
        {
            if (team[i] == true)
            {
                turncount++;
            }
        }
        int[] Result = new int[turncount];
        int secondIndex = 0;
        for (int i = 0; i < team.Length; i++)
        {
            if (team[i] == true)
            {
                Result[secondIndex] = i;
                secondIndex++;
            }
        }

        return Result;
    }
}