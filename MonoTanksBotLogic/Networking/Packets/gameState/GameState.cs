﻿using MonoTanksClientLogic.JsonConverters;
using Newtonsoft.Json;

namespace MonoTanksClientLogic.Models;

/// <summary>
/// Represents game state.
/// </summary>
/// <param name="Id">Represents game state id.</param>
/// <param name="Tick">Represents game tick.</param>
/// <param name="Players">Represents game players.</param>
/// <param name="Map">Represents game map.</param>
/// <param name="Zones">Represents game zones.</param>
[JsonConverter(typeof(GameStateJsonConverter))]
public record class GameState(
    string Id,
    int Tick,
    GamePlayer[] Players,
    Tile[,] Map,
    Zone[] Zones);
