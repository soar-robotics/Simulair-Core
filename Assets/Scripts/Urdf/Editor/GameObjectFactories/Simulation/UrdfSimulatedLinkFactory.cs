﻿/*
© Dyno Robotics, 2019
Author: Samuel Lindgren (samuel@dynorobotics.se)
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
using System;

using UnityEditor;
using UnityEditor.Compilation;

namespace RosSharp.Urdf.Editor
{
    public static class UrdfSimulatedLinkFactory
    {
        public static void Create(Transform parent, UrdfLink link = null, UrdfJoint joint = null)
        {
            GameObject linkObject = new GameObject("link");
            linkObject.transform.SetParentAndAlign(parent);

            if (link != null)
            {
                linkObject.RecursiveImportSimulationLinkData(link, joint);
            }
        }

        private static void RecursiveImportSimulationLinkData(this GameObject linkObject, UrdfLink link, UrdfJoint joint)
        {
            linkObject.gameObject.name = link.name;

            linkObject.AddComponent<UrdfSimulatedLink>();

            UrdfVisualsFactory.Create(linkObject.transform, link?.visuals);
            UrdfCollisionsFactory.Create(linkObject.transform, link?.collisions);

            // TODO(sam): contidionally add Rigidbodies to links from inertia?
            //if (link?.inertial != null)
            //{
            //    UrdfInertial.Create(linkObject, link.inertial);
            //    linkObject.GetComponent<Rigidbody>().isKinematic = true;
            //}

            foreach (var unityComponent in link.unityComponents)
            {
                string name = unityComponent.name;

                // TODO(sam): Figure out why Reflection (Type.GetType) does not seem to work. Assembly name? Use custom dict for now...
                if (UrdfCustomComponentTypes.Dictionary.ContainsKey(name))
                {
                    linkObject.AddComponent(UrdfCustomComponentTypes.Dictionary[name]);
                } else
                {
                    Debug.LogWarning("Urdf unity component with name " + name + " on link " + link.name + " was not found in project. Ignoring!");
                }
            }

            if (joint?.origin != null)
            {
                UrdfOrigin.ImportOriginData(linkObject.transform, joint.origin);
            }

            if (joint != null)
            {
                UrdfSimulatedJointFactory.Create(linkObject, joint);
            }

            foreach (UrdfJoint childJoint in link.joints)
            {
                UrdfLink child = childJoint.ChildLink;
                Create(linkObject.transform, child, childJoint);
            }
        }
    }
}
