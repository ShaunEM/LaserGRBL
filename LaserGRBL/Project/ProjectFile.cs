using LaserGRBLPlus.Settings;
using System.Collections.Generic;

namespace LaserGRBLPlus.Project
{

    public class ProjectFileObject
    {
        public List<FileObject> ProjectFiles { get; set; }
        public List<ProjectFileLayer> Layers { get; set; }
        public AppSettings Config { get; set; }
    }
    public class ProjectFileLayer
    {
        /// <summary>
        /// Index pointing the file used to create layer
        /// </summary>
        public int OrigFileObjectIndex { get; set; }
        public FileObject FileObject { get; set; }
        public LayerType LayerType { get; set; }
        public string LayerDescription { get; set; }
        public LayerConfig Config { get; set; }
    }
}
