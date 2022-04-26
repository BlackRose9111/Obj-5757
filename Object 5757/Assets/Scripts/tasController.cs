using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tasController : MonoBehaviour
{
    public bool isInGame;
    public bool isFinished;

    //
    public Vector3 HomeLocation;

    public Vector3Int[] unitPath;

    //
    public int team;

    //
    private GameObject board;

    //
    public int positionIndex = -1;

    //
    private float duration;

    // Start is called before the first frame update
    private void Start()
    {
        board = GameObject.Find("Board");
        duration = board.GetComponent<GridYoneticisi>().duration;
        UpdatePosition();
        unitPath = getTeamPath();
    }

    public int getX()
    {
        return (int)transform.position.x; ;
    }

    public int getY()
    {
        return (int)transform.position.y;
    }

    private void UpdatePosition()
    {
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;

        board.GetComponent<GridYoneticisi>().taslar[x, y] = gameObject;
    }

    private void UpdatePosition(int x, int y)
    {
        board.GetComponent<GridYoneticisi>().taslar[x, y] = gameObject;
    }

    private void NullPosition()
    {
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;

        board.GetComponent<GridYoneticisi>().taslar[x, y] = null;
    }

    private void NullPosition(int x, int y)
    {
        board.GetComponent<GridYoneticisi>().taslar[x, y] = null;
    }

    private void NullPosition(Vector3Int Position)
    {
        NullPosition(Position.x, Position.y);
    }

    public void EnteredGame(int x, int y)
    {
        isInGame = true;
        isFinished = false;
        positionIndex = 0;
        Move(x, y);
    }

    public void EnteredGame(Vector3 StartPoint)
    {
        isInGame = true;
        isFinished = false;
        positionIndex = 0;
        Move(StartPoint);
    }

    public void ExitedGame()
    {
        isFinished = true;
        isInGame = false;
        NullPosition(unitPath[56]);
    }

    public void GotSlain()
    {
        isInGame = false;
        isFinished = false;
        Move(HomeLocation);
        positionIndex = -1;
    }

    public void SetHome(int x, int y, int z = 1)
    {
        HomeLocation = new Vector3(x, y, z);
    }

    public void Move(int x, int y, int z = 1)
    {
        NullPosition();
        gameObject.transform.DOMove(new Vector3(x, y, 1), duration);
        //slay the piece on the position we went to if there is one.
        if (board.GetComponent<GridYoneticisi>().taslar[x, y] != null)
        {
            board.GetComponent<GridYoneticisi>().taslar[x, y].GetComponent<tasController>().GotSlain();
        }
        UpdatePosition(x, y);
        if (positionIndex == 57)
        {
            ExitedGame();
        }
    }

    public void Move(Vector3 location)
    {
        NullPosition();

        gameObject.transform.DOMove(location, duration);
        //slay the piece on the position we went to if there is one.
        if (board.GetComponent<GridYoneticisi>().taslar[(int)location.x, (int)location.y] != null)
        {
            board.GetComponent<GridYoneticisi>().taslar[(int)location.x, (int)location.y].GetComponent<tasController>().GotSlain();
        }

        UpdatePosition((int)location.x, (int)location.y);
        if (positionIndex == 56)
        {
            ExitedGame();
        }
    }

    public void PieceAdvance(int step)
    {
        //did we reach max steps?

        if ((positionIndex + step) <= 56 && !isFinished)
        {
            //Can we enter the game?
            if (!isInGame)
            {
                if (step == 6)
                {
                    EnteredGame(unitPath[0]);
                }
            }
            else
            {
                //do we have a piece that is from our team in front of us? First we need to get the location we are going to.

                Vector3Int aim = unitPath[positionIndex + step];

                //then we ask if that location is null to the board.

                if (board.GetComponent<GridYoneticisi>().taslar[aim.x, aim.y] == null)
                {
                    Move(aim);
                    PositionIncrement(step);
                    //if we reached the end then we win.
                    //if (positionIndex == 56) { ExitedGame(); }
                }
                //if there is a piece and that piece is not our piece, slay that piece.
                else if (board.GetComponent<GridYoneticisi>().taslar[aim.x, aim.y].GetComponent<tasController>().team != team)
                {
                    board.GetComponent<GridYoneticisi>().taslar[aim.x, aim.y].GetComponent<tasController>().GotSlain();
                    Move(aim);
                    PositionIncrement(step);
                    //if we reached the end, then we win.
                }
            }
        }
        if (positionIndex == 56) { ExitedGame(); }
    }

    public bool CanWeMove(int roll)
    {
        bool condition = false;

        if (isInGame && positionIndex + roll <= 56 && !isFinished)
        {
            Vector3Int aim = unitPath[positionIndex + roll];

            if (board.GetComponent<GridYoneticisi>().taslar[aim.x, aim.y] == null)
            {
                condition = true;
            }
            else
            {
                if (board.GetComponent<GridYoneticisi>().taslar[aim.x, aim.y].GetComponent<tasController>().team != team)
                {
                    condition = true;
                }
            }
        }

        return condition;
    }

    public bool CanWeStart()
    {
        bool condition = false;
        if (!isInGame && !isFinished && 6 + positionIndex <= 56)
        {
            if (board.GetComponent<GridYoneticisi>().taslar[unitPath[0].x, unitPath[0].y] == null)
            {
                condition = true;
            }
            else
            {
                if (board.GetComponent<GridYoneticisi>().taslar[unitPath[0].x, unitPath[0].y].GetComponent<tasController>().team != team)
                {
                    condition = true;
                }
            }
        }

        return condition;
    }

    private Vector3Int[] getTeamPath()
    {
        Vector3Int[] Path;

        switch (team)
        {
            case 0:
                Path = board.GetComponent<GridYoneticisi>().bluePath;

                return Path;

            case 1:
                Path = board.GetComponent<GridYoneticisi>().redPath;

                return Path;

            case 2:
                Path = board.GetComponent<GridYoneticisi>().greenPath;
                return Path;

            case 3:
                Path = board.GetComponent<GridYoneticisi>().yellowPath;

                return Path;

            default:
                Path = board.GetComponent<GridYoneticisi>().bluePath;

                return Path;
        }
    }

    public void PositionIncrement(int movement)
    {
        positionIndex += movement;
    }
}