using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using WinCopies.Util;

namespace WinCopies.SettingsManagement
{
    public static class SettingsManagement
    {

        public static string DefaultXmlSettings => Properties.Resources.defaultSettings;

        private static string ReturnValidXmlNodeName(string typeOrPropertyName)

        {

            string nodeName = typeOrPropertyName.Substring(0, 1).ToLower();

            if (typeOrPropertyName.Length > 1) nodeName += typeOrPropertyName.Substring(1);

            return nodeName;

        }

        private static XmlNode GetPropertyXmlNode(string[] path)

        {

            XmlNode xmlNode = Settings["settings"];



            foreach (string item in path)

            {

                xmlNode = xmlNode[ReturnValidXmlNodeName(item)];

                if (xmlNode == null)

                    return null;

            }



            return xmlNode;

        }

        public static string GetProperty(string[] path)

        {

            XmlNode xmlNode = GetPropertyXmlNode(path);

            if (xmlNode == null)

                return null;

            return xmlNode.InnerText;

        }

        internal static bool GetBoolResult(string[] path) => bool.TryParse(GetProperty(path), out bool result) ? result : false;

        public static void SetProperty(string[] path, object value)

        {

            XmlNode xmlNode = GetPropertyXmlNode(path);

            if (xmlNode == null)

                return;

            xmlNode.InnerText = value?.ToString();

        }

        private static bool CreateSettingFile(XmlDocument xmlDoc, string savingPath, bool createDirectory)

        {

            try
            {

                if (createDirectory)

                    Directory.CreateDirectory(Path.GetDirectoryName(savingPath));

                xmlDoc.LoadXml(Properties.Resources.defaultSettings);

                xmlDoc.Save(SavePath);

            }

            catch (Exception)
            {

                return false;

            }

            return true;

        }

        private static Dictionary<string[], string> GetSettingsToSave(object obj)

        {

            Dictionary<string[], string> dico = new Dictionary<string[], string>();

            Type obj_Type = obj.GetType();

            PropertyInfo[] properties = obj_Type.GetProperties();

            foreach (PropertyInfo property in properties)

            {

                // if (property.GetCustomAttributes(typeof(SerializablePropertyAttribute)).Count((Attribute attribute) => true) > 0)

                if (property.GetCustomAttributes(typeof(SerializablePropertyAttribute), true).Length > 0)

                {

                    object value = property.GetValue(obj);

                    if (value is IEnumerable<CheckableObject>)

                        foreach (CheckableObject item in (IEnumerable<CheckableObject>)value)

                        {

                            string _value = item.IsChecked.ToString().ToLower();

                            string item_Value = ((string)item.Value).Substring(1);

                            if (item_Value == "7z")

                                item_Value = "sevenZip";
                            // foreach (XmlNode xmlNode in xmlPropertyNode)
                            // MessageBox.Show(xmlNode.Name+" 1"+((XmlNode)xmlNode).Value+" 2"+ xmlNode.FirstChild.Value);
                            dico.Add(new string[] { obj_Type.Namespace.Substring(obj_Type.Namespace.IndexOf('.') + 1).ToLower(), ReturnValidXmlNodeName(property.Name), ReturnValidXmlNodeName(item_Value) }, _value);

                        }

                    else

                    {

                        // Debug.WriteLine(property.GetValue(obj));

                        string _value = null;

                        if (typeof(Enum).IsAssignableFrom(property.PropertyType))

                        {

                            Enum @enum = (Enum)property.GetValue(obj);

                            _value = @enum.GetNumValue(@enum.ToString()).ToString();

                        }

                        else

                            _value = property.GetValue(obj).ToString();

                        dico.Add(new string[] { /*obj_Type.Name == "Common" ? ReturnValidXmlNodeName(obj_Type.Namespace) :*/ /*obj_Type.Name*/ obj_Type.Namespace.Substring(obj_Type.Namespace.IndexOf('.') + 1).ToLower(), ReturnValidXmlNodeName(property.Name) }, _value);

                    }

                }

            }

            return dico;

        }

        public static XmlDocument Settings { get; } = null;

        public static string SavePath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\WinCopies\\settings.settings";

        static SettingsManagement()

        {

            XmlDocument xmlDoc = new XmlDocument();

            try
            {

                xmlDoc.Load(SavePath);

            }

            catch (FileNotFoundException)
            {

                if (!CreateSettingFile(xmlDoc, SavePath, false))

                    xmlDoc = null;

            }

            catch (DirectoryNotFoundException)
            {

                if (!CreateSettingFile(xmlDoc, SavePath, true))

                    xmlDoc = null;

            }

            Settings = xmlDoc;

        }

        public static bool Reload()

