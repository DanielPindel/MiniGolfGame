using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsController : MonoBehaviour
{
    GameObject card1;
    GameObject card2;
    GameObject card3;

    private void Awake()
    {
        card1 = GameObject.Find("Card1");
        //card2 = GameObject.Find("Card2");
        //card3 = GameObject.Find("Card3");
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        card1.SetActive(false);
        //card2.SetActive(false);
        //card3.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowCards()
    {
        gameObject.SetActive(true);
        card1.SetActive(true);
        //card2.SetActive(true);
        //card3.SetActive(true);
    }

    public void HideCards()
    {
        gameObject.SetActive(false);
        card1.SetActive(false);
        //card2.SetActive(false);
        //card3.SetActive(false);
    }

    public void InvisibleArrowCard()
    {
        GameManager.Instance.HideArrow();
        GameManager.Instance.showCards = false;
        HideCards();
    }

    public void PowerShotCard()
    {
        GameManager.Instance.BoostBallPower();
        GameManager.Instance.showCards = false;
        HideCards();
    }    
}
