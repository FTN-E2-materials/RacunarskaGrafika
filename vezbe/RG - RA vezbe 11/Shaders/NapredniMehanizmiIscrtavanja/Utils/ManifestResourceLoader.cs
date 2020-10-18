using System.IO;

namespace NapredniMehanizmiIscrtavanja
{
    /// <summary>
    /// A small helper class to load manifest resource files.
    /// </summary>
    public static class ManifestResourceLoader
    {
        /// <summary>
        /// Loads the named manifest resource as a text string.
        /// <param name="textFileName">Name of the text file.</param>
        /// <returns>The contents of the manifest resource.</returns>
        /// </summary>
        public static string LoadTextFile(string textFileName)
        {
            TextReader tr = new StreamReader(@textFileName);
            return tr.ReadToEnd();
        }
    }
}
