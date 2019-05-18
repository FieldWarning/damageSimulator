using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PhysicalDamage.Core
{
    class FireDamage : Damage
    {
        public struct FireData
        {
            public float Power;
        }

        private FireData _fireData;

        public FireDamage(FireData data, Target target) : base(DamageTypes.FIRE, target)
        {
            _fireData = data;
        }

        public override Target CalculateDamage()
        {
            Target finalState = this.CurrentTarget;
            throw new NotImplementedException();
        }
    }
}
