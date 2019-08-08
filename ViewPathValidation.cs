using Penguin.Services.Files;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Penguin.Web.Mvc.Dynamic
{
    public class ViewPathValidation
    {
        #region Properties

        public bool FoundPath => this.ValidationResults.Any(r => r.Exists);

        public List<ViewValidationResult> ValidationResults { get; set; }

        #endregion Properties

        #region Constructors

        public ViewPathValidation(string path, FileService fileService)
        {
            path = path.Replace(".cshtml", "");

            if (path.StartsWith("~"))
            {
                path = path.Substring(1);
            }

            this.ValidationResults = new List<ViewValidationResult>();

            List<string> toCheck = new List<string>()
                {
                    "~/Client" + path + ".cshtml",
                    "~" + path + ".cshtml",
                    "~/Client.Template" + path + ".cshtml",
                };

            foreach (string thisPath in toCheck)
            {
                this.ValidationResults.Add(new ViewValidationResult(thisPath, fileService.Exists(thisPath)));
            }
        }

        #endregion Constructors

        #region Methods

        public string Result(bool SurpressError = false)
        {
            if (!SurpressError && !this.FoundPath)
            {
                throw new FileNotFoundException("No views found in the following paths: \r\n\r\n" + string.Join("\r\n", this.ValidationResults.Select(v => v.Path)));
            }

            return this.ValidationResults.FirstOrDefault(r => r.Exists)?.Path;
        }

        #endregion Methods
    }
}