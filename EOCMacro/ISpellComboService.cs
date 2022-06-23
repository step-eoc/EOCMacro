using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOCMacro
{
    public interface ISpellComboService
    {
        IList<string> FindSpellCombo(String str, int size);
    }
}
