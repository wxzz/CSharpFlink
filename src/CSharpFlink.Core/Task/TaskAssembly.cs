using CSharpFlink.Core.Common;
using CSharpFlink.Core.Log;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CSharpFlink.Core.Task
{
    public class TaskAssembly
    {
        private string Path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,"tasks");

        private Dictionary<string, Assembly> _BuilderCache { get; set; }
        internal Dictionary<string, Assembly> TaskAssemblys { get; private set; }

        public TaskAssembly()
        {
            Path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "tasks");
            if(System.IO.Directory.Exists(Path))
            {
                System.IO.Directory.CreateDirectory(Path);
            }

            _BuilderCache = new Dictionary<string, Assembly>();
            TaskAssemblys = new Dictionary<string, Assembly>();
        }

        internal void LoadTaskAssemblyFiles()
        {
            string[] files=FileUtil.GetFiles(Path);
            if (files.Length > 0)
            {
                foreach (string f in files)
                {
                    string file = System.IO.Path.Combine(Path, f);

                    AddTaskAssembly(file);
                }
            }
        }

        internal void AddTaskAssembly(string taskFile)
        {
            if (System.IO.Path.GetExtension(taskFile) == ".dll")
            {
                if (!TaskAssemblys.ContainsKey(taskFile))
                {
                    Assembly asm = null;
                    try
                    {
                        asm = Assembly.LoadFrom(taskFile);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error(true, "TaskAssembly:", ex);
                    }

                    if (asm != null)
                    {
                        TaskAssemblys.Add(taskFile, asm);
                    }
                }
            }
        }

        internal void CheckArgsTaskFile(Dictionary<string, string> args)
        {
            if (args != null && args.Count > 0)
            {
                if (args.ContainsKey(FormatArgs.c_t))
                {
                    string taskPath = args[FormatArgs.c_t];
                    if (System.IO.File.Exists(taskPath))
                    {
                        AddTaskAssembly(taskPath);
                    }
                    else
                    {
                        LoadTaskAssemblyFiles();
                    }
                }
                else
                {
                    LoadTaskAssemblyFiles();
                }
            }
            else
            {
                LoadTaskAssemblyFiles();
            }
        }

        public void ExcuteMain(string[] args)
        {
            Dictionary<string, string> argsDic = FormatArgs.GetArgsDictionary(args);

            CheckArgsTaskFile(argsDic);

            foreach (KeyValuePair<string,Assembly> kv in TaskAssemblys)
            {
                try
                {
                    if(kv.Value.EntryPoint!=null)
                    {
                        kv.Value.EntryPoint.Invoke(null, new object[] { args });
                    }
                }
                catch(Exception ex)
                {
                    Logger.Log.Error(true, "TaskAssembly:", ex);
                }
            }
        }

        public T Builder<T>(string instance)
        {
            T t = default(T);
            if (!_BuilderCache.ContainsKey(instance))
            {
                foreach (KeyValuePair<string, Assembly> kv in TaskAssemblys)
                {
                    try
                    {
                        t = (T)kv.Value.CreateInstance(instance);
                        _BuilderCache.Add(instance, kv.Value);
                        break;
                    }
                    catch
                    { }
                }
            }
            else
            {
                Assembly asm;
                if(_BuilderCache.TryGetValue(instance,out asm))
                {
                    t = (T)asm.CreateInstance(instance);
                }
            }
            return t;
        }
    }
}
