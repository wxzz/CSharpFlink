using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Expression.Operator;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Task;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;

namespace CSharpFlink.Core.Expression
{
    /// <summary>
    /// 表达式任务
    /// </summary>
    public class ExpressionTask : IExpressionTask
    {
        public ExpressionTask(string expId, string expName, ExpressionCalculateType expCalculateType, int timerInterval, string script,List<ICalculate> calcs)
        {
            Id = expId;
            Name = expName;

            Script = script;

            PatternDataList = new List<string>();

            PatternDataList.AddRange(GetPatternDataList(Script));

            if (calcs == null || calcs.Count<=0)
            {
                CalculateOperators = new List<ICalculate>() { new ExpressionCalculate($"{Id}_result") };
            }
            else
            {
                CalculateOperators = calcs;
            }

            _timer = new System.Timers.Timer(timerInterval);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;

            TimerInterval = timerInterval;

            ExpressionCalculateType = expCalculateType;
        }

        ~ExpressionTask()
        {
            Dispose(false);
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            this.PublishCalculate();
        }

        private bool _disposed = false;

        internal void PublishCalculate()
        {
            DateTime nowTime = DateTime.Now;

            IMetaData[] mds = new MetaData[PatternDataList.Count];
            for (int i = 0; i < mds.Length; i++)
            {
                mds[i] = new MetaData()
                {
                    TagId = PatternDataList[i]
                };
            }

            ICalculateContext calculateContext = new CalculateContext(Name, ExpressionCalculateType.ToString(), nowTime, nowTime, CalculateType.Expression, new CalculateInpute(Id, Script, nowTime, mds), null,CalculateOperators);

            GlobalContext.ActionBlock.Post(calculateContext);

            mds = null;
        }

        private System.Timers.Timer _timer;

        public string Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 计算间隔
        /// </summary>
        public int TimerInterval 
        { 
            get
            {
                return (int)_timer.Interval;
            }

            set
            {
                _timer.Interval = value;
            }
        }

        /// <summary>
        /// [id]*[id]
        /// </summary>
        public string Script { get; set; }

        private ExpressionCalculateType _expressionCalculateType = ExpressionCalculateType.TimerCalculate;

        /// <summary>
        /// 表达式计算类型
        /// </summary>
        public ExpressionCalculateType ExpressionCalculateType
        {
            get
            {
                return _expressionCalculateType;
            }
            set
            {
                _expressionCalculateType = value;
                if (_expressionCalculateType == ExpressionCalculateType.TimerCalculate)
                {
                    _timer.Enabled = true;
                }
                else if (_expressionCalculateType == ExpressionCalculateType.ValueChangedCalculate)
                {
                    _timer.Enabled = false;
                }
            }
        }

        /// <summary>
        /// 脚本中的数据点集合
        /// </summary>
        public List<string> PatternDataList { get; set; }

        public List<ICalculate> CalculateOperators { get; set; }

        public IGlobalContext GlobalContext { get; internal set; }

        /// <summary>
        /// 获得表达中的数据点ID
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public static string[] GetPatternDataList(string script)
        {
            string _pattern = @"(?<=[[{]+)[^]}]+";
            List<string> tagIds = new List<string>();
            MatchCollection matches = Regex.Matches(script, _pattern);
            foreach (var v in matches)
            {
                tagIds.Add(v.ToString());
            }
            return tagIds.ToArray();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (PatternDataList != null)
                {
                    PatternDataList.Clear();
                }

                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
