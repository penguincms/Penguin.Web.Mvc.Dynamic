namespace Penguin.Web.Mvc.Dynamic
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class Paths
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// The path to check for admin dynamic rendering views
        /// </summary>
        public static string AdminRenderBase { get; set; } = "/Areas/Admin/Views/Render/";

        /// <summary>
        /// The path that the dynamic wrapper resides in
        /// </summary>
        public static string EditBase { get; set; } = "/Areas/Admin/Views/Edit/";

        /// <summary>
        /// The path that the dynamic client visible renderers exists in
        /// </summary>
        public static string RenderBase { get; set; } = "/Views/Render/";
    }
}