        {

            try

            {

                Settings.Load(SavePath);

            }

            catch (IOException)

            {

                return false;

            }

            return true;

        }

        private struct SettingToLoad

        {

            public PropertyInfo Property { get; }

            public object Obj { get; }

            public SettingToLoad(PropertyInfo property, object obj)
            {

                Property = property;

                Obj = obj;

            }

        }

        private static Dictionary<SettingToLoad, string[]> GetSettingsToLoad(object obj)

        {

            var dico = new Dictionary<SettingToLoad, string[]>();

            var obj_Type = obj.GetType();

            var properties = obj_Type.GetProperties();

            foreach (var property in properties)

            {

                // if (property.GetCustomAttributes(typeof(SerializablePropertyAttribute)).Count((Attribute attribute) => true) > 0)

                if (property.GetCustomAttributes(typeof(SerializablePropertyAttribute), true).Length > 0)

                {

                    object value = property.GetValue(obj);

                    if (value is IEnumerable<CheckableObject>)

                        foreach (CheckableObject item in (IEnumerable<CheckableObject>)value)

                        {

                            string _value = item.IsChecked.ToString().ToLower();

                            string item_Value = ((string)item.Value).Substring(1);

                            if (item_Value == "7z")

                                item_Value = "sevenZip";
                            // foreach (XmlNode xmlNode in xmlPropertyNode)
                            // MessageBox.Show(xmlNode.Name+" 1"+((XmlNode)xmlNode).Value+" 2"+ xmlNode.FirstChild.Value);
                            string[] path = ((SerializablePropertyAttribute)property.GetCustomAttribute(typeof(SerializablePropertyAttribute))).RefersTo;
                            if (path == null)
                                path = new string[] { obj_Type.Namespace.Substring(obj_Type.Namespace.IndexOf('.') + 1).ToLower(), ReturnValidXmlNodeName(property.Name) };
                            path = path.Append( /*obj_Type.Name*/ ReturnValidXmlNodeName(item_Value)).OfType<string>().ToArray();
                            dico.Add(new SettingToLoad(typeof(CheckableObject).GetProperty(nameof(CheckableObject.IsChecked)), item), path );

                        }

                    else

                        // Debug.WriteLine(property.GetValue(obj));

                        dico.Add(new SettingToLoad(property, obj), ((SerializablePropertyAttribute)property.GetCustomAttribute(typeof(SerializablePropertyAttribute))).RefersTo ?? new string[] { /*obj_Type.Name == "Common" ? ReturnValidXmlNodeName(obj_Type.Namespace) : obj_Type.Name*/ obj_Type.Namespace.Substring(obj_Type.Namespace.IndexOf('.') + 1).ToLower(), ReturnValidXmlNodeName(property.Name) });

                }

            }

            return dico;

        }

        public static bool LoadSettings(object obj)

        {

            var settingsToLoad = GetSettingsToLoad(obj);

            bool errorOccurred = false;

            object value = null;

            XmlNode xmlPropertyNode = null;

            foreach (KeyValuePair<SettingToLoad, string[]> item in settingsToLoad)

            {

                xmlPropertyNode = Settings["settings"];

                foreach (string nodeName in item.Value)

                    if (xmlPropertyNode != null)

                        xmlPropertyNode = xmlPropertyNode[nodeName];

                    else

                        return false;

                if (xmlPropertyNode != null)

                    try

                    {

                        if (typeof(Enum).IsAssignableFrom(item.Key.Property.PropertyType))

                            value = Enum.Parse(item.Key.Property.PropertyType, Enum.GetName(item.Key.Property.PropertyType, Convert.ChangeType(xmlPropertyNode.InnerText, Enum.GetUnderlyingType(item.Key.Property.PropertyType))));

                        else

                            value = xmlPropertyNode.InnerText;

                        item.Key.Property.SetValue(item.Key.Obj, Convert.ChangeType(value, item.Key.Property.PropertyType));

                    }

                    catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException || ex is ArgumentNullException)

                    {

                        errorOccurred = true;

                    }

                else

                    return false;

            }

            return !errorOccurred;

        }

        public static bool SaveSettings(object obj)
        {

            var settingsToSave = GetSettingsToSave(obj);

            XmlNode xmlPropertyNode = null;

            foreach (KeyValuePair<string[], string> item in settingsToSave)

            {

                xmlPropertyNode = Settings["settings"];

                foreach (string nodeName in item.Key)

                    if (xmlPropertyNode != null)

                        xmlPropertyNode = xmlPropertyNode[nodeName];

                    else

                        return false;

                if (xmlPropertyNode != null)

                    xmlPropertyNode.InnerText = item.Value.ToString();

                else

                    return false;

            }

            Settings.Save(SavePath);

            return true;

        }

    }

}
