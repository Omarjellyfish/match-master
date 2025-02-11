using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    void Start()
    {
        // Find the Board object in the scene
        board = FindFirstObjectByType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

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
                    // Check for horizontal matches
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftCandy = board.allCandies[i - 1, j];
                        GameObject rightCandy = board.allCandies[i + 1, j];

                        if (leftCandy != null && rightCandy != null)
                        {
                            if (leftCandy.CompareTag(currentCandy.tag) && rightCandy.CompareTag(currentCandy.tag))
                            {
                                if (currentCandy.GetComponent<Candy>().isRowBomb
                                    || leftCandy.GetComponent<Candy>().isRowBomb
                                    || rightCandy.GetComponent<Candy>().isRowBomb)
                                {
                                    currentMatches = currentMatches.Union(GetRowPieces(j)).ToList();
                                }
                                // Add matched candies to currentMatches, check for col bombs in the row(only the three candies in match)
                                if (currentCandy.GetComponent<Candy>().isColBomb)
                                {
                                    currentMatches = currentMatches.Union(GetColPieces(i)).ToList();
                                }
                                if (leftCandy.GetComponent<Candy>().isColBomb)
                                {
                                    currentMatches = currentMatches.Union(GetColPieces(i - 1)).ToList();
                                }
                                if (rightCandy.GetComponent<Candy>().isColBomb)
                                {
                                    currentMatches = currentMatches.Union(GetColPieces(i + 1)).ToList();
                                }
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
                            if (upCandy.CompareTag(currentCandy.tag) && downCandy.CompareTag(currentCandy.tag))
                            {

                                if (currentCandy.GetComponent<Candy>().isColBomb
                                    || upCandy.GetComponent<Candy>().isColBomb
                                    || downCandy.GetComponent<Candy>().isColBomb)
                                {
                                    currentMatches = currentMatches.Union(GetColPieces(i)).ToList();
                                }
                                // Add matched candies to currentMatches, check for row bombs in the col(only the three candies in match)
                                if (currentCandy.GetComponent<Candy>().isRowBomb)
                                {
                                    currentMatches = currentMatches.Union(GetRowPieces(j)).ToList();
                                }
                                if (upCandy.GetComponent<Candy>().isRowBomb)
                                {
                                    currentMatches = currentMatches.Union(GetRowPieces(j + 1)).ToList();
                                }
                                if (downCandy.GetComponent<Candy>().isRowBomb)
                                {
                                    currentMatches = currentMatches.Union(GetRowPieces(j - 1)).ToList();
                                }
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
                candy.GetComponent<Candy>().isMatched = true;
            }
        }
    }
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
                        board.allCandies[i, j].GetComponent<Candy>().isMatched = true;

                    }
                }
            }
        }
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