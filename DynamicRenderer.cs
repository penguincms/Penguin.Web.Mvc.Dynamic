using Penguin.Extensions.Collections;
using Penguin.Extensions.String;
using Penguin.Reflection.Abstractions;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Abstractions.Wrappers;
using Penguin.Services.Files;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Penguin.Web.Mvc.Dynamic
{
    /// <summary>
    /// Analyzes a serialized object and returns a list of validated view paths representing 
    /// Prefered rendering paths
    /// </summary>
    public class DynamicRenderer
    {
        #region Properties

        /// <summary>
        /// A list of ViewResults that exist on disk
        /// </summary>
        public IEnumerable<ViewValidationResult> ExistingHandlers => this.Results.Where(r => r.Exists);

        /// <summary>
        /// Figure out what this does and comment it
        /// </summary>
        public bool HasHtml { get; set; }

        /// <summary>
        /// True if there are any views set up to handle this object
        /// </summary>
        public bool HasMatch => this.Results.Any(r => r.Exists);

        /// <summary>
        /// True if the only view that can handle this object is the recursive view
        /// </summary>
        public bool IsDynamic { get; set; }

        /// <summary>
        /// Chooses the first existing view with the highest priority
        /// </summary>
        public ViewValidationResult Match => this.Results.Where(r => r.Exists).First();

        /// <summary>
        /// The path to the view of highest matching priority
        /// </summary>
        public string MatchedPath => this.Match.Path;

        /// <summary>
        /// A list of results for potential paths, as well as whether or not the paths exist on disk
        /// </summary>
        public List<ViewValidationResult> Results { get; set; }

        #endregion Properties

        #region Classes

        /// <summary>
        /// Settings for the dynamic renderer
        /// </summary>
        public class Settings
        {
            #region Properties

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

            #endregion Properties

            #region Constructors

            /// <summary>
            /// Constructs a new instance of the settings object
            /// </summary>
            /// <param name="type">An optional override type for the renderer</param>
            /// <param name="property">The IMetaProperty that references the object being resolved</param>
            /// <param name="fileService">A file service used for checking for the existance of views</param>
            public Settings(IMetaType type, IMetaProperty property, FileService fileService)
            {
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
            public Settings(IMetaProperty property, FileService fileService)
            {
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
            public Settings(Type type, FileService fileService) : this(new MetaTypeHolder(type), null, fileService)
            {
            }

            /// <summary>
            /// Constructs a new instance of the settings object
            /// </summary>
            /// <param name="property">The IMetaProperty that references the object being resolved</param>
            /// <param name="fileService">A file service used for checking for the existance of views</param>
            public Settings(System.Reflection.PropertyInfo property, FileService fileService) : this(new MetaPropertyHolder(property), fileService)
            {
            }

            /// <summary>
            /// Constructs a new instance of the settings object
            /// </summary>
            /// <param name="o">An IMetaObject instance to render out</param>
            /// <param name="fileService">A file service used for checking for the existance of views</param>
            public Settings(IMetaObject o, FileService fileService)
            {
                this.FileService = fileService;
                this.Type = o.Type;
                this.TypeFullName = this.Type.FullName;
            }

            /// <summary>
            /// Constructs a new instance of the settings object
            /// </summary>
            /// <param name="typeFullName">If no type is set, the renderer can attempt to use this string value to determine routing</param>
            /// <param name="fileService">A file service used for checking for the existance of views</param>
            public Settings(string typeFullName, FileService fileService)
            {
                this.FileService = fileService;
                this.TypeFullName = typeFullName;
            }

            #endregion Constructors
        }

        #endregion Classes

        #region Constructors

        /// <summary>
        /// Constructs a new instance of this renderer
        /// </summary>
        /// <param name="type">An optional override type</param>
        /// <param name="property">The IMetaProperty leading to this serialized object</param>
        /// <param name="fileService">A file service used for checking for the existance of views</param>
        public DynamicRenderer(IMetaType type, IMetaProperty property, FileService fileService) : this(new Settings(type, property, fileService))
        {
        }

        /// <summary>
        /// Constructs a new instance of this renderer
        /// </summary>
        /// <param name="property">The IMetaProperty leading to this serialized object</param>
        /// <param name="fileService">A file service used for checking for the existance of views </param>
        public DynamicRenderer(IMetaProperty property, FileService fileService) : this(new Settings(property, fileService))
        {
        }

        /// <summary>
        /// Constructs a new instance of this renderer
        /// </summary>
        /// <param name="settings">A collection of settings for the dynamic renderer</param>
        public DynamicRenderer(Settings settings)
        {
            List<string> renderingOrder = null;

            if (settings.Type is null)
            {
                renderingOrder = new List<string> { settings.TypeFullName };
            }
            else
            {
                renderingOrder = new List<string>();

                IMetaType CascadeType = settings.Type.CoreType == CoreType.Collection ? settings.Type.CollectionType : settings.Type;

                string Prepend = string.Empty;

                if (settings.Type.CoreType == CoreType.Collection)
                {
                    if (settings.Type.IsArray)
                    {
                        Prepend = "Array.";
                    }
                    else
                    {
                        Prepend = $"{settings.Type.Namespace}.{settings.Type.Name.To("`")}.";
                    }
                }

                do
                {
                    string TypeName = CascadeType.FullName;

                    if (settings.Type.CoreType == CoreType.Collection && settings.Type.IsArray)
                    {
                        TypeName = CascadeType.FullName.To("[");
                    }

                    if (settings.Property != null && settings.Property.Attributes.AnyNotNull())
                    {
                        //search based on attribute
                        foreach (IMetaAttribute a in settings.Property.Attributes.Where(a => !a.IsInherited))
                        {
                            renderingOrder.Add($"{Prepend}@{a.Instance.Value}");
                        }
                    }

                    renderingOrder.Add($"{Prepend}{TypeName}");

                    if (CascadeType.Attributes.AnyNotNull())
                    {
                        //search based on attribute
                        foreach (IMetaAttribute a in CascadeType.Attributes.Where(a => !a.IsInherited))
                        {
                            renderingOrder.Add($"{Prepend}@{a.Instance.Value}");
                        }
                    }

                    CascadeType = CascadeType.BaseType;
                }
                while (!settings.ExactOnly && CascadeType != null);

                //for all objects in namespace
                renderingOrder.Add($"{settings.Type.Namespace}.{Prepend}$Object");

                if (settings.StartAtBottom)
                {
                    renderingOrder.Reverse();
                }
            }

            this.Results = new List<ViewValidationResult>();

            foreach (string thisType in renderingOrder)
            {
                string @namespace = thisType.Remove(Constants.RootNamespace + ".").Replace(".", "/");
                ViewPathValidation pathValidation = new ViewPathValidation(settings.BasePath + @namespace, settings.FileService);

                foreach (ViewValidationResult result in pathValidation.ValidationResults)
                {
                    this.Results.Add(result);
                }
            }

            if (!this.HasMatch)
            {
                this.IsDynamic = true;
                ViewPathValidation pathValidation = new ViewPathValidation(settings.BasePath + settings.DynamicViewName, settings.FileService);

                foreach (ViewValidationResult result in pathValidation.ValidationResults)
                {
                    this.Results.Add(result);
                }
            }
        }

        #endregion Constructors
    }
}