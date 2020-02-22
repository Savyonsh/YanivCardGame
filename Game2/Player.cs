using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;


namespace Game2
{    
    class Player
    {
        readonly private List<Card> cards;
        readonly private List<Card> clubsCards;
        readonly private List<Card> diamondsCards;
        readonly private List<Card> heartsCards;
        readonly private List<Card> spadesCards;
        private Card lastCardPicked;        
        private Card replaceJokerOne, replaceJokerTwo;
        private byte numOfSeriaCards;
        readonly private byte playerNumber;

        public byte GetPlayerNumber() { return playerNumber; }
        public Player(byte number)
        {
            playerNumber = number;
            Score = 0;
            cards = new List<Card>();
            clubsCards = new List<Card>();
            diamondsCards = new List<Card>();
            heartsCards = new List<Card>();
            spadesCards = new List<Card>();
            numOfSeriaCards = 0;
            Joker = 0;
        }

        public int Score { set; get; }

        public List<Card> GetCards() { return cards; }

        public void SetCards(List<Card> cards)
        {
            foreach (Card card in cards)
            {
                this.cards.Add(card);
                card.CardState = CardState.NONE;
                switch(card.Shape)
                {
                    case Shapes.CLUBS:
                        clubsCards.Add(card);
                        break;
                    case Shapes.DIAMONDS:
                        diamondsCards.Add(card);
                        break;
                    case Shapes.HEARTS:
                        heartsCards.Add(card);
                        break;
                    case Shapes.SPADES:
                        spadesCards.Add(card);
                        break;
                    case Shapes.JOKER:
                        Joker++;
                        break;
                    default:
                        break;
                }
                Card[] arrayCards = this.cards.ToArray();
                int middle = (arrayCards.Length - 1) / 2;
                Vector2 temp = arrayCards[middle].Vector;
                arrayCards[middle].Vector = arrayCards[middle * 2].Vector;
                arrayCards[middle * 2].Vector = temp;
            }
        }

        public int CardSum()
        {
            int sum = 0;
            foreach (Card card in cards)
            {
                if (card.Shape == Shapes.JOKER) continue;
                if (card.Value % 13 < 11) sum += (card.Value % 13 + 1);
                else sum += 10;
            }
            return sum;
        }

        public void PickCard(Card card)
        {
            if (card.Picked)
            {
                card.Picked = false;
                if (numOfSeriaCards > 0)
                {
                    lastCardPicked.Value--;
                    numOfSeriaCards--;
                }
                else lastCardPicked = null;
                if (card.Shape == Shapes.JOKER)
                {
                    replaceJokerOne = null;
                    replaceJokerTwo = null;
                }                
                card.CardState = CardState.PUT_DOWN;
            }
            else
            {
                /* Pick the card if
                    - This is the first card picked
                    - Last card that was picked has the same value 
                    - Last card that was picked has the same shape and smaller by one */
                // If this card is a Joker, use it as the missing card for seria.                

                if (lastCardPicked != null)
                {
                    if (lastCardPicked.Shape == Shapes.JOKER)
                    {
                        lastCardPicked.Value = (byte)(card.Value - 1);
                        lastCardPicked.Shape = card.Shape;
                        if (replaceJokerOne == null)
                            replaceJokerOne = lastCardPicked;
                        else replaceJokerTwo = lastCardPicked;
                    }
                    if (card.Shape == Shapes.JOKER)
                    {
                        lastCardPicked.Value += 1;                        
                        if (replaceJokerOne == null)
                            replaceJokerOne = lastCardPicked;
                        else replaceJokerTwo = lastCardPicked;
                        numOfSeriaCards++;
                        card.Picked = true;
                        card.CardState = CardState.LIFT;                        
                        return;
                    }
                    if (lastCardPicked.Value != card.Value)
                    {
                        if (lastCardPicked.Value + 1 != card.Value ||
                            ((replaceJokerOne == null || replaceJokerTwo == null)
                            ^ lastCardPicked.Value + 2 != card.Value) ||
                            lastCardPicked.Shape != card.Shape)
                        {                            
                            return;
                        }
                        else numOfSeriaCards++;

                    }
                }
                card.Picked = true;                
                card.CardState = CardState.LIFT;
                lastCardPicked = new Card(card);               
            }
        }
        public void ResetPlayer()
        {
            cards.Clear();
            clubsCards.Clear();
            diamondsCards.Clear();
            heartsCards.Clear();
            spadesCards.Clear();
            lastCardPicked = null;
            numOfSeriaCards = 0;
            Joker = 0;
            replaceJokerOne = null;
            replaceJokerTwo = null;
    }

        public List<Card> Play(Card newCard)
        {
            List<Card> thrownCards = new List<Card>();
            List<Card> oldCards = new List<Card>(cards);
            if (numOfSeriaCards == 1)
            {
                foreach (Card card in cards)
                {
                    if (card.Picked)
                    {                        
                        card.CardState = CardState.PUT_DOWN;
                        card.Picked = false;
                    }
                }
                numOfSeriaCards = 0;
                lastCardPicked = null;
                return null;
            }
            foreach (Card card in oldCards)
            {
                if (card.Picked)
                {
                    if (card.Shape == Shapes.JOKER) Joker--;
                    thrownCards.Add(card);
                    card.Picked = false;
                    card.CardState = CardState.NONE;
                    cards.Remove(card);
                    clubsCards.Remove(card);
                    heartsCards.Remove(card);
                    diamondsCards.Remove(card);
                    spadesCards.Remove(card);                    
                } 
            }
            List<Card> newCardList = new List<Card>{ newCard };
            SetCards(newCardList);
            thrownCards.Sort();
            if (replaceJokerOne != null)
            {
                Card joker1 = thrownCards[0];
                Card joker2 = null;
                byte jokerValue;
                if (joker1.Shape != Shapes.JOKER)
                {
                    if (replaceJokerTwo == null) joker1 = thrownCards[1];
                    else
                    {
                        joker1 = thrownCards[2];
                        joker2 = thrownCards[1];
                    }
                }
                jokerValue = joker1.Value;                               
                joker1.Value = replaceJokerOne.Value;
                if (joker2 != null) joker2.Value = (byte)(joker1.Value + 1);
                thrownCards.Sort();
                joker1.Value = jokerValue;
                if (joker2 != null) joker2.Value = (byte)(joker1.Value ^ 1);
                replaceJokerOne = null;
                replaceJokerTwo = null;
            }
            numOfSeriaCards = 0;
            lastCardPicked = null;
            return thrownCards;
        }
        public List<Card> GetClubCards() { return clubsCards; }
        public List<Card> GetDiamondsCards() { return diamondsCards; }
        public List<Card> GetHeartsCards() { return heartsCards; }
        public List<Card> GetSpadesCards() { return spadesCards; }

        public void FixList(Card[] cards)
        {
            this.cards.Clear();
            this.cards.AddRange(cards);
        }
        public void PickJokerUp(Card card)
        {
            foreach (Card curr in cards)
            {
                if(curr.Shape == Shapes.JOKER)
                {
                    curr.Picked = true;
                    if (replaceJokerOne == null)
                    {
                        replaceJokerOne = new Card(card);
                        replaceJokerOne.Value += 1;
                    }else
                    {
                        replaceJokerTwo = new Card(card);
                        replaceJokerTwo.Value += 1;
                    }
                    return;
                }
            }
        }
        
        public void ScorePlayer(bool palenty)
        {
            Score += CardSum();
            if (palenty) Score += 30;
            if (Score == 50) Score = 0; 
            else if (Score % 50 == 0) Score /= 2;
        }
        public byte Joker { get; set; }
    }
    
}
