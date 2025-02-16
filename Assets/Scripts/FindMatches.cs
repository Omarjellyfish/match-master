using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();
    public static FindMatches Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }
    void Start()
    {
        // Find the Board object in the scene
        board = Board.Instance;
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsAjacentBomb(Candy candy1, Candy candy2, Candy candy3)
    {
        List<GameObject> currentCandies = new List<GameObject>();
        if (candy1.isAdjacentBomb)
        {
            currentMatches = currentMatches.Union(GetAdjacentPieces(candy1.col,candy1.row)).ToList();
        }
        if (candy2.isAdjacentBomb)
        {
            currentMatches = currentMatches.Union(GetAdjacentPieces(candy2.col, candy2.row)).ToList();
        }
        if (candy3.isAdjacentBomb)
        {
            currentMatches = currentMatches.Union(GetAdjacentPieces(candy3.col, candy3.row)).ToList();
        }
        return currentCandies;
    }
    private List<GameObject> IsRowBomb(Candy candy1, Candy candy2, Candy candy3)
    {
        List<GameObject> currentCandies = new List<GameObject>();
        if (candy1.isRowBomb)
        {
            currentMatches = currentMatches.Union(GetRowPieces(candy1.row)).ToList();
        }
        if (candy2.isRowBomb)
        {
            currentMatches = currentMatches.Union(GetRowPieces(candy2.row)).ToList();
        }
        if (candy3.isRowBomb)
        {
            currentMatches = currentMatches.Union(GetRowPieces(candy3.row)).ToList();
        }
        return currentCandies;
    }
    private List<GameObject> IsColBomb(Candy candy1, Candy candy2, Candy candy3)
    {
        List<GameObject> currentCandies = new List<GameObject>();
        if (candy1.isColBomb)
        {
            currentMatches = currentMatches.Union(GetColPieces(candy1.col)).ToList();
        }
        if (candy2.isColBomb)
        {
            currentMatches = currentMatches.Union(GetColPieces(candy2.col)).ToList();
        }
        if (candy3.isColBomb)
        {
            currentMatches = currentMatches.Union(GetColPieces(candy3.col)).ToList();
        }
        return currentCandies;
    }
    //local to client, result sent via rpc
    private IEnumerator FindAllMatchesCo()
    {
        // Wait a bit before starting to find matches
        yield return new WaitForSeconds(.2f);

        // Loop through all candies on the board
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentCandy = board.allCandies[i, j];
                if (currentCandy != null)
                {
                    Candy currentCandyObject = currentCandy.GetComponent<Candy>();

                    // Check for horizontal matches
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftCandy = board.allCandies[i - 1, j];
                        GameObject rightCandy = board.allCandies[i + 1, j];

                        if (leftCandy != null && rightCandy != null)
                        {
                            Candy leftCandyObject = leftCandy.GetComponent<Candy>();
                            Candy rightCandyObject = rightCandy.GetComponent<Candy>();
                            if (leftCandy.CompareTag(currentCandy.tag) && rightCandy.CompareTag(currentCandy.tag))
                            {
                                //current matches should be a network var aswell>
                                currentMatches=currentMatches.Union(IsRowBomb(currentCandyObject, leftCandyObject, rightCandyObject)).ToList();
                                // Add matched candies to currentMatches, check for col bombs in the row(only the three candies in match)
                                currentMatches=currentMatches.Union(IsColBomb(currentCandyObject,leftCandyObject,rightCandyObject)).ToList();
                                currentMatches = currentMatches.Union(IsAjacentBomb(currentCandyObject, leftCandyObject, rightCandyObject)).ToList();
                                AddMatches(leftCandy, currentCandy, rightCandy);
                            }
                        }
                    }

                    // Check for vertical matches
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upCandy = board.allCandies[i, j + 1];

                        GameObject downCandy = board.allCandies[i, j - 1];


                        if (upCandy != null && downCandy != null)
                        {
                            Candy downCandyObject = downCandy.GetComponent<Candy>();
                            Candy upCandyObject = upCandy.GetComponent<Candy>();
                            if (upCandy.CompareTag(currentCandy.tag) && downCandy.CompareTag(currentCandy.tag))
                            {

                                
                                // Add matched candies to currentMatches, check for row bombs in the col(only the three candies in match)

                                currentMatches=currentMatches.Union(IsRowBomb(currentCandyObject,upCandyObject, downCandyObject)).ToList();
                                currentMatches = currentMatches.Union(IsColBomb(currentCandyObject, upCandyObject, downCandyObject)).ToList();
                                currentMatches = currentMatches.Union(IsAjacentBomb(currentCandyObject, upCandyObject, downCandyObject)).ToList();

                                //if (currentCandy.GetComponent<Candy>().isRowBomb)
                                //{
                                //    currentMatches = currentMatches.Union(GetRowPieces(j)).ToList();
                                //}
                                //if (upCandy.GetComponent<Candy>().isRowBomb)
                                //{
                                //    currentMatches = currentMatches.Union(GetRowPieces(j + 1)).ToList();
                                //}
                                //if (downCandy.GetComponent<Candy>().isRowBomb)
                                //{
                                //    currentMatches = currentMatches.Union(GetRowPieces(j - 1)).ToList();
                                //}
                                AddMatches(upCandy, currentCandy, downCandy);
                            }
                        }
                    }
                }
            }
        }
    }

    private void AddMatches(params GameObject[] candies)
    {
        foreach (GameObject candy in candies)
        {
            if (candy != null)
            {
                if (!currentMatches.Contains(candy))
                {
                    currentMatches.Add(candy);
                }
                candy.GetComponent<Candy>().isMatched = true; // only send this via rpc to server to destroy matches
            }
        }
    }
    //specific for (color) bombs
    public void MatchPiecesOfColor(string color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allCandies[i, j] != null)
                {
                    if (board.allCandies[i, j].tag == color)
                    {
                        board.allCandies[i, j].GetComponent<Candy>().isMatched = true; // send this to server via rpc

                    }
                }
            }
        }
    }
    List<GameObject> GetAdjacentPieces(int col,int row) {
        List<GameObject> candies = new List<GameObject>();
        for (int i = col-1; i<=col+1; i++)
        {
            for(int j = row-1; j<=row+1; j++)
            {
                //Check for piece inside the board
                if(i>=0 && i<board.width && j>=0 && j < board.height)
                {
                    candies.Add(board.allCandies[i, j]);
                    board.allCandies[i, j].GetComponent<Candy>().isMatched = true;    
                }
            }
        }
        return candies;
    }
    List<GameObject> GetColPieces(int col)
    {
        List<GameObject> candies = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.allCandies[col, i] != null)
            {
                candies.Add(board.allCandies[col, i]);
                board.allCandies[col, i].GetComponent<Candy>().isMatched = true;
            }
        }
        return candies;
    }

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> candies = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allCandies[i, row] != null)
            {
                candies.Add(board.allCandies[i, row]);
                board.allCandies[i, row].GetComponent<Candy>().isMatched = true;
            }
        }
        return candies;
    }

    public void CheckBombs()
    {
        // Check if a candy was moved
        if (board.currentCandy != null)
        {
            // Reference to the other candy involved in the swap
            GameObject otherCandyObject = board.currentCandy.otherCandy;

            // Check if the current candy is matched
            if (board.currentCandy.isMatched)
            {
                // Unmatch it
                board.currentCandy.isMatched = false;

                // Decide bomb kind
                if ((board.currentCandy.swipeAngle > -45 && board.currentCandy.swipeAngle <= 45) ||
                    (board.currentCandy.swipeAngle < -135 || board.currentCandy.swipeAngle >= 135)
                    )
                {
                    //l-r swipe right bomb
                    board.currentCandy.MakeRowBomb();
                }
                else
                {
                    board.currentCandy.MakeColBomb();
                }
            }
            // Else, check if the other candy is matched
            else if (otherCandyObject != null)
            {
                Candy otherCandy = otherCandyObject.GetComponent<Candy>();
                if (otherCandy != null && otherCandy.isMatched)
                {
                    // Unmatch it
                    otherCandy.isMatched = false;

                    // Decide bomb 
                    if ((board.currentCandy.swipeAngle > -45 && board.currentCandy.swipeAngle <= 45) ||
                    (board.currentCandy.swipeAngle < -135 || board.currentCandy.swipeAngle >= 135)
                    )
                    {
                        //l-r swipe right bomb
                        otherCandy.MakeRowBomb();
                    }
                    else
                    {
                        otherCandy.MakeColBomb();
                    }
                }
            }
        }
    }
}
//int typeOfBomb = Random.Range(0, 100);
//if (typeOfBomb < 50)
//{
//    // Make a row bomb on the other candy
//    otherCandy.MakeRowBomb();
//}
//else
//{
//    // Make a column bomb on the other candy
//    otherCandy.MakeColBomb();
//}