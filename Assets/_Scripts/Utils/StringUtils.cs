using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class StringUtils {

    /// <summary>
    /// Insert spaces after uppercase letters (excluding start);
    /// </summary>
    /// <param name="str"> String to manipulate; </param>
    /// <returns> String with spaces after uppercase letters; </returns>
    public static string ToCamelSpace(this string str) {
        var nStr = new List<char>(str.ToCharArray());
        for (int i = 1; i < nStr.Count; i++) {
            if (char.IsUpper(nStr[i])) {
                nStr.Insert(i, ' ');
                i++;
            }
        } return string.Join("", nStr);
    }

    /// <summary>
    /// Contains extension for strings;
    /// </summary>
    /// <param name="comparison"> Comparison mode for the contains operation; </param>
    /// <returns> Whether the string contains a case insensitive substring; </returns>
    public static bool Contains(this string str, string substring, System.StringComparison comparison) {
        return str?.IndexOf(substring, comparison) >= 0;
    }

    /// <summary>
    /// Remove the ending of a string up to a given delimeter;<br></br>
    /// Used in the script to remove substrings pertaining to file pathing;
    /// </summary>
    /// <param name="str"> String to manipulate; </param>
    /// <param name="delimiters"> Use "\\/" to remove the file part;
    /// <br></br> Use "." to remove the file extension; </param>
    /// <returns> String representing the full path without the final part (ex: without the file name); </returns>
    public static string RemovePathEnd(this string str, string delimiters) {
        var index = 0;
        for (int i = str.Length - 1; i >= 0; i--) {
            if (delimiters.Contains(str[i])) {
                index = i;
                break;
            }
        } var nArr = new List<char>(str.ToCharArray()).GetRange(0, index).ToArray();
        return new string(nArr);
    }

    /// <summary>
    /// Remove the ending of a string up to a given delimeter;<br></br>
    /// Used in the script to remove substrings pertaining to file pathing;
    /// </summary>
    /// <param name="str"> String to manipulate; </param>
    /// <param name="delimiters"> Use "\\/" to remove the file part;
    /// <param name="endStr"> Portion of the string removed; </param>
    /// <br></br> Use "." to remove the file extension; </param>
    /// <returns> String representing the full path without the final part (ex: without the file name); </returns>
    public static string RemovePathEnd(this string str, string delimiters, out string endStr) {
        int index = 0;
        for (int i = str.Length - 1; i >= 0; i--) {
            if (delimiters.Contains(str[i])) {
                index = i;
                break;
            }
        } char[] newArr = new List<char>(str.ToCharArray()).GetRange(0, index).ToArray();
        char[] endArr = new List<char>(str.ToCharArray()).GetRange(index + 1 < str.Length ? index + 1 : 0,
                                                                   str.Length - index - 1).ToArray();
        endStr = new string(endArr);
        return new string(newArr);
    }

    public static string RemovePathEnd(this string str, string delimiters, int occurrences) {
        if (occurrences <= 0) return str;
        int index = 0;
        for (int i = str.Length - 1; i >= 0; i--) {
            if (delimiters.Contains(str[i])) {
                index = i;
                if (--occurrences == 0) break;
            }
        } char[] newArr = new List<char>(str.ToCharArray()).GetRange(0, index).ToArray();
        return new string(newArr);
    }

    /// <summary>
    /// Isolate the ending of a string up to a given delimeter;
    /// <br></br> Used in the script to retrieve file and folder names with our without file extensions, if applicable;
    /// </summary>
    /// <param name="str"> String to manipulate </param>
    /// <param name="delimiters"> Use "\\/" to isolate the final part of the path, either a folder or a file name;
    /// <br></br> Use "." to isolate the file extension of a file, if applicable; </param>
    /// <param name="trimExtension"> Whether or not to trim the file extension from the isolated file name; </param>
    /// <returns> String representing the end of the path only; </returns>
    public static string IsolatePathEnd(this string str, string delimiters, bool trimExtension = false) {
        var nStr = "";
        for (int i = str.Length - 1; i >= 0; i--) {
            if (delimiters.Contains(str[i])) break;
            else nStr += str[i];
        } var nArr = nStr.ToCharArray(); System.Array.Reverse(nArr);
        if (trimExtension) {
            for (int i = nArr.Length - 1; i >= 0; i--) {
                if (nArr[i] == '.') {
                    nArr = new List<char>(nArr).GetRange(0, i).ToArray();
                    break;
                }
            }
        } return new string(nArr);
    }

    #if UNITY_EDITOR

    /// <summary>
    /// Parse a System path into a Project Relative path;
    /// </summary>
    /// <param name="path"> System path; </param>
    /// <returns> Relative path if system path is valid, an empty string otherwise; </returns>
    public static string ToRelativePath(this string path) {
        int relativeStart = path.LastIndexOf("Assets");
        return relativeStart > 0 ? path.Substring(relativeStart) : "";
    } 

    /// <summary>
    /// Transforms a Model path into a Prefab path by changing the name of the root;
    /// <br></br> May trim the file name or replace it by the default Model name if indicated;
    /// </summary>
    /// <param name="modelPath"> Model path to modify; </param>
    /// <param name="includeDefaultName"> Whether to replace the file name with the default Model name; </param>
    /// <returns> Path string pointing towards the Prefab file hierarchy; </returns>
    public static string ToPrefabPath(this string modelPath, bool includeDefaultName = false) {
        string targetPath = modelPath.RemovePathEnd("\\/") + "/Prefabs";
        if (includeDefaultName) targetPath += "/" + modelPath.IsolatePathEnd("\\/", true) + ".prefab";
        return targetPath;
    }

    /// <summary>
    /// Transforms a Model path into a Prefab path by changing the name of the root;
    /// <br></br> Replaces the file name by the given name;
    /// </summary>
    /// <param name="modelPath"> Model path to modify; </param>
    /// <param name="fileName"> Name that will replace the default file name; </param>
    /// <returns> Path string pointing towards the Prefab file hierarchy; </returns>
    public static string ToPrefabPathWithName(this string modelPath, string fileName) {
        string targetPath = modelPath.RemovePathEnd("\\/") + "/Prefabs";
        targetPath += "/" + fileName + ".prefab";
        return targetPath;
    }

    /// <summary>
    /// Transforms a Model path into a Prefab path by changing the name of the root;
    /// <br></br> Replaces the file name by the name of an existing prefab whose ID is known;
    /// </summary>
    /// <param name="modelPath"> Model path to modify; </param>
    /// <param name="fileName"> GUID of the object whose name will replace the default file name; </param>
    /// <returns> Path string pointing towards the Prefab file hierarchy; </returns>
    public static string ToPrefabPathWithGUID(this string modelPath, string guid) {
        string targetPath = modelPath.RemovePathEnd("\\/") + "/Prefabs";
        targetPath += "/" + AssetDatabase.GUIDToAssetPath(guid).IsolatePathEnd("\\/", true) + ".prefab";
        return targetPath;
    }

    #endif
}