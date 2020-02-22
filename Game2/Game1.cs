using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Game2
{
    public enum CardDrawing { DECK, LEFT, RIGHT, NONE }

    public enum CardState { LIFT, PUT_DOWN, HOVER_UP, HOVER_DOWN, NONE}
    public class Game1 : Game
    {
        private struct GameTextureProperties
        {
            public Texture2D texture;
            public Vector2 vector;
            public Rectangle rectangle;
        }
        private struct GameTextures
        {
            public SpriteFont sumOfCardsFont;
            public SpriteFont scoreFont;
            public SpriteFont scoreTitle;

            public GameTextureProperties background,
                                         backgroundEmpty,                                         
                                         startupScreen,
                                         chooseButtonSmall,
                                         chooseButtonBig,
                                         deck,
                                         player1Title,
                                         player2Title,
                                         yaniv,
                                         youWon,
                                         again,
                                         cantTakeCard,
                                         cardLeftPlayer,
                                         callIt,
                                         tookCard,
                                         sortButton, 
                                         throwCardSign;


            public Texture2D player1TitleBold,
                             player2TitleBold,
                             assaf,
                             player1Won,
                             player2Won,
                             cardRightPlayer;

            public Vector2[] player1CardsVectors, player2CardsVectors;

            public Vector2 leftScoreVector,
                            RightScoreVector,
                            MyScoreVector,
                            sumCardVector,
                            rotationVector; 

        }

        readonly byte[] deck;
        readonly GraphicsDeviceManager graphics;
        readonly Player player1, player2, me;
        readonly List<Card> tableCards;
        readonly Random random;
        float currentTime;
        CardDrawing currentCardDrawing;
        CardDrawing player1CardDrawing;
        CardDrawing player2CardDrawing;
        bool sort, startGame, sameCardThrow;

        int j = 0;
        int i = 0;
        byte numOfPlayers, roundNumber;
        float rotation = 0;

        bool assaf;
        byte randomIndex, shape, value, winner, turnCounter;
        GameTextures gameTextures;
        MouseState mouseCurrent;
        MouseState mousePrevious;
        List<Card> thrownCards;
        Card firstTableCard, lastTableCard, deckCard;
        SpriteBatch spriteBatch;
        readonly Player[] players;

        SoundEffect cardShuffle;
        SoundEffect cardBeingThrown;
        bool openingCardShufflePlayed = false;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 700;
            graphics.PreferredBackBufferHeight = 700;
            graphics.ApplyChanges();
            this.IsMouseVisible = true;
            deck = new byte[54];
            player1 = new Player(1);
            player2 = new Player(2);
            me = new Player(0);
            players = new Player[3];
            players[0] = me;
            players[1] = player1;
            players[2] = player2;
            random = new Random();
            tableCards = new List<Card>();
            thrownCards = new List<Card>();
            gameTextures.player1CardsVectors = new Vector2[7];
            gameTextures.player2CardsVectors = new Vector2[7];
            startGame = false;
            roundNumber = 0;
            numOfPlayers = 0;
        }

        protected override void Initialize()
        {            
            randomIndex = 53;            
            tableCards.Clear();
            thrownCards.Clear();
            for (byte i = 0; i < 54; i++) deck[i] = i;
            Deal(me);
            Deal(player1);
            if (numOfPlayers > 2) Deal(player2);
            PlaceTableCard();
            PlaceCardsOnMat();
            if (winner != 4 && !startGame)
                turnCounter = winner;
            else turnCounter = 0;
            winner = 4;
            assaf = false;
            currentTime = 0f;
            currentCardDrawing = CardDrawing.NONE;
            player1CardDrawing = CardDrawing.NONE;
            player2CardDrawing = CardDrawing.NONE;
            i = 0;            
            sameCardThrow = false;
            if(numOfPlayers == 0) numOfPlayers = 3;
            openingCardShufflePlayed = false;
            base.Initialize();
        }

        private void PlaceTableCard()
        {
            firstTableCard = GenerateCard();
            firstTableCard.Vector.X = 310;
            firstTableCard.Vector.Y = 220;
            firstTableCard.SetRectangle();
            lastTableCard = firstTableCard;            
            tableCards.Add(firstTableCard);
        }

        private Card GenerateCard()
        {
            Card card;
            byte index = (byte)random.Next(0, randomIndex);
            shape = (byte)(deck[index] / 13);
            value = (byte)(deck[index] % 13);
            deck[index] = deck[randomIndex];
            card = new Card((Shapes)shape, value, this.Content.Load<Texture2D>("card"));
            card.SetRectangle();
            randomIndex--;
            return card;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gameTextures.sumOfCardsFont = Content.Load<SpriteFont>("amount");
            gameTextures.scoreFont = Content.Load<SpriteFont>("score");
            gameTextures.scoreTitle = Content.Load<SpriteFont>("scoreTitle");

            gameTextures.background.texture = Content.Load<Texture2D>("backgroundLight");
            gameTextures.background.rectangle = new Rectangle(0, 0, 700, 700);


            gameTextures.backgroundEmpty.texture = Content.Load<Texture2D>("backgroundEmpty");

            gameTextures.startupScreen.texture = Content.Load<Texture2D>("startupScreen");

            gameTextures.chooseButtonSmall.texture = Content.Load<Texture2D>("chooseCircleSmall");
            gameTextures.chooseButtonSmall.vector = new Vector2(433, 265);
            gameTextures.chooseButtonBig.texture = Content.Load<Texture2D>("chooseCircleSmall");
            gameTextures.chooseButtonBig.vector = new Vector2(387, 400);

            gameTextures.deck.texture = Content.Load<Texture2D>("deckRedBack");
            gameTextures.deck.vector = new Vector2(300, 60);
            gameTextures.deck.rectangle = new Rectangle(0, 0, 98, 122);

            deckCard = new Card(0, 0, Content.Load<Texture2D>("card"));

            gameTextures.player1Title.texture = Content.Load<Texture2D>("player1");
            gameTextures.player1Title.vector = new Vector2(10, 60);
            gameTextures.player1Title.rectangle = new Rectangle(0, 0, 164, 36);

            gameTextures.player2Title.texture = Content.Load<Texture2D>("player2");
            gameTextures.player2Title.vector = new Vector2(530, 60);

            gameTextures.player1TitleBold = Content.Load<Texture2D>("player1Turn");
            gameTextures.player2TitleBold = Content.Load<Texture2D>("player2Turn");

            gameTextures.throwCardSign.texture = Content.Load<Texture2D>("throwCard");
            gameTextures.throwCardSign.vector = new Vector2(235, 460);

            gameTextures.yaniv.texture = Content.Load<Texture2D>("yaniv");
            gameTextures.yaniv.vector = new Vector2(150, 200);
            gameTextures.yaniv.rectangle = new Rectangle(0, 0, 402, 121);

            gameTextures.assaf = Content.Load<Texture2D>("assaf");

            gameTextures.youWon.texture = Content.Load<Texture2D>("youWon");
            gameTextures.youWon.vector = new Vector2(200, 330);
            gameTextures.youWon.rectangle = new Rectangle(0, 0, 289, 70);

            gameTextures.player1Won = Content.Load<Texture2D>("player1Won");
            gameTextures.player2Won = Content.Load<Texture2D>("player2Won");

            gameTextures.again.texture = Content.Load<Texture2D>("again");
            gameTextures.again.vector = new Vector2(210, 520);
            gameTextures.again.rectangle = new Rectangle(0, 0, 283, 73);

            gameTextures.cantTakeCard.texture = Content.Load<Texture2D>("cantTakeCard");

            gameTextures.cantTakeCard.rectangle = new Rectangle(0, 0, 79, 123);

            gameTextures.cardLeftPlayer.texture = Content.Load<Texture2D>("cardLeftPlayer");
            for (int i = 0; i < 7; i++)
            {
                gameTextures.player1CardsVectors[i] = new Vector2(30, 165 + 25 * i);
                gameTextures.player2CardsVectors[i] = new Vector2(570, 165 + 25 * i);

            }
            gameTextures.cardLeftPlayer.rectangle = new Rectangle(0, 0, 111, 79);

            gameTextures.cardRightPlayer = Content.Load<Texture2D>("cardRightPlayer");

            gameTextures.MyScoreVector = new Vector2(500, 410);
            gameTextures.sumCardVector = new Vector2(330, 410);
            gameTextures.leftScoreVector = new Vector2(65, 100);
            gameTextures.RightScoreVector = new Vector2(620, 100);

            gameTextures.callIt.texture = Content.Load<Texture2D>("callIt");
            gameTextures.callIt.vector = new Vector2(140, 390);
            gameTextures.callIt.rectangle = new Rectangle(0, 0, 183, 64);

            gameTextures.tookCard.texture = Content.Load<Texture2D>("tookCard");
            gameTextures.tookCard.vector = gameTextures.deck.vector;
            gameTextures.tookCard.vector.Y = gameTextures.player1CardsVectors[0].Y + 50;
            gameTextures.tookCard.rectangle = new Rectangle(0, 0, 79, 111);

            gameTextures.sortButton.texture = Content.Load<Texture2D>("sort");
            gameTextures.sortButton.vector = new Vector2(312, 638);
            gameTextures.sortButton.rectangle = new Rectangle(0, 0, 76, 33);

            gameTextures.rotationVector = new Vector2(
                                          79 / 2,
                                          123 / 2);

            cardShuffle = Content.Load<SoundEffect>("CardsShuffle");
            cardBeingThrown = Content.Load<SoundEffect>("CardThrown");
           
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            if (startGame)
            {
                //  BACKGROUND   |   DECK    |   PLAYERS TITLES   //
                spriteBatch.Draw(gameTextures.background.texture, gameTextures.background.rectangle, Color.White);
                spriteBatch.Draw(gameTextures.deck.texture, gameTextures.deck.vector, gameTextures.deck.rectangle,
                                Color.White);
                spriteBatch.Draw(gameTextures.player1Title.texture, gameTextures.player1Title.vector,
                                gameTextures.player1Title.rectangle, Color.White);
                spriteBatch.Draw(gameTextures.player2Title.texture, gameTextures.player2Title.vector,
                                gameTextures.player1Title.rectangle, Color.White);

                // Highlighting a player's title on his turn
                if (turnCounter % numOfPlayers == 1)
                    spriteBatch.Draw(gameTextures.player1TitleBold, gameTextures.player1Title.vector,
                                    gameTextures.player1Title.rectangle, Color.White);
                else if (turnCounter % numOfPlayers == 2)
                    spriteBatch.Draw(gameTextures.player2TitleBold, gameTextures.player2Title.vector,
                                    gameTextures.player1Title.rectangle, Color.White);

                // SCORES //
                spriteBatch.DrawString(gameTextures.scoreTitle, "SCORE",
                    new Vector2(gameTextures.MyScoreVector.X - 15, gameTextures.MyScoreVector.Y + 25), Color.White);
                spriteBatch.DrawString(gameTextures.scoreTitle, "SCORE",
                    new Vector2(gameTextures.leftScoreVector.X - 15, gameTextures.leftScoreVector.Y + 25), Color.White);
                spriteBatch.DrawString(gameTextures.scoreTitle, "SCORE",
                    new Vector2(gameTextures.RightScoreVector.X - 15, gameTextures.RightScoreVector.Y + 25), Color.White);
                spriteBatch.DrawString(gameTextures.sumOfCardsFont, me.CardSum().ToString(),
                                       gameTextures.sumCardVector, Color.Black);
                spriteBatch.DrawString(gameTextures.scoreFont, me.Score.ToString(),
                                       gameTextures.MyScoreVector, Color.Black);
                spriteBatch.DrawString(gameTextures.scoreFont, player1.Score.ToString(),
                                       gameTextures.leftScoreVector, Color.Black);
                spriteBatch.DrawString(gameTextures.scoreFont, player2.Score.ToString(),
                                       gameTextures.RightScoreVector, Color.Black);

                if (me.CardSum() <= 7 && turnCounter % numOfPlayers == 0)
                    spriteBatch.Draw(gameTextures.callIt.texture, gameTextures.callIt.vector,
                                     gameTextures.callIt.rectangle, Color.White);

                //      CARD    PRINTING       //
                i = 0;
                foreach (Card tableCard in tableCards)
                {
                    spriteBatch.Draw(tableCard.Texture, tableCard.Vector, tableCard.Rectangle, Color.White);
                    if (i != 0 && i != tableCards.Count - 1)
                        spriteBatch.Draw(gameTextures.cantTakeCard.texture, tableCard.Vector,
                        gameTextures.cantTakeCard.rectangle, Color.White);
                    i++;
                }

                // Players cards (including animation)
                if (currentCardDrawing != CardDrawing.NONE)
                {
                    spriteBatch.Draw(deckCard.Texture, gameTextures.tookCard.vector,
                           gameTextures.tookCard.rectangle, Color.White);
                    if (gameTextures.tookCard.vector.Y + 14.5f < 500)
                        gameTextures.tookCard.vector.Y += 14.5f;
                    else
                    {
                        gameTextures.tookCard.vector.Y = 60;
                        currentCardDrawing = CardDrawing.NONE;
                    }
                    for (int j = 0; j < me.GetCards().Count - 1; j++)
                    {
                        spriteBatch.Draw(me.GetCards()[j].Texture, me.GetCards()[j].Vector,
                            me.GetCards()[j].Rectangle, Color.White);
                    }

                }
                else
                {
                    foreach (Card card in me.GetCards())
                    {
                        switch (card.CardState)
                        {
                            case CardState.LIFT:
                                if (card.Vector.Y > 450)
                                    card.Vector.Y -= 10;
                                break;
                            case CardState.PUT_DOWN:
                                if (card.Vector.Y < 500)
                                    card.Vector.Y += 10;
                                else card.CardState = CardState.NONE;
                                break;
                            case CardState.HOVER_UP:
                                if (card.Vector.Y > 490)
                                    card.Vector.Y -= 5;
                                break;
                            case CardState.HOVER_DOWN:
                                if (card.Vector.Y < 500)
                                    card.Vector.Y += 5;
                                else card.CardState = CardState.NONE;
                                break;
                        }

                        spriteBatch.Draw(card.Texture, card.Vector, card.Rectangle, Color.White);
                    }
                }

                switch (player1CardDrawing)
                {
                    case CardDrawing.DECK:
                        {
                            if (j < 30)
                            {
                                spriteBatch.Draw(gameTextures.tookCard.texture, gameTextures.tookCard.vector,
                                                 null, Color.White, rotation,
                                                 gameTextures.rotationVector, 1.0f, SpriteEffects.None, 1.0f);
                                rotation += 0.06f;
                                j++;
                            }
                            else
                            {
                                gameTextures.tookCard.vector.Y = gameTextures.player1CardsVectors[0].Y;
                                spriteBatch.Draw(gameTextures.cardLeftPlayer.texture, gameTextures.tookCard.vector,
                                                 null, Color.White);
                                if (gameTextures.tookCard.vector.X > gameTextures.player1CardsVectors[0].X)
                                    gameTextures.tookCard.vector.X -= 14.5f;
                                else
                                {
                                    gameTextures.tookCard.vector.X = gameTextures.deck.vector.X;
                                    player1CardDrawing = CardDrawing.NONE;
                                    rotation = 0;
                                    j = 0;
                                }
                            }
                            for (i = player1.GetCards().Count - 1; i > 0; i--)
                                spriteBatch.Draw(gameTextures.cardLeftPlayer.texture, gameTextures.player1CardsVectors[i],
                                                gameTextures.cardLeftPlayer.rectangle, Color.White);
                            break;
                        }
                    case CardDrawing.RIGHT:
                    case CardDrawing.LEFT:
                        {
                            if (j < 27)
                            {
                                spriteBatch.Draw(gameTextures.tookCard.texture, gameTextures.tookCard.vector,
                                                 gameTextures.tookCard.rectangle, Color.White, rotation,
                                                 gameTextures.rotationVector, 1.0f, SpriteEffects.None, 1.0f);
                                rotation += 0.06f;
                                j++;
                            }
                            else
                            if (gameTextures.tookCard.vector.X >
                                 gameTextures.player1CardsVectors[player1.GetCards().Count - 1].X)
                            {
                                if (gameTextures.tookCard.vector.X <= gameTextures.deck.vector.X -
                                    gameTextures.player1CardsVectors[player1.GetCards().Count - 1].X / 2)
                                {
                                    gameTextures.tookCard.texture = gameTextures.cardLeftPlayer.texture;
                                    gameTextures.tookCard.rectangle = gameTextures.cardLeftPlayer.rectangle;
                                    gameTextures.tookCard.vector.Y =
                                        gameTextures.player1CardsVectors[player1.GetCards().Count - 1].Y;
                                    spriteBatch.Draw(gameTextures.tookCard.texture, gameTextures.tookCard.vector,
                                        gameTextures.tookCard.rectangle, Color.White);
                                }
                                else
                                {
                                    spriteBatch.Draw(gameTextures.tookCard.texture, gameTextures.tookCard.vector,
                                    gameTextures.tookCard.rectangle, Color.White, 29.85f,
                                    gameTextures.rotationVector,
                                    1.0f, SpriteEffects.None, 1.0f);
                                }
                                gameTextures.tookCard.vector.X -= 14.5f;
                            }
                            else
                            {
                                gameTextures.tookCard.vector.X = gameTextures.deck.vector.X;
                                player1CardDrawing = CardDrawing.NONE;
                                j = 0;
                                rotation = 0;
                            }
                            for (i = player1.GetCards().Count - 2; i >= 0; i--)
                                spriteBatch.Draw(gameTextures.cardLeftPlayer.texture, gameTextures.player1CardsVectors[i],
                                                gameTextures.cardLeftPlayer.rectangle, Color.White);
                            break;
                        }
                    default:
                        {
                            for (i = player1.GetCards().Count - 1; i >= 0; i--)
                                spriteBatch.Draw(gameTextures.cardRightPlayer, gameTextures.player1CardsVectors[i],
                                                gameTextures.cardLeftPlayer.rectangle, Color.White);
                            break;
                        }
                }

                if (numOfPlayers == 3)
                {
                    switch (player2CardDrawing)
                    {
                        case CardDrawing.DECK:
                            {
                                if (j < 30)
                                {
                                    spriteBatch.Draw(gameTextures.tookCard.texture, gameTextures.tookCard.vector,
                                                     null, Color.White, rotation,
                                                     gameTextures.rotationVector, 1.0f, SpriteEffects.None, 1.0f);
                                    rotation -= 0.06f;
                                    j++;
                                }
                                else
                                {
                                    gameTextures.tookCard.vector.Y = gameTextures.player2CardsVectors[0].Y;
                                    spriteBatch.Draw(gameTextures.cardRightPlayer, gameTextures.tookCard.vector,
                                                     null, Color.White);
                                    if (gameTextures.tookCard.vector.X < gameTextures.player2CardsVectors[0].X)
                                        gameTextures.tookCard.vector.X += 14.5f;
                                    else
                                    {
                                        gameTextures.tookCard.vector.X = gameTextures.deck.vector.X;
                                        player2CardDrawing = CardDrawing.NONE;
                                        rotation = 0;
                                        j = 0;
                                    }
                                }
                                for (i = player2.GetCards().Count - 1; i > 0; i--)
                                    spriteBatch.Draw(gameTextures.cardRightPlayer, gameTextures.player2CardsVectors[i],
                                                    null, Color.White);
                                break;
                            }

                        case CardDrawing.LEFT:
                        case CardDrawing.RIGHT:
                            if (j < 27)
                            {
                                spriteBatch.Draw(gameTextures.tookCard.texture, gameTextures.tookCard.vector,
                                                 gameTextures.tookCard.rectangle, Color.White, rotation,
                                                 gameTextures.rotationVector, 1.0f, SpriteEffects.None, 1.0f);
                                rotation -= 0.06f;
                                j++;
                            }
                            else
                            {
                                if (gameTextures.tookCard.vector.X <
                                     gameTextures.player2CardsVectors[player2.GetCards().Count - 1].X)
                                {
                                    if (gameTextures.tookCard.vector.X >= 60 +
                                        gameTextures.player2CardsVectors[player2.GetCards().Count - 1].X / 2)
                                    {
                                        gameTextures.tookCard.texture = gameTextures.cardRightPlayer;
                                        gameTextures.tookCard.rectangle = gameTextures.cardLeftPlayer.rectangle;
                                        gameTextures.tookCard.vector.Y =
                                            gameTextures.player2CardsVectors[player2.GetCards().Count - 1].Y;
                                        spriteBatch.Draw(gameTextures.tookCard.texture, gameTextures.tookCard.vector,
                                            gameTextures.tookCard.rectangle, Color.White);
                                    }
                                    else
                                    {
                                        spriteBatch.Draw(gameTextures.tookCard.texture, gameTextures.tookCard.vector,
                                        gameTextures.tookCard.rectangle, Color.White, -29.85f,
                                        new Vector2(lastTableCard.Rectangle.Width / 2, lastTableCard.Rectangle.Height / 2),
                                        1.0f, SpriteEffects.None, 1.0f);
                                    }
                                    gameTextures.tookCard.vector.X += 14.5f;
                                }
                                else
                                {
                                    gameTextures.tookCard.vector.X = gameTextures.deck.vector.X;
                                    player2CardDrawing = CardDrawing.NONE;
                                    j = 0;
                                    rotation = 0;
                                }
                            }

                            for (i = player2.GetCards().Count - 2; i >= 0; i--)
                                spriteBatch.Draw(gameTextures.cardRightPlayer, gameTextures.player2CardsVectors[i],
                                                gameTextures.cardLeftPlayer.rectangle, Color.White);
                            break;

                        default:
                            {
                                for (i = player2.GetCards().Count - 1; i >= 0; i--)
                                    spriteBatch.Draw(gameTextures.cardRightPlayer, gameTextures.player2CardsVectors[i],
                                                    gameTextures.cardLeftPlayer.rectangle, Color.White);
                                break;
                            }
                    }
                }

                spriteBatch.Draw(gameTextures.sortButton.texture, gameTextures.sortButton.vector,
                                 gameTextures.sortButton.rectangle, Color.White);

                if(sameCardThrow)
                spriteBatch.Draw(gameTextures.throwCardSign.texture, gameTextures.throwCardSign.vector,
                    null, Color.White);


                // Winner Details display
                if (winner < 4)
                {
                    if (!assaf)
                    {
                        spriteBatch.Draw(gameTextures.yaniv.texture, gameTextures.yaniv.vector,
                                         gameTextures.yaniv.rectangle, Color.White);
                        switch (winner)
                        {
                            case 0:
                                spriteBatch.Draw(gameTextures.youWon.texture, gameTextures.youWon.vector,
                                                          gameTextures.youWon.rectangle, Color.White);
                                break;
                            case 1:
                                spriteBatch.Draw(gameTextures.player1Won, gameTextures.youWon.vector,
                                              gameTextures.youWon.rectangle, Color.White);
                                break;
                            case 2:
                                spriteBatch.Draw(gameTextures.player2Won, gameTextures.youWon.vector,
                                              gameTextures.youWon.rectangle, Color.White);
                                break;

                        }
                    }
                    else
                    {
                        if (currentTime < 2.5f)
                        {
                            spriteBatch.Draw(gameTextures.yaniv.texture, gameTextures.yaniv.vector,
                                             gameTextures.yaniv.rectangle, Color.White);                            
                        }
                        else
                        {
                            spriteBatch.Draw(gameTextures.assaf, gameTextures.yaniv.vector,
                                          gameTextures.yaniv.rectangle, Color.White);
                            switch (winner)
                            {
                                case 0:
                                    spriteBatch.Draw(gameTextures.youWon.texture, gameTextures.youWon.vector,
                                                              gameTextures.youWon.rectangle, Color.White);
                                    break;
                                case 1:
                                    spriteBatch.Draw(gameTextures.player1Won, gameTextures.youWon.vector,
                                                  gameTextures.youWon.rectangle, Color.White);
                                    break;
                                case 2:
                                    spriteBatch.Draw(gameTextures.player2Won, gameTextures.youWon.vector,
                                                  gameTextures.youWon.rectangle, Color.White);
                                    break;

                            }
                        }

                    }
                    if (currentTime > 5f)
                    {
                        spriteBatch.End();
                        base.Draw(gameTime);
                        roundNumber--;
                        if (roundNumber == 0) { 
                        startGame = false;
                        return;
                    }
                        me.ResetPlayer();
                        player1.ResetPlayer();
                        player2.ResetPlayer();
                        Initialize();
                        return;
                    }
                }
            }
            else
            {
                // Empty background
                spriteBatch.Draw(gameTextures.backgroundEmpty.texture, gameTextures.background.rectangle, Color.White);

                // Displaying setting for game background
                spriteBatch.Draw(gameTextures.startupScreen.texture, gameTextures.background.rectangle, Color.White);
                spriteBatch.Draw(gameTextures.chooseButtonSmall.texture, gameTextures.chooseButtonSmall.vector
                                   , null, Color.White);
                spriteBatch.Draw(gameTextures.chooseButtonBig.texture, gameTextures.chooseButtonBig.vector,
                    null, Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {            
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            mousePrevious = mouseCurrent;
            mouseCurrent = Mouse.GetState();
            if (startGame)
            {
                if (!openingCardShufflePlayed)
                {
                    cardShuffle.Play();
                    openingCardShufflePlayed = true;
                }
              
                foreach (Card card in me.GetCards())
                {
                    if (mouseCurrent.X >= card.Vector.X &&
                        mouseCurrent.X < card.Vector.X + card.Rectangle.Width &&
                        mouseCurrent.Y >= card.Vector.Y &&
                        mouseCurrent.Y < card.Vector.Y + card.Rectangle.Height &&
                        card.CardState != CardState.LIFT)
                    {
                        card.CardState = CardState.HOVER_UP;
                    }
                    else if (card.CardState == CardState.HOVER_UP)
                        card.CardState = CardState.HOVER_DOWN;
                }
             

                    if (turnCounter % numOfPlayers == 0)
                {

                    if (mousePrevious.LeftButton == ButtonState.Pressed &&
   mouseCurrent.LeftButton == ButtonState.Released &&
   mouseCurrent.X >= gameTextures.sortButton.vector.X &&
   mouseCurrent.X < gameTextures.sortButton.vector.X + gameTextures.sortButton.rectangle.Width &&
   mouseCurrent.Y >= gameTextures.sortButton.vector.Y &&
   mouseCurrent.Y < gameTextures.sortButton.vector.Y + gameTextures.sortButton.rectangle.Height)
                    {
                        sort = true;
                        PlaceCardsOnMat();
                    }

                    if (mousePrevious.LeftButton == ButtonState.Pressed && mouseCurrent.LeftButton == ButtonState.Released)
                    {
                        j = 0;
                        if (mouseCurrent.X >= gameTextures.sortButton.vector.X &&
                           mouseCurrent.X < gameTextures.sortButton.vector.X + gameTextures.sortButton.rectangle.Width &&
                           mouseCurrent.Y >= gameTextures.sortButton.vector.Y &&
                           mouseCurrent.Y < gameTextures.sortButton.vector.Y + gameTextures.sortButton.rectangle.Height)
                        {
                            sort = true;
                            PlaceCardsOnMat();
                        }
                        if (me.CardSum() <= 7 && mouseCurrent.X >= 140 &&
                            mouseCurrent.X < 323 && mouseCurrent.Y >= 390 && mouseCurrent.Y < 454)
                        {
                            CheckWhoIsTheWinner(me);
                            base.Update(gameTime);
                            return;
                        }

                        foreach (Card card in me.GetCards())
                        {
                            if (mouseCurrent.X >= card.Vector.X && mouseCurrent.X < card.Vector.X + 79 && mouseCurrent.Y >= card.Vector.Y && mouseCurrent.Y < card.Vector.Y + 123)
                            {
                                me.PickCard(card);
                            }
                        }

                        if (lastTableCard != firstTableCard)
                        {
                            if (mouseCurrent.X >= firstTableCard.Vector.X &&
                                mouseCurrent.X < firstTableCard.Vector.X + 30 &&
                                mouseCurrent.Y >= firstTableCard.Vector.Y &&
                                mouseCurrent.Y < firstTableCard.Vector.Y + 123)
                            {
                                thrownCards = me.Play(firstTableCard);
                                if (thrownCards != null)
                                {
                                    cardBeingThrown.CreateInstance().Play();
                                    currentCardDrawing = CardDrawing.LEFT;
                                    GetTableCardLocationForAnimation();
                                    ReplaceTableCards();
                                    PlaceCardsOnMat();
                                    turnCounter++;
                                    currentTime = 0;                                    
                                    base.Update(gameTime);
                                    return;
                                }
                            }
                        }


                        if (mouseCurrent.X >= lastTableCard.Vector.X &&
                              mouseCurrent.X < lastTableCard.Vector.X + 79 &&
                              mouseCurrent.Y >= lastTableCard.Vector.Y &&
                              mouseCurrent.Y < lastTableCard.Vector.Y + 123)
                        {
                            thrownCards = me.Play(new Card(lastTableCard));
                            if (thrownCards != null)
                            {
                                cardBeingThrown.CreateInstance().Play();
                                currentCardDrawing = CardDrawing.RIGHT;
                                GetTableCardLocationForAnimation();
                                ReplaceTableCards();
                                PlaceCardsOnMat();
                                turnCounter++;
                                base.Update(gameTime);
                                currentTime = 0;                                
                                return;
                            }
                        }
                        if (mouseCurrent.X >= gameTextures.deck.vector.X &&
                            mouseCurrent.X < gameTextures.deck.vector.X + gameTextures.deck.rectangle.Width &&
                            mouseCurrent.Y >= gameTextures.deck.vector.Y &&
                            mouseCurrent.Y < gameTextures.deck.vector.Y + gameTextures.deck.rectangle.Height)
                        {
                            deckCard = GenerateCard();
                            thrownCards = me.Play(deckCard);
                            if (thrownCards != null)
                            {
                                cardBeingThrown.CreateInstance().Play();
                                currentCardDrawing = CardDrawing.DECK;
                                GetTableCardLocationForAnimation();
                                PlaceCardsOnMat();
                                ReplaceTableCards();
                                if (tableCards.Count == 1 && tableCards[0].Value == deckCard.Value ||
                                    tableCards.Count > 1 && tableCards[0].Value == tableCards[1].Value &&
                                    tableCards[0].Value == deckCard.Value && deckCard.Shape != Shapes.JOKER) sameCardThrow = true;
                                turnCounter++;
                                currentTime = 0;                                
                                base.Update(gameTime);
                                return;
                            }
                        }

                    }
                }

                else if (turnCounter % numOfPlayers == 1 && winner == 4)
                {
                    if (currentTime > 3.5f)
                    {
                        sameCardThrow = false;
                        if (ComputerStrategy(player1)) turnCounter = 0;
                        else
                            turnCounter++;
                        currentTime = 0f;
                    }
                    else if (sameCardThrow && mousePrevious.LeftButton == ButtonState.Pressed &&
                            mouseCurrent.LeftButton == ButtonState.Released &&
                            mouseCurrent.X >= gameTextures.throwCardSign.vector.X &&
                            mouseCurrent.X < gameTextures.throwCardSign.vector.X + 
                            gameTextures.throwCardSign.texture.Width &&
                            mouseCurrent.Y >= gameTextures.throwCardSign.vector.Y &&
                            mouseCurrent.Y < gameTextures.throwCardSign.vector.Y +
                            gameTextures.throwCardSign.texture.Height)
                    {
                        cardBeingThrown.CreateInstance().Play();
                        tableCards.Clear();
                        tableCards.Add(deckCard);
                        ReplaceTableCards();
                        me.GetCards().Remove(deckCard);
                        me.GetClubCards().Remove(deckCard);
                        me.GetDiamondsCards().Remove(deckCard);
                        me.GetHeartsCards().Remove(deckCard);
                        me.GetSpadesCards().Remove(deckCard);
                        PlaceCardsOnMat();                        
                        sameCardThrow = false;
                    }

                }
                else if (winner == 4)
                {
                    if (currentTime > 3.5f)
                    {
                        if (ComputerStrategy(player2)) turnCounter = 0;
                        else
                            turnCounter++;
                        currentTime = 0f;
                    }
                }
            }
            else
            {
                if (mouseCurrent.X >= 348 &&
                   mouseCurrent.X < 405 &&  
                   mouseCurrent.Y >= 275 && 
                   mouseCurrent.Y < 332)
                {
                    gameTextures.chooseButtonSmall.vector.X = 345;
                    numOfPlayers = 2;
                }
                if(mouseCurrent.X >= 442 &&
                   mouseCurrent.X < 500 &&
                    mouseCurrent.Y >= 264 &&
                    mouseCurrent.Y < 320)
                {
                    gameTextures.chooseButtonSmall.vector.X = 433;
                    numOfPlayers = 3;                    

                }
                if(mouseCurrent.X >= 200 &&
                    mouseCurrent.X < 257 &&
                    mouseCurrent.Y >= 400 &&
                    mouseCurrent.Y < 457)
                {
                    gameTextures.chooseButtonBig.vector.X = 200;
                    roundNumber = 1;
                    
                    
                }
                if(mouseCurrent.X >= 295 && 
                    mouseCurrent.X < 295+57 &&
                    mouseCurrent.Y >= 400 &&
                    mouseCurrent.Y < 457)
                {
                    gameTextures.chooseButtonBig.vector.X = 295;
                    roundNumber = 3;

                }
                if (mouseCurrent.X >= 390 &&
                    mouseCurrent.X < 490 &&
                    mouseCurrent.Y >= 420 &&
                    mouseCurrent.Y < 467)
                {
                    gameTextures.chooseButtonBig.vector.X = 387;
                    roundNumber = 5;

                }
                if (mousePrevious.LeftButton == ButtonState.Pressed && mouseCurrent.LeftButton == ButtonState.Released)                    
                {
                    if (mouseCurrent.X >= 229 &&
                        mouseCurrent.X < 500 &&
                        mouseCurrent.Y >= 558 &&
                        mouseCurrent.Y < 645)
                    {
                        me.ResetPlayer();
                        player1.ResetPlayer();
                        player2.ResetPlayer();
                        Initialize();
                        startGame = true;
                    }
                }
            }
            base.Update(gameTime);
        }

        private void ReplaceTableCards()
        {           
            Card[] arrayTableCards = thrownCards.ToArray();
            int middle;
            if (arrayTableCards.Length % 2 == 0)
                middle = arrayTableCards.Length / 2;
            else
                middle = (arrayTableCards.Length - 1) / 2;

            arrayTableCards[middle].Vector.X = 310;
            arrayTableCards[middle].Vector.Y = 220;

            for (int i = 1; i <= middle; i++)
            {
                arrayTableCards[middle - i].Vector.X = 310 - (30 * i);
                arrayTableCards[middle - i].Vector.Y = 220;
                if (middle + i < arrayTableCards.Length)
                {
                    arrayTableCards[middle + i].Vector.X = 310 + (30 * i);
                    arrayTableCards[middle + i].Vector.Y = 220;                    
                }
            }
            tableCards.Clear();
            tableCards.AddRange(arrayTableCards);
            firstTableCard = arrayTableCards[0];
            lastTableCard = arrayTableCards[arrayTableCards.Length - 1];


/*            foreach (Card card in thrownCards)
            {
                tableCards.Add(card);
                card.Vector = new Vector2(300 + 30 * i, 220);
                i++;
            }*/
/*            firstTableCard = tableCards[0];
            lastTableCard = new Card(tableCards[tableCards.Count - 1]);*/

        }

        private bool ComputerStategy(Player player, Card cardToTake, bool deckCard)
        {
            Card[] cards;
            byte sameCardCounter = 0, twoCardValue = 0;
            bool cardWasPicked = false;
            // --- PICKING CARDS FROM LIST --- //   
            //// PICKING SERIAS ////            
            if (player.GetClubCards().Count >= 3 || player.Joker > 0 && player.GetClubCards().Count >= 2)
            {
                cards = player.GetClubCards().ToArray();
                Array.Sort(cards);
                if (CheckSeria(cards, cardToTake, player)) return true;
                else UnpickCards(player.GetCards());
            }
            if (player.GetDiamondsCards().Count >= 3 || player.Joker > 0 && player.GetDiamondsCards().Count >= 2)
            {
                cards = player.GetDiamondsCards().ToArray();
                Array.Sort(cards);
                if (CheckSeria(cards, cardToTake, player)) return true;
                else UnpickCards(player.GetCards());
            }
            if (player.GetHeartsCards().Count >= 3 || player.Joker > 0 && player.GetHeartsCards().Count >= 2)
            {
                cards = player.GetHeartsCards().ToArray();
                Array.Sort(cards);
                if (CheckSeria(cards, cardToTake, player)) return true;
                else UnpickCards(player.GetCards());
            }
            if (player.GetSpadesCards().Count >= 3 || player.Joker > 0 && player.GetSpadesCards().Count >= 2)
            {
                cards = player.GetSpadesCards().ToArray();
                Array.Sort(cards);
                if (CheckSeria(cards, cardToTake, player)) return true;
                else UnpickCards(player.GetCards());
            }

            //// PICKING SAME VALUE CARDS ////
            cards = player.GetCards().ToArray();
            Array.Sort(cards);
            for (int i = cards.Length - 1; i > 1; i--)
            {
                // If there is two or more cards with the same value
                if (cards[i].Value == cards[i - 1].Value && cards[i].Shape != Shapes.JOKER)
                {
                    // If player chose a certian table card he wouldn't throw cards with
                    // it's value so he would throw them on the next round.
                    // If player chose a deck card he would throw it anyway. 
                    if (!deckCard && cards[i].Value == cardToTake.Value) continue;
                    else
                    {
                        sameCardCounter++;
                        cards[i].Picked = true;
                        cards[i - 1].Picked = true;
                        cardWasPicked = true;
                        twoCardValue = cards[i].Value;
                    }
                }
                else if (sameCardCounter > 0)
                    break;
            }

            /// THROW LARGEST CARD ///            
            for (int i = cards.Length - 1; i > 0; i--)
            {
                if (!deckCard && cards[i].Value == cardToTake.Value) continue;
                else
                {
                    if (sameCardCounter > 0 && cards[i].Value < twoCardValue * 2) break;
                    else
                    {
                        UnpickCards(player.GetCards());
                        cards[i].Picked = true;
                        cardWasPicked = true;
                        break;
                    }
                }
            }
            if (cardWasPicked)
            {
                thrownCards = player.Play(cardToTake);
                ReplaceTableCards();
                return true;
            }
            return false;
        }
        private bool ComputerStrategy(Player player)
        {
            Card cardToTake;
            bool deckCard = false;

            if (player.CardSum() <= 7)
            {
                CheckWhoIsTheWinner(player);
                return true;
            }

            // If table card is a low value card or the player has another card in his cards with the same value
            // He will take it for the next round
            // If the card on the table isn't helpful, the player will take a deck card   
            if(firstTableCard.Shape == Shapes.JOKER)
            {
                if (player.GetPlayerNumber() == 1) player1CardDrawing = CardDrawing.LEFT;
                else player2CardDrawing = CardDrawing.LEFT;
                cardToTake = new Card(firstTableCard);
            }
            else if (lastTableCard.Shape == Shapes.JOKER)
            {
                if (player.GetPlayerNumber() == 1) player1CardDrawing = CardDrawing.RIGHT;
                else player2CardDrawing = CardDrawing.RIGHT;
                cardToTake = new Card(lastTableCard);
            }
            else if (firstTableCard.Value < 3 ||
                ListContainCard(firstTableCard.Value, player.GetCards()))
            {
                if (player.GetPlayerNumber() == 1) player1CardDrawing = CardDrawing.LEFT;
                else player2CardDrawing = CardDrawing.LEFT;
                cardToTake = new Card(firstTableCard);
            }
            else if (lastTableCard.Value < 3 ||
                    ListContainCard(lastTableCard.Value, player.GetCards()))
            {
                if (player.GetPlayerNumber() == 1) player1CardDrawing = CardDrawing.RIGHT;
                else player2CardDrawing = CardDrawing.RIGHT;
                cardToTake = new Card(lastTableCard);
            }
            else
            {
                if (player.GetPlayerNumber() == 1) player1CardDrawing = CardDrawing.DECK;
                else player2CardDrawing = CardDrawing.DECK;
                cardToTake = GenerateCard();
                deckCard = true;
            }
            GetTableCardLocationForAnimation();

            if (!ComputerStategy(player, cardToTake, deckCard))
            {
                if (player.GetPlayerNumber() == 1) player1CardDrawing = CardDrawing.DECK;
                else player2CardDrawing = CardDrawing.DECK;
                cardToTake = GenerateCard();
                deckCard = true;
                ComputerStategy(player, cardToTake, deckCard);
            }
            cardBeingThrown.CreateInstance().Play();
            return false;
        }

        private void CheckWhoIsTheWinner(Player player)
        {
            int smallestScore;
            smallestScore = me.CardSum();
            winner = 0;

            if (player1.CardSum() < smallestScore)
            {
                smallestScore = player1.CardSum();
                winner = 1;
            }

            if (numOfPlayers > 2 && player2.CardSum() < smallestScore)
            {
                smallestScore = player2.CardSum();
                winner = 2;
            }

            if (smallestScore < player.CardSum()) assaf = true;

            for (int i = 0; i < numOfPlayers; i++)
            {
                if (assaf)
                {
                    if (player == players[i]) players[i].ScorePlayer(true);
                    else players[i].ScorePlayer(false);
                }
                else if (players[i].GetPlayerNumber() != winner) players[i].ScorePlayer(false);
            }
        }
        private void UnpickCards(List<Card> cards)
        {
            foreach (Card card in cards) card.Picked = false;
        }

        private bool CheckSeria(Card[] cards, Card cardToTake, Player player)
        {
            byte seriaCounter = 0;
            Card seriaValue = null;
            for (int i = 0; i < cards.Length - 1; i++)
            {
                if (cards[i].Value + 1 == cards[i + 1].Value)
                {
                    cards[i].Picked = true;
                    if (seriaValue == null)
                        seriaValue = cards[i + 1];
                    cards[i + 1].Picked = true;
                    seriaCounter++;
                }
                else if (cards[i].Value + 2 == cards[i + 1].Value && player.Joker > 0)
                {
                    cards[i].Picked = true;
                    seriaValue = cards[i];
                    cards[i + 1].Picked = true;
                    seriaCounter++;
                }
                else if (seriaCounter > 1 || (seriaCounter >= 1 && player.Joker > 0))
                {
                    if (player.Joker > 0) player.PickJokerUp(seriaValue);
                    thrownCards = player.Play(cardToTake);
                    ReplaceTableCards();
                    return true;
                }
            }
            return false;
        }

        private bool ListContainCard(byte card, List<Card> cards)
        {
            foreach (Card curr in cards) { if (curr.Value == card) return true; }
            return false;
        }

        void Deal(Player player)
        {
            List<Card> playersDeck = new List<Card>();
            for (int i = 0; i < 7; i++)
            {
                playersDeck.Add(GenerateCard());
            }
            player.SetCards(playersDeck);
        }

        private void PlaceCardsOnMat()
        {
            Card[] cards = me.GetCards().ToArray();
            Vector2 temp;
            int middle, addition = 0; ;
            if (sort)
            {
                Array.Sort(cards);
            }
            if (cards.Length % 2 == 1)
                middle = (cards.Length - 1) / 2;
            else
            {
                middle = cards.Length / 2;
                addition = 40;
            }

            cards[middle].Vector.X = 320 + addition;
            cards[middle].Vector.Y = 500;

            gameTextures.tookCard.vector.X = cards[middle].Vector.X;

            for (int i = 1; i <= middle; i++)
            {
                cards[middle - i].Vector.X = 320 - (85 * i) + addition;
                cards[middle - i].Vector.Y = 500;
                if (middle + i < cards.Length)
                {
                    cards[middle + i].Vector.X = 320 + (85 * i) + addition;
                    cards[middle + i].Vector.Y = 500;
                }
            }
            if (!sort)
            {
                temp = cards[middle].Vector;
                cards[middle].Vector = cards[cards.Length - 1].Vector;
                cards[cards.Length - 1].Vector = temp;
            }
            else
            {
                me.FixList(cards);
                sort = false;
            }

        }

        private void GetTableCardLocationForAnimation()
        {
            switch (currentCardDrawing)
            {
                case CardDrawing.DECK:                    
                    gameTextures.tookCard.rectangle = deckCard.Rectangle;
                    gameTextures.tookCard.vector.Y = gameTextures.deck.vector.Y;
                    return;
                case CardDrawing.LEFT:                   
                    gameTextures.tookCard.rectangle = firstTableCard.Rectangle;
                    gameTextures.tookCard.vector.Y = firstTableCard.Vector.Y + 30;
                    break;
                case CardDrawing.RIGHT:
                    gameTextures.tookCard.texture = deckCard.Texture;
                    gameTextures.tookCard.rectangle = lastTableCard.Rectangle;
                    gameTextures.tookCard.vector.Y = firstTableCard.Vector.Y + 30;
                    break;
                default:
                    break;
            }

            switch (player1CardDrawing)
            {
                case CardDrawing.DECK:
                    gameTextures.tookCard.texture = Content.Load<Texture2D>("tookCard");
                    gameTextures.tookCard.vector.X = gameTextures.deck.vector.X;
                    gameTextures.tookCard.vector.Y = gameTextures.player1CardsVectors[0].Y + 50;
                    break;
                case CardDrawing.RIGHT:
                    gameTextures.tookCard.texture = lastTableCard.Texture;
                    gameTextures.tookCard.rectangle = lastTableCard.Rectangle;
                    gameTextures.tookCard.vector.Y =
                            gameTextures.player1CardsVectors[player1.GetCards().Count - 1].Y;
                    gameTextures.tookCard.vector.X = gameTextures.deck.vector.X;
                    break;
                case CardDrawing.LEFT:
                    gameTextures.tookCard.texture = firstTableCard.Texture;
                    gameTextures.tookCard.rectangle = firstTableCard.Rectangle;
                    gameTextures.tookCard.vector.Y = 
                            gameTextures.player1CardsVectors[player1.GetCards().Count - 1].Y;
                    gameTextures.tookCard.vector.X = gameTextures.deck.vector.X;
                    break;
                default:
                    break;
            }

            switch (player2CardDrawing)
            {
                case CardDrawing.DECK:
                    gameTextures.tookCard.texture = Content.Load<Texture2D>("tookCard");
                    gameTextures.tookCard.vector.X = gameTextures.deck.vector.X;
                    gameTextures.tookCard.vector.Y = gameTextures.player2CardsVectors[0].Y + 50;
                    break;
                case CardDrawing.LEFT:
                    gameTextures.tookCard.texture = firstTableCard.Texture;
                    gameTextures.tookCard.rectangle = firstTableCard.Rectangle;
                    gameTextures.tookCard.vector.X = gameTextures.deck.vector.X;
                    gameTextures.tookCard.vector.Y =
                            gameTextures.player2CardsVectors[player2.GetCards().Count - 1].Y + 50;
                    break;
                case CardDrawing.RIGHT:
                    gameTextures.tookCard.texture = lastTableCard.Texture;
                    gameTextures.tookCard.rectangle = lastTableCard.Rectangle;
                    gameTextures.tookCard.vector.X = gameTextures.deck.vector.X;
                    gameTextures.tookCard.vector.Y =
                            gameTextures.player2CardsVectors[player2.GetCards().Count - 1].Y + 50;
                    break;
                default:
                    break;
            }

        }


    }
           
}
