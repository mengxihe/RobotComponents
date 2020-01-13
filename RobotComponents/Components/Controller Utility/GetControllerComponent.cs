﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Resources;
using RobotComponents.Goos;
using RobotComponents.Parameters;

namespace RobotComponents.Components
{
    public class GetControllerComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GetController class.
        /// </summary>
        public GetControllerComponent()
          : base("Get Controller", "GC",
              "Connects to a virtual or real ABB controller to extract data from it."
                + System.Environment.NewLine +
                "RobotComponent V : " + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Controller Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Update", "U", "Update Controller", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // To do: replace generic parameter with an RobotComponents Parameter
            pManager.AddGenericParameter("Robot Controller", "RC", "Robotic Controller", GH_ParamAccess.item);
        }

        // Global component variables
        public int pickedIndex = 0;
        public static List<ABB.Robotics.Controllers.Controller> controllerInstance = new List<ABB.Robotics.Controllers.Controller>();
        public ControllerGoo controllerGoo;
        public string outputString = "";
        public bool fromMenu = false;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            bool update = false;

            // Catch the input data
            if (!DA.GetData(0, ref update)) { return; }

            // Pick a new controller when the input is toggled or the user selects one sfrom the menu
            if (update || fromMenu)
            {
                Rhino.RhinoApp.WriteLine("GetController call component");

                var controllerNow = GetController();
                if (controllerNow != null)
                {
                    controllerGoo = new ControllerGoo(controllerNow as ABB.Robotics.Controllers.Controller);
                    Rhino.RhinoApp.WriteLine("Controller");
                }
                else
                {
                    Rhino.RhinoApp.WriteLine("No controller");
                    return;
                }
            }
            else
            {
                Rhino.RhinoApp.WriteLine("Get Controller not call component in menu mode");
            }

            // Output
            DA.SetData(0, controllerGoo);
        }

        //  Additional methods
        #region additional methods
        /// <summary>
        /// Get the controller
        /// </summary>
        /// <returns> The picked controller. </returns>
        private ABB.Robotics.Controllers.Controller GetController()
        {
            // Initiate and clear variables
            controllerInstance.Clear();
            ABB.Robotics.Controllers.ControllerInfo[] controllers;
            List<string> controllerNames = new List<string>() { };

            // Scan for a network with controller
            ABB.Robotics.Controllers.Discovery.NetworkScanner scanner = new ABB.Robotics.Controllers.Discovery.NetworkScanner();
            scanner.Scan();

            // Try to get the controllers from the netwerok
            try
            {
                controllers = scanner.GetControllers();
            }
            // Else return no controllers
            catch (Exception)
            {
                Rhino.RhinoApp.WriteLine("Null Controller");
                controllers = null;
            }
            
            // Get the names of all the controllers in the scanned network
            for (int i = 0; i < controllers.Length; i++)
            {
                controllerInstance.Add(ABB.Robotics.Controllers.ControllerFactory.CreateFrom(controllers[i]));
                controllerNames.Add(controllerInstance[i].Name);
            }

            // Automatically pick the controller when one controller is available. 
            if (controllerNames.Count == 1)
            {
                pickedIndex = 0;
            }
            // Display the form and let the user pick a controller when more then one controller is available. 
            else if (controllerNames.Count > 1)
            {
                // Display the form and return the index of the picked controller. 
                pickedIndex = DisplayForm(controllerNames);

                // Return a null value when the picked index is incorrect. 
                if (pickedIndex < 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Station picked from menu!");
                    return null;
                }
            }

            // Return a null value when no controller was found
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Station found!");
                return null;
            }

            // Select the picked controller
            return controllerInstance[pickedIndex];
        }

        /// <summary>
        /// This method displays the form and return the index number of the picked controlller.
        /// </summary>
        /// <param name="controllerNames"> A list with controller names. </param>
        /// <returns> The index number of the picked controller. </returns>
        private int DisplayForm(List<string> controllerNames)
        {
            // Create the form with all the available controller names
            PickControllerForm frm = new PickControllerForm(controllerNames);

            // Display the form
            Grasshopper.GUI.GH_WindowsFormUtil.CenterFormOnEditor(frm, false);
            frm.ShowDialog();

            // Return the index number of the picked controller
            return PickControllerForm.stationIndex;
        }

        /// <summary>
        /// Adds the additional item "Pick controller" to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            // Add menu separator
            Menu_AppendSeparator(menu);

            // Create the menu item
            menu.Items.Add("Pick Controller", null, MenuItemClick);

            // Add the menu item
            base.AppendAdditionalMenuItems(menu);
        }

        /// <summary>
        /// Registers the event when the custom menu item is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        public void MenuItemClick(object sender, EventArgs e)
        {
            fromMenu = true;
            ExpireSolution(true);
            fromMenu = false;
        }
        #endregion

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.GetController_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6fd61c34-c262-4d10-b6e5-5c1762411aac"); }
        }
    }
}