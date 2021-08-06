using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System;
using System.Reflection;
namespace Bm
{
    public class EnumNode
    {
        public string key;
        public string name;
        public int value;
    }

    public class EnumEditorWindow : EditorWindow
    {
        [MenuItem("BmTools/EnumEditor/打开编辑器", false)]
        static void Open()
        {
            EditorWindow.GetWindow<EnumEditorWindow>(false, "枚举编辑器", true).Show();
        }

        public List<EnumNode> enumNodes = new List<EnumNode>();
        public string CurrentEnumName;
        public string storePath = "Assets/Project/Script/EnumData";
        public int selectId;
        public string[] fileName;
        #region Editor Fun
        private void OnEnable()
        {
            storePath = EditorPrefs.GetString("EnumEditor_storePath", storePath);
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        Vector2 scroll;
        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorTools.DrawTitle("基本信息", new Color(0.15f, 0.15f, 0.15f));
            EditorGUILayout.BeginHorizontal();
            storePath = EditorGUILayout.TextField("路径", storePath);
            if (GUILayout.Button("保存"))
            {
                SaveStorePath();
            }
            if (GUILayout.Button("扫描"))
            {
                ScanPath(storePath);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            CurrentEnumName = EditorGUILayout.TextField("名称", CurrentEnumName);
            if(GUILayout.Button("创建"))
            {
                Create();
            }
            if (GUILayout.Button("保存"))
            {
                SaveEnumFile();
            }
            EditorGUILayout.EndHorizontal();

            if(fileName!=null)
            {
                EditorGUILayout.BeginHorizontal();
                selectId = EditorGUILayout.Popup("列表", selectId, fileName);
                if (GUILayout.Button("读取"))
                {
                    Read();
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorTools.DrawTitle("枚举信息列表", new Color(0.15f, 0.15f, 0.15f));
           

            if (GUILayout.Button("添加"))
                {
                    var t = new EnumNode();
                    t.value = enumNodes.Count == 0 ? 0 : enumNodes[enumNodes.Count - 1].value + 1;
                    enumNodes.Add(t);
                }
                EditorGUILayout.Space();
                scroll = EditorGUILayout.BeginScrollView(scroll);
                for (int i = 0; i < enumNodes.Count; i++)
                {
                    var node = enumNodes[i];
                    EditorGUILayout.BeginHorizontal();
                    node.key = EditorGUILayout.TextField("键值", node.key);
                    node.name = EditorGUILayout.TextField("显示名称", node.name);
                    node.value = EditorGUILayout.IntField("数值", node.value);
                    if (GUILayout.Button("删除"))
                    {
                        enumNodes.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
            
        }
        #endregion

        #region draw fun
        #endregion

        #region private fun
        private void Update()
        {
            if (!EditorApplication.isCompiling)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update -= Update;
            }
        }
        public void Create()
        {
            
        }

        public void SaveStorePath()
        {
            EditorPrefs.SetString("EnumEditor_storePath", storePath);
        }
        public void SaveEnumFile()
        {
            EditorApplication.update += Update;
            EditorUtility.DisplayCancelableProgressBar("提示", "正在编译~", 0.9f);

            if (CurrentEnumName.Length==0)
            {
                Log("名字为空");
                return;
            }
            StringBuilder sb = new StringBuilder(100);
            sb.AppendLine(string.Format("public enum {0}", CurrentEnumName));  //分配到堆区
            sb.AppendLine("{");
            for (int i = 0; i < enumNodes.Count; i++)
            {
                var node = enumNodes[i];                
                sb.AppendLine(string.Format("    [EnumName(\"{0}\")]", node.name));
                sb.AppendLine(string.Format("    {0}={1},", node.key, node.value));
            }
            sb.AppendLine("}");

            if(!File.Exists(storePath))
            {
                Directory.CreateDirectory(storePath);
            }

            File.WriteAllText(string.Format("{0}/{1}.cs", storePath, CurrentEnumName), sb.ToString());

            AssetDatabase.Refresh();
        }
        public void ScanPath(string _path)
        {
            string [] data = Directory.GetFiles(_path, "*.cs");
            fileName = new string[data.Length];
            for (int i=0; i<data.Length; i++)
            {
                var a = data[i].Split(new char[] {'/', '.', '\\' });
                fileName[i] = a[a.Length - 2];
            }
            
        }
        public void Read()
        {
            enumNodes.Clear();
            List<string> conent= ReadFileToList(string.Format("{0}/{1}.cs", storePath, fileName[selectId]));
            CurrentEnumName = conent[0].Replace("public enum ", "");
            for (int i=2; i<conent.Count-1; i+=2)
            {
                var name = GetStringInBrackets(conent[i]);
                name = name.Replace("\"", "");

                string[] arr = conent[i+1].Split(new char[] { '=', ',' });

                var node = new EnumNode();
                node.key = arr[0].Trim();
                node.name = name;
                node.value = int.Parse(arr[1]);
                
                enumNodes.Add(node);
            }
        }


        void Log(string _string)
        {
            Debug.Log("EnumEditor: "+ _string);
        }
        #endregion

        #region public fun
        #endregion


        private List<string> ReadFileToList(string FilePath)
        {
            //新建文件流
            FileStream fsRead;
            //用FileStream文件流打开文件
            try
            {
                fsRead = new FileStream(FilePath, FileMode.Open);
            }
            catch (Exception)
            {
                throw;
            }

            //"GB2312"用于显示中文字符，写其他的，中文会显示乱码
            StreamReader reader = new StreamReader(fsRead, UnicodeEncoding.GetEncoding("UTF-8"));

            List<string> list = new List<string>();
            string search;
            while ((search = reader.ReadLine()) != null)
            {

                list.Add(search);
            }

            fsRead.Close();
            return list;
        }

        string GetStringInBrackets(string _in)
        {
            return System.Text.RegularExpressions.Regex.Replace(_in, @"(.*\()(.*)(\).*)", "$2");
        }
    }
}

