using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraTCG.Common.GameSystem.GameState.GameActions;

namespace TerraTCG.Common.GameSystem.GameState
{
	public delegate void DoSkill(GamePlayer player, Zone cardZone, Zone endZone);
	public struct Skill()
    {
        public Asset<Texture2D> Texture { get; internal set; }
		public string Name { get; set; }
		public int Cost { get; set; }
		public string Description { get; set; }

        // Flag used by bots to determine how to select targets for skills
		public ZoneRole Role { get; set; } = ZoneRole.OFFENSE;

		public ActionType SkillType { get; set; } = ActionType.SKILL;
		public DoSkill DoSkill { get; set; }

        public Skill Copy()
        {
            return new()
            {
                Texture = Texture,
                Name = Name,
                Cost = Cost,
                Description = Description,
                Role = Role,
                SkillType = SkillType,
                DoSkill = DoSkill,
            };
        }

    }
}
