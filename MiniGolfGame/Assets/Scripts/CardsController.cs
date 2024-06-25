using NUnit.Framework.Internal.Commands;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


/**
 *  A cards controller class. It controls the power-up and hindrances cards in the game.
 */
public class CardsController : MonoBehaviour
{
    /**
     * A private GameObject variable to reference a card.
     */
    GameObject card1;

    /**
     * A private GameObject variable to reference a card.
     */
    GameObject card2;

    /**
     * A private GameObject variable to reference a card.
     */
    GameObject card3;

    /**
     * A public Sprite object to reference and store card icons.
     */
    public Sprite[] cardIcons = new Sprite[5];

    /**
     * A public Sprite object to store texts from cards.
     */
    public string[] cardTexts = new string[5];

    /**
     * A private list of integers to reference selected cards
     */
    private List<int> selectedCards = new List<int>();

    /**
     * A public Sprite object to reference used cards
     */
    private List<int> usedCards = new List<int>();

    /**
    * A method called when the script instance is being loaded
    */
    private void Awake()
    {
        card1 = GameObject.Find("Card1");
        card2 = GameObject.Find("Card2");
        card3 = GameObject.Find("Card3");
    }

    /**
    * A member function called at the start of the scene.
    */
    void Start()
    {
        gameObject.SetActive(false);
        card1.SetActive(false);
        card2.SetActive(false);
        card3.SetActive(false);
    }

    /**
    * A member function called every frame.
    */
    void Update()
    {

    }

    /**
    * A public member function for showing power-up and hindrance cards in the game UI.
    */
    public void ShowCards()
    {
        if(GameManager.Instance.cardsAreBlocked()) { return; }

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

    /**
    * A public member function for hiding power-up and hindrance cards in the game UI.
    */
    public void HideCards()
    {
        gameObject.SetActive(false);
        card1.SetActive(false);
        card2.SetActive(false);
        card3.SetActive(false);
        GameManager.Instance.FreezeInputs(true);
    }

    /**
    * A public member function for activating the first card after it was chosen by the player.
    */
    public void ActivateCard1()
    {
        ApplyCardEffect(selectedCards[0]);
        usedCards.Add(selectedCards[0]);
        GameManager.Instance.showCards = false;
        HideCards();
    }

    /**
    * A public member function for activating the second card after it was chosen by the player.
    */
    public void ActivateCard2()
    {
        ApplyCardEffect(selectedCards[1]);
        usedCards.Add(selectedCards[1]);
        GameManager.Instance.showCards = false;
        HideCards();
    }

    /**
    * A public member function for activating the third card after it was chosen by the player.
    */
    public void ActivateCard3()
    {
        ApplyCardEffect(selectedCards[2]);
        usedCards.Add(selectedCards[2]);
        GameManager.Instance.showCards = false;
        HideCards();
    }

    /**
    * A private member function for selecting random cards to be shown to the player
    */
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

    /**
    * A private member function for updating the cards as they appear.
    */
    private void UpdateCardUI()
    {
        if (selectedCards.Count > 0) UpdateCard(card1, selectedCards[0]);
        if (selectedCards.Count > 1) UpdateCard(card2, selectedCards[1]);
        if (selectedCards.Count > 2) UpdateCard(card3, selectedCards[2]);
    }

    /**
    * A private member function for changing the images and text on the cards.
    * @param card the GameObject object referencing a card
    * @param index the index of the card
    */
    private void UpdateCard(GameObject card, int index)
    {
        string cardNumber = card.name.Substring(card.name.Length - 1);
        card.transform.Find($"Card{cardNumber}Image").GetComponent<UnityEngine.UI.Image>().sprite = cardIcons[index];
        card.transform.Find($"Card{cardNumber}Text").GetComponent<TextMeshProUGUI>().text = cardTexts[index];
    }

    /**
    * A private member function for applying the cards' effect in the game.
    * @param index the index of the card
    */
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
        }
    }

    /**
    * A private member function for resetting the cards that were used
    */
    public void ResetUsedCards()
    {
        usedCards.Clear();
    }

    /**
    * A private member function for applying Invisible Arrow effect via Game Manager
    */
    public void InvisibleArrowCard()
    {
        GameManager.Instance.HideArrow();
    }

    /**
    * A private member function for applying Power Shot effect via Game Manager
    */
    public void PowerShotCard()
    {
        GameManager.Instance.BoostBallPower();
    }

    /**
    * A private member function for applying Inverse Controls effect via Game Manager
    */
    public void InverseControlsCard()
    {
        GameManager.Instance.InverseControls();
    }

    /**
    * A private member function for applying Fog effect via Game Manager
    */
    public void FogCard()
    {
        GameManager.Instance.EnvironmentFog(true);
    }

    /**
    * A private member function for applying Magnet effect via Game Manager
    */
    public void MagnetBallCard()
    {
        GameManager.Instance.MagnetForce();
    }
}
