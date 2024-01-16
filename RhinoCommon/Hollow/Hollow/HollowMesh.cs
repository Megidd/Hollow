using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;

namespace Hollow
{
    public class HollowMesh : Command
    {
        public HollowMesh()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static HollowMesh Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "HollowMesh";

        private static RhinoDoc docCurrent; // Accessed by async post-process code.
        private static RhinoObject inObj = null; // Input object.

        // Includes all the temp folder and files.
        private Paths paths = null;
        
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // Initialize here to have new temp folder for every command run.
            paths = new Paths();

            docCurrent = doc; // Accessed by async post-process code.
            inObj = Helper.GetInputStl(paths.stlIn);
            if (inObj == null)
            {
                return Result.Failure;
            }

            bool infill = Helper.GetYesNoFromUser("Do you want infill for hollowed mesh?");

            float thickness = Helper.GetFloatFromUser(1.8, 0.0, 100.0, "Enter wall thickness for hollowing.");

            uint precision = Helper.GetUint32FromUser("Enter precision: Low=1, Medium=2, High=3", 2, 1, 3);
            switch (precision)
            {
                case 1:
                case 2:
                case 3:
                    break;
                default:
                    RhinoApp.WriteLine("Precision must be 1, 2, or 3 i.e. Low=1, Medium=2, High=3");
                    return Result.Failure;
            }

            // Prepare arguments as text fields.
            string args = "";
            args += paths.stlIn;
            args += " ";
            args += infill ? "true" : "false";
            args += " ";
            args += thickness.ToString();
            args += " ";
            args += precision.ToString();
            args += " ";
            args
            += paths.stlOut;

            Helper.RunLogic(paths.logic, args, PostProcess);

            RhinoApp.WriteLine("Process is started. Please wait...");

            return Result.Success;
        }

        private void PostProcess(object sender, EventArgs e)
        {
            try
            {
                RhinoApp.WriteLine("Post process started.");
                Mesh meshOut = Helper.LoadStlAsMesh(paths.stlOut);

                // Run the CheckValidity method on the mesh.
                MeshCheckParameters parameters = new MeshCheckParameters();
                // Enable all the checks:
                parameters.CheckForBadNormals = true;
                parameters.CheckForDegenerateFaces = true;
                parameters.CheckForDisjointMeshes = true;
                parameters.CheckForDuplicateFaces = true;
                parameters.CheckForExtremelyShortEdges = true;
                parameters.CheckForInvalidNgons = true;
                parameters.CheckForNakedEdges = true;
                parameters.CheckForNonManifoldEdges = true;
                parameters.CheckForRandomFaceNormals = true;
                parameters.CheckForSelfIntersection = true;
                parameters.CheckForUnusedVertices = true;
                Rhino.FileIO.TextLog log = new Rhino.FileIO.TextLog(paths.logs);
                bool isValid = meshOut.Check(log, ref parameters);
                bool hasInvalidVertexIndices = Helper.HasInvalidVertexIndices(meshOut);
                if (!isValid || hasInvalidVertexIndices)
                {
                    RhinoApp.WriteLine("Total cound of disjoint meshes: {0}", parameters.DisjointMeshCount);
                    RhinoApp.WriteLine("Output mesh cannot be added to scene due to being invalid");
                }
                else
                {
                    // If the mesh is valid, add it to the document.

                    // Create a new object attributes with the desired name
                    ObjectAttributes attributes = new ObjectAttributes();
                    attributes.Name = "Hollowed: " + inObj.Attributes.Name;

                    // Add the mesh to the document with the specified attributes
                    docCurrent.Objects.AddMesh(meshOut, attributes);

                    // Redraw the viewports to update the display
                    docCurrent.Views.Redraw();

                    if (docCurrent.Objects.Delete(inObj.Id, true))
                    {
                        // Good.
                    }
                    else
                    {
                        RhinoApp.WriteLine("Post process couldn't delete the original object.");
                    }
                }

                RhinoApp.WriteLine("Post process finished: hollowed mesh is added to scene.");
            }

            catch (Exception ex)
            {
                RhinoApp.WriteLine("Error on post process: {0}", ex.Message);
            }
        }
    }
}
