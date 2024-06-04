using NUnit.Framework.Internal.Commands;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class CardsController : MonoBehaviour
{
    GameObject card1;
    GameObject card2;
    GameObject card3;

    public Sprite[] cardIcons = new Sprite[5];
    public string[] cardTexts = new string[5];

    private List<int> selectedCards = new List<int>();
    private List<int> usedCards = new List<int>();

    private void Awake()
    {
        card1 = GameObject.Find("Card1");
        card2 = GameObject.Find("Card2");
        card3 = GameObject.Find("Card3");
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        card1.SetActive(false);
        card2.SetActive(false);
        card3.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowCards()
    {
        SelectRandomCards();

        if(selectedCards.Count == 0)
        {
            HideCards();
            return;
        }

        UpdateCardUI();
        gameObject.SetActive(true);
        card1.SetActive(selectedCards.Count > 0);
        card2.SetActive(selectedCards.Count > 1);
        card3.SetActive(selectedCards.Count > 2);
        GameManager.Instance.FreezeInputs(false);
    }

    public void HideCards()
    {
        gameObject.SetActive(false);
        card1.SetActive(false);
        card2.SetActive(false);
        card3.SetActive(false);
        GameManager.Instance.FreezeInputs(true);
    }

    public void ActivateCard1()
    {
        ApplyCardEffect(selectedCards[0]);
        usedCards.Add(selectedCards[0]);
        GameManager.Instance.showCards = false;
        HideCards();
    }

    public void ActivateCard2()
    {
        ApplyCardEffect(selectedCards[1]);
        usedCards.Add(selectedCards[1]);
        GameManager.Instance.showCards = false;
        HideCards();
    }

    public void ActivateCard3()
    {
        ApplyCardEffect(selectedCards[2]);
        usedCards.Add(selectedCards[2]);
        GameManager.Instance.showCards = false;
        HideCards();
    }

    private void SelectRandomCards()
    {
        selectedCards.Clear();
        List<int> availableCards = new List<int> { 0, 1, 2, 3, 4 };

        foreach (int usedIndex in usedCards)
        {
            availableCards.Remove(usedIndex);
        }

        if(availableCards.Count == 0)
        {
            return;
        }

        int cardsToSelect = Mathf.Min(3, availableCards.Count);

        for (int i = 0; i < cardsToSelect; i++)
        {
            int randomIndex = Random.Range(0, availableCards.Count);
            selectedCards.Add(availableCards[randomIndex]);
            availableCards.RemoveAt(randomIndex);
        }
    }

    private void UpdateCardUI()
    {
        if (selectedCards.Count > 0) UpdateCard(card1, selectedCards[0]);
        if (selectedCards.Count > 1) UpdateCard(card2, selectedCards[1]);
        if (selectedCards.Count > 2) UpdateCard(card3, selectedCards[2]);
    }

    private void UpdateCard(GameObject card, int index)
    {
        string cardNumber = card.name.Substring(card.name.Length - 1);
        card.transform.Find($"Card{cardNumber}Image").GetComponent<UnityEngine.UI.Image>().sprite = cardIcons[index];
        card.transform.Find($"Card{cardNumber}Text").GetComponent<TextMeshProUGUI>().text = cardTexts[index];
    }

    private void ApplyCardEffect(int index)
    {
        switch (index)
        {
            case 0:
                InvisibleArrowCard();
                break;
            case 1:
                PowerShotCard();
                break;
            case 2:
                InverseControlsCard();
                break;
            case 3:
                FogCard();
                break;
            case 4:
                MagnetBallCard();
                break;
            //case 5:
            //    SpinningArrowCard();
            //    break;
        }
    }

    public void ResetUsedCards()
    {
        usedCards.Clear();
    }

    public void InvisibleArrowCard()
    {
        GameManager.Instance.HideArrow();
    }

    public void PowerShotCard()
    {
        GameManager.Instance.BoostBallPower();
    }    

    public void InverseControlsCard()
    {
        GameManager.Instance.InverseControls();
    }

    public void FogCard()
    {
        GameManager.Instance.EnvironmentFog(true);
    }

    public void MagnetBallCard()
    {
        GameManager.Instance.MagnetForce();
    }

    public void SpinningArrowCard()
    {
        GameManager.Instance.SpinArrow();
    }
}
