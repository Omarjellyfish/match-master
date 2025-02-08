using JetBrains.Annotations;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public SpriteRenderer tilePrefab;
    public Sprite darkBackground;
    public Sprite lightBackground;
    private BackgroundTile[,] allTiles;
    public GameObject[] candies;
    public GameObject[,] allCandies; 
    void Start()
    {
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
                Vector2 tempPosition= new Vector2(i,j);
                SpriteRenderer currentTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
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

            }
}
