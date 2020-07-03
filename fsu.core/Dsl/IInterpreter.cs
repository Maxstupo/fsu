namespace Maxstupo.Fsu.Core.Dsl {

    using System.Collections.Generic;

    public interface IInterpreter<V> where V : class {

        List<V> Eval(params string[] text);

    }

}