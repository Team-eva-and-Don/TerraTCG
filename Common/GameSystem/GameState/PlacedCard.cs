using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal class PlacedCard(Card template)
    {
        internal Card Template { get; set; } = template;

        internal int CurrentHealth { get; set; } = template.MaxHealth;

        internal List<ICardModifier> CardModifiers { get; set; } = [];

        internal TimeSpan PlaceTime { get; set; }
        internal bool IsExerted { get; set; } = false;

        internal bool IsDamaged => CurrentHealth < Template.MaxHealth;

        internal void Heal(int amount)
        {
            CurrentHealth = Math.Min(Template.MaxHealth, CurrentHealth + amount);
        }

        internal void AddModifiers(List<ICardModifier> modifiers)
        {
            CardModifiers.AddRange(modifiers);
        }

        public Attack GetAttackWithModifiers(Zone startZone, Zone endZone)
        {
            var attack = Template.Attacks[0].Copy();
            foreach (var modifier in CardModifiers.Concat(startZone.Owner.Field.CardModifiers))
            {
                modifier.ModifyAttack(ref attack, startZone, endZone);
            }
            if (endZone?.PlacedCard is PlacedCard endCard)
            {
                foreach (var modifier in endCard.CardModifiers.Concat(endZone.Owner.Field.CardModifiers))
                {
                    modifier.ModifyIncomingAttack(ref attack, startZone, endZone);
                }
            }
            return attack;
        }
        public Skill GetSkillWithModifiers(Zone startZone, Zone endZone)
        {
            if(Template.Skills == null || Template.Skills.Count == 0)
            {
                return default;
            }

            var skill = Template.Skills[0].Copy();
            foreach (var modifier in CardModifiers.Concat(startZone.Owner.Field.CardModifiers))
            {
                modifier.ModifySkill(ref skill, startZone, endZone);
            }
            if (endZone?.PlacedCard is PlacedCard endCard)
            {
                foreach (var modifier in endCard.CardModifiers.Concat(endZone.Owner.Field.CardModifiers))
                {
                    modifier.ModifySkill(ref skill, startZone, endZone);
                }
            }
            return skill;
        }

        public Skill ModifyIncomingSkill(Card sourceCard)
        {
            var skill = sourceCard.Skills[0].Copy();
            foreach(var modifier in CardModifiers)
            {
                modifier.ModifyIncomingSkill(ref skill, sourceCard);

            }
            return skill;
        }

        internal List<Zone> GetValidAttackZones(Zone startZone, Zone endZone) 
        {
            // default list of zones: Attack zones and unblocked defense zones
            var targetZones = endZone.Owner.Field.Zones.Where(z => !z.IsEmpty() && !z.IsBlocked()).ToList();

            foreach(var modifier in CardModifiers.Concat(startZone.Owner.Field.CardModifiers))
            {
                modifier.ModifyZoneSelection(startZone, endZone, ref targetZones);
            }

            foreach (var modifier in endZone.Owner.Field.CardModifiers)
            {
                modifier.ModifyIncomingZoneSelection(startZone, endZone, ref targetZones);
            }
            foreach(var modifier in endZone.Owner.Field.Zones.Where(z=>!z.IsEmpty()).SelectMany(z=>z.PlacedCard.CardModifiers))
            {
                modifier.ModifyIncomingZoneSelection(startZone, endZone, ref targetZones);
            }

            return targetZones;
        }

    }
}
