using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCalculator
{
    public enum TEAM
    {
        BLUE,
        RED,
        GREEN,
        YELLOW
    }

    private enum DIRECTION
    {
        TOP,
        RIGHT,
        LEFT,
        BOTTOM
    }

    private static DIRECTION[] directions = new DIRECTION[]
    {
                DIRECTION.TOP, //BLUE_START
                DIRECTION.LEFT,
                DIRECTION.TOP,
                DIRECTION.RIGHT, //RED_START
                DIRECTION.TOP,
                DIRECTION.RIGHT,
                DIRECTION.BOTTOM, //GREEN_START
                DIRECTION.RIGHT,
                DIRECTION.BOTTOM,
                DIRECTION.LEFT, //YELLOW_START
                DIRECTION.BOTTOM,
                DIRECTION.LEFT
     };

    private static int GetStartingDirIndex(TEAM team)
    {
        const int BLUE_DIRECTION_START = 0;
        const int RED_DIRECTION_START = 3;
        const int GREEN_DIRECTION_START = 6;
        const int YELLOW_DIRECTION_START = 9;

        switch (team)
        {
            case TEAM.BLUE: return BLUE_DIRECTION_START;
            case TEAM.RED: return RED_DIRECTION_START;
            case TEAM.GREEN: return GREEN_DIRECTION_START;
            case TEAM.YELLOW: return YELLOW_DIRECTION_START;
            default: return -1;
        }
    }

    private static DIRECTION GetDirFromIndex(int index)
    {
        return directions[index];
    }

    private static Vector3Int GetStartingPoint(TEAM team)
    {
        switch (team)
        {
            case TEAM.BLUE: return new Vector3Int(6, 1, 1);
            case TEAM.RED: return new Vector3Int(1, 8, 1);
            case TEAM.GREEN: return new Vector3Int(8, 13, 1);
            case TEAM.YELLOW: return new Vector3Int(13, 6, 1);
            default: return new Vector3Int(-1, -1, 1);
        }
    }

    private static Vector3Int ExtendPosToDir(Vector3Int pos, DIRECTION dir)
    {
        int xAddition = dir == DIRECTION.RIGHT ? 1 : (dir == DIRECTION.LEFT ? -1 : 0);
        int yAddition = dir == DIRECTION.TOP ? 1 : (dir == DIRECTION.BOTTOM ? -1 : 0);

        return new Vector3Int(pos.x + xAddition, pos.y + yAddition, pos.z);
    }

    public static Vector3Int[] GeneratePath(int team)
    {
        return GeneratePath((TEAM)team);
    }

    public static Vector3Int[] GeneratePath(TEAM team)
    {
        int dirIndex = GetStartingDirIndex(team);
        Vector3Int currentPoint = GetStartingPoint(team);

        Vector3Int[] points = new Vector3Int[57];
        points[0] = currentPoint;

        int numberOfTilesInserted = 1;
        for (int i = 2; i < 100; i++)
        {
            currentPoint = ExtendPosToDir(currentPoint, GetDirFromIndex(dirIndex));

            if ((i - (i / 14) * 2) % 6 == 0)
            {
                dirIndex = (dirIndex + 1) % directions.Length;
            }

            if (i == 6 || (i - 6) % 14 == 0)
            {
                //SKIP THIS TILE
            }
            else
            {
                points[numberOfTilesInserted] = currentPoint;
                numberOfTilesInserted++;

                if (numberOfTilesInserted == 51)
                    break;
            }
        }
        DIRECTION dir = GetDirFromIndex(GetStartingDirIndex(team));

        for (int i = 0; i < 6; i++)
        {
            currentPoint = ExtendPosToDir(currentPoint, dir);
            points[51 + i] = currentPoint;
        }

        return points;
    }
}