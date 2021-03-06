﻿namespace CloudinaryDotNet.Actions
{
    using System.Collections.Generic;

    /// <summary>
    /// Stores a set of parameters for updating the access_mode of resources.
    /// </summary>
    public class UpdateResourceAccessModeParams : BaseParams
    {
        private List<string> m_publicIds = new List<string>();
        private ResourceType m_resourceType = ResourceType.Image;
        private string m_accessMode = "public";
        private string m_type = "upload";

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateResourceAccessModeParams"/> class.
        /// </summary>
        public UpdateResourceAccessModeParams()
        {
        }

        /// <summary>
        /// Update all resources with the given public IDs (array of up to 100 public_ids).
        /// </summary>
        public List<string> PublicIds
        {
            get { return m_publicIds; }
            set { m_publicIds = value; }
        }

        /// <summary>
        /// The new access mode to be set ("public" or "authenticated").
        /// </summary>
        public string AccessMode
        {
            get { return m_accessMode; }
            set { m_accessMode = value; }
        }

        /// <summary>
        /// Update resources with the given resource type. Default resource type: "image".
        /// </summary>
        public ResourceType ResourceType
        {
            get { return m_resourceType; }
            set { m_resourceType = value; }
        }

        /// <summary>
        /// Update resources with the given type. Default resource type: "upload".
        /// </summary>
        public string Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        private bool PublicIdsExist
        {
            get { return PublicIds != null && PublicIds.Count > 0; }
        }

        /// <summary>
        /// Validate object model.
        /// </summary>
        public override void Check()
        {
        }

        /// <summary>
        /// Maps object model to dictionary of parameters in cloudinary notation.
        /// </summary>
        /// <returns>Sorted dictionary of parameters.</returns>
        public override SortedDictionary<string, object> ToParamsDictionary()
        {
            SortedDictionary<string, object> dict = base.ToParamsDictionary();

            if (PublicIdsExist)
            {
                dict.Add("public_ids", PublicIds);
            }

            dict.Add("access_mode", m_accessMode);

            return dict;
        }
    }
}
