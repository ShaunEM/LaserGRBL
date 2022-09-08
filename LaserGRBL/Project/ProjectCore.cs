using LaserGRBLPlus.Libraries.GRBLLibrary;
using LaserGRBLPlus.Project;
using System.Collections.Generic;

namespace LaserGRBLPlus
{
    public class ProjectCore
    {
        public List<FileObject> ProjectFiles;
        public List<Layer> layers;
        public GlobalConfig Config;

        //[Obsolete("use grblFileGlobal")]
        //public ProgramRange programRange; 
        
        public GrblFileGlobal grblFileGlobal;
       
        public ProjectCore(float tableWidth, float tableHeight)
        {
            
            this.ProjectFiles = new List<FileObject>();
            this.Config = new GlobalConfig();
            this.layers = new List<Layer>();
            //this.programRange = new ProgramRange();
            //this.programRange.UpdateXYRange(new GrblElement('X', 0), new GrblElement('Y', 0), false);


            // Move grblFile globals into here (currenlty has a copy on each layer)
            this.grblFileGlobal = new GrblFileGlobal();
        }
        public int AddFileToProject(string fileName, byte[] data = null)
        {
            ProjectFiles.Add(new FileObject(fileName, data));
            return ProjectFiles.Count - 1;
        }
        //public void RemoveFileFromProject(int fileIndex)
        //{
        //    ProjectFiles.RemoveAt(fileIndex);
        //}
        //public FileObject GetFileObject(int fileIndex)
        //{
        //    return ProjectFiles[fileIndex];
        //}
        ////public FileObject GetLayerFileObject(int layerIndex)
        //{
        //    return ProjectFiles[layers[layerIndex].FileObject .FileObjectIndex];
        //}




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
        //public void SetLayerSetting(Dictionary<string, object> setting, int layerIndex)
        //{
        //    this.layers[layerIndex].  Settings = setting;
        //}
        //public Dictionary<string, object> GetLayerSettings(int layerIndex)
        //{
        //    return (Dictionary<string, object>)this.layers[layerIndex].Settings;
        //}







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
            ProjectFile.WriteToJsonFile(fileName, projectFileFormat);
        }

        internal void UnSelectAllLayers()
        {
            foreach(Layer layer in layers)
            {
                layer.Selected = false;
            }
        }


        //public void LoadProject(string fileName)
        //{
        //    // reset all
        //    this.layers.Clear();

        //    // Load project files
        //    ProjectFileFormat projectFileFormat = ProjectFile.ReadFromJsonFile<ProjectFileFormat>(fileName);
        //    this.Config = projectFileFormat.Config;
        //    this.ProjectFiles = projectFileFormat.ProjectFiles;

        //    // load layers
        //    foreach (ProjectFileLayer layer in projectFileFormat.Layers)
        //    {
        //        this.layers.Add(new Layer()
        //        {
        //            Config = layer.Config,
        //            FileObjectIndex = layer.FileObjectIndex,
        //            LayerDescription = layer.LayerDescription,
        //            PreviewColor = layer.PreviewColor,
        //            GRBLFile = new GrblFile(this.programRange),  // TODO: add range
        //            XElement = layer.xElement
        //        });
        //    }
        //}
    }

    //FileName = filename,
    //LayerDescription = $"{Path.GetFileName(filename)}",
    //PreviewColor = colorLayer.Item2,
    //GRBLFile = new GrblFile(ProjectCore.programRange),
    //XElement = colorLayer.Item1







}