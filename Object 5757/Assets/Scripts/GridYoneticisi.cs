using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class GridYoneticisi : MonoBehaviour
{
    //Taþ seçmek için raycast

    //Tahtanýn boyutlarýný belirle
    private int satir = 15;

    private int sutun = 15;

    //Smooth hareket süresi
    public float duration = 0.5f;

    //Tilerin tutulduðu array
    public GameObject[,] indexes = new GameObject[15, 15];

    //taþlarýn renklerine göre tutulduðu arrayler

    public GameObject[] bluePieces = new GameObject[4];
    public GameObject[] redPieces = new GameObject[4];
    public GameObject[] greenPieces = new GameObject[4];
    public GameObject[] yellowPieces = new GameObject[4];

    //
    public Vector3Int[] bluePath;

    public Vector3Int[] redPath;

    public Vector3Int[] greenPath;

    public Vector3Int[] yellowPath;

    //tile prefabý ve renk materyalleri

    public GameObject prefab;

    //Renk Materyalleri

    public Material red;
    public Material blue;
    public Material green;
    public Material yellow;
    public Material white;
    public Material StartPoints;

    //tas prefabi
    public GameObject piece;

    //Tüm taþlarýn ortak tutulduðu array
    public GameObject[,] taslar = new GameObject[15, 15];

    //spawn edilecek takýmlar
    public bool spawnBlue, spawnRed, spawnGreen, spawnYellow;

    //Zar sprite
    public Sprite[] dieFaces = new Sprite[6];

    public TextMeshProUGUI diceObject;

    //Indikatör text
    public TextMeshProUGUI indicator;

    //Oyun Logic

    private int roll = 0;
    private System.Random rnd = new System.Random();
    private int firstTeamValue = 4;
    private int lastTeamValue = 0;
    private bool switchTurn = true;
    private GameRuleSystem gameMaster;

    //private bool buttonLock = true;
    public bool OnlineMode;

    //Takýmlar
    public int teamCount = 0;

    private bool[] teamArray = new bool[4];
    public int teamTurnCounter = 0;
    private int[] turnTeams;
    private int turnIndex = 0;

    private void Start()
    {
        //Build the grid,
        generateGrid();
        //Generate the pieces,
        generatePieces();
        //set the turn indicator to its default value
        IndicatorSet(4);
        //Dotween animations need extra juice to work properly.
        DOTween.SetTweensCapacity(10000, 20000);
        //gameMaster object requires the made grid to highlight the necessary tiles. It has to be initialised after the grid is made.
        gameMaster = new GameRuleSystem(indexes);
        //turn teams keeps the team IDs in the order they will start. if there are all four teams it will be {0,1,2,3} if there are
        //ex. blue and green teams only {0,2}
        turnTeams = gameMaster.GetTurnOrder(teamArray);
        //First team to go will be the first member of the turnTeams array
        firstTeamValue = turnTeams[0];
        //teamTurnCounter holds the team that is making their play. In the beginning it has to be the first team.
        teamTurnCounter = firstTeamValue;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit)
            {
                GameObject Tile = hit.collider.gameObject;

                if (Tile.GetComponent<TileYoneticisi>().highlit)
                {
                    taslar[(int)Tile.transform.position.x, (int)Tile.transform.position.y].GetComponent<tasController>().PieceAdvance(roll);
                    gameMaster.ResetHighlights();
                    //buttonLock = true;
                    //IncrementTurn();
                }
            }
        }
    }

    /// <summary>
    /// roll the dice.
    /// </summary>
    /// <param name="roller"></param>
    public void RollTheDice(bool roller = true)
    {
        if (/* buttonLock */ true)
        {
            switchTurn = true;
            Debug.Log(turnIndex);
            roll = rnd.Next(1, 7);
            diceObject.text = roll.ToString();

            if (roll == 6)
            {
                switchTurn = false;
            }
            Advance();
            IndicatorSet(teamTurnCounter);
            IncrementTurn();
            //buttonLock = false;
        }
    }

    /// <summary>
    /// Hangi takýmýn oynayacaðýna göre highlight seçeniðini uygular ya da tek taþ hareketini yapar.
    /// </summary>
    private void Advance()
    {
        GameObject[] pieces;

        Debug.Log(teamTurnCounter);

        switch (teamTurnCounter)
        {
            case 0:
                pieces = bluePieces;

                break;

            case 1:

                pieces = redPieces;

                break;

            case 2:

                pieces = greenPieces;

                break;

            case 3:

                pieces = yellowPieces;

                break;

            default:
                pieces = bluePieces;

                break;
        }

        if (gameMaster.HowManyPiecesInGame(pieces) == 1 && roll != 6)
        {
            gameMaster.GetActivePiece(pieces).GetComponent<tasController>().PieceAdvance(roll);
            gameMaster.ResetHighlights();
            //buttonLock = true;
        }
        else
        {
            gameMaster.PreparePlayPieces(roll, pieces);
        }
    }

    /// <summary>
    /// Geçici Indicatoru deðiþtirir.
    /// </summary>
    /// <param name="input">Takým inputu</param>
    private void IndicatorSet(int input)
    {
        switch (input)
        {
            case 0:

                indicator.text = "Mavi " + roll + " Oynadý";
                indicator.color = Color.blue;
                //Debug.Log("Sýra Mavi");

                break;

            case 1:
                indicator.text = "Kýrmýzý " + roll + " Oynadý";
                indicator.color = Color.red;
                //Debug.Log("Sýra Kýrmýzý");

                break;

            case 2:
                indicator.text = "Yeþil " + roll + " Oynadý";
                indicator.color = Color.green;
                //Debug.Log("Sýra Yeþil");

                break;

            case 3:
                indicator.text = "Sarý " + roll + " Oynadý";
                indicator.color = Color.yellow;
                //Debug.Log("Sýra Sarý");
                break;

            default:
                indicator.text = "Oyuna Baþlamak Ýçin Zarý At";
                break;
        }
    }

    /// <summary>
    /// Bütün taþlarý oluþturmak için kullanýlýr, hangi takýmlarýn oyunda olmasýný istiyorsak ona göre renk parametresi girilmelidir.
    /// </summary>
    /// <param name="blue"></param>
    /// <param name="red"></param>
    /// <param name="green"></param>
    /// <param name="yellow"></param>
    private void generatePieces()
    {
        if (spawnBlue)
        {
            bluePieces[0] = generateSinglePiece(2, 3, "Blue");

            //

            bluePieces[1] = generateSinglePiece(3, 3, "Blue");

            //
            bluePieces[2] = generateSinglePiece(2, 2, "Blue");

            //
            bluePieces[3] = generateSinglePiece(3, 2, "Blue");

            //
            NamePieces(bluePieces, "Mavi ");
            teamCount++;
            lastTeamValue = 1;
            teamArray[0] = true;
            SetBluePath();
        }
        if (spawnRed)
        {
            redPieces[0] = generateSinglePiece(2, 12, "Red");

            //
            redPieces[1] = generateSinglePiece(3, 12, "Red");

            //
            redPieces[2] = generateSinglePiece(2, 11, "Red");

            //
            redPieces[3] = generateSinglePiece(3, 11, "Red");

            //
            NamePieces(redPieces, "Kirmizi ");
            lastTeamValue = 2;
            teamCount++;
            teamArray[1] = true;
            SetRedPath();
        }
        if (spawnGreen)
        {
            greenPieces[0] = generateSinglePiece(11, 12, "Green");

            //
            greenPieces[1] = generateSinglePiece(12, 12, "Green");

            //
            greenPieces[2] = generateSinglePiece(11, 11, "Green");

            //
            greenPieces[3] = generateSinglePiece(12, 11, "Green");

            //
            NamePieces(greenPieces, "Yesil ");
            lastTeamValue = 3;
            teamCount++;
            teamArray[2] = true;
            SetGreenPath();
        }
        if (spawnYellow)
        {
            yellowPieces[0] = generateSinglePiece(11, 3, "Yellow");

            //
            yellowPieces[1] = generateSinglePiece(12, 3, "Yellow");

            //
            yellowPieces[2] = generateSinglePiece(11, 2, "Yellow");

            //
            yellowPieces[3] = generateSinglePiece(12, 2, "Yellow");

            //
            NamePieces(yellowPieces, "Sari ");
            lastTeamValue = 4;
            teamCount++;
            teamArray[3] = true;
            SetYellowPath();
        }

        //for (int i = 0; i < teamArray.Length; i++)
        //{
        //    if (teamArray[i] == true)
        //    {
        //        firstTeamValue = i;
        //        Debug.Log("First team is " + firstTeamValue);

        //        break;
        //    }
        //}

        Debug.Log("Last team is " + lastTeamValue);
        //IndicatorSet(firstTeamValue);
    }

    /// <summary>
    /// Yeþil takýmýn yolunu kaydetmek için
    /// </summary>
    private void SetGreenPath()
    {
        greenPath = PathCalculator.GeneratePath(2);
        //greenPath = new Vector3Int[57];
        ////int x = 6, y = 1;
        //SetPath(greenPath, "Assets/Assets/Paths/GreenPath.csv");
    }

    /// <summary>
    /// Mavi takýmýn yolunu kaydetmek için.
    /// </summary>
    private void SetBluePath()
    {
        bluePath = PathCalculator.GeneratePath(0);

        //bluePath = new Vector3Int[57];
        ////int x = 6, y = 1;
        //SetPath(bluePath, "Assets/Assets/Paths/BluePath.csv");
    }

    /// <summary>
    /// Kýrmýzý takýmýn yolunu kaydetmek için
    /// </summary>
    private void SetRedPath()
    {
        redPath = PathCalculator.GeneratePath(1);

        //redPath = new Vector3Int[57];

        //SetPath(redPath, "Assets/Assets/Paths/RedPath.csv");
    }

    /// <summary>
    /// Sarý takýmýn yolunu kaydetmek için
    /// </summary>
    private void SetYellowPath()
    {
        yellowPath = PathCalculator.GeneratePath(3);

        //yellowPath = new Vector3Int[57];
        ////int x = 6, y = 1;
        //SetPath(yellowPath, "Assets/Assets/Paths/YellowPath.csv");
    }

    /// <summary>
    /// Dosyadan yol koordinatlarýný çeken fonksiyon.
    /// </summary>
    /// <param name="Path">editlenecek yol tutan array</param>
    /// <param name="address">dosya konumu</param>
    private void SetPath(Vector3Int[] Path, string address)
    {
        string[] values = System.IO.File.ReadAllLines(address);
        for (int i = 0; i < Path.Length; i++)
        {
            Path[i] = new Vector3Int(Convert.ToInt32(values[i].Split(',')[0]), Convert.ToInt32(values[i].Split(',')[1]), -1);
        }
    }

    /// <summary>
    /// Taþlarýn isimlerini ayarlar
    /// </summary>
    /// <param name="array"></param>
    /// <param name="name"></param>
    private void NamePieces(GameObject[] array, string name)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i].name = name + (i + 1).ToString();
        }
    }

    /// <summary>
    /// Girilen konumda tek taþ oluþturur ve bu taþý dönderir.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="colour">Taþýn rengi</param>
    /// <returns></returns>
    private GameObject generateSinglePiece(int x, int y, string colour = "Beyaz")
    {
        GameObject Obje = Instantiate(piece, new Vector3(x, y, 1), Quaternion.identity);
        Obje.GetComponent<tasController>().SetHome(x, y);

        switch (colour)
        {
            case "Blue":
                Obje.GetComponent<SpriteRenderer>().material = blue;
                Obje.GetComponent<tasController>().team = 0;

                break;

            case "Red":
                Obje.GetComponent<SpriteRenderer>().material = red;
                Obje.GetComponent<tasController>().team = 1;

                break;

            case "Green":
                Obje.GetComponent<SpriteRenderer>().material = green;
                Obje.GetComponent<tasController>().team = 2;

                break;

            case "Yellow":
                Obje.GetComponent<SpriteRenderer>().material = yellow;
                Obje.GetComponent<tasController>().team = 3;

                break;

            default:
                Obje.GetComponent<tasController>().team = -1;
                break;
        }

        return Obje;
    }

    /// <summary>
    /// Tahtayý oluþturan method
    /// </summary>
    private void generateGrid()
    {
        for (int i = 0; i < satir; i++)
        {
            for (int j = 0; j < sutun; j++)
            {
                generateATile(i, j);
            }
        }
        setWalkTilesColour();
        setWhiteAroundFortress();
    }

    /// <summary>
    /// Girilen konumda kare oluþturur
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void generateATile(int x, int y)
    {
        indexes[x, y] = Instantiate(prefab, new Vector3(x, y, 1), Quaternion.identity);
        setColour(x, y);
        indexes[x, y].name = x.ToString() + "," + y.ToString();
    }

    /// <summary>
    /// Kareleri konumlarýna göre renklendirir
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void setColour(int x, int y)
    {
        if (x >= 0 && x < 6 && y >= 0 && y < 6)
        {
            indexes[x, y].GetComponent<SpriteRenderer>().material = blue;
            indexes[x, y].GetComponent<TileYoneticisi>().SetDefaultColour(blue);
        }
        else if (x >= 9 && x < 15 && y >= 0 && y < 6)
        {
            indexes[x, y].GetComponent<SpriteRenderer>().material = yellow;
            indexes[x, y].GetComponent<TileYoneticisi>().SetDefaultColour(yellow);
        }
        else if (x >= 0 && x < 6 && y >= 9 && y < 15)
        {
            indexes[x, y].GetComponent<SpriteRenderer>().material = red;
            indexes[x, y].GetComponent<TileYoneticisi>().SetDefaultColour(red);
        }
        else if (x >= 9 && x < 15 && y >= 9 && y < 15)
        {
            indexes[x, y].GetComponent<SpriteRenderer>().material = green;
            indexes[x, y].GetComponent<TileYoneticisi>().SetDefaultColour(green);
        }
    }

    /// <summary>
    /// Yürünebilir renkli alanlarý çizer
    /// </summary>
    private void setWalkTilesColour()
    {
        indexes[6, 1].GetComponent<SpriteRenderer>().material = blue;
        indexes[6, 1].GetComponent<TileYoneticisi>().SetDefaultColour(blue);
        for (int i = 1; i < 7; i++)
        {
            indexes[7, i].GetComponent<SpriteRenderer>().material = blue;
            indexes[7, i].GetComponent<TileYoneticisi>().SetDefaultColour(blue);
        }
        indexes[1, 8].GetComponent<SpriteRenderer>().material = red;
        indexes[1, 8].GetComponent<TileYoneticisi>().SetDefaultColour(red);
        for (int i = 1; i < 7; i++)
        {
            indexes[i, 7].GetComponent<SpriteRenderer>().material = red;
            indexes[i, 7].GetComponent<TileYoneticisi>().SetDefaultColour(red);
        }
        indexes[8, 13].GetComponent<SpriteRenderer>().material = green;
        indexes[8, 13].GetComponent<TileYoneticisi>().SetDefaultColour(green);
        for (int i = 13; i >= 8; i--)
        {
            indexes[7, i].GetComponent<SpriteRenderer>().material = green;
            indexes[7, i].GetComponent<TileYoneticisi>().SetDefaultColour(green);
        }
        indexes[13, 6].GetComponent<SpriteRenderer>().material = yellow;
        indexes[13, 6].GetComponent<TileYoneticisi>().SetDefaultColour(yellow);
        for (int i = 13; i >= 8; i--)
        {
            indexes[i, 7].GetComponent<SpriteRenderer>().material = yellow;
            indexes[i, 7].GetComponent<TileYoneticisi>().SetDefaultColour(yellow);
        }
    }

    /// <summary>
    /// Baþlangýç kalelerindeki iç beyaz kareyi çizer
    /// </summary>
    private void setWhiteAroundFortress()
    {
        for (int i = 1; i <= 4; i++)
        {
            indexes[i, 1].GetComponent<SpriteRenderer>().material = white;
            indexes[i + 9, 1].GetComponent<SpriteRenderer>().material = white;
            indexes[i + 9, 4].GetComponent<SpriteRenderer>().material = white;
            indexes[i, 4].GetComponent<SpriteRenderer>().material = white;
            indexes[i, 10].GetComponent<SpriteRenderer>().material = white;
            indexes[i, 13].GetComponent<SpriteRenderer>().material = white;
            indexes[i + 9, 10].GetComponent<SpriteRenderer>().material = white;
            indexes[i + 9, 13].GetComponent<SpriteRenderer>().material = white;
            indexes[1, i + 9].GetComponent<SpriteRenderer>().material = white;
            indexes[4, i + 9].GetComponent<SpriteRenderer>().material = white;
            indexes[1, i].GetComponent<SpriteRenderer>().material = white;
            indexes[10, i].GetComponent<SpriteRenderer>().material = white;
            indexes[4, i].GetComponent<SpriteRenderer>().material = white;
            indexes[13, i].GetComponent<SpriteRenderer>().material = white;
            indexes[10, i + 9].GetComponent<SpriteRenderer>().material = white;
            indexes[13, i + 9].GetComponent<SpriteRenderer>().material = white;
        }
    }

    /// <summary>
    /// Sýradaki takýma geçen metod
    /// </summary>
    private void IncrementTurn()
    {
        if (switchTurn)
        {
            turnIndex++;
            if (turnIndex >= turnTeams.Length)
            {
                turnIndex = 0;
            }
            teamTurnCounter = turnTeams[turnIndex];
        }
    }
}