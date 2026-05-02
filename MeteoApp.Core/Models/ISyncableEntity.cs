using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Models;

public interface ISyncableEntity : IDatabaseEntity
{
    DateTime UpdatedAt { get; set; }
    bool IsDeleted { get; set; }
    bool NeedsSync { get; set; }
}
