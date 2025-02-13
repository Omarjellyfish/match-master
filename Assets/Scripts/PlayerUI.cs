using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject hostPlayerIndicator;
    [SerializeField] private GameObject playerTwoIndicator;
    [SerializeField] private GameObject playerTwoYouText;
    [SerializeField] private GameObject hostYouText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        hostPlayerIndicator.SetActive(false);
        playerTwoIndicator.SetActive(false);
        playerTwoYouText.SetActive(false);
        hostYouText.SetActive(false);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
