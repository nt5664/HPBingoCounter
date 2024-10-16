using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPBingoCounter.Core.Models
{
    public interface IGoal
    {
        string? Id { get; }
        string? Name { get; }
        bool CollectMultiple { get; }
        int RequiredAmount { get; }
    }
}
