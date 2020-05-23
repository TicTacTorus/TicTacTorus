﻿using System;
using TicTacTorus.Pages;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Persistence
{
    public interface IPersistentStorage
    {
        void SavePlayer(HumanPlayer savePlayer);
        IPlayer LoadPlayer(string loadPlayer);
        void SavePlayerStats(IPlayerStats savePlayStats);// PlayerStats implementiert?

        IPlayerStats LoadPlayerStats(string loadPlayStats);
        void SaveGame(Game game);
        Replay LoadGame(Base64 base64);

    }
}