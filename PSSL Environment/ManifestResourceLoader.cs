﻿using System.IO;
using System.Reflection;

namespace PSSL_Environment
{
    /// <summary>
    /// A small helper class to load manifest resource files.
    /// </summary>
    public static class ManifestResourceLoader
    {
        /// <summary>
        /// Loads the named manifest resource as a text string.
        /// </summary>
        /// <param name="textFileName">Name of the text file.</param>
        /// <returns>The contents of the manifest resource.</returns>
        public static string LoadTextFile(string textFileName)
        {
            //var executingAssembly = Assembly.GetExecutingAssembly();
            //var pathToDots = textFileName.Replace("\\", ".");
            //var location = string.Format("{0}.{1}", executingAssembly.GetName().Name, pathToDots);

            //using (var stream = executingAssembly.GetManifestResourceStream(location))
            //{
            //    using (var reader = new StreamReader(stream))
            //    {
            //        return reader.ReadToEnd();
            //    }
            //}
            string contents = File.ReadAllText(textFileName);
            return contents;
        }
    }
}