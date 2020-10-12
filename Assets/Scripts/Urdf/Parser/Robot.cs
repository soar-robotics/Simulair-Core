﻿/*
© Siemens AG, 2017
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

<http://www.apache.org/licenses/LICENSE-2.0>.

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

Added urdf from string option (C) Dyno Robotics, 2019, Samuel Lindgren (samuel@dynorobotics.se)>
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace RosSharp.Urdf
{
    public class Robot
    {
        public string filename;
        public string name;
        public UrdfLink root;
        public List<UrdfLink.Visual.Material> materials;

        public List<UrdfLink> links;
        public List<UrdfJoint> joints;
        public List<Plugin> plugins;
        public List<UrdfUnityComponent> unityComponets;

        public Robot()
        {
        }

        public void ConstructFromString(string urdfString, string name = "")
        {
            XDocument xdoc = XDocument.Parse(urdfString);
            XElement node = xdoc.Element("robot");

            Construct(node, name);
        }

        public void ConstructFromFile(string filename, string name = "")
        {
            this.filename = filename;
            XDocument xdoc = XDocument.Load(filename);
            XElement node = xdoc.Element("robot");

            Construct(node, name);
        }

        private void Construct(XElement node, string name)
        {
            if (name == "")
            {
                this.name = node.Attribute("name").Value;
            }
            else
            {
                this.name = name;
            }

            materials = ReadMaterials(node);
            links = ReadLinks(node);
            joints = ReadJoints(node);
            plugins = ReadPlugins(node);
            unityComponets = ReadUnityComponents(node);

            // build tree structure from link ,joint and component lists:
            foreach (UrdfLink link in links)
            {
                link.joints = joints.FindAll(v => v.parent == link.name);
                link.unityComponents = unityComponets.FindAll(v => v.reference == link.name);
            }
            foreach (UrdfJoint joint in joints)
            {
                joint.ChildLink = links.Find(v => v.name == joint.child);
            }

            // save root node only:
            root = FindRootLink(links, joints);
        }

        private static List<UrdfLink.Visual.Material> ReadMaterials(XElement node)
        {
            var materials =
                from child in node.Elements("material")
                select new UrdfLink.Visual.Material(child);
            return materials.ToList();
        }

        private static List<UrdfLink> ReadLinks(XElement node)
        {
            var links =
                from child in node.Elements("link")
                select new UrdfLink(child);
            return links.ToList();
        }

        private static List<UrdfJoint> ReadJoints(XElement node)
        {
            var joints =
                from child in node.Elements("joint")
                select new UrdfJoint(child);
            return joints.ToList();
        }

        private List<Plugin> ReadPlugins(XElement node)
        {
            var plugins =
                from child in node.Elements()
                where child.Name != "link" && child.Name != "joint" && child.Name != "material"
                select new Plugin(child.ToString());
            return plugins.ToList();
        }
        private List<UrdfUnityComponent> ReadUnityComponents(XElement node)
        {
            var unityComponents = new List<UrdfUnityComponent>();

            foreach(var child in node.Elements("unity"))
            {
                string reference = (string)child.Attribute("reference");
                foreach(var component in child.Elements("component"))
                {
                    unityComponents.Add(new UrdfUnityComponent(reference, component));
                }
            }

            return unityComponents;
        }

        private static UrdfLink FindRootLink(List<UrdfLink> links, List<UrdfJoint> joints)
        {
            if (joints.Count == 0)
                return links[0];

            UrdfJoint joint = joints[0];
            string parent;
            do
            {
                parent = joint.parent;
                joint = joints.Find(v => v.child == parent);
            }
            while (joint != null);
            return links.Find(v => v.name == parent);
        }

        public void WriteToUrdf()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            XmlWriterSettings settings = new XmlWriterSettings { Indent = true, NewLineOnAttributes = false };

            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("robot");
                writer.WriteAttributeString("name", name);

                foreach (var material in materials)
                    material.WriteToUrdf(writer);
                foreach (var link in links)
                    link.WriteToUrdf(writer);
                foreach (var joint in joints)
                    joint.WriteToUrdf(writer);
                foreach (var plugin in plugins)
                    plugin.WriteToUrdf(writer);
                
                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Close();
            }
        }
    }
}
