﻿// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.Utils;

namespace RobotComponents.BaseClasses.Definitions.Presets
{
    public static class IRB4600_20_250
    {
        /// <summary>
        /// Defines the IRB4600-20/2.5
        /// </summary>
        /// <param name="positionPlane"></param>
        /// <param name="tool"></param>
        /// <param name="externalAxis"></param>
        /// <returns></returns>
        public static RobotInfo GetRobotInfo(string name, Plane positionPlane, RobotTool tool, List<ExternalAxis> externalAxis = null)
        {
            List<Mesh> meshes = GetMeshes();
            List<Plane> axisPlanes = GetAxisPlanes();
            List<Interval> axisLimits = GetAxisLimits();
            Plane mountingFrame = GetToolMountingFrame();

            // Override position plane when an external linear axis is coupled
            for (int i = 0; i < externalAxis.Count; i++)
            {
                if (externalAxis[i] is ExternalLinearAxis)
                {
                    positionPlane = (externalAxis[i] as ExternalLinearAxis).AttachmentPlane;
                    break;
                }
            }

            RobotInfo robotInfo = new RobotInfo(name, meshes, axisPlanes, axisLimits, Plane.WorldXY, mountingFrame, tool, externalAxis);
            Transform trans = Transform.PlaneToPlane(Plane.WorldXY, positionPlane);
            robotInfo.Transfom(trans);

            return robotInfo;
        }

        /// <summary>
        /// Defines the base and link meshes a robot coordinate space. 
        /// </summary>
        /// <returns> Returns a list with meshes. </returns>
        public static List<Mesh> GetMeshes()
        {
            // Initialize 
            List<Mesh> meshes = new List<Mesh>() { };
            string linkString;

            // Base
            linkString = RobotComponents.Properties.Resources.IRB4600_20_2_50_link_0;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 1
            linkString = RobotComponents.Properties.Resources.IRB4600_20_2_50_link_1;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 2
            linkString = RobotComponents.Properties.Resources.IRB4600_20_2_50_link_2;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 3
            linkString = RobotComponents.Properties.Resources.IRB4600_20_2_50_link_3;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 4
            linkString = RobotComponents.Properties.Resources.IRB4600_20_2_50_link_4;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 5
            linkString = RobotComponents.Properties.Resources.IRB4600_20_2_50_link_5;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));
            // Axis 6
            linkString = RobotComponents.Properties.Resources.IRB4600_20_2_50_link_6;
            meshes.Add((Mesh)HelperMethods.ByteArrayToObject(System.Convert.FromBase64String(linkString)));

            return meshes;
        }

        /// <summary>
        /// Defines the axis planes in robot coordinate space.
        /// </summary>
        /// <returns> Returns a list with planes. </returns>
        public static List<Plane> GetAxisPlanes()
        {
            List<Plane> axisPlanes = new List<Plane>() { };

            // Axis 1
            axisPlanes.Add(new Plane(
                new Point3d(0.00, 0.00, 0.00),
                new Vector3d(0.00, 0.00, 1.00)));
            // Axis 2
            axisPlanes.Add(new Plane(
                new Point3d(175.0, 0.00, 495.00),
                new Vector3d(0.00, 1.00, 0.00)));
            // Axis 3
            axisPlanes.Add(new Plane(
                new Point3d(175.00, 0.00, 495.00 + 1095.00),
                new Vector3d(0.00, 1.00, 0.00)));
            // Axis 4
            axisPlanes.Add(new Plane(
                new Point3d(505.60, 0.00, 495.00 + 1095.00 + 175.00),
                new Vector3d(1.00, 0.00, 0.00)));
            // Axis 5
            axisPlanes.Add(new Plane(
                new Point3d(175.0 + 1230.5, 0.00, 495.00 + 1095.00 + 175.00),
                new Vector3d(0.00, 1.00, 0.00)));
            // Axis 6
            axisPlanes.Add(new Plane(
                new Point3d(175.0 + 1230.5 + 85.0, 0.00, 495.00 + 1095.00 + 175.00),
                new Vector3d(1.00, 0.00, 0.00)));

            return axisPlanes;
        }

        /// <summary>
        /// Defines the axis limits. 
        /// </summary>
        /// <returns> Returns a list with intervals. </returns>
        public static List<Interval> GetAxisLimits()
        {
            List<Interval> axisLimits = new List<Interval> { };

            axisLimits.Add(new Interval(-180, 180));
            axisLimits.Add(new Interval(-90, 150));
            axisLimits.Add(new Interval(-180, 75));
            axisLimits.Add(new Interval(-400, 400));
            axisLimits.Add(new Interval(-120, 120));
            axisLimits.Add(new Interval(-400, 400));

            return axisLimits;
        }

        /// <summary>
        /// Defines the tool mounting frame in robot coordinate space. 
        /// </summary>
        /// <returns> Returns the mounting frame. </returns>
        public static Plane GetToolMountingFrame()
        {
            Plane mountingFrame = new Plane(
                new Point3d(175.0 + 1230.5 + 85.0, 0.00, 495.00 + 1095.00 + 175.00),
                new Vector3d(1.00, 0.00, 0.00));

            mountingFrame.Rotate(Math.PI* -0.5, mountingFrame.Normal);

            return mountingFrame;
        }
    }
}