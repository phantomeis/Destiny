﻿using Destiny.Core.IO;
using Destiny.Data;
using Destiny.Maple.Characters;
using Destiny.Maple.Social;
using System.Collections.ObjectModel;

namespace Destiny.Server
{
    public sealed class WorldGuilds : KeyedCollection<int, Guild>
    {
        public WorldServer Parent { get; private set; }

        public WorldGuilds(WorldServer parent)
        {
            this.Parent = parent;
        }

        public void Load()
        {
            using (Log.Load(this.Parent.Name + " Guilds"))
            {
                foreach (Datum datum in new Datums("guilds").Populate())
                {
                    this.Add(new Guild(this.Parent, datum));
                }

                foreach (Datum datum in new Datums("characters").Populate("GuildID > 0"))
                {
                    this[(int)datum["GuildID"]].Add(new GuildMember(datum));
                }
            }
        }

        protected override int GetKeyForItem(Guild item)
        {
            return item.ID;
        }
    }

    public sealed class WorldServer : KeyedCollection<byte, ChannelServer>
    {
        public byte ID { get; private set; }
        public string Name { get; private set; }
        public WorldFlag Flag { get; private set; }
        public string EventMessage { get; private set; }
        public string TickerMessage { get; private set; }
        public int ExperienceRate { get; private set; }
        public int QuestExperienceRate { get; private set; }
        public int PartyQuestExperienceRate { get; private set; }
        public int MesoRate { get; private set; }
        public int DropRate { get; private set; }

        public WorldGuilds Guilds { get; private set; }

        public WorldServer()
            : base()
        {
            this.ID = 0;
            this.Name = "Scania";
            this.Flag = WorldFlag.New;
            this.EventMessage = "Welcome to #rDestiny#k!";
            this.TickerMessage = "Welcome to Destiny!";
            this.ExperienceRate = 1;
            this.QuestExperienceRate = 1;
            this.PartyQuestExperienceRate = 1;
            this.MesoRate = 1;
            this.DropRate = 1;

            this.Guilds = new WorldGuilds(this);

            byte channels = 2;

            for (byte i = 0; i < channels; i++)
            {
                this.Add(new ChannelServer(i, this, (short)(8585 + i)));
            }
        }

        public void Start()
        {
            this.Guilds.Load();

            foreach (ChannelServer channel in this)
            {
                channel.Start();
            }
        }

        public void Stop()
        {
            foreach (ChannelServer channel in this)
            {
                channel.Stop();
            }
        }

        public void Broadcast(OutPacket oPacket)
        {
            foreach (ChannelServer channel in this)
            {
                channel.Broadcast(oPacket);
            }
        }

        public void Notify(string text, NoticeType type)
        {
            foreach (ChannelServer channel in this)
            {
                channel.Notify(text, type);
            }
        }

        public bool IsCharacterOnline(int id)
        {
            foreach (ChannelServer channel in this)
            {
                if (channel.Characters.Contains(id))
                {
                    return true;
                }
            }

            return false;
        }

        public Character GetCharacter(int id)
        {
            Character character = null;

            foreach (ChannelServer channel in this)
            {
                character = channel.Characters.GetCharacter(id);

                if (character != null)
                {
                    break;
                }
            }

            return character;
        }

        public bool IsCharacterOnline(string name)
        {
            foreach (ChannelServer channel in this)
            {
                if (channel.Characters.Contains(name))
                {
                    return true;
                }
            }

            return false;
        }

        public Character GetCharacter(string name)
        {
            Character character = null;

            foreach (ChannelServer channel in this)
            {
                character = channel.Characters.GetCharacter(name);

                if (character != null)
                {
                    break;
                }
            }

            return character;
        }

        protected override byte GetKeyForItem(ChannelServer item)
        {
            return item.ID;
        }
    }
}