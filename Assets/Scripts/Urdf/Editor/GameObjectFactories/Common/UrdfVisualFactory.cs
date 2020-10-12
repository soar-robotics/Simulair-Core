﻿/*
© Siemens AG, 2018
Author: Suzannah Smith (suzannah.smith@siemens.com)
Modified by: Samuel Lindgren

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

<http://www.apache.org/licenses/LICENSE-2.0>.

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;

namespace RosSharp.Urdf.Editor
{
    public static class UrdfVisualFactory
    {
        public static void Create(Transform parent, GeometryTypes type)
        {
            GameObject visualObject = new GameObject("unnamed");
            visualObject.transform.SetParentAndAlign(parent);
            UrdfVisual urdfVisual = visualObject.AddComponent<UrdfVisual>();

            urdfVisual.GeometryType = type;
            UrdfGeometryVisualFactory.Create(visualObject.transform, type);
            UnityEditor.EditorGUIUtility.PingObject(visualObject);
        }

        public static void Create(Transform parent, UrdfLink.Visual visual)
        {
            GameObject visualObject = new GameObject(visual.name ?? "unnamed");
            visualObject.transform.SetParentAndAlign(parent);
            UrdfVisual urdfVisual = visualObject.AddComponent<UrdfVisual>();

            urdfVisual.GeometryType = UrdfGeometryFactory.GetGeometryType(visual.geometry);
            UrdfGeometryVisualFactory.Create(visualObject.transform, urdfVisual.GeometryType, visual.geometry);

            UrdfMaterialFactory.SetUrdfMaterial(visualObject, visual.material);
            UrdfOrigin.ImportOriginData(visualObject.transform, visual.origin);
        }

        public static void AddCorrespondingCollision(this UrdfVisual urdfVisual)
        {
            // TODO(sam): Remove completly if not used
            //UrdfCollisions collisions = urdfVisual.GetComponentInParent<UrdfLink>().GetComponentInChildren<UrdfCollisions>();
            //UrdfCollisionExtensions.Create(collisions.transform, urdfVisual.GeometryType, urdfVisual.transform);
        }

        public static UrdfLink.Visual ExportVisualData(this UrdfVisual urdfVisual)
        {
            UrdfGeometryFactory.CheckForUrdfCompatibility(urdfVisual.transform, urdfVisual.GeometryType);

            UrdfLink.Geometry geometry = UrdfGeometryFactory.ExportGeometryData(urdfVisual.GeometryType, urdfVisual.transform);

            UrdfLink.Visual.Material material = null;
            if (!(geometry.mesh != null && geometry.mesh.filename.ToLower().EndsWith(".dae"))) //Collada files contain their own materials
                material = UrdfMaterialFactory.ExportMaterialData(urdfVisual.GetComponentInChildren<MeshRenderer>().sharedMaterial);

            string visualName = urdfVisual.name == "unnamed" ? null : urdfVisual.name;

            return new UrdfLink.Visual(geometry, visualName, UrdfOrigin.ExportOriginData(urdfVisual.transform), material);
        }
    }
}