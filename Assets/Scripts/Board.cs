using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

//add a new state to check for player turn change(need to figure out how to do it)
//setup an event for OnBoardChange(changeType),invoke different things depending on changetype?

public enum ChangeType
{
    destroy,
    refill,
    reshuffle
}
public enum GameState{
    wait,
    move
     
}
public class Board : NetworkBehaviour
{

    public GameState state = GameState.move;
    public int width;
    public int height;
    public int offSet;

    public SpriteRenderer tilePrefab;
    public Sprite darkBackground;
    public Sprite lightBackground;
    private BackgroundTile[,] allTiles;

    public GameObject[] candies;
    public GameObject[,] allCandies;
    public Candy currentCandy;

    public GameObject DestroyEffect;
    private FindMatches findMatches;

    //setup only when both players join, dont forget network behavior to various scripts

    //public override void OnNetworkSpawn()
    //{
    //    base.OnNetworkSpawn();

    //    // Only the server (host) runs Board setup
    //    if (IsServer)
    //    {
    //        SetUp();
    //    }
    //}
    public static Board Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }
    void Start()
    {
        findMatches = FindMatches.Instance;
        allTiles=new BackgroundTile[width,height]; //client side tile generation
        allCandies=new GameObject[width,height]; //make this a network variable and have it modified throughout different scripts
        SetUp();
    }
    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition= new Vector2(i,j+offSet);
                Vector2 tilePosition= new Vector2(i,j);
                SpriteRenderer currentTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity); //split the tiles to be client side
                currentTile.sprite = ((i + j)%2==0) ? lightBackground : darkBackground;
                currentTile.transform.parent=this.transform;
                currentTile.name="("+i+','+j+")";

                int candyToUse = Random.Range(0, candies.Length);
                int maxIter =0;
                while (MatchesAt(i, j, candies[candyToUse])&&maxIter<100)
                {
                    maxIter++;
                    candyToUse=Random.Range(0,candies.Length);
                }
                maxIter = 0;
                GameObject candy = Instantiate(candies[candyToUse], tempPosition, Quaternion.identity);//split visual logic
                candy.GetComponent<Candy>().row = j;
                candy.GetComponent<Candy>().col = i;
                candy.transform.parent = this.transform; //network transform
                candy.name = "(" + i + ',' + j + ")";
                allCandies[i, j] = candy; // network var
            }
        }
        if (IsDeadLock())
        {
            Debug.Log("DEADLOCKED");
            ShuffleBoard();
        }
    }
    //checking matches for board creation
    private bool MatchesAt(int col,int row,GameObject piece)
    {
        if(col>1 && row > 1)
        {
            if (allCandies[col - 1, row].CompareTag(piece.tag)&& allCandies[col - 2, row].CompareTag(piece.tag)){
                return true;
            }
            if (allCandies[col, row-1].CompareTag(piece.tag) && allCandies[col, row-2].CompareTag(piece.tag))
            {
                return true;
            }
        }else if (col <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allCandies[col, row-1].CompareTag(piece.tag) && allCandies[col, row-2].CompareTag(piece.tag))
                {
                    return true;
                }
            }
            if (col > 1)
            {
                if (allCandies[col - 1, row].CompareTag(piece.tag) && allCandies[col - 2, row].CompareTag(piece.tag))
                {
                    return true;
                }
            }
        }
        return false;
    }
    //private bool ColumnOrRow()
    //{
    //    int numberHorizontal = 0;
    //    int numberVertical = 0;
    //    Candy firstPiece = findMatches.currentMatches[0].GetComponent<Candy>();
    //    if (firstPiece != null)
    //    {
    //        foreach (GameObject currentPiece in findMatches.currentMatches)
    //        {
    //            if (currentPiece.GetComponent<Candy>() != null)
    //            {
    //                if (currentPiece.GetComponent<Candy>().row == firstPiece.row)
    //                {
    //                    numberHorizontal++;
    //                }
    //                if (currentCandy.GetComponent<Candy>().col == firstPiece.col)
    //                {
    //                    numberVertical++;
    //                }
    //            }
    //        }
    //    }
    //    return (numberVertical == 5 || numberHorizontal == 5);
    //}
    //bomb creation
    private void CheckToMakeBombs()
    {
        if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count==7)
        {
            findMatches.CheckBombs();//figure how to do this across network or make it client side and result is sent
        }
        //if (findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8)
        //{
        //    if (ColumnOrRow())
        //    {
        //        // color bomb
        //        if (currentCandy != null)
        //        {
        //            if (currentCandy.isMatched)
        //            {
        //                if (!currentCandy.isColorBomb)
        //                {
        //                    currentCandy.isMatched = false;
        //                    currentCandy.MakeColorBomb();
        //                }
        //            }
        //            else
        //            {
        //                Candy otherCandy = currentCandy.otherCandy.GetComponent<Candy>();
        //                if (!otherCandy.isColorBomb)
        //                {
        //                    otherCandy.isMatched = false;
        //                    otherCandy.MakeColorBomb();
        //                }
        //            }
        //        }

        //        Debug.Log("color bommb");
        //    }
        //    else
        //    {
        //        Debug.Log("adjacent bomb");
        //        if (currentCandy != null)
        //        {
        //            if (currentCandy.isMatched)
        //            {
        //                if (!currentCandy.isAdjacentBomb)
        //                {
        //                    currentCandy.isMatched = false;
        //                    currentCandy.MakeAdjacentBomb();
        //                }
        //            }
        //            else
        //            {
        //                Candy otherCandy = currentCandy.otherCandy.GetComponent<Candy>();
        //                if (!otherCandy.isAdjacentBomb)
        //                {
        //                    otherCandy.isMatched = false;
        //                    otherCandy.MakeAdjacentBomb();
        //                }
        //            }
        //        }

        //    }
        //    findMatches.CheckBombs();
        //}
    }
    private void DestroyMatchesAt(int col , int row) {
        if (allCandies[col, row].GetComponent<Candy>().isMatched) {
            //How Many Elements are in the matched pieces list from findMatches?
            if(findMatches.currentMatches.Count>=4) {
                CheckToMakeBombs();
                    }
            GameObject particle = Instantiate(DestroyEffect, allCandies[col, row].transform.position, Quaternion.identity);
            Destroy(particle, .2f);
            Destroy(allCandies[col, row]);
            allCandies[col, row] = null;
        }
    }
    //destruction only server sided
    public void DestroyMatches()
    {
        if (allCandies != null)
        {
            for(int i = 0; i<width; i++)
            {
                for(int j = 0; j<height; j++)
                {
                    if (allCandies[i, j] != null)
                    {
                        DestroyMatchesAt(i, j);
                    }
                }
            }
        }
        findMatches.currentMatches.Clear();

        StartCoroutine(DecreaseRowCo());
    }
    //server side
    private IEnumerator DecreaseRowCo() {
        int nullCount = 0;
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allCandies[i, j] == null)
                {
                    nullCount++;
                }else if (nullCount > 0)
                {
                    allCandies[i, j].GetComponent<Candy>().row -= nullCount;
                    allCandies[i,j]=null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());

    }
    //event refill
    private void RefillBoard()
    {
        for (int i =0; i < width; i++)
        {
            for (int j = 0; j < height;j++)
            {
                if (allCandies[i, j] == null)
                {
                    Vector2 tempPos=new Vector2(i,j+offSet);
                    int candyToUse = Random.Range(0, candies.Length);
                    GameObject piece = Instantiate(candies[candyToUse], tempPos,Quaternion.identity);//visual serpartion here
                    allCandies[i, j] = piece;
                    piece.GetComponent<Candy>().row = j;
                    piece.GetComponent<Candy>().col = i;
                }
            }
        }
    }
    private bool MatchesOnBoard()
    {
        for (int i =0; i < width; i++)
        {
            for (int j=0; j<height; j++)
            {
                if (allCandies[i, j] != null)
                {
                    if (allCandies[i, j].GetComponent<Candy>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private IEnumerator FillBoardCo() {
        RefillBoard();
        yield return new WaitForSeconds(.5f);
        state=GameState.wait;

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        currentCandy = null;
        yield return new WaitForSeconds(.5f);
        if (IsDeadLock())
        {
            Debug.Log("DEADLOCKED");
            ShuffleBoard();
        }
        state = GameState.move;

    }
    private void SwitchPieces(int col, int row , Vector2 direction)
    {
        //take second piece in holder
        GameObject holder = allCandies[col + (int)direction.x, row + (int)direction.y] as GameObject;//switching location logic, split
        //witching the first candy to be second position
        allCandies[col+(int)direction.x, row+(int)direction.y] = allCandies[col,row];
        allCandies[col,row]= holder;
    }
    private bool CheckForMatches()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allCandies[i, j] != null)
                {
                    //make sure we are in the board
                    if (i < width - 2)
                    {
                        //check to the right and two over
                        if (allCandies[i + 1, j] != null && allCandies[i + 2, j] != null)
                        {
                            if (allCandies[i + 1, j].CompareTag(allCandies[i, j].tag) && allCandies[i + 2, j].CompareTag(allCandies[i, j].tag))
                            {
                                return true;
                            }
                        }
                    }
                }
                if (allCandies[i, j] != null)
                {
                    if (j < height - 2)
                    {
                        //check to the up and two over
                        if (allCandies[i, j + 1] != null && allCandies[i, j + 2] != null)
                        {
                            if (allCandies[i, j + 1].CompareTag(allCandies[i, j].tag) && allCandies[i, j + 2].CompareTag(allCandies[i, j].tag))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
    //for deadlock checking
    public bool SwitchAndCheck(int col, int row, Vector2 direction)
    {
        SwitchPieces(col, row, direction);
        if (CheckForMatches()){
            SwitchPieces(col, row, direction);
            return true;
        }
        SwitchPieces(col, row, direction);
        return false;
    }
    private bool IsDeadLock() {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++) {
                if (allCandies[i, j] != null)
                {
                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if(j<height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
    //shuffle event
    private void ShuffleBoard()
    {
        List<GameObject> newBoard = new List<GameObject>();
        //add all pieces to list
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandies[i, j] != null)
                {
                    newBoard.Add(allCandies[i, j]);
                }
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if (!blankSpaces[i, j])
                //{
                //random placement through getting a random piece
                int pieceToUse = Random.Range(0, newBoard.Count);

                int maxIter = 0;
                while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIter < 100)
                {
                    pieceToUse = Random.Range(0, newBoard.Count);
                    maxIter++;
                    pieceToUse = Random.Range(0, newBoard.Count);
                }
                maxIter = 0;
                Candy piece = newBoard[pieceToUse].GetComponent<Candy>();
                piece.col = i;
                piece.row = j;
                allCandies[i, j] = newBoard[pieceToUse];
                newBoard.Remove(newBoard[pieceToUse]);
                //}
            }
        }
        //check for deadlock
        if (IsDeadLock())
        {
            ShuffleBoard();
        }
    }
}
