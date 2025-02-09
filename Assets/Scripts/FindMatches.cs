using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();
    void Start()
    {
        board = FindFirstObjectByType<Board>();
    }
    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }
    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentCandy = board.allCandies[i, j];
                if (currentCandy != null)
                {
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftCandy = board.allCandies[i - 1, j];
                        GameObject rightCandy = board.allCandies[i + 1, j];

                        if(leftCandy != null && rightCandy != null)
                        {
                            if (leftCandy.CompareTag(currentCandy.tag)&&rightCandy.CompareTag(currentCandy.tag)){
                                if (!currentMatches.Contains(leftCandy))
                                {
                                    currentMatches.Add(leftCandy);  
                                }
                                leftCandy.GetComponent<Candy>().isMatched = true;
                                if (!currentMatches.Contains(rightCandy))
                                {
                                    currentMatches.Add(rightCandy);
                                }
                                rightCandy.GetComponent<Candy>().isMatched = true;
                                if (!currentMatches.Contains(currentCandy))
                                {
                                    currentMatches.Add(currentCandy);
                                }
                                currentCandy.GetComponent<Candy>().isMatched = true;
                            }
                        }
                    }
                    if (j > 0 && j < board.height- 1)
                    {
                        GameObject upCandy = board.allCandies[i, j+1];
                        GameObject bottomCandy = board.allCandies[i, j-1];

                        if (upCandy != null && bottomCandy != null)
                        {
                            if (upCandy.CompareTag(currentCandy.tag) && bottomCandy.CompareTag(currentCandy.tag))
                            {
                                if (!currentMatches.Contains(upCandy))
                                {
                                    currentMatches.Add(upCandy);
                                }
                                upCandy.GetComponent<Candy>().isMatched = true;
                                if (!currentMatches.Contains(bottomCandy))
                                {
                                    currentMatches.Add(bottomCandy);
                                }
                                bottomCandy.GetComponent<Candy>().isMatched = true;
                                if (!currentMatches.Contains(currentCandy))
                                {
                                    currentMatches.Add(currentCandy);
                                }
                                currentCandy.GetComponent<Candy>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }

        // Update is called once per frame

    }
}
