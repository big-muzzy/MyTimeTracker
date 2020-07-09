using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MyTimeTracker
{
  public static class MergeModes
  {
    public static ObservableCollection<KeyValuePair<MergeEnum, string>> GetMergeModes()
    {
      var result = new ObservableCollection<KeyValuePair<MergeEnum, string>>();
      result.Add(new KeyValuePair<MergeEnum, string>(MergeEnum.Always, "Объеденять проекты с однаковыми именами"));
      result.Add(new KeyValuePair<MergeEnum, string>(MergeEnum.Ask, "Спрашивать при совпадении имён проектов"));
      result.Add(new KeyValuePair<MergeEnum, string>(MergeEnum.Never, "Не объеденять проекты"));

      return result;
    }
  }
}
