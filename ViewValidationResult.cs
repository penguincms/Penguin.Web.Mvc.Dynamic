namespace Penguin.Web.Mvc.Dynamic
{
    public class ViewValidationResult
    {
        #region Properties

        public bool Exists { get; set; }

        public string Path { get; set; }

        #endregion Properties

        #region Constructors

        public ViewValidationResult(string path, bool exists)
        {
            this.Path = path;
            this.Exists = exists;
        }

        #endregion Constructors

        #region Methods

        public override string ToString() => $"{this.Path} - {this.Exists}";

        #endregion Methods
    }
}