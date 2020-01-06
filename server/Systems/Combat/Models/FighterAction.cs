using System.Collections.Generic;

namespace Rebronx.Server.Systems.Combat.Models
{
    public class FighterAction
    {
        public int Fighter { get; set; }
        public int Move { get; set; }
        public List<FighterAttack> Attacks { get; set; }
        public FighterDeployment Deployment { get; set; }

        public FighterAction()
        {
            Attacks = new List<FighterAttack>();
        }
    }

    public class FighterAttack
    {
        public int Position { get; set; }
        public double DamagePercentage { get; set; }
        public List<FighterAttackDamage> Damages { get; set; }

        public FighterAttack()
        {
            Damages = new List<FighterAttackDamage>();
        }
    }

    public class FighterAttackDamage
    {
        public int Victim { get; set; }
        public double Damage { get; set; }
    }

    public class FighterDeployment
    {

    }
}