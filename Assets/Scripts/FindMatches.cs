using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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
                                // Add matched candies to currentMatches
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
                                // Add matched candies to currentMatches
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
}