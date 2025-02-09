using JetBrains.Annotations;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum GameState{
    wait,
    move
     
}
public class Board : MonoBehaviour
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
    private FindMatches findMatches;

    void Start()
    {
        findMatches = FindFirstObjectByType<FindMatches>();
        allTiles=new BackgroundTile[width,height];
        allCandies=new GameObject[width,height];
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
                SpriteRenderer currentTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
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
                GameObject candy = Instantiate(candies[candyToUse], tempPosition, Quaternion.identity);
                candy.GetComponent<Candy>().row = j;
                candy.GetComponent<Candy>().col = i;
                candy.transform.parent = this.transform;
                candy.name = "(" + i + ',' + j + ")";
                allCandies[i, j] = candy;
            }
        }
    }
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
    private void DestroyMatchesAt(int col , int row) {
        if (allCandies[col, row].GetComponent<Candy>().isMatched) {
            findMatches.currentMatches.Remove(allCandies[col, row]);
            Destroy(allCandies[col, row]);
            allCandies[col, row] = null;
        }
    }
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
        StartCoroutine(DecreaseRowCo());
    }
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
        StartCoroutine(FillBoardcol());

    }
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
                    GameObject piece = Instantiate(candies[candyToUse], tempPos,Quaternion.identity);
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
    private IEnumerator FillBoardcol() {
        RefillBoard();
        yield return new WaitForSeconds(.5f);
        state=GameState.wait;

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(.5f);
        state = GameState.move;

    }
}
