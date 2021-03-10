using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.EntityFrameworkCore.Events
{
    public class OverrideDbContextOptionsEvent
    {
        public bool UseDbContextAsOptionsParameter { get; set; }
    }
}
