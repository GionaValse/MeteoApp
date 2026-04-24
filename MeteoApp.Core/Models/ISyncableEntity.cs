using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Models;

public interface ISyncableEntity
{
    string Id { get; set; }
    DateTime UpdatedAt { get; set; }
    bool IsDeleted { get; set; }
    bool NeedsSync { get; set; }
}
