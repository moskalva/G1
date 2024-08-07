﻿using System;
using ProtoBuf;

namespace G1.Model
{
    [ProtoContract]
    public enum WorldEntityType
    {
        Unknown = 0,
        Ship = 1,
    }

    [ProtoContract]
    public struct WorldEntityId
    {
        [ProtoMember(1)]
        public Guid Id { get; set; }

        public override string ToString() => Id.ToString();

        public static WorldEntityId Create() => new WorldEntityId { Id = Guid.NewGuid() };

        public static bool TryParse(string str, out WorldEntityId clientId)
        {
            if (Guid.TryParse(str, out var id))
            {
                clientId = new WorldEntityId { Id = id };
                return true;
            }

            clientId = default;
            return false;
        }
    }

    [ProtoContract]
    public struct World3dVector
    {
        public static World3dVector Zero => new World3dVector();

        [ProtoMember(1)]
        public float X { get; set; }

        [ProtoMember(2)]
        public float Y { get; set; }

        [ProtoMember(3)]
        public float Z { get; set; }

        public override string ToString() => $"{X},{Y},{Z}";
    }

    [ProtoContract]
    public struct WorldEntityState
    {
        [ProtoMember(1)]
        public WorldEntityId Id { get; set; }

        [ProtoMember(2)]
        public WorldEntityType Type { get; set; }

        [ProtoMember(3)]
        public World3dVector? Position { get; set; }

        [ProtoMember(4)]
        public World3dVector? Velocity { get; set; }

        public override string ToString() => $"WorldEntityState '{Id}' type '{Type}', position: {Position}', velocity: '{Velocity}'";
    }

    [ProtoContract]
    [ProtoInclude(1, typeof(StateChange))]
    public class RemoteCommand
    {
    }

    [ProtoContract]
    public class StateChange : RemoteCommand, IEquatable<StateChange>
    {
        public StateChange(){}

        public StateChange(WorldEntityState state)
        {
            this.NewState = state;
        }

        [ProtoMember(1)]
        public WorldEntityState NewState { get; set; } = new WorldEntityState();

        public bool Equals(StateChange other) => this.NewState.Equals(other?.NewState);
#pragma warning disable CS8604 // Possible null reference argument.
        public override bool Equals(object other) => this.Equals(other as StateChange);
#pragma warning restore CS8604 // Possible null reference argument.

        public override int GetHashCode() => this.NewState.GetHashCode();


        public static bool operator ==(StateChange obj1, StateChange obj2) => obj1.Equals(obj2);

        public static bool operator !=(StateChange obj1, StateChange obj2) => !obj1.Equals(obj2);

        public override string ToString() => $"StateChange: '{this.NewState}'";
    }
}
