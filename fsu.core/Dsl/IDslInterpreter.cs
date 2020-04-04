using System.Collections.Generic;

namespace Maxstupo.Fsu.Core.Dsl {
    public interface IDslInterpreter<V> where V : class {
        List<V> Eval(params string[] text);
    }
}