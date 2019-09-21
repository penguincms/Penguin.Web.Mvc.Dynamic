using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Abstractions.Wrappers;
using Penguin.Services.Files;
using System;
using System.Diagnostics.Contracts;

namespace Penguin.Web.Mvc.Dynamic
{
    /// <summary>
    /// Settings for the dynamic renderer
    /// </summary>
    public class DynamicRendererSettings
    {
        /// <summary>
        /// A link to the base path for views for the editor.
        /// Points to Paths.EditBase
        /// </summary>
        public string BasePath { get; set; } = Paths.EditBase;

        /// <summary>
        /// The name of the view used to render dynamic objects
        /// Default: Edit
        /// </summary>
        public string DynamicViewName { get; set; } = "Edit";

        /// <summary>
        /// Only render Dynamic or Exact Matches. No intermediate types
        /// </summary>
        public bool ExactOnly { get; set; }

        /// <summary>
        /// A fileService implementation for checking for views on disk
        /// </summary>
        public FileService FileService { get; set; }

        /// <summary>
        /// The IMetaProperty that references the object being resolved
        /// </summary>
        public IMetaProperty Property { get; set; }

        /// <summary>
        /// Render base types before inherited types
        /// </summary>
        public bool StartAtBottom { get; set; } = false;

        /// <summary>
        /// An optional override type for the renderer
        /// </summary>
        public IMetaType Type { get; set; }

        /// <summary>
        /// If no type is set, the renderer can attempt to use this string value to determine routing
        /// </summary>
        public string TypeFullName { get; set; }

        /// <summary>
        /// Constructs a new instance of the settings object
        /// </summary>
        /// <param name="type">An optional override type for the renderer</param>
        /// <param name="property">The IMetaProperty that references the object being resolved</param>
        /// <param name="fileService">A file service used for checking for the existance of views</param>
        public DynamicRendererSettings(IMetaType type, IMetaProperty property, FileService fileService)
        {
            Contract.Requires(type != null);

            this.Type = type;
            this.TypeFullName = this.Type.FullName;
            this.Property = property;
            this.FileService = fileService;
        }

        /// <summary>
        /// Constructs a new instance of the settings object
        /// </summary>
        /// <param name="property">The IMetaProperty that references the object being resolved</param>
        /// <param name="fileService">A file service used for checking for the existance of views</param>
        public DynamicRendererSettings(IMetaProperty property, FileService fileService)
        {
            Contract.Requires(property != null);

            this.Type = property.Type;
            this.TypeFullName = this.Type.FullName;
            this.Property = property;
            this.FileService = fileService;
        }

        /// <summary>
        /// Constructs a new instance of the settings object
        /// </summary>
        /// <param name="type">An optional override type for the renderer</param>
        /// <param name="fileService">A file service used for checking for the existance of views</param>
        public DynamicRendererSettings(Type type, FileService fileService) : this(new MetaTypeHolder(type), null, fileService)
        {
        }

        /// <summary>
        /// Constructs a new instance of the settings object
        /// </summary>
        /// <param name="property">The IMetaProperty that references the object being resolved</param>
        /// <param name="fileService">A file service used for checking for the existance of views</param>
        public DynamicRendererSettings(System.Reflection.PropertyInfo property, FileService fileService) : this(new MetaPropertyHolder(property), fileService)
        {
        }

        /// <summary>
        /// Constructs a new instance of the settings object
        /// </summary>
        /// <param name="o">An IMetaObject instance to render out</param>
        /// <param name="fileService">A file service used for checking for the existance of views</param>
        public DynamicRendererSettings(IMetaObject o, FileService fileService)
        {
            Contract.Requires(o != null);

            this.FileService = fileService;
            this.Type = o.Type;
            this.TypeFullName = this.Type.FullName;
        }

        /// <summary>
        /// Constructs a new instance of the settings object
        /// </summary>
        /// <param name="typeFullName">If no type is set, the renderer can attempt to use this string value to determine routing</param>
        /// <param name="fileService">A file service used for checking for the existance of views</param>
        public DynamicRendererSettings(string typeFullName, FileService fileService)
        {
            this.FileService = fileService;
            this.TypeFullName = typeFullName;
        }
    }
}