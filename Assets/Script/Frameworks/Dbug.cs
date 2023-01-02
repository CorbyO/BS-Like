using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Corby.Frameworks
{
    public static class Dbug
    {
        private const char TAG_ENTER = '[';
        private const char TAG_EXIT = ']';
        private static readonly StringBuilder _sb = new(1024);

        private static void AddTag(string tag)
        {
            _sb.Append(TAG_ENTER);
            _sb.Append(tag);
            _sb.Append(TAG_EXIT);
        }
        [Conditional("CORBY_DEVELOPMENT")]
        public static void Log(Type caller, string message, int skipFrame, params string[] customTags)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var st = new StackTrace(skipFrame);
            var callerMethod = st.GetFrame(0).GetMethod();
            
            _sb.Clear();
            AddTag($"{caller.Name}:{callerMethod.Name}({callerMethod.GetParameters().Length})");
            foreach (var t in customTags)
            {
                AddTag(t);
            }
            _sb.Append(' ');
            _sb.Append(message);

            UnityEngine.Debug.Log(_sb);
        }
        
        [Conditional("CORBY_DEVELOPMENT")]
        public static void Warning(Type caller, string message, int skipFrame, params string[] customTags)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var st = new StackTrace(skipFrame);
            var callerMethod = st.GetFrame(0).GetMethod();
            
            _sb.Clear();
            AddTag($"{caller.Name}:{callerMethod.Name}({callerMethod.GetParameters().Length})");
            foreach (var t in customTags)
            {
                AddTag(t);
            }
            _sb.Append(' ');
            _sb.Append(message);

            UnityEngine.Debug.LogWarning(_sb);
        }
        
        [Conditional("CORBY_DEVELOPMENT")]
        public static void Error(Type caller, string message, int skipFrame, params string[] customTags)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var st = new StackTrace(skipFrame);
            var callerMethod = st.GetFrame(0).GetMethod();
            
            _sb.Clear();
            AddTag($"{caller.Name}:{callerMethod.Name}({callerMethod.GetParameters().Length})");
            foreach (var t in customTags)
            {
                AddTag(t);
            }
            _sb.Append(' ');
            _sb.Append(message);

            UnityEngine.Debug.LogError(_sb);
        }
    }
}