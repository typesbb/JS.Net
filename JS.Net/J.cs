using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Newtonsoft.Json;

namespace JS.Net
{
    /// <summary>
    /// 用于创建js的静态类，无法使用&&和||，使用and和or方法代替，递增（++）和递减（--）只能表示成前缀形式
    /// </summary>
    public static class J
    {
        public static dynamic syntax(string value)
        {
            return new Jsyntax(value);
        }
        public static dynamic syntax(object obj)
        {
            return new Jsyntax(obj);
        }

        #region 变量
        public static Jvar var(string name, dynamic value = null)
        {
            return new Jvar(name, value);
        }
        public static Jsyntax set(string name, dynamic value)
        {
            return syntax(string.Format("{0}={1}", name, J.GetJs(value)));
        }
        public static dynamic use
        {
            get { return syntax(null); }
        }

        public static Jsyntax delete(Jsyntax name)
        {
            return syntax(string.Format("delete {0}", name));
        }
        public static Jsyntax @typeof(object obj)
        {
            return syntax(string.Format("typeof({0})", J.GetJs(obj)));
        }
        public static dynamic undefined
        {
            get { return syntax("undefined"); }
        }
        public static dynamic @null
        {
            get { return syntax("null"); }
        }
        public static dynamic arguments
        {
            get { return syntax("arguments"); }
        }
        #endregion

        #region 其他

        public static Jsyntax and(Jsyntax a, Jsyntax b)
        {
            return syntax(string.Format("{0}&&{1}", a, b));
        }
        public static Jsyntax or(Jsyntax a, Jsyntax b)
        {
            return syntax(string.Format("{0}||{1}", a, b));
        }
        public static Jsyntax @break
        {
            get { return syntax("break"); }
        }
        public static Jsyntax @continue
        {
            get { return syntax("continue"); }
        }
        #endregion

        #region debugger
        public static Jsyntax debugger
        {
            get { return syntax("debugger"); }
        }

        public static Jconsole console
        {
            get { return new Jconsole(); }
        }

        public static Jsyntax alert(dynamic message)
        {
            return syntax(string.Format("alert({0})", J.GetJs(message)));
        }
        #endregion

        #region jquery
        public static readonly dynamic window = jquery(syntax("window"));
        public static readonly dynamic document = jquery(syntax("document"));
        public static readonly dynamic body = jquery(syntax("document.body"));

        public static dynamic jquery(string selector)
        {
            return new Jquery(selector);
        }
        public static dynamic jquery(Jsyntax obj)
        {
            return new Jquery(obj);
        }
        public static dynamic jqueryById(string id)
        {
            return jquery(string.Format(@"#{0}", id));
        }
        public static dynamic jqueryByClass(string @class)
        {
            return jquery(string.Format(@".{0}", @class));
        }
        #endregion

