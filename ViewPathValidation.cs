using Penguin.Services.Files;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Penguin.Web.Mvc.Dynamic
{
    /// <summary>
    /// Represents the result of validating a view path for existence
    /// </summary>
    public class ViewPathValidation
    {
        /// <summary>
        /// True if any path was found
        /// </summary>
        public bool FoundPath => this.ValidationResults.Any(r => r.Exists);

        /// <summary>
        /// A collection of results for the checked paths
        /// </summary>
        public List<ViewValidationResult> ValidationResults { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileService"></param>
        /// <param name="pathsToCheck">A list of {0} format strings to inject the path into</param>
        public ViewPathValidation(string path, FileService fileService, List<string> pathsToCheck = null)
        {
            Contract.Requires(path != null);
            Contract.Requires(fileService != null);

            path = path.Replace(".cshtml", "");

            if (path.StartsWith("~", System.StringComparison.Ordinal))
            {
                path = path.Substring(1);
            }

            this.ValidationResults = new List<ViewValidationResult>();

            pathsToCheck = pathsToCheck ?? new List<string>()
                {
                    "~/Client{0}.cshtml",
                    "~{0}.cshtml",
                    "~/Client.Template{0}.cshtml",
                };

            foreach (string thisPath in pathsToCheck)
            {
                string toCheck = string.Format(CultureInfo.CurrentCulture, thisPath, path);
                this.ValidationResults.Add(new ViewValidationResult(toCheck, fileService.Exists(toCheck)));
            }
        }

        /// <summary>
        /// Returns the first matching result
        /// </summary>
        /// <param name="SurpressError">If false, throws a file not found exception if no matches are found</param>
        /// <returns>A matching path or null if errors are surpressed</returns>
        public string Result(bool SurpressError = false)
        {
            if (!SurpressError && !this.FoundPath)
            {
                throw new FileNotFoundException("No views found in the following paths: \r\n\r\n" + string.Join("\r\n", this.ValidationResults.Select(v => v.Path)));
            }

            return this.ValidationResults.FirstOrDefault(r => r.Exists)?.Path;
        }
    }
}