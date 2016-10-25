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
        public static Jreturn @return(object value = null)
        {
            return new Jreturn(value);
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
            return new Jsyntax(string.Format("{0}&&{1}", a, b));
        }
        public static Jsyntax or(Jsyntax a, Jsyntax b)
        {
            return new Jsyntax(string.Format("{0}||{1}", a, b));
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

        public static dynamic console
        {
            get { return J.syntax("console"); }
        }

        public static Jsyntax alert(dynamic message)
        {
            return syntax(string.Format("alert({0})", J.GetJs(message)));
        }
        #endregion

        #region jquery
        public static dynamic jquery
        {
            get { return new Jsyntax("$"); }
        }
        public static dynamic jqueryById(string id)
        {
            return new Jquery(string.Format(@"#{0}", id));
        }
        public static dynamic jqueryByClass(string @class)
        {
            return new Jquery(string.Format(@".{0}", @class));
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

    public abstract class Jexpression : IDynamicMetaObjectProvider
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

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new MyDynamicMetaObject(parameter, this);
        }
        private class MyDynamicMetaObject : DynamicMetaObject
        {
            internal MyDynamicMetaObject(Expression expression, Jexpression value) : base(expression, BindingRestrictions.Empty, value) { }

            private DynamicMetaObject BindDynamicMetaObject(string methodName, Expression[] parameters)
            {
                var getEntry = new DynamicMetaObject(
                    Expression.Call(
                        Expression.Convert(Expression, LimitType),
                        typeof(Jexpression).GetMethod(methodName),
                        parameters),
                    BindingRestrictions.GetTypeRestriction(Expression, LimitType));
                return getEntry;
            }
            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                const string methodName = "GetMember";
                var parameters = new Expression[] { Expression.Constant(binder.Name) };
                return BindDynamicMetaObject(methodName, parameters);
            }

            public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
            {
                const string methodName = "SetMember";
                var parameters = new Expression[] { Expression.Constant(binder.Name), Expression.Convert(value.Expression, typeof(object)) };
                return BindDynamicMetaObject(methodName, parameters);
            }

            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            {
                const string methodName = "InvokeMember";
                var parameters = new Expression[] { Expression.Constant(binder.Name), Expression.Constant(args.Select(arg => arg.Value).ToArray()) };
                return BindDynamicMetaObject(methodName, parameters);
            }

            public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
            {
                const string methodName = "Invoke";
                var parameters = new Expression[] { Expression.Constant(args.Select(arg => arg.Value).ToArray()) };
                return BindDynamicMetaObject(methodName, parameters);
            }

            public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
            {
                const string methodName = "GetIndex";
                var parameters = new Expression[] { Expression.Constant(indexes.Select(arg => arg.Value).ToArray()) };
                return BindDynamicMetaObject(methodName, parameters);
            }
        }
        public virtual object GetMember(string name)
        {
            var obj = Value;
            if (!string.IsNullOrEmpty(obj))
                obj += ".";
            return new Jsyntax(string.Format("{0}{1}", obj, name));
        }
        public virtual object SetMember(string name, object value)
        {
            var syn = value as Jsyntax;
            if ((object)syn != null && (syn.Value.StartsWith("++") || syn.Value.StartsWith("--")) && syn.Other == name)
                return value;

            var obj = Value;
            if (!string.IsNullOrEmpty(obj))
                obj += ".";
            return new Jsyntax(string.Format("{0}{1}={2}", obj, name, J.GetJs(value)));
        }
        public virtual object InvokeMember(string name, object[] args)
        {
            if (name == "Call")
            {
                if (args.Length > 1)
                {
                    var parameters = new object[args.Length - 1];
                    Array.Copy(args, 1, parameters, 0, args.Length - 1);
                    return Call(args[0].ToString(), parameters);
                }
                else
                {
                    return Call(args[0].ToString());
                }
            }
            var obj = Value;
            if (!string.IsNullOrEmpty(obj))
                obj += ".";
            return GetInvokeMemberResult(string.Format("{0}{1}({2})", obj, name, string.Join(",", args.Select(J.GetJs))));
        }
        protected virtual Jsyntax GetInvokeMemberResult(string str)
        {
            return new Jsyntax(str);
        }

        public virtual object Invoke(object[] args)
        {
            return new Jsyntax(string.Format("{0}({1})", this, string.Join(",", args.Select(J.GetJs))));
        }
        public virtual object GetIndex(object[] indexes)
        {
            return new Jsyntax(string.Format("{0}[{1}]", Value, J.GetJs(indexes[0])));
        }
        public Jsyntax Call(string name, object value = null)
        {
            var str = "";
            var args = value as IEnumerable<object>;
            if (args != null)
                str = string.Format("({0})", string.Join(",", args.Select(J.GetJs)));
            else if (value != null)
                str = string.Format("({0})", J.GetJs(value));
            return new Jsyntax(string.Format("{0}.{1}{2}", this, name, str));
        }

        #region Operator Overloaded
        public static implicit operator string(Jexpression value)
        {
            if ((object)value == null) return string.Empty;
            return value.ToString();
        }
        public static implicit operator Jexpression(string value)
        {
            return new Jsyntax(@"""" + value + @"""");
        }
        public static implicit operator Jexpression(int value)
        {
            return new Jsyntax(@"" + value + @"");
        }
        public static implicit operator Jexpression(long value)
        {
            return new Jsyntax(@"" + value + @"");
        }
        public static implicit operator Jexpression(short value)
        {
            return new Jsyntax(@"" + value + @"");
        }
        public static implicit operator Jexpression(uint value)
        {
            return new Jsyntax(@"" + value + @"");
        }
        public static implicit operator Jexpression(ulong value)
        {
            return new Jsyntax(@"" + value + @"");
        }
        public static implicit operator Jexpression(ushort value)
        {
            return new Jsyntax(@"" + value + @"");
        }
        public static implicit operator Jexpression(float value)
        {
            return new Jsyntax(@"" + value + @"");
        }
        public static implicit operator Jexpression(double value)
        {
            return new Jsyntax(@"" + value + @"");
        }
        public static implicit operator Jexpression(decimal value)
        {
            return new Jsyntax(@"" + value + @"");
        }
        public static implicit operator Jexpression(bool value)
        {
            return new Jsyntax(@"" + value.ToString().ToLower() + @"");
        }
        public static Jsyntax operator ++(Jexpression j)
        {
            var syn = new Jsyntax(string.Format("++{0}", j));
            syn.Other = j;
            return syn;
        }
        public static Jsyntax operator --(Jexpression j)
        {
            var syn = new Jsyntax(string.Format("--{0}", j));
            syn.Other = j;
            return syn;
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
            return new Jsyntax(string.Format("{0}/{1}", j, j1));
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
        internal string Other;
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

    public class Jvar : Jsyntax
    {
        private Jvar _var;
        private readonly dynamic _value;
        public Jvar(string name, dynamic value = null)
            : base(name)
        {
            _value = value;
        }

        public override object InvokeMember(string name, object[] args)
        {
            if (name == "var")
            {
                if (args.Length > 1)
                    return var(args[0].ToString(), args[1]);
                else
                    return var(args[0].ToString());
            }
            return base.InvokeMember(name, args);
        }

        public Jvar var(string name, dynamic value = null)
        {
            var var = new Jvar(name, value);
            var._var = this;
            return var;
        }

        private string Parse()
        {
            var sb = new StringBuilder();
            if ((object)_var != null)
            {
                sb.Append(_var.Parse() + ",");
            }
            sb.Append(Value);
            if ((object)_value != null)
            {
                sb.Append("=");
                sb.Append(J.GetJs(_value));
            }
            return sb.ToString();
        }
        public override string ToString()
        {
            return string.Format("var {0}", Parse());
        }
    }

    public class Jreturn : Jsyntax
    {
        public Jreturn(object value)
            : base(null)
        {
            if (value == null)
                Value = J.syntax(string.Format("return"));
            else if (value is string)
                Value = J.syntax(string.Format(@"return ""{0}""", value));
            else
                Value = J.syntax(string.Format("return {0}", J.GetJs(value)));
        }
    }

    public class Jquery : Jsyntax
    {
        public Jquery(string selector) : base(selector.StartsWith("$") || selector.StartsWith("JQuery") ? selector : string.Format(@"$(""{0}"")", selector)) { }

        public Jquery(Jsyntax obj) : base(obj.ToString().StartsWith("$") || obj.ToString().StartsWith("JQuery") ? obj.ToString() : obj) { }

        protected override Jsyntax GetInvokeMemberResult(string value)
        {
            return new Jquery(value);
        }

        public override object InvokeMember(string name, object[] args)
        {
            if (name == "bind")
            {
                bind(args[0].ToString(), (JEvent)args[1]);
            }
            return null;
        }
        public string bind(string eventName, JEvent func)
        {
            dynamic obj = this;
            Jfunction f;
            if (J.use.e.Method.ReturnType == typeof(Jfunction))
            {
                f = func(J.use.e);
            }
            else
            {
                f = new Jfunction(J.use.e) { func(J.use.e) };
            }
            return obj.bind(eventName, f);
        }
    }

    public class Jbody : Jexpression, IEnumerable<Jexpression>
    {
        private readonly List<Jexpression> _items = new List<Jexpression>();
        IEnumerator<Jexpression> IEnumerable<Jexpression>.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }
        public override object InvokeMember(string name, object[] args)
        {
            if (name == "Add")
            {
                Add((Jexpression)args[0]);
            }
            return null;
        }

        public override object GetMember(string name)
        {
            return null;
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
            if (args == null || args.Length == 0)
                Value = null;
            else
                Value = string.Join(",", args.Select(arg => arg.ToString()));
        }

        public override object GetMember(string name)
        {
            return new Jsyntax(string.Format("{0}", name));
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

    public class Jwhile : Jbody
    {
        public Jwhile(Jsyntax condition)
        {
            Value = condition;
        }

        public override string ToString()
        {
            return string.Format(@"while({0}){{{1}}}", Value, base.ToString());
        }
    }
    public class Jdowhile : Jbody
    {
        public Jdowhile(Jsyntax condition)
        {
            Value = condition;
        }

        public override string ToString()
        {
            return string.Format(@"do{{{1}}}while({0})", Value, base.ToString());
        }
    }

    public class Jswitch : Jbody
    {
        public Jswitch(Jsyntax syntax)
        {
            Value = syntax;
        }

        public override string ToString()
        {
            return string.Format(@"switch({0}){{{1}}}", Value, base.ToString());
        }
    }
    public class Jcase : Jbody
    {
        public Jcase(object value)
        {
            Value = new Jsyntax(value);
        }

        public override string ToString()
        {
            return string.Format(@"case {0}:{1}", Value, base.ToString());
        }
    }
    public class Jdefault : Jbody
    {
        public override string ToString()
        {
            return string.Format(@"default:{0}", base.ToString());
        }
    }


}
