using log4net.Appender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TerraTCG.Common.GameSystem.GameState
{
    readonly struct PlayerResources(int health, int mana, int townsfolkMana)
    {
        public int Health { get; } = health;
        public int Mana { get; } = mana;
        public int TownsfolkMana { get; } = townsfolkMana;

        public TimeSpan SetTime { get; } = TCGPlayer.TotalGameTime;

        public PlayerResources UseResource(int health = 0, int mana = 0, int townsfolkMana = 0)
            => new(Health - health, Mana - mana, TownsfolkMana - townsfolkMana);

		public static PlayerResources operator -(PlayerResources a, PlayerResources b)
			=> a.UseResource(b.Health, b.Mana, b.TownsfolkMana);
    }
}
