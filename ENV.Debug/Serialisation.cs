using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ENV.Many;
using ENV.Runtime;

namespace ENV.Debug
{
    public static class Serialisation
    {
        public static readonly PluginManager<ISpecialSerialisation> SpecialSerialisationManager =
            new PluginManager<ISpecialSerialisation>();

        public static ISpecialSerialisation[] SpecialSerialisations = new ISpecialSerialisation[0];

        static Serialisation()
        {
            LoadSpecialSerialisations();
        }

        public static ISpecialSerialisation[] LoadSpecialSerialisations()
        {
            return SpecialSerialisations = SpecialSerialisationManager
                .LoadPlugins()
                .Select(pluginType => SpecialSerialisationManager.InstantiatePlugin(pluginType))
                .ToArray();
        }

        private static string GetTypeName(Type t, Object o, ref Array array)
        {
            string typeName = t.Name;
            Type[] generics;
            string genericsString = "";
            string size = "";

            // Read generic type info if applicable
            if ((generics = t.GenericTypeArguments).Length > 0)
            {
                int count = generics.Length;
                genericsString += "<";

                // Remove generic type indicator from base name 
                typeName = t.Name.Substring(0, t.Name.IndexOf('`'));

                for (int i = 0; i < count; i++)
                {
                    genericsString += GetTypeName(generics[i], null, ref array);
                    genericsString += (i < count - 1) ? ", " : ">";
                }
            }

            // Read array size info if applicable
            if ((array = o as Array) != null)
            {
                size += "[";
                int rank = array.Rank;

                // Remove array indicator from base name 
                typeName = t.Name.Substring(0, t.Name.IndexOf('['));

                for (int i = 0; i < rank; i++)
                {
                    size += array.GetLength(i);
                    size += (i < rank - 1) ? ", " : "]";
                }
            }

            return typeName + genericsString + size;
        }

        public static string Serialize(this object o, bool multiline = true, int maxCascadeDepth = -1)
        {
            if (maxCascadeDepth < 0)
                maxCascadeDepth = 10;
            
            return o.Serialize(0, multiline, maxCascadeDepth);
        }

        private static string Serialize(this object o, int depth, bool multiline = true, int maxCascadeDepth = -1)
        {
            if (o == null)
                return "null";

            Type t = o.GetType();
            Array array = null;
            string typeName = GetTypeName(t, o, ref array);
            string result = null;
            bool stopCascade = depth == maxCascadeDepth;

            // Check for SpecialSerialisations exactly matching o's type
            foreach (var serialisation in SpecialSerialisations)
            {
                Type[] matchingTypes = serialisation
                    .CompatibleTypes
                    .Where(compatibleType => compatibleType == t)
                    .ToArray();

                if (matchingTypes.Length > 0)
                {
                    result = serialisation.Serialize(o, stopCascade);
                    break;
                }
            }

            // Check if o is of a subtype of any SpecialSerialisation's compatible types
            if (result == null)
            {
                foreach (var serialisation in SpecialSerialisations)
                {
                    Type[] matchingTypes = serialisation
                        .CompatibleTypes
                        .Where(compatibleType => compatibleType.IsAssignableFrom(t))
                        .ToArray();

                    if (matchingTypes.Length > 0)
                    {
                        result = serialisation.Serialize(o, stopCascade);
                        break;
                    }
                }
            }

            // Array serializer
            if (result == null)
            {
                if (array != null)
                {
                    if (stopCascade)
                    {
                        result += "[ ... ]";
                    }
                    else
                    {
                        int rank = array.Rank;
                        int flatIndex = 0;
                        int maxFlatIndex = 1;
                        int[] indices = new int[rank];
                        int[] dimensions = new int[rank];
                        for (var i = 0; i < dimensions.Length; i++)
                        {
                            dimensions[i] = array.GetLength(i);
                            maxFlatIndex *= dimensions[i];
                        }

                        int dim = 0;

                        while (flatIndex < maxFlatIndex)
                        {
                            result += "\n";
                            while (dim < rank)
                            {
                                result += "\t".Repeat(depth) + "[\n";
                                dim++;
                                depth++;
                            }

                            result += "\t".Repeat(depth) + "[";
                            for (var i = 0; i < indices.Length; i++)
                                result += indices[i] + ((i == indices.Length - 1) ? "] = " : ", ");

                            result += array.GetValue(indices).Serialize(depth + 1, multiline, maxCascadeDepth);

                            for (int i = rank - 1; i >= 0; i--)
                            {
                                indices[i]++;
                                if (indices[i] == dimensions[i])
                                {
                                    indices[i] = 0;
                                    dim--;
                                    depth--;
                                    result += "\n" + "\t".Repeat(depth) + "]";
                                }
                                else
                                {
                                    result += ",";
                                    break;
                                }
                            }

                            flatIndex++;
                        }
                    }
                }
                else if (t.IsClass)
                {
                    PropertyInfo[] properties = t.GetRuntimeProperties().ToArray();

                    if (properties.Length == 0)
                    {
                        result += "{}";
                    }
                    else if (stopCascade)
                    {
                        result += "{ ... }";
                    }
                    else
                    {
                        result += "\n" + "\t".Repeat(depth) + "{\n";
                        for (int i = 0; i < properties.Length; i++)
                        {
                            if (properties[i].GetIndexParameters().Length == 0)
                            {
                                object subObject = properties[i].GetValue(o);
                                result += "\t".Repeat(depth + 1) + properties[i].Name + " = " +
                                          subObject.Serialize(depth + 1, multiline, maxCascadeDepth) +
                                          (i < properties.Length - 1 ? ", " : "") + "\n";
                            }
                        }

                        result += "\t".Repeat(depth) + "}";
                    }
                }
                else
                {
                    result += "\"" + o.ToString() + "\"";
                }
            }

            return result + " : " + typeName;
        }

        public static void Print(this object o, string label = "", bool multiline = true, int maxCascadeDepth = -1)
        {
            Console.WriteLine((string.IsNullOrEmpty(label) ? "" : label + " = ") +
                              o.Serialize(multiline, maxCascadeDepth));
        }
    }
}