namespace Maxstupo.Fsu.Core.Dsl {

    using System.Collections.Generic;

    public interface IInterpreter<T> where T : class {

        List<T> Eval(params string[] text);

    }

}