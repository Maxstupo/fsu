namespace Maxstupo.Fsu.CommandTree {

    using System;

    /// <summary>
    /// Determines the visibility of Command nodes and their decendents.
    /// </summary>
    [Flags]
    public enum Visibility {

        /// <summary>
        /// This Command node is hidden and its children aren't discoverable.
        /// </summary>
        None = 0,

        /// <summary>
        /// The Command node is visible. 
        /// </summary>
        Self = 1,

        /// <summary>
        /// Child Command nodes are discoverable.
        /// </summary>
        /// <remarks>Child nodes must have Self to be visible.</remarks>
        Decendents = 2,

        /// <summary>
        /// The Command node is visible and its children.
        /// </summary>
        All = Self | Decendents

    }

}