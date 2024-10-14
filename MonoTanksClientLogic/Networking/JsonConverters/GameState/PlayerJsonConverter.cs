﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoTanksClientLogic.Networking.GameState;

/// <summary>
/// Represents a player json converter.
/// </summary>
/// <param name="context">The serialization context.</param>
internal class PlayerJsonConverter(GameSerializationContext context) : JsonConverter<Player>
{
    /// <inheritdoc/>
    public override Player? ReadJson(JsonReader reader, Type objectType, Player? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);

        var id = jObject["id"]!.Value<string>()!;
        var nickname = jObject["nickname"]!.Value<string>()!;
        var color = jObject["color"]!.Value<uint>()!;
        var ping = jObject["ping"]!.Value<int>()!;

        Grid.VisibilityPayload? visibility = default;

        if (context is GameSerializationContext.Spectator || context.IsPlayerWithId(id))
        {
            var score = jObject["score"]!.Value<int>()!;
            var remainingTicksToRegen = jObject["ticksToRegen"]!.Value<int?>();
            var isUsingRadar = jObject["isUsingRadar"]!.Value<bool>();

            if (context is GameSerializationContext.Spectator)
            {
                visibility = jObject["visibility"]!.ToObject<Grid.VisibilityPayload>(serializer)!;
            }

            return new Player(id, nickname, color, remainingTicksToRegen, visibility?.VisibilityGrid)
            {
                Ping = ping,
                Score = score,
                IsUsingRadar = isUsingRadar,
            };
        }

        return new Player(id, nickname, color)
        {
            Ping = ping,
        };
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, Player? value, JsonSerializer serializer)
    {
        var jObject = new JObject
        {
            ["id"] = value!.Id,
            ["nickname"] = value.Nickname,
            ["color"] = value.Color,
            ["ping"] = value.Ping,
        };

        if (context is GameSerializationContext.Spectator || context.IsPlayerWithId(value.Id))
        {
            jObject["score"] = value.Score;
            jObject["ticksToRegen"] = value.RemainingTicksToRegen;
            jObject["isUsingRadar"] = value.IsUsingRadar;
        }

        if (context is GameSerializationContext.Spectator)
        {
            var visibilityPayload = new Grid.VisibilityPayload(value.VisibilityGrid!);
            jObject["visibility"] = JToken.FromObject(visibilityPayload, serializer);
        }

        jObject.WriteTo(writer);
    }
}
