using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace MyTimeTracker
{
    [Serializable]
    public enum States
    {
        [Description("Не определено")]
        Unknown,
        [Description("Открыт")]
        Opened,
        [Description("Закрыт")]
        Closed
    }
}
