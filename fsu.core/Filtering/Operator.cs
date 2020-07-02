namespace Maxstupo.Fsu.Core.Filtering {

    using System;

    [Flags]
    public enum Operator {
        
        Equal = 1,

        LessThan = 2,
        GreaterThan = 4,

        // String operations.
        EndsWith = 8,
        StartsWith = 16,
        Contains = 32,
        Regex = 64,

        /// <summary>Invert the operation.</summary>
        Not = 128,

        /// <summary>If an operand property doesn't exist, ignore the condition.</summary>
        Ignore = 256

    }

}