        #region Serializer
        public static string GetJs(object obj)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new JConverter());
            var stringWriter = new StringWriter();
            using (var writer = new JsonTextWriter(stringWriter))
            {
                writer.QuoteName = false;
                serializer.Serialize(writer, obj);
            }
            return stringWriter.ToString();
        }
        public class JConverter : JsonConverter
        {
            public override bool CanWrite { get { return true; } }
            public override bool CanRead { get { return false; } }
            public override bool CanConvert(Type objectType)
            {
                return typeof(Jexpression).IsAssignableFrom(objectType) || typeof(IEnumerable<Jexpression>).IsAssignableFrom(objectType);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteRawValue(value.ToString());
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return existingValue;
            }
        }
        #endregion
    }

    public abstract class Jexpression : DynamicObject
    {
        private string _value;

        protected string Value
        {
            get
            {
                if (_value == null)
                    return string.Empty;
                return _value;
            }
            set { _value = value; }
        }

        public override string ToString()
        {
            return Value;
        }

        #region Dynamic
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (base.TryGetMember(binder, out result)) return true;
            var obj = Value;
            if (!string.IsNullOrEmpty(obj))
                obj += ".";
            result = new Jsyntax(string.Format("{0}{1}", obj, binder.Name));
            return true;
        }

        protected virtual Jsyntax GetInvokeMemberResult(string str)
        {
            return new Jsyntax(str);
        }
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (base.TryInvokeMember(binder, args, out result)) return true;
            var obj = Value;
            if (!string.IsNullOrEmpty(obj))
                obj += ".";
            result = GetInvokeMemberResult(string.Format("{0}{1}({2})", obj, binder.Name, string.Join(",", args.Select(J.GetJs))));
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (base.TryGetIndex(binder, indexes, out result)) return true;
            result = new Jsyntax(string.Format("{0}[{1}]", Value, J.GetJs(indexes[0])));
            return true;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            if (base.TryInvoke(binder, args, out result)) return true;
            result = new Jsyntax(string.Format("{0}({1})", this, string.Join(",", args.Select(J.GetJs))));
            return true;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            return base.TryConvert(binder, out result);
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            if (base.TryUnaryOperation(binder, out result)) return true;
            string op;
            switch (binder.Operation)
            {
                case ExpressionType.UnaryPlus:
                    op = "+{0}";
                    break;
                case ExpressionType.Negate:
                    op = "-{0}";
                    break;
                default:
                    return false;
            }
            result = new Jsyntax(string.Format(op, this));
            return true;
        }

        #endregion

        #region Operator Overloaded
        public static implicit operator string(Jexpression j)
        {
            if ((object)j == null) return string.Empty;
            return j.ToString();
        }
        public static implicit operator Jexpression(string str)
        {
            return new Jsyntax(@"""" + str + @"""");
        }
        public static Jsyntax operator ++(Jexpression j)
        {
            return new Jsyntax(string.Format("++{0}", j));
        }
        public static Jsyntax operator --(Jexpression j)
        {
            return new Jsyntax(string.Format("--{0}", j));
        }
        public static Jsyntax operator !(Jexpression j)
        {
            return new Jsyntax(string.Format("!{0}", j));
        }
        public static Jsyntax operator ~(Jexpression j)
        {
            return new Jsyntax(string.Format("~{0}", j));
        }
        public static Jsyntax operator +(Jexpression j, Jexpression j1)
        {
            return new Jsyntax(string.Format("{0}+{1}", j, j1));
        }
        public static Jsyntax operator -(Jexpression j, Jexpression j1)
        {
            return new Jsyntax(string.Format("{0}-{1}", j, j1));
        }
        public static Jsyntax operator *(Jexpression j, Jexpression j1)
        {
            return new Jsyntax(string.Format("{0}*{1}", j, j1));
        }
        public static Jsyntax operator /(Jexpression j, Jexpression j1)
        {
            return new Jsyntax(string.Format("{0}*{1}", j, j1));
        }
        public static Jsyntax operator %(Jexpression j, Jexpression j1)
        {
            return new Jsyntax(string.Format("{0}%{1}", j, j1));
        }
        public static Jsyntax operator ^(Jexpression j, Jexpression j1)
        {
            return new Jsyntax(string.Format("{0}^{1}", j, j1));
        }
        public static Jsyntax operator ==(Jexpression j, Jexpression j1)
        {
            return new Jsyntax(string.Format("{0}=={1}", j, j1));
        }
        public static Jsyntax operator !=(Jexpression j, Jexpression j1)
        {
            return new Jsyntax(string.Format("{0}!={1}", j, j1));
        }
        public static Jsyntax operator >(Jexpression j, Jexpression j1)
        {
            return new Jsyntax(string.Format("{0}>{1}", j, j1));
        }
        public static Jsyntax operator >=(Jexpression j, Jexpression j1)
        {
            return new Jsyntax(string.Format("{0}>={1}", j, j1));
        }
        public static Jsyntax operator <(Jexpression j, Jexpression j1)
        {
            return new Jsyntax(string.Format("{0}<{1}", j, j1));
        }
        public static Jsyntax operator <=(Jexpression j, Jexpression j1)
        {
            return new Jsyntax(string.Format("{0}<={1}", j, j1));
        }
        public static Jsyntax operator &(Jexpression j, Jexpression j1)
        {
            return new Jsyntax(string.Format("{0}&{1}", j, j1));
        }
        public static Jsyntax operator |(Jexpression j, Jexpression j1)
        {
            return new Jsyntax(string.Format("{0}|{1}", j, j1));
        }
        public static Jsyntax operator >>(Jexpression j, int i)
        {
            return new Jsyntax(string.Format("{0}>>{1}", j, i));
        }
        public static Jsyntax operator <<(Jexpression j, int i)
        {
            return new Jsyntax(string.Format("{0}<<{1}", j, i));
        }
        #endregion
    }

    public class Jsyntax : Jexpression
    {
        public Jsyntax(object obj)
        {
            Value = J.GetJs(obj);
        }

        public Jsyntax(string value)
        {
            Value = value;
        }
    }

    public class JArray : Jsyntax
    {
        public JArray() : base("new Array()") { }
        public JArray(int count) : base(string.Format("new Array({0})", count)) { }
        public JArray(params object[] args) : base(string.Format("new Array({0})", string.Join(",", args.Select(J.GetJs)))) { }
    }

    public class JObject : Jsyntax
    {
        public JObject() : base("new Object()") { }
    }

    public class Jconsole : Jsyntax
    {
        public Jconsole() : base("console") { }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="message">%s代替字符串,%d代替整数,%f代替浮点值,%o代替Object</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public dynamic log(object message, params object[] args)
        {
            return J.syntax(Value).log(message, J.GetJs(args));
        }
        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="message">%s代替字符串,%d代替整数,%f代替浮点值,%o代替Object</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public dynamic info(object message, params object[] args)
        {
            return J.syntax(Value).info(message, J.GetJs(args));
        }
        /// <summary>
        /// 输出调试
        /// </summary>
        /// <param name="message">%s代替字符串,%d代替整数,%f代替浮点值,%o代替Object</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public dynamic debug(object message, params object[] args)
        {
            return J.syntax(Value).debug(message, J.GetJs(args));
        }
        /// <summary>
        /// 输出警告
        /// </summary>
        /// <param name="message">%s代替字符串,%d代替整数,%f代替浮点值,%o代替Object</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public dynamic warn(object message, params object[] args)
        {
            return J.syntax(Value).warn(message, J.GetJs(args));
        }
        /// <summary>
        /// 输出错误
        /// </summary>
        /// <param name="message">%s代替字符串,%d代替整数,%f代替浮点值,%o代替Object</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public dynamic error(object message, params object[] args)
        {
            return J.syntax(Value).error(message, J.GetJs(args));
        }
    }

    public class Jvar : Jsyntax
    {
        private Jvar _var;
        private dynamic _value;
        public Jvar(string name, dynamic value = null)
            : base(name)
        {
            _value = value;
        }

        public Jvar var(string name, dynamic value = null)
        {
            _var = new Jvar(name, value);
            return this;
        }

        private string Parse()
        {
            var sb = new StringBuilder();
            sb.Append(Value);
            if ((object)_value != null)
            {
                sb.Append("=");
                sb.Append(J.GetJs(_value));
            }
            if ((object)_var != null)
            {
                sb.Append("," + _var.Parse());
            }
            return sb.ToString();
        }
        public override string ToString()
        {
            return string.Format("var {0}", Parse());
        }
    }

    public class Jquery : Jsyntax
    {
        public Jquery(string selector) : base(selector.StartsWith("$") || selector.StartsWith("JQuery") ? selector : string.Format(@"$(""{0}"")", selector)) { }

        public Jquery(Jsyntax obj) : base(obj.ToString().StartsWith("$") || obj.ToString().StartsWith("JQuery") ? obj.ToString() : string.Format("$({0})", obj)) { }

        protected override Jsyntax GetInvokeMemberResult(string value)
        {
            return new Jquery(value);
        }

        public string bind(string eventName, JEvent func)
        {
            dynamic obj = this;
            return obj.bind(eventName, new Jfunction(J.use.e)
            {
                func(J.use.e)
            });
        }
    }

    public class Jbody : Jexpression, IEnumerable<Jexpression>
    {
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            return false;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            return false;
        }

        private readonly List<Jexpression> _items = new List<Jexpression>();
        IEnumerator<Jexpression> IEnumerable<Jexpression>.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }
        public void Add(Jexpression item)
        {
            _items.Add(item);
        }
        public static implicit operator Jbody(string value)
        {
            return new Jbody { new Jsyntax(value) };
        }
        public static implicit operator Jbody(Jsyntax j)
        {
            return new Jbody { j };
        }
        public override string ToString()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] is Jelse || _items[i] is Jelse_if)
                {
                    if (i == 0 || (!(_items[i - 1] is Jif) && !(_items[i - 1] is Jelse_if)))
                    {
                        throw new InvalidOperationException("else和else if应该在if或者else if之后！");
                    }
                }
            }
            return string.Join(string.Empty, this.Select(t => t.ToString() + ((t is Jbody) ? "" : ";")));
        }
    }

    public delegate Jbody JEvent(dynamic e);

    public class Jfunction : Jbody
    {
        public Jfunction(params Jsyntax[] args)
        {
            Value = string.Join(",", args.Select(arg => arg.ToString()));
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = new Jsyntax(string.Format("{0}", binder.Name));
            return true;
        }
        public override string ToString()
        {
            return string.Format(@"function({0}){{{1}}}", Value, base.ToString());
        }
    }

    public class Jwith : Jbody
    {
        public Jwith(Jsyntax arg)
        {
            Value = arg;
        }

        public override string ToString()
        {
            return string.Format(@"with({0}){{{1}}}", Value, base.ToString());
        }
    }

    public class Jif : Jbody
    {
        public Jif(Jsyntax condition)
        {
            Value = condition;
        }

        public override string ToString()
        {
            return string.Format(@"if({0}){{{1}}}", Value, base.ToString());
        }
    }
    public class Jelse : Jbody
    {
        public override string ToString()
        {
            return string.Format(@"else{{{0}}}", base.ToString());
        }
    }
    public class Jelse_if : Jbody
    {
        public Jelse_if(Jsyntax condition)
        {
            Value = condition;
        }
        public override string ToString()
        {
            return string.Format(@"else if({0}){{{1}}}", Value, base.ToString());
        }
    }

    public class Jfor : Jbody
    {
        public Jfor(Jvar var, Jsyntax condition, Jsyntax step)
        {
            Value = string.Format("{0};{1};{2}", var, condition, step);
        }
        public Jfor(Jvar var, Jsyntax @in)
        {
            Value = string.Format("{0} in {1}", var, @in);
        }

        public override string ToString()
        {
            return string.Format(@"for({0}){{{1}}}", Value, base.ToString());
        }
    }

}
