using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    private Board board;
    public float hintDelay;
    private float hintDelaySeconds;
    public GameObject hintParticle;
    public GameObject currentHint;
    void Start()
    {
        board = Board.Instance;
        hintDelaySeconds =hintDelay;
    }

    void Update()
    {
        hintDelaySeconds-=Time.deltaTime;
        Debug.Log(hintDelaySeconds);
        if (hintDelaySeconds <= 0 && currentHint == null)
        {
            MarkHint();
            hintDelaySeconds = hintDelay;
        }
    }
    // i want to find all possible matches on the board then pick one to hint to
    // and then destroy the hint.
    List <GameObject> FindAllMatches()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allCandies[i, j] != null)
                {
                    if (i < board.width - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.right))
                        {
                            possibleMoves.Add(board.allCandies[i, j]);  
                        }
                    }
                    if (j < board.height - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.up))
                        {
                            possibleMoves.Add(board.allCandies[i,j]);
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }
    GameObject PickOneRandom() {
        List<GameObject>possibleMoves=new List<GameObject>();
        possibleMoves=FindAllMatches();
        if (possibleMoves.Count > 0)
        {
            int pieceToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pieceToUse];
        }
        return null;
    }
    //hind behind match
    private void MarkHint()
    {
        GameObject move= PickOneRandom();
        if(move != null)
        {
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
        }
    }
    public void DestroyHint()
    {
        if(currentHint != null)
        {
            Destroy(currentHint);
            currentHint = null;
            hintDelaySeconds = hintDelay;
        }
    }
}
