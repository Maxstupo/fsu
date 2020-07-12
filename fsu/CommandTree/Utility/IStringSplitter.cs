namespace Maxstupo.Fsu.CommandTree.Utility {

    public interface IStringSplitter {

        string[] Split(string line, char delimiter = ' ', char ignoreToggle = '"');

    }

}