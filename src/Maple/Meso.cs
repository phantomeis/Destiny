﻿using Destiny.Maple.Maps;
using Destiny.Core.IO;
using Destiny.Core.Network;

namespace Destiny.Maple
{
    public sealed class Meso : Drop
    {
        public int Amount { get; private set; }

        public Meso(int amount)
             : base()
        {
            this.Amount = amount;
        }

        public override OutPacket GetShowGainPacket()
        {
            OutPacket oPacket = new OutPacket(SendOps.Message);

            oPacket
                .WriteByte()
                .WriteShort(1)
                .WriteInt(this.Amount)
                .WriteShort();

            return oPacket;
        }
    }
}