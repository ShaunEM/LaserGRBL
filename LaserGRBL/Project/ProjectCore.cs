using GRBLLibrary;
using LaserGRBLPlus.Libraries;
using LaserGRBLPlus.Project;
using LaserGRBLPlus.Settings;
using System.Collections.Generic;

namespace LaserGRBLPlus
{
    public class ProjectCore
    {

        /// <summary>
        /// App related configuration
        /// </summary>
        public AppSettings Config { get; set; }


        public List<FileObject> ProjectFiles;
        public List<Layer> layers;
        public GrblFileGlobal grblFileGlobal;
       
        public ProjectCore(float tableWidth, float tableHeight)
        {
            this.ProjectFiles = new List<FileObject>();
            this.Config = new AppSettings();
            this.layers = new List<Layer>();

            // Move grblFile globals into here (currenlty has a copy on each layer)
            this.grblFileGlobal = new GrblFileGlobal();
        }
        public int AddFileToProject(string fileName, byte[] data = null)
        {
            ProjectFiles.Add(new FileObject(fileName, data));
            return ProjectFiles.Count - 1;
        }
        public void RemoveFileFromProject(int index)
        {
            ProjectFiles[index] = null;
        }

        public int AddLayer(Layer layer)
        {
            layers.Add(layer);
            return layers.Count - 1; // retun added index
        }
        public int GetLayerIndex(string layerName)
        {
            return this.layers.FindIndex(n => n.LayerDescription == layerName);
        }
        public Layer GetLayer(int layerIndex)
        {
            return this.layers[layerIndex];
        }








        public void SaveProject(string fileName)
        {
            // Add Project files 
            ProjectFileObject projectFileFormat = new ProjectFileObject
            {
                ProjectFiles = this.ProjectFiles,
                Config = this.Config,
                Layers = new List<ProjectFileLayer>(),
            };

            // Add Layers
            foreach (Layer layer in this.layers)
            {
                projectFileFormat.Layers.Add(new ProjectFileLayer()
                {
                    LayerDescription = layer.LayerDescription,      // Description
                    Config = layer.Config,                          // Layer Configuration
                    FileObject = layer.FileObject,
                    OrigFileObjectIndex = layer.OrigFileObjectIndex,
                    LayerType = layer.LayerType,
                });
            }
            JSONFileManager.Save(fileName, projectFileFormat);
        }

        internal void UnSelectAllLayers()
        {
            foreach(Layer layer in layers)
            {
                layer.Selected = false;
            }
        }

    }
}