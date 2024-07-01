using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiceProject4
{
    internal class TargetManager
    {
        private Dictionary<Rice, Tractor> targetedRices = new Dictionary<Rice, Tractor>();
        
        public bool TryTargetRice(Rice rice, Tractor tractor)
        {
            if (!rice.IsTargeted)
            {
                targetedRices[rice] = tractor;
                rice.IsTargeted = true; // Mark rice as targeted
                return true;
            }
            return false;
        }

        public void ReleaseTarget(Rice rice)
        {
            if (targetedRices.ContainsKey(rice))
            {
                rice.IsTargeted = false; // Mark rice as not targeted
                targetedRices.Remove(rice);
            }
        }

        public Rice GetTarget(Tractor tractor)
        {
            return targetedRices.FirstOrDefault(kv => kv.Value == tractor).Key;
        }

        public int TractorCount()
        {
            return targetedRices.Count;
        }

        public int RemainingRiceCount()
        {
            return targetedRices.Keys.Count(rice => !rice.IsCollected);
        }

    }
}
