using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPBingoCounter.Core.Models
{
    public class HPBingoGoal : IGoal
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public bool CollectMultiple => RequiredAmount > 0;

        public int RequiredAmount { get; set; }
    }
}
