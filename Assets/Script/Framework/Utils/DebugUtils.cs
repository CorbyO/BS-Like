using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Corby.Framework.Utils
{
    public static class DebugUtils
    {
        private static readonly Stack<StringBuilder> s_stringBuilderPool = new();
        private static readonly Stopwatch s_stopwatch = new();
        
        private static StringBuilder PopStringBuilder()
        {
            return s_stringBuilderPool.Count > 0 ? s_stringBuilderPool.Pop() : new StringBuilder(1024);
        }
        
        private static void PushStringBuilder(StringBuilder sb)
        {
            sb.Clear();
            s_stringBuilderPool.Push(sb);
        }
        
        public static void StartStopwatch()
        {
            s_stopwatch.Reset();
            s_stopwatch.Start();
        }
        
        public static void StopStopwatchAndLog(string message)
        {
            s_stopwatch.Stop();
            Log($"{message}: {s_stopwatch.ElapsedMilliseconds}ms", 3);
        }
        
        /// <summary>
        /// 디버그 로그를 출력합니다.<br/>
        /// 이 로그는 개발자 빌드, 에디터에서만 출력됩니다.
        /// </summary>
        /// <param name="message">메세지</param>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string message, int stackFrameIndex = 2)
        {
            InnerLog(message, LogType.Log, stackFrameIndex);
        }
        
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string name, IDictionary dictionary, bool isPretty, int stackFrameIndex = 2)
        {
            var sb = PopStringBuilder();
            sb.Append(name).Append(' ').Append('[').Append(' ');
            if (isPretty)
            {
                sb.Append('\n');
            }
            foreach (DictionaryEntry entry in dictionary)
            {
                sb = sb.Append('\t').Append(entry.Key).Append(' ').Append(':').Append(' ').Append(entry.Value).Append(',').Append(' ');
                if (isPretty)
                {
                    sb.Append('\n');
                }
            }
            if (isPretty)
            {
                sb.Append('\n');
            }
            sb.Remove(sb.Length - 2, 2).Append(' ').Append(']');
            InnerLog(sb.ToString(), LogType.Log, stackFrameIndex);
            PushStringBuilder(sb);
        }
        
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string name, IList list, int stackFrameIndex = 2)
        {
            var sb = PopStringBuilder();
            sb.Append(name).Append(' ').Append('[').Append(' ');
            foreach (var item in list)
            {
                sb = sb.Append(item).Append(',').Append(' ');
            }
            sb.Remove(sb.Length - 2, 2).Append(' ').Append(']');
            InnerLog(sb.ToString(), LogType.Log, stackFrameIndex);
            PushStringBuilder(sb);
        }
        
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string name, IEnumerable list, int stackFrameIndex = 2)
        {
            var sb = PopStringBuilder();
            sb.Append(name).Append(' ').Append('[').Append(' ');
            foreach (var item in list)
            {
                sb = sb.Append(item).Append(',').Append(' ');
            }
            sb.Remove(sb.Length - 2, 2).Append(' ').Append(']');
            InnerLog(sb.ToString(), LogType.Log, stackFrameIndex);
            PushStringBuilder(sb);
        }

        /// <summary>
        /// 디버그 경고 로그를 출력합니다.<br/>
        /// 이 로그는 개발자 빌드, 에디터에서만 출력됩니다.
        /// </summary>
        /// <param name="message">메세지</param>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(string message, int stackFrameIndex = 2)
        {
            InnerLog(message, LogType.Warning, stackFrameIndex);
        }

        /// <summary>
        /// 디버그 오류 로그를 출력합니다.<br/>
        /// 이 로그는 개발자 빌드, 에디터에서만 출력됩니다.
        /// </summary>
        /// <param name="message">메세지</param>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(string message, int stackFrameIndex = 2)
        {
            InnerLog(message, LogType.Error, stackFrameIndex);
        }
        
        // public static void Panic(string message)
        // {
        //     throw new PanicException(message);
        // }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Assert<T>(this T obj, string valueName, int stackFrameIndex = 2)
            where T : Object
        {
            if (obj.IsUnityNull())
            {
                InnerLog($"{valueName} is null", LogType.Error, stackFrameIndex);
            }
        }

        private static void InnerLog(string message, LogType logType, int stackFrameIndex)
        {
            var stackFrame = new StackFrame(stackFrameIndex, true);
            var prevMethod = stackFrame.GetMethod();
            var prevMethodName = prevMethod.Name;
            var preClassName = prevMethod.ReflectedType.Name;
            
            if (prevMethodName[0] == '<')
            {
                var count = prevMethodName.IndexOf('>') - 1;
                prevMethodName = prevMethodName.Substring(1, count);
                
                var temp = prevMethod.DeclaringType.FullName;
                var startIndex = temp.LastIndexOf('.') + 1;
                if (startIndex != 0)
                {
                    var endIndex = temp.IndexOf('+', startIndex);
                    if (endIndex != -1)
                    {
                        preClassName = temp.Substring(startIndex, endIndex - startIndex);
                    }
                }
            }
            else if (prevMethodName == "MoveNext")
            {
                prevMethodName = prevMethod.DeclaringType.Name;
                var count = prevMethodName.IndexOf('>') - 1;
                prevMethodName = prevMethodName.Substring(1, count);
                
                var temp = prevMethod.DeclaringType.FullName;
                var startIndex = temp.LastIndexOf('.') + 1;
                if (startIndex != 0)
                {
                    var endIndex = temp.IndexOf('+', startIndex);
                    if (endIndex != -1)
                    {
                        preClassName = temp.Substring(startIndex, endIndex - startIndex);
                    }
                }
            }
            
            switch (logType)
            {
                case LogType.Log:
                    UnityEngine.Debug.Log($"[{preClassName}.{prevMethodName}] {message}");
                    break;
                case LogType.Warning:
                    UnityEngine.Debug.LogWarning($"[{preClassName}.{prevMethodName}] {message}");
                    break;
                case LogType.Error:
                    UnityEngine.Debug.LogError($"[{preClassName}.{prevMethodName}] {message}");
                    break;
            }
        }
    }
}