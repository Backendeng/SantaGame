﻿namespace Santase.Logic.GameMechanics
{
    using Santase.Logic.Logger;
    using Santase.Logic.Players;

    // TODO: Unit test this class
    public class SantaseGame : ISantaseGame
    {
        private readonly IPlayer firstPlayer;

        private readonly IPlayer secondPlayer;

        private readonly ILogger logger;

        private PlayerPosition firstToPlay;

        private int secondPlayerTotalPoints;

        private int firstPlayerTotalPoints;

        public SantaseGame(IPlayer firstPlayer, IPlayer secondPlayer, PlayerPosition firstToPlay, ILogger logger)
        {
            this.firstPlayerTotalPoints = 0;
            this.secondPlayerTotalPoints = 0;
            this.RoundsPlayed = 0;
            this.firstPlayer = firstPlayer;
            this.secondPlayer = secondPlayer;
            this.firstToPlay = firstToPlay;
            this.logger = logger;
        }

        public SantaseGame(
            IPlayer firstPlayer,
            IPlayer secondPlayer,
            PlayerPosition firstToPlay = PlayerPosition.FirstPlayer)
            : this(firstPlayer, secondPlayer, firstToPlay, new NoLogger())
        {
        }

        public int FirstPlayerTotalPoints => this.firstPlayerTotalPoints;

        public int SecondPlayerTotalPoints => this.secondPlayerTotalPoints;

        public int RoundsPlayed { get; private set; }

        public PlayerPosition Start()
        {
            while (this.GameWinner() == PlayerPosition.NoOne)
            {
                this.PlayRound();
                this.RoundsPlayed++;
            }

            return this.GameWinner();
        }

        private void PlayRound()
        {
            var round = this.firstToPlay == PlayerPosition.FirstPlayer
                            ? new Round(this.firstPlayer, this.secondPlayer)
                            : new Round(this.secondPlayer, this.firstPlayer);

            var roundResult = round.Play();

            this.logger.LogLine(
                this.firstToPlay == PlayerPosition.FirstPlayer
                    ? $"{roundResult.FirstPlayer.RoundPoints} - {roundResult.SecondPlayer.RoundPoints}"
                    : $"{roundResult.SecondPlayer.RoundPoints} - {roundResult.FirstPlayer.RoundPoints}");

            var gameWinnerLogic = new GameWinnerLogic();
            this.firstToPlay = this.firstToPlay == PlayerPosition.FirstPlayer
                                   ? gameWinnerLogic.UpdatePointsAndGetFirstToPlay(
                                       roundResult,
                                       ref this.firstPlayerTotalPoints,
                                       ref this.secondPlayerTotalPoints)
                                   : gameWinnerLogic.UpdatePointsAndGetFirstToPlay(
                                       roundResult,
                                       ref this.secondPlayerTotalPoints,
                                       ref this.firstPlayerTotalPoints);
        }

        private PlayerPosition GameWinner()
        {
            if (this.FirstPlayerTotalPoints >= 11)
            {
                return PlayerPosition.FirstPlayer;
            }

            if (this.SecondPlayerTotalPoints >= 11)
            {
                return PlayerPosition.SecondPlayer;
            }

            return PlayerPosition.NoOne;
        }
    }
}
