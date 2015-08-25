﻿namespace Santase.ConsoleUI
{
    using System;
    using System.Threading;

    using Santase.Logic.Cards;
    using Santase.Logic.PlayerActionValidate;
    using Santase.Logic.Players;

    public class ConsolePlayer : BasePlayer
    {
        private readonly int row;

        private readonly int col;

        public ConsolePlayer(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public override void AddCard(Card card)
        {
            base.AddCard(card);

            Console.SetCursorPosition(this.col, this.row);
            foreach (var item in this.Cards)
            {
                Console.Write("{0} ", item);
            }

            Thread.Sleep(150);
        }

        public override PlayerAction GetTurn(PlayerTurnContext context, IPlayerActionValidator actionValidator)
        {
            this.PrintGameInfo(context);
            while (true)
            {
                PlayerAction playerAction;

                Console.SetCursorPosition(0, this.row + 1);
                Console.Write(new string(' ', 79));
                Console.SetCursorPosition(0, this.row + 1);
                Console.Write(
                    "Turn? [1-{0}]=Card{1}",
                    this.Cards.Count,
                    context.AmITheFirstPlayer ? "; [T]=Change trump; [C]=Close: " : ": ");
                var userActionAsString = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userActionAsString))
                {
                    Console.WriteLine("Empty turn!                 ");
                    continue;
                }

                if (userActionAsString[0] >= '1' && userActionAsString[0] <= '6')
                {
                    var cardIndex = int.Parse(userActionAsString[0].ToString()) - 1;
                    if (cardIndex >= this.Cards.Count)
                    {
                        Console.WriteLine("Invalid card!              ");
                        continue;
                    }

                    var card = this.Cards[cardIndex];
                    var possibleAnnounce = this.AnnounceValidator.GetPossibleAnnounce(
                        this.Cards,
                        card,
                        context.TrumpCard,
                        context.AmITheFirstPlayer);

                    playerAction = PlayerAction.PlayCard(card, possibleAnnounce);
                }
                else if (userActionAsString[0] == 'T')
                {
                    playerAction = PlayerAction.ChangeTrump();
                }
                else if (userActionAsString[0] == 'C')
                {
                    playerAction = PlayerAction.CloseGame();
                }
                else
                {
                    Console.WriteLine("Invalid turn!                ");
                    continue;
                }

                if (actionValidator.IsValid(playerAction, context, this.Cards))
                {
                    if (playerAction.Type == PlayerActionType.PlayCard)
                    {
                        this.Cards.Remove(playerAction.Card);
                    }

                    if (playerAction.Type == PlayerActionType.ChangeTrump)
                    {
                        this.Cards.Remove(new Card(context.TrumpCard.Suit, CardType.Nine));
                    }

                    this.PrintGameInfo(context);

                    return playerAction;
                }
                else
                {
                    Console.WriteLine("Invalid action!                  ");
                }
            }
        }

        public override void EndTurn(PlayerTurnContext context)
        {
        }

        private void PrintGameInfo(PlayerTurnContext context)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Trump card: {0}            ", context.TrumpCard);
            Console.SetCursorPosition(0, 1);
            Console.WriteLine("Cards left in deck: {0}    ", context.CardsLeftInDeck);
            Console.SetCursorPosition(0, 2);
            Console.WriteLine("Board: {0}{1}              ", context.FirstPlayedCard, context.SecondPlayedCard);
            Console.SetCursorPosition(0, 3);
            Console.WriteLine("Game state: {0}            ", context.State.GetType().Name);
        }
    }
}
