namespace Maxstupo.Fsu.Core.Filtering {

    using System;

    [Flags]
    public enum Operator {
        LessThan = 1,
        GreaterThan = 2,
        Equal = 4,
        EndsWith = 8,
        StartsWith = 16,
        Contains = 32,
        Not = 64,
        Regex = 128
    }

}