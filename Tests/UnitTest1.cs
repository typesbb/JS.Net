using System;
using JS.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void JExpression()
        {
            Assert.AreEqual(J.syntax("abcd"), "abcd");
            Assert.AreEqual((Jsyntax)"abcd", @"""abcd""");
            Assert.AreEqual((Jsyntax)3, @"3");
            Assert.AreEqual((Jsyntax)false, @"false");

            Assert.AreEqual(J.syntax(new { name = "abcd", age = 20 }), @"{name:""abcd"",age:20}");

            var syntax = J.syntax("abcd");
            Assert.AreEqual(++syntax, @"++abcd");

            syntax = J.syntax("abcd");
            Assert.AreEqual(--syntax, @"--abcd");

            Assert.AreEqual(!J.syntax("abcd"), @"!abcd");
            Assert.AreEqual(~J.syntax("abcd"), @"~abcd");

            Assert.AreEqual(+J.syntax("abcd"), @"+abcd");
            Assert.AreEqual(-J.syntax("abcd"), @"-abcd");

            Assert.AreEqual(J.syntax("abcd") + J.syntax("efg"), @"abcd+efg");
            Assert.AreEqual(J.syntax("abcd") - J.syntax("efg"), @"abcd-efg");
            Assert.AreEqual(J.syntax("abcd") * J.syntax("efg"), @"abcd*efg");
            Assert.AreEqual(J.syntax("abcd") / J.syntax("efg"), @"abcd/efg");
            Assert.AreEqual(J.syntax("abcd") % J.syntax("efg"), @"abcd%efg");
            Assert.AreEqual(J.syntax("abcd") ^ J.syntax("efg"), @"abcd^efg");

            Assert.AreEqual(J.syntax("abcd") == J.syntax("efg"), @"abcd==efg");
            Assert.AreEqual(J.syntax("abcd") != J.syntax("efg"), @"abcd!=efg");
            Assert.AreEqual(J.syntax("abcd") > J.syntax("efg"), @"abcd>efg");
            Assert.AreEqual(J.syntax("abcd") >= J.syntax("efg"), @"abcd>=efg");
            Assert.AreEqual(J.syntax("abcd") < J.syntax("efg"), @"abcd<efg");
            Assert.AreEqual(J.syntax("abcd") <= J.syntax("efg"), @"abcd<=efg");
            Assert.AreEqual(J.and(J.syntax("abcd"), J.syntax("efg")), @"abcd&&efg");
            Assert.AreEqual(J.or(J.syntax("abcd"), J.syntax("efg")), @"abcd||efg");

            Assert.AreEqual(J.syntax("abcd") & J.syntax("efg"), @"abcd&efg");
            Assert.AreEqual(J.syntax("abcd") | J.syntax("efg"), @"abcd|efg");
            Assert.AreEqual(J.syntax("abcd") >> 2, @"abcd>>2");
            Assert.AreEqual(J.syntax("abcd") << 2, @"abcd<<2");

            Assert.AreEqual(J.syntax("abcd").name, @"abcd.name");
            Assert.AreEqual(J.syntax("abcd").getName(), @"abcd.getName()");
            Assert.AreEqual(J.syntax("abcd").getName(J.syntax("efg")), @"abcd.getName(efg)");
            Assert.AreEqual(J.syntax("abcd").getName("efg"), @"abcd.getName(""efg"")");
            Assert.AreEqual(J.syntax("abcd")[1], @"abcd[1]");
            Assert.AreEqual(J.syntax("abcd")(J.syntax("efg")), @"abcd(efg)");
            Assert.AreEqual(J.syntax("abcd")("efg"), @"abcd(""efg"")");

            Assert.AreEqual(J.syntax("abcd").Call("efg"), @"abcd.efg");
            Assert.AreEqual(J.syntax("abcd").Call("efg", 2), @"abcd.efg(2)");
        }

        [TestMethod]
        public void JSyntax()
        {
            Assert.AreEqual(new JObject(), "new Object()");
            Assert.AreEqual(new JArray(), "new Array()");
            Assert.AreEqual(new JArray(5), "new Array(5)");
            Assert.AreEqual(new JArray(5, 6, 9), "new Array(5,6,9)");
            Assert.AreEqual(new JArray("a", "b", 9), @"new Array(""a"",""b"",9)");

            Assert.AreEqual(J.var("abcd"), "var abcd");
            Assert.AreEqual(J.var(J.use.abcd, 3), "var abcd=3");
            Assert.AreEqual(J.var(J.use.abcd).var(J.use.efg, J.use.node.Id).var(J.use.hi, new JArray()).var(J.use.jk), "var abcd,efg=node.Id,hi=new Array(),jk");
            Assert.AreEqual(J.var(J.use.abcd, J.or(J.use.a, J.use.b)), "var abcd=a||b");

            Assert.AreEqual(J.@return("abcd"), @"return ""abcd""");
            Assert.AreEqual(J.@return(new { id = "d60ccc72-23e2-e311-a9b3-2c59e5355b8f" }), @"return {id:""d60ccc72-23e2-e311-a9b3-2c59e5355b8f""}");
            Assert.AreEqual(J.@return(new Jfunction(J.use.e) { J.console.log("test%d", 2) }), @"return function(e){console.log(""test%d"",2);}");

            Assert.AreEqual(J.@return(
                new Jswitch(J.use.e) { 
                    new Jcase(1)
                    {
                        J.console.log("test%d", 1),
                        J.@break
                    },
                    new Jcase(2)
                    {
                        J.console.log("test%d", 2),
                        J.@break
                    },
                    new Jdefault()
                    {
                        J.console.log("test%d", 0),
                    }
                }), @"return switch(e){case 1:console.log(""test%d"",1);break;case 2:console.log(""test%d"",2);break;default:console.log(""test%d"",0);}");

            Assert.AreEqual(J.@return(
                new Jdowhile(J.use.e > 0)
                {
                    J.console.log("test%d", 1),
                }
            ), @"return do{console.log(""test%d"",1);}while(e>0)");
        }
    }
}
